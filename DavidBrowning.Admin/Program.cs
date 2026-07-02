using System;
using Azure.Identity;
using DavidBrowning.Helpers;
using DavidBrowning.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DavidBrowning.Admin;

public static class Program
{
   public static void Main(string[] args)
   {
      var builder = WebApplication.CreateBuilder(args);

      builder.Logging.ClearProviders();
      builder.Logging.AddConsole();
      builder.Logging.AddDebug();

      ConfigureSecrets(builder);

      builder.Services.AddDavidBrowningInfrastructure(
         builder.Configuration,
         builder.Environment);

      //builder.Services.AddAdminAuthoringServices();
      builder.Services.AddControllersWithViews();

      var app = builder.Build();

      app.ConfigureErrorHandling(builder.Configuration);

      app.UseHttpsRedirection();
      app.UseStaticFiles();
      app.UseRouting();
      app.UseAuthentication();
      app.UseAuthorization();

      app.MapControllerRoute(
         name: ConfigurationHelpers.DefaultRouteName,
         pattern: ConfigurationHelpers.DefaultRoutePattern);

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
}