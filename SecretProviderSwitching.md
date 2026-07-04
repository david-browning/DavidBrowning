# Switching Between Local Secrets and Azure Key Vault

This project can run with either local development secrets or Azure Key Vault as the secrets provider.

Use **Local** when developing against local resources or when you want the app to run without Azure authentication.

Use **AzureKeyVault** when the local app should load secrets directly from Azure Key Vault using your signed-in Azure developer identity.

Do not store real Key Vault names, real secret names, connection strings, API keys, or passwords in this document. Use placeholders such as `<KEYVAULT NAME>`, `<SECRET NAME>`, and `<CONFIGURATION KEY>`.

## Required tools

Install the Azure CLI.

On Windows:

```powershell
winget install --exact --id Microsoft.AzureCLI
```

Verify the install:

```powershell
az --version
```

The application project needs these NuGet packages if they are not already installed:

```powershell
dotnet add package Azure.Extensions.AspNetCore.Configuration.Secrets
dotnet add package Azure.Identity
```

## Configuration switch

The active secrets provider is selected through configuration.

For local secrets:

```json
{
  "Secrets": {
    "Provider": "Local"
  }
}
```

For Azure Key Vault:

```json
{
  "Secrets": {
    "Provider": "AzureKeyVault"
  },
  "KeyVault": {
    "VaultUri": "https://<KEYVAULT NAME>.vault.azure.net/"
  }
}
```

The Key Vault URI is not itself a secret. The secret values remain in Azure Key Vault. Access is controlled by Azure identity and Key Vault permissions.

## Local secrets provider

Use the local provider when the app should read secrets from ASP.NET Core user secrets, local configuration, or other local development configuration sources.

Example `appsettings.Development.json`:

```json
{
  "Secrets": {
    "Provider": "Local"
  }
}
```

Initialize user secrets for a project:

```powershell
dotnet user-secrets init --project <PROJECT PATH>
```

Set a local secret:

```powershell
dotnet user-secrets set "<CONFIGURATION KEY>" "<VALUE>" --project <PROJECT PATH>
```

List local secrets:

```powershell
dotnet user-secrets list --project <PROJECT PATH>
```

Remove a local secret:

```powershell
dotnet user-secrets remove "<CONFIGURATION KEY>" --project <PROJECT PATH>
```

Use this mode when working offline, using a local SQL Server, using Azurite, or when Azure authentication is not relevant to the current task.

## Azure Key Vault secrets provider

Use the Azure Key Vault provider when the app should read secrets directly from Azure Key Vault.

Example `appsettings.Development.json`:

```json
{
  "Secrets": {
    "Provider": "AzureKeyVault"
  },
  "KeyVault": {
    "VaultUri": "https://<KEYVAULT NAME>.vault.azure.net/"
  }
}
```

In this mode, the local app authenticates to Azure as your signed-in developer identity.

## Logging in locally

Sign in with Azure CLI:

```powershell
az login
```

If you have more than one subscription, select the correct one:

```powershell
az account list --output table

az account set --subscription "<SUBSCRIPTION NAME OR ID>"

az account show --output table
```

A good rule for local development is:

```text
If Azure CLI cannot read the Key Vault, the ASP.NET Core app will not read it either.
```

## Verifying Key Vault access from the command line

Before debugging the application, verify that the signed-in Azure account can access the vault.

List visible secrets:

```powershell
az keyvault secret list `
  --vault-name "<KEYVAULT NAME>" `
  --query "[].name" `
  --output table
```

Read one known secret:

```powershell
az keyvault secret show `
  --vault-name "<KEYVAULT NAME>" `
  --name "<SECRET NAME>" `
  --query value `
  --output tsv
```

If these commands fail, fix Azure login, subscription selection, Key Vault permissions, or Key Vault firewall access before debugging the app.

## Key Vault secret naming convention

ASP.NET Core configuration uses `:` for hierarchical keys.

Example configuration key:

```text
<SECTION>:<SETTING NAME>
```

Azure Key Vault secret names cannot use `:`, so use double dashes instead:

```text
<SECTION>--<SETTING NAME>
```

For deeper hierarchy:

```text
<SECTION>--<SUBSECTION>--<SETTING NAME>
```

The Azure Key Vault configuration provider maps `--` to `:` when loading configuration.

Do not put real secret names in shared documentation. Document the pattern, not the deployed values.

## Granting local developer access

The local app runs as your signed-in Azure user. That user must have permission to read Key Vault secrets.

If the vault uses Azure RBAC, assign the local developer account the `Key Vault Secrets User` role at the vault scope.

```powershell
$vaultId = az keyvault show `
  --name "<KEYVAULT NAME>" `
  --query id `
  --output tsv

$userId = az ad signed-in-user show `
  --query id `
  --output tsv

az role assignment create `
  --assignee $userId `
  --role "Key Vault Secrets User" `
  --scope $vaultId
