using DavidBrowning.Diagnostics;
using DavidBrowning.Helpers;
using DavidBrowning.Infrastructure.Assets;
using DavidBrowning.Infrastructure.Cache;
using DavidBrowning.Infrastructure.Cache.Estimators;
using DavidBrowning.Infrastructure.Data;
using DavidBrowning.Infrastructure.Data.Stores;
using DavidBrowning.Infrastructure.Middleware;
using DavidBrowning.Infrastructure.Options;
using DavidBrowning.Infrastructure.Rendering;
using DavidBrowning.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DavidBrowning.Infrastructure;
public static class ServiceCollectionExtensions
{
   public static IServiceCollection AddDavidBrowningInfrastructure(
      this IServiceCollection services,
      IConfiguration configuration,
      IHostEnvironment environment)
   {
      services.AddDavidBrowningCommonOptions(configuration);
      services.AddDavidBrowningCommonServices(configuration);
      services.AddDavidBrowningDatabases(configuration, environment);
      services.AddDavidBrowningStores(configuration);
      services.AddDavidBrowningLookupServices(configuration);
      services.AddDavidBrowningContent(configuration);
      return services;
   }

   public static WebApplication ConfigureErrorHandling(
      this WebApplication app,
      IConfiguration configuration)
   {
      if (app.Environment.IsDevelopment())
      {
         app.UseDeveloperExceptionPage();
         if (configuration.GetValue<bool>(ConfigurationHelpers.EnableCustomErrorsKey))
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
      return app;
   }

   private static IServiceCollection AddDavidBrowningCommonOptions(
      this IServiceCollection services,
      IConfiguration configuration)
   {
      services.Configure<DiagnosticsOptions>(
         configuration.GetSection(ConfigurationHelpers.DiagnosticsSectionName));

      services.Configure<JsonCacheOptions>(
         configuration.GetSection($"{ConfigurationHelpers.CacheSectionName}:JsonCache"));

      services.Configure<RenderedContentCacheOptions>(
         configuration.GetSection(
            $"{ConfigurationHelpers.CacheSectionName}:RenderedContentCache"));

      services.Configure<SlugCacheOptions>(
         configuration.GetSection($"{ConfigurationHelpers.CacheSectionName}:SlugCache"));

      services.Configure<DateTimeDisplayOptions>(
         configuration.GetSection("DateTimeDisplayOptions"));

      services.Configure<SiteMetadataOptions>(
         configuration.GetSection("MetadataOptions"));

      return services;

   }

   private static IServiceCollection AddDavidBrowningCommonServices(
      this IServiceCollection services,
      IConfiguration configuration)
   {
      // Basic Services
      services.AddMemoryCache();
      services.AddSingleton<UrlBuilder>();
      services.AddSingleton<TimezoneConverter>();
      services.AddSingleton<StructuredDataBuilder>();
      services.AddSingleton<ISlugService, BasicSlugService>();

      // These services are used to estimate the size of objects for use in
      // caching.
      services.AddSingleton(typeof(ICacheSizeEstimator<>), typeof(DefaultCacheSizeEstimator<>));
      services.AddSingleton(typeof(ICacheSizeEstimator<string>), typeof(StringSizeEstimator));
      services.AddSingleton<ICacheSizeEstimator<byte[]>, ByteArraySizeEstimator>();
      services.AddSingleton<ICacheSizeEstimator<RenderedContent>, RenderedContentSizeEstimator>();
      services.AddSingleton<ICacheSizeEstimator<object>, SingleObjectSizeEstimator<object>>();

      // Add specialized caching services
      services.AddSingleton<JsonCache>();
      services.AddSingleton<JsonMemoryCache>();
      services.AddSingleton<RenderedContentMemoryCache>();
      services.AddSingleton(typeof(SlugMemoryCache<>));

      // Configure how to get content
      string contentStoreProvider = GetConfiguredStoreProvider(
         configuration, ConfigurationHelpers.ContentStoreName, ConfigurationHelpers.DummyProviderName);
      if (contentStoreProvider.EqualsOrdinalIgnoreCase(ConfigurationHelpers.LocalProviderName))
      {
         services.AddSingleton<IContentStore, LocalContentStore>();
      }
      else if (contentStoreProvider.EqualsOrdinalIgnoreCase(ConfigurationHelpers.AzureStorageBlobsProviderName))
      {
         services.AddSingleton<IContentStore, AzureBlobContentStore>();
      }
      else
      {
         throw new InvalidOperationException(
            $"Unknown content store provider: {contentStoreProvider}");
      }

      // Add the basic renderer.
      services.AddSingleton<IContentRenderer, BasicContentRenderer>();

      // Configure the rendering pipeline depending on whether caching is 
      // enabled.
      var enableCache = configuration.GetValue<bool>($"{ConfigurationHelpers.CacheSectionName}:EnableContentCache");
      if (enableCache)
      {
         services.AddSingleton<BasicContentPipeline>();
         services.AddSingleton<IContentPipeline>(serviceProvider =>
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
         services.AddSingleton<IContentPipeline, BasicContentPipeline>();
      }

      services.AddSingleton<MarkdownDocumentRenderer>();
      services.AddSingleton(serviceProvider =>
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

      services.AddSingleton<MarkdownPostContentRenderer>();
      services.AddSingleton<MarkdownProjectContentRenderer>();
      return services;
   }

   private static IServiceCollection AddDavidBrowningDatabases(
      this IServiceCollection services,
      IConfiguration configuration,
      IHostEnvironment environment)
   {
      string databaseProvider =
         configuration[ConfigurationHelpers.DatabaseProviderKey] ?? ConfigurationHelpers.SqlServerProviderName;
      bool enableSensitiveDataLogging =
         configuration.GetValue<bool>(ConfigurationHelpers.EnableSensitiveDataLoggingKey);
      bool enableDetailedErrors =
         configuration.GetValue<bool>(ConfigurationHelpers.EnableDetailedErrorsKey);
      bool enableSqlCommandLogging =
         configuration.GetValue<bool>(ConfigurationHelpers.EnableSqlCommandLoggingKey);

      services.AddDbContext<SiteDbContext>(options =>
      {
         if (databaseProvider.EqualsOrdinalIgnoreCase(ConfigurationHelpers.SqlServerProviderName))
         {
            string? siteDatabaseConnectionString =
               configuration.GetConnectionString(
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
            string databaseName = configuration[ConfigurationHelpers.InMemoryDatabaseNameKey] ?? ConfigurationHelpers.DefaultInMemoryDatabaseName;

            options.UseInMemoryDatabase(databaseName);
         }
         else
         {
            throw new InvalidOperationException(
               $"Unsupported database provider: {databaseProvider}");
         }

         ConfigureEntityFrameworkDiagnostics(
            environment,
            options,
            enableSensitiveDataLogging,
            enableDetailedErrors,
            enableSqlCommandLogging);
      });
      return services;
   }

   private static void ConfigureEntityFrameworkDiagnostics(
      IHostEnvironment environment,
      DbContextOptionsBuilder options,
      bool enableSensitiveDataLogging,
      bool enableDetailedErrors,
      bool enableSqlCommandLogging)
   {
      if (environment.IsDevelopment())
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

   private static IServiceCollection AddDavidBrowningLookupServices(
      this IServiceCollection services,
      IConfiguration configuration)
   {
      string lookupProvider = GetConfiguredStoreProvider(
         configuration, ConfigurationHelpers.LookupStoreName, ConfigurationHelpers.DummyProviderName);

      if (lookupProvider.EqualsOrdinalIgnoreCase(ConfigurationHelpers.SqlServerProviderName))
      {
         services.AddScoped(typeof(ISlugLookupService<>), typeof(SlugLookupCache<>));
         return services;
      }

      throw new InvalidOperationException(
         $"Unknown lookup store provider: {lookupProvider}");
   }

   private static IServiceCollection AddDavidBrowningStores(
      this IServiceCollection services,
      IConfiguration configuration)
   {
      services.ConfigureErrorStore(configuration);
      services.ConfigureWritingStore(configuration);
      services.ConfigureProjectStore(configuration);
      services.ConfigureWorkStore(configuration);
      services.ConfigureUncategorizedStore(configuration);
      return services;
   }

   private static IServiceCollection ConfigureErrorStore(
      this IServiceCollection services,
      IConfiguration configuration)
   {
      string provider = GetConfiguredStoreProvider(
         configuration,
         ConfigurationHelpers.ErrorStoreName,
         ConfigurationHelpers.DummyProviderName);
      if (provider.EqualsOrdinalIgnoreCase(ConfigurationHelpers.SqlServerProviderName))
      {
         services.AddScoped<IErrorStore, SqlErrorStore>();
         return services;
      }

      throw new InvalidOperationException(
         $"Unknown error store provider: {provider}");
   }

   private static IServiceCollection ConfigureWritingStore(
      this IServiceCollection services,
      IConfiguration configuration)
   {
      string provider = GetConfiguredStoreProvider(
         configuration,
         ConfigurationHelpers.WritingStoreName,
         ConfigurationHelpers.DummyProviderName);
      if (provider.EqualsOrdinalIgnoreCase(ConfigurationHelpers.SqlServerProviderName))
      {
         services.AddScoped<IWritingStore, SqlWritingStore>();
         return services;
      }

      throw new InvalidOperationException(
         $"Unknown writing store provider: {provider}");
   }

   private static IServiceCollection ConfigureProjectStore(
      this IServiceCollection services,
      IConfiguration configuration)
   {
      string provider = GetConfiguredStoreProvider(
         configuration,
         ConfigurationHelpers.ProjectStoreName,
         ConfigurationHelpers.DummyProviderName);
      if (provider.EqualsOrdinalIgnoreCase(
         ConfigurationHelpers.SqlServerProviderName))
      {
         services.AddScoped<IProjectStore, SqlProjectStore>();
         return services;
      }

      throw new InvalidOperationException(
         $"Unknown project store provider: {provider}");
   }

   private static IServiceCollection ConfigureWorkStore(
      this IServiceCollection services,
      IConfiguration configuration)
   {
      services.AddScoped<IWorkStore, SqlWorkStore>();
      return services;
   }

   private static IServiceCollection ConfigureUncategorizedStore(
      this IServiceCollection services,
      IConfiguration configuration)
   {
      services.AddScoped<IUncategorizedStore, SqlUncategorizedStore>();
      return services;
   }

   private static IServiceCollection AddDavidBrowningContent(
      this IServiceCollection services,
      IConfiguration configuration)
   {
      // These services are used to estimate the size of objects for use in
      // caching.
      services.AddSingleton(typeof(ICacheSizeEstimator<>), typeof(DefaultCacheSizeEstimator<>));
      services.AddSingleton(typeof(ICacheSizeEstimator<string>), typeof(StringSizeEstimator));
      services.AddSingleton<ICacheSizeEstimator<byte[]>, ByteArraySizeEstimator>();
      services.AddSingleton<ICacheSizeEstimator<RenderedContent>, RenderedContentSizeEstimator>();

      services.AddSingleton<ICacheSizeEstimator<object>, SingleObjectSizeEstimator<object>>();

      // Add specialized caching services
      services.AddSingleton<JsonCache>();
      services.AddSingleton<JsonMemoryCache>();
      services.AddSingleton<RenderedContentMemoryCache>();
      services.AddSingleton(typeof(SlugMemoryCache<>));

      // Configure how to get content
      string contentStoreProvider = GetConfiguredStoreProvider(
         configuration, ConfigurationHelpers.ContentStoreName, ConfigurationHelpers.DummyProviderName);
      if (contentStoreProvider.EqualsOrdinalIgnoreCase(ConfigurationHelpers.LocalProviderName))
      {
         services.AddSingleton<IContentStore, LocalContentStore>();
      }
      else if (contentStoreProvider.EqualsOrdinalIgnoreCase(ConfigurationHelpers.AzureStorageBlobsProviderName))
      {
         services.AddSingleton<IContentStore, AzureBlobContentStore>();
      }
      else
      {
         throw new InvalidOperationException(
            $"Unknown content store provider: {contentStoreProvider}");
      }

      // Add the basic renderer.
      services.AddSingleton<IContentRenderer, BasicContentRenderer>();

      // Configure the rendering pipeline depending on whether caching is 
      // enabled.
      var enableCache = configuration.GetValue<bool>(
         $"{ConfigurationHelpers.CacheSectionName}:EnableContentCache");
      if (enableCache)
      {
         services.AddSingleton<BasicContentPipeline>();
         services.AddSingleton<IContentPipeline>(serviceProvider =>
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
         services.AddSingleton<IContentPipeline, BasicContentPipeline>();
      }

      services.AddSingleton<MarkdownDocumentRenderer>();
      services.AddSingleton(
         serviceProvider =>
         {
            IMarkdownDocumentRenderer innerRenderer = serviceProvider.GetRequiredService<MarkdownDocumentRenderer>();
            if (!enableCache)
            {
               return innerRenderer;
            }

            return new CachedMarkdownDocumentRenderer(
               innerRenderer, serviceProvider.GetRequiredService<RenderedContentMemoryCache>());
         });

      services.AddSingleton<MarkdownPostContentRenderer>();
      services.AddSingleton<MarkdownProjectContentRenderer>();
      return services;
   }

   private static string GetConfiguredStoreProvider(
      IConfiguration configuration,
      string storeName,
      string defaultProviderName)
   {
      string key = GetStoreProviderKey(storeName);
      return configuration[key] ?? defaultProviderName;
   }

   private static string GetStoreProviderKey(string storeName)
   {
      return $"{ConfigurationHelpers.StoresSectionName}:{storeName}:{ConfigurationHelpers.ProviderSectionName}";
   }
}
