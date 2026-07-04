// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using System;
using Azure;
using Azure.Core;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
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
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// Azure also has a DiagnosticsOptions. Use ours.
using DiagnosticsOptions = DavidBrowning.Diagnostics.DiagnosticsOptions;

namespace DavidBrowning.Infrastructure;

public static class ServiceCollectionExtensions
{
   public static IServiceCollection AddDavidBrowningInfrastructure(
       this IServiceCollection services,
       ConfigurationManager configuration,
       IHostEnvironment environment)
   {
      services.AddDavidBrowningCommonOptions(configuration);
      services.AddDavidBrowningSecrets(configuration, environment);
      services.AddDavidBrowningCommonServices(environment);
      services.AddDavidBrowningDatabases(configuration, environment);
      services.AddDavidBrowningStores(configuration);
      services.AddDavidBrowningLookupServices(configuration, environment);
      services.AddDavidBrowningContent(configuration,environment);
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

      services.Configure<WarmupOptions>(
         configuration.GetSection("Diagnostics:Warmup"));

      return services;
   }

   private static IServiceCollection AddDavidBrowningCommonServices(
       this IServiceCollection services,
       IHostEnvironment environment)
   {
      services.AddMemoryCache();
      services.AddSingleton<UrlBuilder>();
      services.AddSingleton<TimezoneConverter>();
      services.AddSingleton<StructuredDataBuilder>();
      services.AddSingleton<ISlugService, BasicSlugService>();
      services.AddSingleton(_ => CreateAzureCredential(environment));

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

      var connectTimeoutSeconds =
         configuration.GetValue<int?>(ConfigurationHelpers.SqlConnectTimeoutSeconds) ?? 15;
      var connectRetryDelaySeconds =
         configuration.GetValue<int?>(ConfigurationHelpers.SqlConnectRetryDelay) ?? 5;
      var retryCount =
         configuration.GetValue<int?>(ConfigurationHelpers.SqlConnectMaxRetry) ?? 5;
      var commandTimeoutSeconds =
         configuration.GetValue<int?>(ConfigurationHelpers.SqlCommandTimeoutSeconds) ?? 30;

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

            SqlConnectionStringBuilder connectionStringBuilder = new(
               siteDatabaseConnectionString)
            {
               ConnectTimeout = connectTimeoutSeconds,
            };

            options.UseSqlServer(
               connectionStringBuilder.ConnectionString,
               sqlOptions =>
               {
                  sqlOptions.CommandTimeout(commandTimeoutSeconds);

                  if (retryCount > 0)
                  {
                     sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: retryCount,
                        maxRetryDelay: TimeSpan.FromSeconds(connectRetryDelaySeconds),
                        errorNumbersToAdd: null);
                  }
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
       IConfiguration configuration,
       IHostEnvironment environment)
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
       IConfiguration configuration,
       IHostEnvironment environment)
   {
      services.AddContentCacheServices(environment);
      services.AddContentStore(configuration, environment);
      services.AddContentRenderingServices(configuration);

      return services;
   }

   private static IServiceCollection AddContentCacheServices(
       this IServiceCollection services,
       IHostEnvironment environment)
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
       IConfiguration configuration,
       IHostEnvironment environment)
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

         if (string.IsNullOrWhiteSpace(options.ServiceUri))
         {
            throw new InvalidOperationException(
               "Missing Azure Blob service URI. Set " +
               "Stores:ContentStore:AzureStorageBlobs:ServiceUri.");
         }

         if (!Uri.TryCreate(options.ServiceUri, UriKind.Absolute, out _))
         {
            throw new InvalidOperationException(
               "Azure Blob service URI must be a valid absolute URI. Set " +
               "Stores:ContentStore:AzureStorageBlobs:ServiceUri.");
         }

         if (string.IsNullOrWhiteSpace(options.ContainerName))
         {
            throw new InvalidOperationException(
               "Missing Azure Blob container name. Set " +
               "Stores:ContentStore:AzureStorageBlobs:ContainerName.");
         }
      });

      return services;
   }

   private static IServiceCollection AddDavidBrowningSecrets(
      this IServiceCollection services,
      ConfigurationManager configuration,
      IHostEnvironment environment)
   {
      string secretsProvider =
         configuration[ConfigurationHelpers.SecretsProviderKey] ??
         ConfigurationHelpers.LocalProviderName;

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
         return services;
      }

      if (secretsProvider.EqualsOrdinalIgnoreCase(ConfigurationHelpers.AzureKeyVaultProviderName))
      {
         AddWebsiteKeyVault(configuration, environment);

         return services;
      }

      throw new InvalidOperationException(
         $"Unknown secrets provider: {secretsProvider}");
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

   private static void AddWebsiteKeyVault(
      ConfigurationManager configuration,
      IHostEnvironment environment)
   {
      string? keyVaultUriText = configuration["KeyVault:VaultUri"];

      if (string.IsNullOrWhiteSpace(keyVaultUriText))
      {
         throw new InvalidOperationException(
            "Secrets:Provider is AzureKeyVault, but KeyVault:VaultUri is missing.");
      }

      if (!Uri.TryCreate(keyVaultUriText, UriKind.Absolute, out Uri? keyVaultUri))
      {
         throw new InvalidOperationException(
            $"Secrets:Provider is AzureKeyVault, but KeyVault:VaultUri is not a valid absolute URI: '{keyVaultUriText}'.");
      }

      try
      {
         AzureKeyVaultConfigurationOptions options = new()
         {
            ReloadInterval = TimeSpan.FromMinutes(60)
         };

         configuration.AddAzureKeyVault(
            keyVaultUri, CreateAzureCredential(environment), options);
      }
      catch (CredentialUnavailableException exception)
      {
         throw new InvalidOperationException(
            "Azure Key Vault authentication is unavailable. For local development, sign in with Azure CLI using 'az login' or sign in through Visual Studio.",
            exception);
      }
      catch (AuthenticationFailedException exception)
      {
         throw new InvalidOperationException(
            "Azure Key Vault authentication failed. Check that your local Azure account is signed in, using the correct tenant, and has access to the configured vault.",
            exception);
      }
      catch (RequestFailedException exception) when (exception.Status == 403)
      {
         throw new InvalidOperationException(
            "Azure Key Vault rejected the request with 403 Forbidden. The signed-in identity probably lacks secret read/list permissions, or the vault firewall is blocking this machine.",
            exception);
      }
      catch (RequestFailedException exception) when (exception.Status == 404)
      {
         throw new InvalidOperationException(
            $"Azure Key Vault returned 404 Not Found. Check the vault URI: '{keyVaultUri}'.",
            exception);
      }
      catch (RequestFailedException exception)
      {
         throw new InvalidOperationException(
            $"Azure Key Vault could not be loaded. Status={exception.Status}, ErrorCode={exception.ErrorCode}, Message={exception.Message}",
            exception);
      }
   }

   private static TokenCredential CreateAzureCredential(
      IHostEnvironment environment)
   {
      if (environment.IsProduction())
      {
         return new ManagedIdentityCredential(ManagedIdentityId.SystemAssigned);
      }

      return new DefaultAzureCredential(new DefaultAzureCredentialOptions()
      {
         ExcludeEnvironmentCredential = true,
         ExcludeWorkloadIdentityCredential = true,
         ExcludeManagedIdentityCredential = true,
      });
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