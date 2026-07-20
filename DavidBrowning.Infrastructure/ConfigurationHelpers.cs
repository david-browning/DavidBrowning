// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

namespace DavidBrowning.Infrastructure;

public class ConfigurationHelpers
{
   public const string AzureKeyVaultProviderName = "AzureKeyVault";
   public const string AzureStorageBlobsProviderName = "AzureStorageBlobs";
   public const string DataFolderName = "Data";
   public const string DefaultInMemoryDatabaseName = "DavidBrowning";
   public const string DefaultRouteName = "default";
   public const string DefaultRoutePattern = "{controller=Home}/{action=Index}/{id?}";
   public const string DefaultTablePrefix = "db_";

   public const string DiagnosticsSectionName = "Diagnostics";
   public const string CacheSectionName = "Cache";
   public const string OutputCacheDurationKey = "Cache:OutputCacheDuration";
   public const string SitemapCacheDurationKey = "Cache:SitemapCacheDuration";
   public const string DummyProviderName = "Dummy";
   public const string ErrorStoreName = "ErrorStore";
   public const string InMemoryProviderName = "InMemory";
   public const string LocalProviderName = "Local";
   public const string LookupCacheSectionName = "LookupCache";
   public const string LookupStoreName = "LookupStore";
   public const string ProjectStoreName = "ProjectStore";
   public const string ProviderSectionName = "Provider";
   public const string SeedFolderName = "Seed";

   public const string LocalSiteDatabaseConnectionName = "LocalSiteDatabase";
   public const string AzureSiteDatabaseConnectionName = "AzureSiteDatabase";
   public const string AzureAdminSiteDatabaseConnectionName = "AzureAdminSiteDatabase";
   public const string DefaultSiteDatabaseConnectionName = LocalSiteDatabaseConnectionName;

   public const string LocalContentStorageConnectionName = "LocalContentStorage";
   public const string AzureContentStorageConnectionName = "AzureContentStorage";
   public const string DefaultContentStorageConnectionName = LocalContentStorageConnectionName;

   public const string SqlServerProviderName = "SqlServer";
   public const string StartupLoggerName = "Startup";
   public const string StoresSectionName = "Stores";
   public const string WritingStoreName = "WritingStore";
   public const string ContentStoreName = "ContentStore";

   public const string DatabaseConnectionNameKey = "Database:ConnectionName";
   public const string DatabaseProviderKey = "Database:Provider";
   public const string ContentStorageConnectionNameKey = "Stores:ContentStore:AzureStorageBlobs:ConnectionName";

   public const string EnableCustomErrorsKey = "Diagnostics:EnableCustomErrors";
   public const string EnableDetailedErrorsKey = "Diagnostics:EntityFrameworkOptions:EnableDetailedErrors";
   public const string EnableSensitiveDataLoggingKey = "Diagnostics:EntityFrameworkOptions:EnableSensitiveDataLogging";
   public const string EnableSqlCommandLoggingKey = "Diagnostics:EntityFrameworkOptions:EnableSqlCommandLogging";

   public const string SqlConnectTimeoutSeconds = "Database:ConnectTimeoutSeconds";
   public const string SqlCommandTimeoutSeconds = "Database:CommandTimeoutSeconds";
   public const string SqlConnectRetryDelay = "Database:MaxRetryDelaySeconds";
   public const string SqlConnectMaxRetry = "Database:RetryCount";

   public const string InMemoryDatabaseNameKey = "Database:InMemoryDatabaseName";
   public const string InternalServerErrorPath = "/Error/StatusCode/500";
   public const string KeyVaultUriKey = "KeyVault:VaultUri";
   public const string SecretsProviderKey = "Secrets:Provider";
   public const string SeedEnabledKey = "Database:Seed:Enabled";
   public const string SeedRootFolderKey = "Database:Seed:RootFolder";
   public const string SkipFileWhenTargetTableHasRowsKey = "Database:Seed:SkipFileWhenTargetTableHasRows";
   public const string StatusCodePagePath = "/Error/StatusCode/{0}";
   public const string TablePrefixKey = "Database:TablePrefix";
   public const string ThrowOnUnmatchedJsonFilesKey = "Database:Seed:ThrowOnUnmatchedJsonFiles";
   public const string UseSqlServerIdentityInsertWhenNeededKey = "Database:Seed:UseSqlServerIdentityInsertWhenNeeded";
}
