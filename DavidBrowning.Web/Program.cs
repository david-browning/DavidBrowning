// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Identity;
using DavidBrowning.Diagnostics;
using DavidBrowning.Helpers;
using DavidBrowning.Infrastructure;
using DavidBrowning.Infrastructure.Assets;
using DavidBrowning.Infrastructure.Cache;
using DavidBrowning.Infrastructure.Cache.Estimators;
using DavidBrowning.Infrastructure.Data;
using DavidBrowning.Infrastructure.Data.Stores;
using DavidBrowning.Infrastructure.Middleware;
using DavidBrowning.Infrastructure.Options;
using DavidBrowning.Infrastructure.Rendering;
using DavidBrowning.Infrastructure.Seo;
using DavidBrowning.Models;
using DavidBrowning.Web.Data.Seeding;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DavidBrowning.Web;

public static partial class Program
{
   public static async Task Main(string[] args)
   {
      var builder = WebApplication.CreateBuilder(args);
      ConfigureLogging(builder);
      ConfigureOptions(builder);
      ConfigureSecrets(builder);
      ConfigureServices(builder);
      ConfigureDatabase(builder);
      ConfigureLookupServices(builder);
      ConfigureStores(builder);
      ConfigureAuthorization(builder);

      var app = builder.Build();
      ConfigureWebApp(builder.Configuration, app);

      await SeedDatabaseAsync(app);

      app.Run();
   }

   private static void ConfigureLogging(WebApplicationBuilder builder)
   {
      builder.Logging.ClearProviders();
      builder.Logging.AddConsole();
      builder.Logging.AddDebug();
      //logger.AddAzureWebAppDiagnostics();
   }

   private static void ConfigureOptions(WebApplicationBuilder builder)
   {
      builder.Services.Configure<DiagnosticsOptions>(
         builder.Configuration.GetSection(ConfigurationHelpers.DiagnosticsSectionName));

      builder.Services.Configure<JsonCacheOptions>(
         builder.Configuration.GetSection($"{ConfigurationHelpers.CacheSectionName}:JsonCache"));

      builder.Services.Configure<RenderedContentCacheOptions>(
         builder.Configuration.GetSection(
            $"{ConfigurationHelpers.CacheSectionName}:RenderedContentCache"));

      builder.Services.Configure<SlugCacheOptions>(
         builder.Configuration.GetSection($"{ConfigurationHelpers.CacheSectionName}:SlugCache"));

      builder.Services.Configure<DateTimeDisplayOptions>(
         builder.Configuration.GetSection("DateTimeDisplayOptions"));

      builder.Services.Configure<SiteMetadataOptions>(
         builder.Configuration.GetSection("MetadataOptions"));
   }

   private static void ConfigureSecrets(WebApplicationBuilder builder)
   {
      string secretsProvider =
         builder.Configuration[ConfigurationHelpers.SecretsProviderKey] ?? ConfigurationHelpers.LocalProviderName;

      if (secretsProvider.EqualsOrdinalIgnoreCase(ConfigurationHelpers.LocalProviderName))
      {
         // Do nothing.
         //
         // In Development, WebApplication.CreateBuilder already loads
         // User Secrets
         // when the project has a UserSecretsId.
         //
         // For local non-Development environments, prefer environment variables
         // or a machine-local file that is not committed.
         return;
      }

      if (secretsProvider.EqualsOrdinalIgnoreCase(ConfigurationHelpers.AzureKeyVaultProviderName))
      {
         string? keyVaultUriText = builder.Configuration[ConfigurationHelpers.KeyVaultUriKey];
         if (string.IsNullOrWhiteSpace(keyVaultUriText))
         {
            throw new InvalidOperationException(
               "Secrets:Provider is KeyVault, but KeyVault:Uri is missing.");
         }

         builder.Configuration.AddAzureKeyVault(
            new Uri(keyVaultUriText),
            new DefaultAzureCredential());
         return;
      }

      throw new InvalidOperationException(
         $"Unknown secrets provider: {secretsProvider}");
   }

