// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

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
      services.AddDavidBrowningCommonServices();
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
          configuration.GetSection($"{ConfigurationHelpers.CacheSectionName}:RenderedContentCache"));

      services.Configure<SlugCacheOptions>(
          configuration.GetSection($"{ConfigurationHelpers.CacheSectionName}:SlugCache"));

      services.Configure<DateTimeDisplayOptions>(
          configuration.GetSection("DateTimeDisplayOptions"));

      services.Configure<SiteMetadataOptions>(
          configuration.GetSection("MetadataOptions"));

      return services;
   }

   private static IServiceCollection AddDavidBrowningCommonServices(
       this IServiceCollection services)
   {
      services.AddMemoryCache();
      services.AddSingleton<UrlBuilder>();
      services.AddSingleton<TimezoneConverter>();
      services.AddSingleton<StructuredDataBuilder>();
      services.AddSingleton<ISlugService, BasicSlugService>();

      return services;
   }

   private static IServiceCollection AddDavidBrowningDatabases(
       this IServiceCollection services,
       IConfiguration configuration,
       IHostEnvironment environment)
   {
      string databaseProvider =
          configuration[ConfigurationHelpers.DatabaseProviderKey] ??
          ConfigurationHelpers.SqlServerProviderName;

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
            string siteDatabaseConnectionName =
                configuration[ConfigurationHelpers.DatabaseConnectionNameKey] ??
                ConfigurationHelpers.DefaultSiteDatabaseConnectionName;

            string? siteDatabaseConnectionString =
                configuration.GetConnectionString(siteDatabaseConnectionName);

            if (string.IsNullOrWhiteSpace(siteDatabaseConnectionString))
            {
               throw new InvalidOperationException(
                   $"Missing connection string: ConnectionStrings:{siteDatabaseConnectionName}. " +
                   $"Set {ConfigurationHelpers.DatabaseConnectionNameKey} to the name of a configured connection string.");
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
                configuration[ConfigurationHelpers.InMemoryDatabaseNameKey] ??
                ConfigurationHelpers.DefaultInMemoryDatabaseName;

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
             "EnableSensitiveDataLogging must not be enabled outside Development.");
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

   private static IServiceCollection AddDavidBrowningStores(
       this IServiceCollection services,
       IConfiguration configuration)
   {
      services.AddConfiguredScopedStore<IErrorStore, SqlErrorStore>(
          configuration,
          ConfigurationHelpers.ErrorStoreName,
          ConfigurationHelpers.SqlServerProviderName);

      services.AddConfiguredScopedStore<IWritingStore, SqlWritingStore>(
          configuration,
          ConfigurationHelpers.WritingStoreName,
          ConfigurationHelpers.SqlServerProviderName);

      services.AddConfiguredScopedStore<IProjectStore, SqlProjectStore>(
          configuration,
          ConfigurationHelpers.ProjectStoreName,
          ConfigurationHelpers.SqlServerProviderName);

      services.AddScoped<IWorkStore, SqlWorkStore>();
      services.AddScoped<IUncategorizedStore, SqlUncategorizedStore>();

      return services;
   }

   private static IServiceCollection AddConfiguredScopedStore<TService, TImplementation>(
       this IServiceCollection services,
       IConfiguration configuration,
       string storeName,
       string supportedProviderName)
       where TService : class
       where TImplementation : class, TService
   {
      string provider = GetConfiguredStoreProvider(
          configuration,
          storeName,
          ConfigurationHelpers.DummyProviderName);

      if (!provider.EqualsOrdinalIgnoreCase(supportedProviderName))
      {
         throw new InvalidOperationException(
             $"Unknown {storeName} provider: {provider}");
      }

      services.AddScoped<TService, TImplementation>();

      return services;
   }

   private static IServiceCollection AddDavidBrowningLookupServices(
       this IServiceCollection services,
       IConfiguration configuration)
   {
      string lookupProvider = GetConfiguredStoreProvider(
          configuration,
          ConfigurationHelpers.LookupStoreName,
          ConfigurationHelpers.DummyProviderName);

      if (lookupProvider.EqualsOrdinalIgnoreCase(ConfigurationHelpers.SqlServerProviderName))
      {
         services.AddScoped(typeof(ISlugLookupService<>), typeof(SlugLookupCache<>));
         return services;
      }

      throw new InvalidOperationException(
          $"Unknown lookup store provider: {lookupProvider}");
   }

   private static IServiceCollection AddDavidBrowningContent(
       this IServiceCollection services,
       IConfiguration configuration)
   {
      services.AddContentCacheServices();
      services.AddContentStore(configuration);
      services.AddContentRenderingServices(configuration);

      return services;
   }

   private static IServiceCollection AddContentCacheServices(
       this IServiceCollection services)
   {
      services.AddSingleton(typeof(ICacheSizeEstimator<>), typeof(DefaultCacheSizeEstimator<>));
      services.AddSingleton<ICacheSizeEstimator<string>, StringSizeEstimator>();
      services.AddSingleton<ICacheSizeEstimator<byte[]>, ByteArraySizeEstimator>();
      services.AddSingleton<ICacheSizeEstimator<RenderedContent>, RenderedContentSizeEstimator>();
      services.AddSingleton<ICacheSizeEstimator<object>, SingleObjectSizeEstimator<object>>();

      services.AddSingleton<JsonCache>();
      services.AddSingleton<JsonMemoryCache>();
      services.AddSingleton<RenderedContentMemoryCache>();
      services.AddSingleton(typeof(SlugMemoryCache<>));

      return services;
   }

   private static IServiceCollection AddContentStore(
       this IServiceCollection services,
       IConfiguration configuration)
   {
      string contentStoreProvider = GetConfiguredStoreProvider(
          configuration,
          ConfigurationHelpers.ContentStoreName,
          ConfigurationHelpers.DummyProviderName);

      if (contentStoreProvider.EqualsOrdinalIgnoreCase(ConfigurationHelpers.LocalProviderName))
      {
         services.AddSingleton<IContentStore, LocalContentStore>();
         return services;
      }

      if (contentStoreProvider.EqualsOrdinalIgnoreCase(ConfigurationHelpers.AzureStorageBlobsProviderName))
      {
         services.ConfigureAzureBlobContentStoreOptions(configuration);
         services.AddSingleton<IContentStore, AzureBlobContentStore>();
         return services;
      }

      throw new InvalidOperationException(
          $"Unknown content store provider: {contentStoreProvider}");
   }

   private static IServiceCollection ConfigureAzureBlobContentStoreOptions(
       this IServiceCollection services,
       IConfiguration configuration)
   {
      services.Configure<AzureBlobContentStoreOptions>(options =>
      {
         IConfigurationSection section = configuration.GetSection(
             $"{ConfigurationHelpers.StoresSectionName}:" +
             $"{ConfigurationHelpers.ContentStoreName}:" +
             $"{ConfigurationHelpers.AzureStorageBlobsProviderName}");

         section.Bind(options);

         string? connectionName = options.ConnectionName;

         if (string.IsNullOrWhiteSpace(connectionName) &&
             !string.IsNullOrWhiteSpace(options.ConnectionString))
         {
            string? legacyNamedConnectionString =
                configuration.GetConnectionString(options.ConnectionString);

            if (!string.IsNullOrWhiteSpace(legacyNamedConnectionString))
            {
               connectionName = options.ConnectionString;
               options.ConnectionString = legacyNamedConnectionString;
            }
         }

         if (string.IsNullOrWhiteSpace(connectionName) &&
             string.IsNullOrWhiteSpace(options.ConnectionString))
         {
            connectionName = ConfigurationHelpers.DefaultContentStorageConnectionName;
         }

         if (!string.IsNullOrWhiteSpace(connectionName))
         {
            string? namedConnectionString = configuration.GetConnectionString(connectionName);

            if (!string.IsNullOrWhiteSpace(namedConnectionString))
            {
               options.ConnectionString = namedConnectionString;
            }
         }

         if (string.IsNullOrWhiteSpace(options.ConnectionString))
         {
            string configuredConnectionName =
                string.IsNullOrWhiteSpace(connectionName)
                    ? ConfigurationHelpers.DefaultContentStorageConnectionName
                    : connectionName;

            throw new InvalidOperationException(
                $"Missing content storage connection string. Set " +
                $"ConnectionStrings:{configuredConnectionName}, or set " +
                $"{ConfigurationHelpers.ContentStorageConnectionNameKey} to a configured connection string name.");
         }

         if (string.IsNullOrWhiteSpace(options.ContainerName))
         {
            throw new InvalidOperationException(
                "Missing Azure Blob content container name: " +
                "Stores:ContentStore:AzureStorageBlobs:ContainerName.");
         }
      });

      return services;
   }

   private static IServiceCollection AddContentRenderingServices(
       this IServiceCollection services,
       IConfiguration configuration)
   {
      services.AddSingleton<IContentRenderer, BasicContentRenderer>();
      services.AddContentPipeline(configuration);
      services.AddMarkdownRenderers(configuration);

      return services;
   }

   private static IServiceCollection AddContentPipeline(
       this IServiceCollection services,
       IConfiguration configuration)
   {
      bool enableCache =
          configuration.GetValue<bool>($"{ConfigurationHelpers.CacheSectionName}:EnableContentCache");

      if (!enableCache)
      {
         services.AddSingleton<IContentPipeline, BasicContentPipeline>();
         return services;
      }

      services.AddSingleton<BasicContentPipeline>();

      services.AddSingleton<IContentPipeline>(serviceProvider =>
      {
         IContentPipeline innerPipeline =
             serviceProvider.GetRequiredService<BasicContentPipeline>();

         RenderedContentMemoryCache memoryCache =
             serviceProvider.GetRequiredService<RenderedContentMemoryCache>();

         return new CachedContentPipeline(innerPipeline, memoryCache);
      });

      return services;
   }

   private static IServiceCollection AddMarkdownRenderers(
       this IServiceCollection services,
       IConfiguration configuration)
   {
      bool enableCache =
          configuration.GetValue<bool>($"{ConfigurationHelpers.CacheSectionName}:EnableContentCache");

      services.AddSingleton<MarkdownDocumentRenderer>();

      services.AddSingleton<IMarkdownDocumentRenderer>(serviceProvider =>
      {
         IMarkdownDocumentRenderer innerRenderer =
             serviceProvider.GetRequiredService<MarkdownDocumentRenderer>();

         if (!enableCache)
         {
            return innerRenderer;
         }

         return new CachedMarkdownDocumentRenderer(
             innerRenderer,
             serviceProvider.GetRequiredService<RenderedContentMemoryCache>());
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