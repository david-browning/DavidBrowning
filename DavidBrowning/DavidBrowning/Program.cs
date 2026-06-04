// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Identity;
using DavidBrowning.Data;
using DavidBrowning.Data.Seeding;
using DavidBrowning.Data.Stores.Error;
using DavidBrowning.Data.Stores.Projects;
using DavidBrowning.Data.Stores.Uncategorized;
using DavidBrowning.Data.Stores.Writing;
using DavidBrowning.Diagnostics;
using DavidBrowning.Extensions;
using DavidBrowning.Middleware;
using DavidBrowning.Models;
using DavidBrowning.Models.ViewModels;
using DavidBrowning.Services;
using DavidBrowning.Services.Assets;
using DavidBrowning.Services.Cache;
using DavidBrowning.Services.Cache.Estimators;
using DavidBrowning.Services.Cache.Options;
using DavidBrowning.Services.Rendering;
using DavidBrowning.Services.Slugs;
using DavidBrowning.Services.Time;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DavidBrowning;

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
         builder.Configuration.GetSection(_diagnosticsSectionName));

      builder.Services.Configure<JsonCacheOptions>(
         builder.Configuration.GetSection($"{_cacheSectionName}:JsonCache"));

      builder.Services.Configure<RenderedContentCacheOptions>(
         builder.Configuration.GetSection(
            $"{_cacheSectionName}:RenderedContentCache"));

      builder.Services.Configure<SlugCacheOptions>(
         builder.Configuration.GetSection($"{_cacheSectionName}:SlugCache"));

      builder.Services.Configure<DateTimeDisplayOptions>(
         builder.Configuration.GetSection("DateTimeDisplayOptions"));
   }

   private static void ConfigureSecrets(WebApplicationBuilder builder)
   {
      string secretsProvider =
         builder.Configuration[_secretsProviderKey] ?? _localProviderName;

      if (secretsProvider.EqualsOrdinalIgnoreCase(_localProviderName))
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

      if (secretsProvider.EqualsOrdinalIgnoreCase(_azureKeyVaultProviderName))
      {
         string? keyVaultUriText = builder.Configuration[_keyVaultUriKey];
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
      builder.Services.AddSingleton<ISystemClock, SystemClock>();
      builder.Services.AddSingleton<ISlugService, BasicSlugService>();
      builder.Services.AddSingleton<UrlBuilder>();
      builder.Services.AddSingleton<
         IDateTimeDisplayService, BasicDateTimeDisplayService>();

      // These services are used to estimate the size of objects for use in
      // caching.
      builder.Services.AddSingleton(
         typeof(ICacheSizeEstimator<>),
         typeof(DefaultCacheSizeEstimator<>));

      builder.Services.AddSingleton(
         typeof(ICacheSizeEstimator<string?>),
         typeof(StringSizeEstimator));

      builder.Services.AddSingleton<
         ICacheSizeEstimator<byte[]?>,
         ByteArraySizeEstimator>();

      builder.Services.AddSingleton<
         ICacheSizeEstimator<RenderedContent?>,
         RenderedContentSizeEstimator>();

      builder.Services.AddSingleton<
         ICacheSizeEstimator<object?>,
         SingleObjectSizeEstimator<object?>>();

      // Add specialized caching services
      builder.Services.AddSingleton<JsonCache>();
      builder.Services.AddSingleton<JsonMemoryCache>();
      builder.Services.AddSingleton<RenderedContentMemoryCache>();
      builder.Services.AddSingleton(typeof(SlugMemoryCache<>));

      // Configure how to get content
      string contentStoreProvider = GetConfiguredStoreProvider(
         builder, _contentStoreName, _dummyProviderName);
      if (contentStoreProvider.EqualsOrdinalIgnoreCase(_dummyProviderName))
      {
         builder.Services.AddSingleton<IContentStore, DummyContentStore>();
      }
      else if (contentStoreProvider.EqualsOrdinalIgnoreCase(_localProviderName))
      {
         builder.Services.AddSingleton<IContentStore, LocalContentStore>();
      }
      else if (contentStoreProvider.EqualsOrdinalIgnoreCase(_azureStorageBlobsProviderName))
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
         $"{_cacheSectionName}:EnableContentCache");
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
      builder.Services.AddSingleton<IMarkdownDocumentRenderer>(
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
         builder.Configuration[_databaseProviderKey] ?? _sqlServerProviderName;
      bool enableSensitiveDataLogging =
         builder.Configuration.GetValue<bool>(_enableSensitiveDataLoggingKey);
      bool enableDetailedErrors =
         builder.Configuration.GetValue<bool>(_enableDetailedErrorsKey);
      bool enableSqlCommandLogging =
         builder.Configuration.GetValue<bool>(_enableSqlCommandLoggingKey);

      builder.Services.AddDbContext<SiteDbContext>(options =>
      {
         if (databaseProvider.EqualsOrdinalIgnoreCase(_sqlServerProviderName))
         {
            string? siteDatabaseConnectionString =
               builder.Configuration.GetConnectionString(
                  _siteDatabaseConnectionName);

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
            _inMemoryProviderName))
         {
            string databaseName =
               builder.Configuration[_inMemoryDatabaseNameKey] ??
               _defaultInMemoryDatabaseName;

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
         builder, _lookupStoreName, _dummyProviderName);

      if (lookupProvider.EqualsOrdinalIgnoreCase(_sqlServerProviderName))
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
      ConfigureUncategorizedStore(builder);
   }

   private static void ConfigureErrorStore(WebApplicationBuilder builder)
   {
      string provider = GetConfiguredStoreProvider(
         builder, _errorStoreName, _dummyProviderName);
      if (provider.EqualsOrdinalIgnoreCase(_sqlServerProviderName))
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
         builder, _writingStoreName, _dummyProviderName);
      if (provider.EqualsOrdinalIgnoreCase(_sqlServerProviderName))
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
         builder, _projectStoreName, _dummyProviderName);
      if (provider.EqualsOrdinalIgnoreCase(_sqlServerProviderName))
      {
         builder.Services.AddScoped<IProjectStore, SqlProjectStore>();
         return;
      }

      throw new InvalidOperationException(
         $"Unknown project store provider: {provider}");
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
         if (config.GetValue<bool>(_enableCustomerErrorsKey))
         {
            app.UseExceptionHandler(_internalServerErrorPath);
         }
      }
      else
      {
         app.UseExceptionHandler(_internalServerErrorPath);
         app.UseHsts();
      }

      app.UseMiddleware<ErrorLoggingMiddleware>();
      app.UseStatusCodePagesWithReExecute(_statusCodePagePath);

      app.UseHttpsRedirection();
      app.UseStaticFiles();
      app.UseRouting();

      app.UseAuthentication();
      app.UseAuthorization();

      app.MapControllerRoute(
         name: _defaultRouteName,
         pattern: _defaultRoutePattern);
   }

   private static async Task SeedDatabaseAsync(WebApplication app)
   {
      using var scope = app.Services.CreateScope();

      var configuration =
         scope.ServiceProvider.GetRequiredService<IConfiguration>();

      // Just create a logger because we cannot get loggers for static classes.
      var loggerFactory =
         scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
      var logger = loggerFactory.CreateLogger(_startupLoggerName);

      bool seedEnabled = configuration.GetValue<bool>(_seedEnabledKey);

      if (!seedEnabled)
      {
         logger.LogInformation("Database seeding is disabled.");
         return;
      }

      string? seedRootFolder = configuration[_seedRootFolderKey];
      var environment =
         scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
      var dbContext =
         scope.ServiceProvider.GetRequiredService<SiteDbContext>();

      if (string.IsNullOrWhiteSpace(seedRootFolder))
      {
         seedRootFolder = Path.Combine(
            environment.ContentRootPath,
            _dataFolderName,
            _seedFolderName);
      }

      string tablePrefix =
         configuration[_tablePrefixKey] ?? _defaultTablePrefix;

      JsonSeedDatabaseSeederOptions seedOptions = new()
      {
         TablePrefix = tablePrefix,
         SkipFileWhenTargetTableHasRows =
            configuration.GetValue(_skipFileWhenTargetTableHasRowsKey, true),
         ThrowOnUnmatchedJsonFiles =
            configuration.GetValue(_throwOnUnmatchedJsonFilesKey, true),
         UseSqlServerIdentityInsertWhenNeeded =
            configuration.GetValue(
               _useSqlServerIdentityInsertWhenNeededKey,
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
      return $"{_storesSectionName}:{storeName}:{_providerSectionName}";
   }

   private const string _azureKeyVaultProviderName = "AzureKeyVault";
   private const string _azureStorageBlobsProviderName = "AzureStorageBlobs";
   private const string _dataFolderName = "Data";
   private const string _defaultInMemoryDatabaseName = "DavidBrowning";
   private const string _defaultRouteName = "default";
   private const string _defaultRoutePattern =
      "{controller=Home}/{action=Index}/{id?}";
   private const string _defaultTablePrefix = "db_";
   private const string _diagnosticsSectionName = "Diagnostics";
   private const string _cacheSectionName = "Cache";
   private const string _dummyProviderName = "Dummy";
   private const string _errorStoreName = "ErrorStore";
   private const string _inMemoryProviderName = "InMemory";
   private const string _localProviderName = "Local";
   private const string _lookupCacheSectionName = "LookupCache";
   private const string _lookupStoreName = "LookupStore";
   private const string _projectStoreName = "ProjectStore";
   private const string _providerSectionName = "Provider";
   private const string _seedFolderName = "Seed";
   private const string _siteDatabaseConnectionName = "SiteDatabase";
   private const string _sqlServerProviderName = "SqlServer";
   private const string _startupLoggerName = "Startup";
   private const string _storesSectionName = "Stores";
   private const string _writingStoreName = "WritingStore";

   private const string _contentStoreName = "ContentStore";

   private const string _databaseProviderKey = "Database:Provider";
   private const string _enableCustomerErrorsKey =
      "Diagnostics:EnableCustomerErrors";
   private const string _enableDetailedErrorsKey =
      "Diagnostics:EntityFrameworkOptions:EnableDetailedErrors";
   private const string _enableSensitiveDataLoggingKey =
      "Diagnostics:EntityFrameworkOptions:EnableSensitiveDataLogging";
   private const string _enableSqlCommandLoggingKey =
      "Diagnostics:EntityFrameworkOptions:EnableSqlCommandLogging";
   private const string _inMemoryDatabaseNameKey =
      "Database:InMemoryDatabaseName";
   private const string _internalServerErrorPath = "/Error/StatusCode/500";
   private const string _keyVaultUriKey = "KeyVault:Uri";
   private const string _secretsProviderKey = "Secrets:Provider";
   private const string _seedEnabledKey = "Database:Seed:Enabled";
   private const string _seedRootFolderKey = "Database:Seed:RootFolder";
   private const string _skipFileWhenTargetTableHasRowsKey =
      "Database:Seed:SkipFileWhenTargetTableHasRows";
   private const string _statusCodePagePath = "/Error/StatusCode/{0}";
   private const string _tablePrefixKey = "Database:TablePrefix";
   private const string _throwOnUnmatchedJsonFilesKey =
      "Database:Seed:ThrowOnUnmatchedJsonFiles";
   private const string _useSqlServerIdentityInsertWhenNeededKey =
      "Database:Seed:UseSqlServerIdentityInsertWhenNeeded";
}