   private static void ConfigureServices(WebApplicationBuilder builder)
   {
      // Basic Services
      builder.Services.AddMemoryCache();
      builder.Services.AddSingleton<UrlBuilder>();
      builder.Services.AddSingleton<TimezoneConverter>();
      builder.Services.AddSingleton<StructuredDataBuilder>();
      builder.Services.AddSingleton<ISlugService, BasicSlugService>();
      builder.Services.AddSingleton<SitemapBuilder>();

      // These services are used to estimate the size of objects for use in
      // caching.
      builder.Services.AddSingleton(
         typeof(ICacheSizeEstimator<>),
         typeof(DefaultCacheSizeEstimator<>));

      builder.Services.AddSingleton(
         typeof(ICacheSizeEstimator<string>),
         typeof(StringSizeEstimator));

      builder.Services.AddSingleton<
         ICacheSizeEstimator<byte[]>,
         ByteArraySizeEstimator>();

      builder.Services.AddSingleton<
         ICacheSizeEstimator<RenderedContent>,
         RenderedContentSizeEstimator>();

      builder.Services.AddSingleton<
         ICacheSizeEstimator<object>,
         SingleObjectSizeEstimator<object>>();

      // Add specialized caching services
      builder.Services.AddSingleton<JsonCache>();
      builder.Services.AddSingleton<JsonMemoryCache>();
      builder.Services.AddSingleton<RenderedContentMemoryCache>();
      builder.Services.AddSingleton(typeof(SlugMemoryCache<>));

      // Configure how to get content
      string contentStoreProvider = GetConfiguredStoreProvider(
         builder, ConfigurationHelpers.ContentStoreName, ConfigurationHelpers.DummyProviderName);
      if (contentStoreProvider.EqualsOrdinalIgnoreCase(ConfigurationHelpers.DummyProviderName))
      {
         builder.Services.AddSingleton<IContentStore, DummyContentStore>();
      }
      else if (contentStoreProvider.EqualsOrdinalIgnoreCase(ConfigurationHelpers.LocalProviderName))
      {
         builder.Services.AddSingleton<IContentStore, LocalContentStore>();
      }
      else if (contentStoreProvider.EqualsOrdinalIgnoreCase(ConfigurationHelpers.AzureStorageBlobsProviderName))
      {
         builder.Services.AddSingleton<IContentStore, AzureBlobContentStore>();
      }
      else
      {
         throw new InvalidOperationException(
            $"Unknown content store provider: {contentStoreProvider}");
      }

      // Add the basic renderer.
      builder.Services.AddSingleton<IContentRenderer, BasicContentRenderer>();

      // Configure the rendering pipeline depending on whether caching is 
      // enabled.
      var enableCache = builder.Configuration.GetValue<bool>(
         $"{ConfigurationHelpers.CacheSectionName}:EnableContentCache");
      if (enableCache)
      {
         builder.Services.AddSingleton<BasicContentPipeline>();
         builder.Services.AddSingleton<IContentPipeline>(serviceProvider =>
         {
            IContentPipeline innerPipeline =
               serviceProvider.GetRequiredService<BasicContentPipeline>();

            var memoryCache =
               serviceProvider.GetRequiredService<RenderedContentMemoryCache>();

            return new CachedContentPipeline(
               innerPipeline,
               memoryCache);
         });
      }
      else
      {
         builder.Services.AddSingleton<IContentPipeline, BasicContentPipeline>();
      }

      builder.Services.AddSingleton<MarkdownDocumentRenderer>();
      builder.Services.AddSingleton(
         serviceProvider =>
         {
            IMarkdownDocumentRenderer innerRenderer =
               serviceProvider.GetRequiredService<
                  MarkdownDocumentRenderer>();

            if (!enableCache)
            {
               return innerRenderer;
            }

            return new CachedMarkdownDocumentRenderer(
               innerRenderer,
               serviceProvider.GetRequiredService<
                  RenderedContentMemoryCache>());
         });

      builder.Services.AddSingleton<MarkdownPostContentRenderer>();
      builder.Services.AddSingleton<MarkdownProjectContentRenderer>();

      builder.Services.AddControllersWithViews();
   }

