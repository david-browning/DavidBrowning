// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Diagnostics;
using DavidBrowning.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DavidBrowning.Web.Controllers;

[ApiController]
[Route("api")]
public class OperationsController : ControllerBase
{
   public const string WarmupTokenHeaderName = "X-Warmup-Token";

   public OperationsController(
      ILogger<OperationsController> logger,
      IOptions<DiagnosticsOptions> options,
      IMemoryCache memoryCache,
      SiteDbContext dbContext)
   {
      _warmupOptions = options.Value.Warmup;
      _logger = logger;
      _memoryCache = memoryCache;
      _siteDbContext = dbContext;
   }

   [HttpGet("warmup")]
   [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
   public async Task<IActionResult> Warmup(CancellationToken cancellationToken)
   {
      bool hasToken = Request.Headers.TryGetValue(
         WarmupTokenHeaderName, out var token);
      if (!_warmupOptions.Enabled || !hasToken ||
         !IsWarmUpTokenValid(token.ToString()))
      {
         return NotFound();
      }

      const string cacheKey = "Operations:Warmup:LastRunUtc";

      if (_memoryCache.TryGetValue(cacheKey, out DateTimeOffset lastRunUtc) &&
          DateTimeOffset.UtcNow - lastRunUtc < _warmupOptions.MinimumInterval)
      {
         // Tried to warm up again too soon.
         return NoContent();
      }

      // If we cannot acquire the semaphore:
      if (!await _warmupSemaphore.WaitAsync(0, cancellationToken))
      {
         // Do not warm up. Someone else may already be trying.
         return NoContent();
      }

      try
      {
         if (_memoryCache.TryGetValue(cacheKey, out lastRunUtc) &&
             DateTimeOffset.UtcNow - lastRunUtc < _warmupOptions.MinimumInterval)
         {
            return NoContent();
         }

         await _siteDbContext.Database.ExecuteSqlRawAsync(
            "SELECT 1;", cancellationToken);

         _memoryCache.Set(
            cacheKey, DateTimeOffset.UtcNow, _warmupOptions.MinimumInterval);
         return Ok("Warmed up");
      }
      catch (Exception exception)
      {
         _logger.LogWarning(exception, "Warmup request failed.");
         return StatusCode(StatusCodes.Status503ServiceUnavailable);
      }
      finally
      {
         _warmupSemaphore.Release();
      }
   }

   private bool IsWarmUpTokenValid(string token)
   {
      var stored = _warmupOptions.Token;
      if (string.IsNullOrEmpty(stored) || string.IsNullOrEmpty(token))
      {
         return false;
      }

      return token.Equals(stored, StringComparison.Ordinal);
   }

   private static readonly SemaphoreSlim _warmupSemaphore = new(1, 1);
   private readonly ILogger<OperationsController> _logger;
   private readonly IMemoryCache _memoryCache;
   private readonly SiteDbContext _siteDbContext;
   private WarmupOptions _warmupOptions;
}