```

If the vault uses access policies instead of Azure RBAC, grant `get` and `list` secret permissions:

```powershell
$userId = az ad signed-in-user show `
  --query id `
  --output tsv

az keyvault set-policy `
  --name "<KEYVAULT NAME>" `
  --object-id $userId `
  --secret-permissions get list
```

The configuration provider needs to list secret metadata and retrieve secret values.

## Application credential behavior

Production should use the App Service managed identity.

Local development should use a developer credential.

A deterministic local setup uses `AzureCliCredential`:

```csharp
using Azure.Core;
using Azure.Identity;

private static TokenCredential CreateKeyVaultCredential(IHostEnvironment environment)
{
   if (environment.IsProduction())
   {
      return new ManagedIdentityCredential(ManagedIdentityId.SystemAssigned);
   }

   return new AzureCliCredential();
}
```

With this version, local Key Vault authentication works when `az login` works.

A broader local setup can use `DefaultAzureCredential` while excluding cloud-only credential types that are not useful on a local PC:

```csharp
using Azure.Core;
using Azure.Identity;

private static TokenCredential CreateKeyVaultCredential(IHostEnvironment environment)
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
```

`AzureCliCredential` is easier to reason about. `DefaultAzureCredential` is more flexible if you want Visual Studio, Azure CLI, or other developer credentials to work.

## Adding Key Vault to configuration

Add Key Vault as a configuration provider during startup before registering services that read secrets, connection strings, or options.

```csharp
using Azure;
using Azure.Core;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;

public static void AddConfiguredSecrets(
   ConfigurationManager configuration,
   IHostEnvironment environment)
{
   string? provider = configuration["Secrets:Provider"];

   if (string.Equals(provider, "Local", StringComparison.OrdinalIgnoreCase))
   {
      return;
   }

   if (!string.Equals(provider, "AzureKeyVault", StringComparison.OrdinalIgnoreCase))
   {
      throw new InvalidOperationException(
         $"Unsupported secrets provider: '{provider}'.");
   }

   string? keyVaultUriText = configuration["KeyVault:VaultUri"];

   if (string.IsNullOrWhiteSpace(keyVaultUriText))
   {
      throw new InvalidOperationException(
         "Secrets:Provider is AzureKeyVault, but KeyVault:VaultUri is missing.");
   }

   if (!Uri.TryCreate(keyVaultUriText, UriKind.Absolute, out Uri? keyVaultUri))
   {
      throw new InvalidOperationException(
         $"KeyVault:VaultUri is not a valid absolute URI: '{keyVaultUriText}'.");
   }

   AzureKeyVaultConfigurationOptions options = new()
   {
      ReloadInterval = TimeSpan.FromMinutes(10),
   };

   try
   {
      configuration.AddAzureKeyVault(
         keyVaultUri,
         CreateKeyVaultCredential(environment),
         options);
   }
   catch (CredentialUnavailableException exception)
   {
      throw new InvalidOperationException(
         "Azure Key Vault authentication is unavailable. For local development, run 'az login' and select the correct subscription.",
         exception);
   }
   catch (AuthenticationFailedException exception)
   {
      throw new InvalidOperationException(
         "Azure Key Vault authentication failed. Check the signed-in Azure account, tenant, and subscription.",
         exception);
   }
   catch (RequestFailedException exception) when (exception.Status == 403)
   {
      throw new InvalidOperationException(
         "Azure Key Vault returned 403 Forbidden. The signed-in identity may lack secret permissions, or the Key Vault firewall may be blocking this machine.",
         exception);
   }
   catch (RequestFailedException exception) when (exception.Status == 404)
   {
      throw new InvalidOperationException(
         $"Azure Key Vault returned 404 Not Found. Check KeyVault:VaultUri: '{keyVaultUri}'.",
         exception);
   }
   catch (RequestFailedException exception)
   {
      throw new InvalidOperationException(
         $"Azure Key Vault could not be loaded. Status={exception.Status}, ErrorCode={exception.ErrorCode}.",
         exception);
   }
}
```

Call it early in startup:

```csharp
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

AddConfiguredSecrets(
   builder.Configuration,
   builder.Environment);

// Register services that depend on secrets after this point.
```

After Key Vault is added to configuration, the rest of the application should read configuration normally:

```csharp
string? value = builder.Configuration["<CONFIGURATION KEY>"];
```

or:

```csharp
string? connectionString =
   builder.Configuration.GetConnectionString("<CONNECTION STRING NAME>");
```