   private static void ConfigureDatabase(WebApplicationBuilder builder)
   {
      string databaseProvider =
         builder.Configuration[ConfigurationHelpers.DatabaseProviderKey] ?? ConfigurationHelpers.SqlServerProviderName;
      bool enableSensitiveDataLogging =
         builder.Configuration.GetValue<bool>(ConfigurationHelpers.EnableSensitiveDataLoggingKey);
      bool enableDetailedErrors =
         builder.Configuration.GetValue<bool>(ConfigurationHelpers._enableDetailedErrorsKey);
      bool enableSqlCommandLogging =
         builder.Configuration.GetValue<bool>(ConfigurationHelpers.EnableSqlCommandLoggingKey);

      builder.Services.AddDbContext<SiteDbContext>(options =>
      {
         if (databaseProvider.EqualsOrdinalIgnoreCase(ConfigurationHelpers.SqlServerProviderName))
         {
            string? siteDatabaseConnectionString =
               builder.Configuration.GetConnectionString(
                  ConfigurationHelpers.SiteDatabaseConnectionName);

            if (string.IsNullOrWhiteSpace(siteDatabaseConnectionString))
            {
               throw new InvalidOperationException(
                  "Missing connection string: ConnectionStrings:SiteDatabase");
            }

            options.UseSqlServer(
               siteDatabaseConnectionString,
               sqlOptions =>
               {
                  sqlOptions.EnableRetryOnFailure(
                     maxRetryCount: 5,
                     maxRetryDelay: TimeSpan.FromSeconds(10),
                     errorNumbersToAdd: null);
               });
         }
         else if (databaseProvider.EqualsOrdinalIgnoreCase(
            ConfigurationHelpers.InMemoryProviderName))
         {
            string databaseName =
               builder.Configuration[ConfigurationHelpers.InMemoryDatabaseNameKey] ??
               ConfigurationHelpers.DefaultInMemoryDatabaseName;

            options.UseInMemoryDatabase(databaseName);
         }
         else
         {
            throw new InvalidOperationException(
               $"Unsupported database provider: {databaseProvider}");
         }

         ConfigureEntityFrameworkDiagnostics(
            builder,
            options,
            enableSensitiveDataLogging,
            enableDetailedErrors,
            enableSqlCommandLogging);
      });
   }

   private static void ConfigureEntityFrameworkDiagnostics(
      WebApplicationBuilder builder,
      DbContextOptionsBuilder options,
      bool enableSensitiveDataLogging,
      bool enableDetailedErrors,
      bool enableSqlCommandLogging)
   {
      if (builder.Environment.IsDevelopment())
      {
         if (enableSensitiveDataLogging)
         {
            options.EnableSensitiveDataLogging();
         }

         if (enableDetailedErrors)
         {
            options.EnableDetailedErrors();
         }

         if (enableSqlCommandLogging)
         {
            options.LogTo(
               Console.WriteLine,
               new[] { DbLoggerCategory.Database.Command.Name },
               LogLevel.Information);
         }

         return;
      }

      if (enableSensitiveDataLogging)
      {
         throw new InvalidOperationException(
            "EnableSensitiveDataLogging must not be enabled outside " +
            "Development.");
      }

      if (enableSqlCommandLogging)
      {
         throw new InvalidOperationException(
            "EnableSqlCommandLogging must not be enabled outside Development.");
      }

      if (enableDetailedErrors)
      {
         options.EnableDetailedErrors();
      }
   }

   private static void ConfigureLookupServices(WebApplicationBuilder builder)
   {
      string lookupProvider = GetConfiguredStoreProvider(
         builder, ConfigurationHelpers.LookupStoreName, ConfigurationHelpers.DummyProviderName);

      if (lookupProvider.EqualsOrdinalIgnoreCase(ConfigurationHelpers.SqlServerProviderName))
      {
         builder.Services.AddScoped(
            typeof(ISlugLookupService<>),
            typeof(SlugLookupCache<>));
         return;
      }

      throw new InvalidOperationException(
         $"Unknown lookup store provider: {lookupProvider}");
   }

   private static void ConfigureStores(WebApplicationBuilder builder)
   {
      ConfigureErrorStore(builder);
      ConfigureWritingStore(builder);
      ConfigureProjectStore(builder);
      ConfigureWorkStore(builder);
      ConfigureUncategorizedStore(builder);
   }

   private static void ConfigureErrorStore(WebApplicationBuilder builder)
   {
      string provider = GetConfiguredStoreProvider(
         builder, ConfigurationHelpers.ErrorStoreName, ConfigurationHelpers.DummyProviderName);
      if (provider.EqualsOrdinalIgnoreCase(ConfigurationHelpers.SqlServerProviderName))
      {
         builder.Services.AddScoped<IErrorStore, SqlErrorStore>();
         return;
      }

      throw new InvalidOperationException(
         $"Unknown error store provider: {provider}");
   }

   private static void ConfigureWritingStore(WebApplicationBuilder builder)
   {
      string provider = GetConfiguredStoreProvider(
         builder, ConfigurationHelpers.WritingStoreName, ConfigurationHelpers.DummyProviderName);
      if (provider.EqualsOrdinalIgnoreCase(ConfigurationHelpers.SqlServerProviderName))
      {
         builder.Services.AddScoped<IWritingStore, SqlWritingStore>();
         return;
      }

      throw new InvalidOperationException(
         $"Unknown writing store provider: {provider}");
   }

   private static void ConfigureProjectStore(WebApplicationBuilder builder)
   {
      string provider = GetConfiguredStoreProvider(
         builder, ConfigurationHelpers.ProjectStoreName, ConfigurationHelpers.DummyProviderName);
      if (provider.EqualsOrdinalIgnoreCase(ConfigurationHelpers.SqlServerProviderName))
      {
         builder.Services.AddScoped<IProjectStore, SqlProjectStore>();
         return;
      }

      throw new InvalidOperationException(
         $"Unknown project store provider: {provider}");
   }

