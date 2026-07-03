// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Identity;
using DavidBrowning.Helpers;
using DavidBrowning.Infrastructure;
using DavidBrowning.Infrastructure.Data;
using DavidBrowning.Infrastructure.Seo;
using DavidBrowning.Web.Data.Seeding;
using DavidBrowning.Web.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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

      builder.Logging.ClearProviders();
      builder.Logging.AddConsole();
      builder.Logging.AddDebug();

      ConfigureSecrets(builder);

      builder.Services.AddDavidBrowningInfrastructure(
         builder.Configuration,
         builder.Environment);

      builder.Services.AddSingleton<SitemapBuilder>();
      //builder.Services.AddAdminAuthoringServices();
      builder.Services.AddControllersWithViews();

      var app = builder.Build();

      app.ConfigureErrorHandling(builder.Configuration);

      app.UseHttpsRedirection();
      app.UseStaticFiles();
      app.UseMiddleware<DatabaseWarmupRedirectMiddleware>();
      app.UseRouting();
      app.UseAuthentication();
      app.UseAuthorization();

      app.MapControllerRoute(
         name: ConfigurationHelpers.DefaultRouteName,
         pattern: ConfigurationHelpers.DefaultRoutePattern);

      await SeedDatabaseAsync(app);

      app.Run();
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

         Uri keyVaultUri = new(keyVaultUriText);
         if (builder.Environment.IsProduction())
         {
            // Managed identity token credential discovered when running in Azure environments
            builder.Configuration.AddAzureKeyVault(
                keyVaultUri, new ManagedIdentityCredential(ManagedIdentityId.SystemAssigned));
         }
         else
         {
            builder.Configuration.AddAzureKeyVault(
                keyVaultUri, new DefaultAzureCredential());
         }

         return;
      }

      throw new InvalidOperationException(
         $"Unknown secrets provider: {secretsProvider}");
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
}
