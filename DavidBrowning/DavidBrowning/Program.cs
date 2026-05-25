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
using DavidBrowning.Data.Stores.Writing;
using DavidBrowning.Diagnostics;
using DavidBrowning.Middleware;
using DavidBrowning.Services;
using DavidBrowning.Services.Assets;
using DavidBrowning.Services.Cache;
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

namespace DavidBrowning
{
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
            builder.Configuration.GetSection("Diagnostics"));
      }

      private static void ConfigureSecrets(WebApplicationBuilder builder)
      {
         string secretsProvider =
            builder.Configuration["Secrets:Provider"] ?? "Local";

         if (string.Equals(secretsProvider, "Local", StringComparison.OrdinalIgnoreCase))
         {
            // Do nothing.
            //
            // In Development, WebApplication.CreateBuilder already loads User Secrets
            // when the project has a UserSecretsId.
            //
            // For local non-Development environments, prefer environment variables
            // or a machine-local file that is not committed.
            return;
         }

         if (string.Equals(secretsProvider, "AzureKeyVault", StringComparison.OrdinalIgnoreCase))
         {
            string? keyVaultUriText = builder.Configuration["KeyVault:Uri"];
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
         builder.Services.Configure<LookupCacheOptions>(
            builder.Configuration.GetSection("LookupCache"));

         builder.Services.AddMemoryCache();
         builder.Services.AddSingleton<ISystemClock, SystemClock>();
         builder.Services.AddSingleton(typeof(ISlugService), typeof(BasicSlugService));
         builder.Services.AddSingleton(typeof(UrlBuilder));

         builder.Services.AddSingleton<ILookupCache, BasicLookupCache>();
         builder.Services.AddScoped(
            typeof(ISlugLookupService<>),
            typeof(SlugLookupService<>));

         var contentStoreProvider = builder.Configuration[
            "Stores:ContentStore:Provider"] ?? _dummyProviderName;
         if (string.Equals(contentStoreProvider, _dummyProviderName, StringComparison.OrdinalIgnoreCase))
         {
            builder.Services.AddSingleton<IContentService, DummyContentService>();
         }
         else if(string.Equals(contentStoreProvider, "Local", StringComparison.OrdinalIgnoreCase))
         {
            builder.Services.AddSingleton<IContentService, LocalContentService>();
         }
         else
         {
            throw new InvalidOperationException(
               $"Unknown content store provider: {contentStoreProvider}");
         }

         builder.Services.AddScoped<IContentRenderer, BasicContentRenderer>();
            
         builder.Services.AddControllersWithViews();
      }

      private static void ConfigureDatabase(WebApplicationBuilder builder)
      {
         var databaseProvider =
            builder.Configuration["Database:Provider"] ?? _sqlServerProviderName;

         var enableSensitiveDataLogging =
            builder.Configuration.GetValue<bool>(
               "Diagnostics:EntityFrameworkOptions:EnableSensitiveDataLogging");
         var enableDetailedErrors =
            builder.Configuration.GetValue<bool>(
               "Diagnostics:EntityFrameworkOptions:EnableDetailedErrors");
         var enableSqlCommandLogging =
            builder.Configuration.GetValue<bool>(
               "Diagnostics:EntityFrameworkOptions:EnableSqlCommandLogging");

         builder.Services.AddDbContext<SiteDbContext>(options =>
         {
            if (string.Equals(databaseProvider, _sqlServerProviderName, StringComparison.OrdinalIgnoreCase))
            {
               var siteDatabaseConnectionString =
                  builder.Configuration.GetConnectionString("SiteDatabase");

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
            else if (string.Equals(databaseProvider, _inMemoryProviderName, StringComparison.OrdinalIgnoreCase))
            {
               var databaseName =
                  builder.Configuration["Database:InMemoryDatabaseName"] ??
                  "DavidBrowning";

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
                  new[]
                  {
               DbLoggerCategory.Database.Command.Name
                  },
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

      private static void ConfigureLookupServices(WebApplicationBuilder builder)
      {
         builder.Services.Configure<LookupCacheOptions>(
            builder.Configuration.GetSection("LookupCache"));

         builder.Services.AddSingleton<ILookupCache, BasicLookupCache>();

         string lookupProvider =
            builder.Configuration["Stores:LookupStore:Provider"] ?? _dummyProviderName;
         if (string.Equals(lookupProvider, _sqlServerProviderName, StringComparison.OrdinalIgnoreCase))
         {
            builder.Services.AddScoped(
               typeof(ISlugLookupService<>),
               typeof(SlugLookupService<>));

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
      }

      private static void ConfigureErrorStore(WebApplicationBuilder builder)
      {
         string provider =
            builder.Configuration["Stores:ErrorStore:Provider"] ?? _dummyProviderName;
         if (string.Equals(provider, _sqlServerProviderName, StringComparison.OrdinalIgnoreCase))
         {
            builder.Services.AddScoped<IErrorStore, SqlErrorStore>();
            return;
         }

         throw new InvalidOperationException(
            $"Unknown error store provider: {provider}");
      }

      private static void ConfigureWritingStore(WebApplicationBuilder builder)
      {
         string provider =
            builder.Configuration["Stores:WritingStore:Provider"] ?? _dummyProviderName;
         if (string.Equals(provider, _sqlServerProviderName, StringComparison.OrdinalIgnoreCase))
         {
            builder.Services.AddScoped<IWritingStore, SqlWritingStore>();
            return;
         }

         throw new InvalidOperationException(
            $"Unknown writing store provider: {provider}");
      }

      private static void ConfigureProjectStore(WebApplicationBuilder builder)
      {
         string provider =
            builder.Configuration["Stores:ProjectStore:Provider"] ?? _dummyProviderName;
         if (string.Equals(provider, _sqlServerProviderName, StringComparison.OrdinalIgnoreCase))
         {
            builder.Services.AddScoped<IProjectStore, SqlProjectStore>();
            return;
         }

         throw new InvalidOperationException(
            $"Unknown project store provider: {provider}");
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
            if (config.GetValue<bool>("Diagnostics:EnableCustomerErrors"))
            {
               app.UseExceptionHandler("/Error/StatusCode/500");
            }
         }
         else
         {
            app.UseExceptionHandler("/Error/StatusCode/500");
            app.UseHsts();
         }

         app.UseMiddleware<ErrorLoggingMiddleware>();
         app.UseStatusCodePagesWithReExecute("/Error/StatusCode/{0}");

         app.UseHttpsRedirection();
         app.UseStaticFiles();
         app.UseRouting();

         app.UseAuthentication();
         app.UseAuthorization();

         app.MapControllerRoute(
             name: "default",
             pattern: "{controller=Home}/{action=Index}/{id?}");
      }

      private static async Task SeedDatabaseAsync(WebApplication app)
      {
         using var scope = app.Services.CreateScope();

         var configuration =
            scope.ServiceProvider.GetRequiredService<IConfiguration>();

         // Just create a logger because we cannot get loggers for static 
         // classes.
         var loggerFactory =
            scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
         var logger =
            loggerFactory.CreateLogger("Startup");

         var seedEnabled =
            configuration.GetValue<bool>("Database:Seed:Enabled");

         if (!seedEnabled)
         {
            logger.LogInformation("Database seeding is disabled.");
            return;
         }

         var seedRootFolder = configuration["Database:Seed:RootFolder"];
         var environment =
            scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
         var dbContext =
            scope.ServiceProvider.GetRequiredService<SiteDbContext>();
         if (string.IsNullOrWhiteSpace(seedRootFolder))
         {
            seedRootFolder = Path.Combine(
               environment.ContentRootPath,
               "Data",
               "Seed");
         }

         var tablePrefix = configuration["Database:TablePrefix"] ?? "db_";

         JsonSeedDatabaseSeederOptions seedOptions = new()
         {
            TablePrefix = tablePrefix,
            SkipFileWhenTargetTableHasRows =
               configuration.GetValue(
                  "Database:Seed:SkipFileWhenTargetTableHasRows",
                  true),
            ThrowOnUnmatchedJsonFiles =
               configuration.GetValue(
                  "Database:Seed:ThrowOnUnmatchedJsonFiles",
                  true),
            UseSqlServerIdentityInsertWhenNeeded =
               configuration.GetValue(
                  "Database:Seed:UseSqlServerIdentityInsertWhenNeeded",
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

      private const string _dummyProviderName = "Dummy";
      private const string _sqlServerProviderName = "SqlServer";
      private const string _inMemoryProviderName = "InMemory";
   }
}