   private static void ConfigureWorkStore(WebApplicationBuilder builder)
   {
      builder.Services.AddScoped<IWorkStore, SqlWorkStore>();
   }

   private static void ConfigureUncategorizedStore(WebApplicationBuilder builder)
   {
      builder.Services.AddScoped<IUncategorizedStore, SqlUncategorizedStore>();
   }

   private static void ConfigureAuthorization(WebApplicationBuilder builder)
   {

   }

   private static void ConfigureWebApp(
      ConfigurationManager config,
      WebApplication app)
   {
      if (app.Environment.IsDevelopment())
      {
         app.UseDeveloperExceptionPage();
         if (config.GetValue<bool>(ConfigurationHelpers.EnableCustomErrorsKey))
         {
            app.UseExceptionHandler(ConfigurationHelpers.InternalServerErrorPath);
         }
      }
      else
      {
         app.UseExceptionHandler(ConfigurationHelpers.InternalServerErrorPath);
         app.UseHsts();
      }

      app.UseMiddleware<ErrorLoggingMiddleware>();
      app.UseStatusCodePagesWithReExecute(ConfigurationHelpers.StatusCodePagePath);

      app.UseHttpsRedirection();
      app.UseStaticFiles();
      app.UseRouting();

      app.UseAuthentication();
      app.UseAuthorization();

      app.MapControllerRoute(
         name: ConfigurationHelpers.DefaultRouteName,
         pattern: ConfigurationHelpers.DefaultRoutePattern);
   }

   private static async Task SeedDatabaseAsync(WebApplication app)
   {
      using var scope = app.Services.CreateScope();

      var configuration =
         scope.ServiceProvider.GetRequiredService<IConfiguration>();

      // Just create a logger because we cannot get loggers for static classes.
      var loggerFactory =
         scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
      var logger = loggerFactory.CreateLogger(ConfigurationHelpers.StartupLoggerName);

      bool seedEnabled = configuration.GetValue<bool>(ConfigurationHelpers.SeedEnabledKey);

      if (!seedEnabled)
      {
         logger.LogInformation("Database seeding is disabled.");
         return;
      }

      string? seedRootFolder = configuration[ConfigurationHelpers.SeedRootFolderKey];
      var environment =
         scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
      var dbContext =
         scope.ServiceProvider.GetRequiredService<SiteDbContext>();

      if (string.IsNullOrWhiteSpace(seedRootFolder))
      {
         seedRootFolder = Path.Combine(
            environment.ContentRootPath,
            ConfigurationHelpers.DataFolderName,
            ConfigurationHelpers.SeedFolderName);
      }

      string tablePrefix =
         configuration[ConfigurationHelpers.TablePrefixKey] ?? ConfigurationHelpers.DefaultTablePrefix;

      JsonSeedDatabaseSeederOptions seedOptions = new()
      {
         TablePrefix = tablePrefix,
         SkipFileWhenTargetTableHasRows =
            configuration.GetValue(ConfigurationHelpers.SkipFileWhenTargetTableHasRowsKey, true),
         ThrowOnUnmatchedJsonFiles =
            configuration.GetValue(ConfigurationHelpers.ThrowOnUnmatchedJsonFilesKey, true),
         UseSqlServerIdentityInsertWhenNeeded =
            configuration.GetValue(
               ConfigurationHelpers.UseSqlServerIdentityInsertWhenNeededKey,
               true)
      };

      JsonSeedDatabaseSeeder<SiteDbContext> seeder = new(
         dbContext,
         seedOptions);

      var seedResult = await seeder.SeedAsync(seedRootFolder);

      logger.LogInformation(
         "Database seeding complete. Inserted {InsertedEntityCount} " +
         "entities from {InsertedFileCount} files. Skipped " +
         "{SkippedFileCount} files.",
         seedResult.InsertedEntityCount,
         seedResult.InsertedFileCount,
         seedResult.SkippedFileCount);
   }

   private static string GetConfiguredStoreProvider(
      WebApplicationBuilder builder,
      string storeName,
      string defaultProviderName)
   {
      string key = GetStoreProviderKey(storeName);

      return builder.Configuration[key] ?? defaultProviderName;
   }

   private static string GetStoreProviderKey(string storeName)
   {
      return $"{ConfigurationHelpers.StoresSectionName}:{storeName}:{ConfigurationHelpers.ProviderSectionName}";
   }
}