Controllers and services should not call Key Vault directly just to retrieve ordinary configuration values. Load Key Vault into configuration once, then bind options and connection strings normally.

## Switching to local secrets

1. Set the provider to `Local`.

   ```json
   {
     "Secrets": {
       "Provider": "Local"
     }
   }
   ```

2. Confirm required values exist in user secrets.

   ```powershell
   dotnet user-secrets list --project <PROJECT PATH>
   ```

3. Run the app.

This mode does not require Azure login.

## Switching to Azure Key Vault

1. Set the provider to `AzureKeyVault`.

   ```json
   {
     "Secrets": {
       "Provider": "AzureKeyVault"
     },
     "KeyVault": {
       "VaultUri": "https://<KEYVAULT NAME>.vault.azure.net/"
     }
   }
   ```

2. Sign in locally.

   ```powershell
   az login
   ```

3. Select the correct subscription.

   ```powershell
   az account set --subscription "<SUBSCRIPTION NAME OR ID>"
   ```

4. Verify Key Vault access.

   ```powershell
   az keyvault secret list `
     --vault-name "<KEYVAULT NAME>" `
     --query "[].name" `
     --output table
   ```

5. Run the app.

## Production App Service behavior

Production should not use your local Azure CLI login. It should use the App Service managed identity.

Checklist:

```text
1. Enable the App Service system-assigned managed identity.
2. Grant that identity permission to read Key Vault secrets.
3. Configure the app with Secrets:Provider = AzureKeyVault.
4. Configure KeyVault:VaultUri with the vault URI.
5. Deploy and verify startup.
```

If using Azure RBAC:

```powershell
$vaultId = az keyvault show `
  --name "<KEYVAULT NAME>" `
  --query id `
  --output tsv

az role assignment create `
  --assignee "<APP SERVICE MANAGED IDENTITY PRINCIPAL ID>" `
  --role "Key Vault Secrets User" `
  --scope $vaultId
```

If using access policies, grant the App Service managed identity `get` and `list` secret permissions.

## Troubleshooting

### `CredentialUnavailableException`

The app could not find a usable Azure credential.

For local development:

```powershell
az login
az account set --subscription "<SUBSCRIPTION NAME OR ID>"
```

If using `DefaultAzureCredential`, confirm the intended developer tool credential is actually signed in.

### `AuthenticationFailedException`

The app found a credential, but Azure rejected it.

Check:

```powershell
az account show --output table
```

Make sure the signed-in account is in the correct tenant and subscription.

### `403 Forbidden`

The identity authenticated, but Key Vault rejected the request.

Common causes:

```text
The user or managed identity lacks Key Vault secret permissions.
The vault uses Azure RBAC and the identity does not have Key Vault Secrets User.
The vault uses access policies and the identity lacks get/list.
The Key Vault firewall is blocking the machine or App Service.
```

Test from the command line:

```powershell
az keyvault secret list `
  --vault-name "<KEYVAULT NAME>" `
  --query "[].name" `
  --output table
```

### `404 Not Found`

Check the vault URI:

```json
{
  "KeyVault": {
    "VaultUri": "https://<KEYVAULT NAME>.vault.azure.net/"
  }
}
```

Confirm the vault exists and the selected subscription is correct:

```powershell
az keyvault show --name "<KEYVAULT NAME>"
```

### A configuration value is still missing

Check the secret naming convention.

Configuration key:

```text
<SECTION>:<SETTING NAME>
```

Key Vault secret name:

```text
<SECTION>--<SETTING NAME>
```

Also confirm that Key Vault is added to configuration before services are registered.

### The app works locally but not in Azure

The local app uses your developer identity. The Azure app uses the App Service managed identity.

Grant the App Service managed identity permission to read the vault. Do not assume local success means production identity access is configured.

## Recommended policy

Use `Local` for fully local development.

Use `AzureKeyVault` when local development should exercise the production-style secret-loading path.

Use managed identity in production.

Do not inject Key Vault clients into controllers just to retrieve configuration. Load Key Vault into configuration at startup, bind options normally, and keep application code independent from where the secret is stored.

## References

- Azure CLI installation: https://learn.microsoft.com/en-us/cli/azure/install-azure-cli-windows
- Azure CLI authentication: https://learn.microsoft.com/en-us/cli/azure/authenticate-azure-cli
- ASP.NET Core Key Vault configuration provider: https://learn.microsoft.com/en-us/aspnet/core/security/key-vault-configuration
- Azure RBAC built-in roles: https://learn.microsoft.com/en-us/azure/role-based-access-control/built-in-roles
- Azure Key Vault RBAC guide: https://learn.microsoft.com/en-us/azure/key-vault/general/rbac-guide
