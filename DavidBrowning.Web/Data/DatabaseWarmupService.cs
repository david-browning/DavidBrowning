// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Diagnostics;
using DavidBrowning.Helpers;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DavidBrowning.Web.Data;

public sealed class DatabaseWarmupService
{
   public DatabaseWarmupService(
      IConfiguration configuration,
      IOptions<WarmupOptions> warmupOptions,
      ILogger<DatabaseWarmupService> logger)
   {
      _connectionString = configuration.GetConnectionString("AzureSiteDatabase") ??
         throw new InvalidOperationException(
            "The AzureSiteDatabase connection string is not configured.");
      _warmupOptions = warmupOptions.Value;
      _logger = logger;
   }


   public async Task WarmupAsync(CancellationToken requestCancellationToken)
   {
      using var timeoutSource = CancellationTokenSource.CreateLinkedTokenSource(
         requestCancellationToken);
      timeoutSource.CancelAfter(_warmupOptions.MaximumWaitTime);
      CancellationToken cancellationToken = timeoutSource.Token;

      while (true)
      {
         try
         {
            await ExecuteWarmupAttemptAsync(cancellationToken);
            return;
         }
         catch (Exception exception)
            when (SqlHelpers.IsWarmupRetryException(exception) && !cancellationToken.IsCancellationRequested)
         {
            _logger.LogInformation(
               exception, "Warmup attempt failed. The database may still be resuming.");

            await Task.Delay(_warmupOptions.RetryDelay, cancellationToken);
         }
      }
   }

   private async Task ExecuteWarmupAttemptAsync(CancellationToken cancellationToken)
   {
      SqlConnectionStringBuilder builder = new(_connectionString)
      {
         ConnectTimeout = _warmupOptions.AttemptConnectTimeoutSeconds,
      };

      await using SqlConnection connection = new(builder.ConnectionString);
      await connection.OpenAsync(cancellationToken);
      await using SqlCommand command = connection.CreateCommand();
      command.CommandText = "SELECT 1;";
      command.CommandTimeout = _warmupOptions.AttemptCommandTimeoutSeconds;
      await command.ExecuteScalarAsync(cancellationToken);
   }

   private readonly string _connectionString;
   private readonly WarmupOptions _warmupOptions;
   private readonly ILogger<DatabaseWarmupService> _logger;
}
