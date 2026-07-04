// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Diagnostics;
using DavidBrowning.Web.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DavidBrowning.Web.Controllers;
public sealed class SystemController : Controller
{
   public const string WarmupTokenHeaderName = "X-Warmup-Token";

   public SystemController(
      ILogger<SystemController> logger,
      IOptionsMonitor<WarmupOptions> options,
      IMemoryCache memoryCache,
      DatabaseWarmupService warmup)
   {
      _warmupOptions = options;
      _logger = logger;
      _memoryCache = memoryCache;
      _databaseWarmupService = warmup;
   }

   [HttpGet("/api/coffee")]
   [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
   public IActionResult Coffee()
   {
      return StatusCode(StatusCodes.Status418ImATeapot);
   }

   [HttpGet("/system/warming-up")]
   [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
   public IActionResult WarmingUp([FromQuery] string? returnUrl)
   {
      if (string.IsNullOrWhiteSpace(returnUrl) || !Url.IsLocalUrl(returnUrl))
      {
         returnUrl = "/";
      }

      return View("WarmingUp", returnUrl);
   }

   [HttpGet("/api/warmup")]
   [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
   public async Task<IActionResult> Warmup(CancellationToken cancellationToken)
   {
      if (!_warmupOptions.CurrentValue.Enabled)
      {
         return NotFound();
      }

      if (!IsWarmupApiKeyValid())
      {
         return Unauthorized();
      }

      const string cacheKey = "Operations:Warmup:LastRunUtc";

      if (_memoryCache.TryGetValue(cacheKey, out DateTimeOffset lastRunUtc) &&
          DateTimeOffset.UtcNow - lastRunUtc < _warmupOptions.CurrentValue.MinimumInterval)
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
             DateTimeOffset.UtcNow - lastRunUtc < _warmupOptions.CurrentValue.MinimumInterval)
         {
            return NoContent();
         }

         await _databaseWarmupService.WarmupAsync(cancellationToken);

         _memoryCache.Set(
            cacheKey, DateTimeOffset.UtcNow, _warmupOptions.CurrentValue.MinimumInterval);
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

   private bool IsWarmupApiKeyValid()
   {
      string? configuredApiKey = _warmupOptions.CurrentValue.ApiKey;
      if (string.IsNullOrWhiteSpace(configuredApiKey))
      {
         return false;
      }

      string submittedApiKey = Request.Headers[WarmupTokenHeaderName].ToString();
      if (string.IsNullOrWhiteSpace(submittedApiKey))
      {
         return false;
      }

      return SecureEquals(submittedApiKey, configuredApiKey);
   }

   private static bool SecureEquals(string left, string right)
   {
      byte[] leftBytes = Encoding.UTF8.GetBytes(left);
      byte[] rightBytes = Encoding.UTF8.GetBytes(right);
      return CryptographicOperations.FixedTimeEquals(leftBytes, rightBytes);
   }

   private static readonly SemaphoreSlim _warmupSemaphore = new(1, 1);
   private readonly ILogger<SystemController> _logger;
   private readonly IMemoryCache _memoryCache;
   private readonly DatabaseWarmupService _databaseWarmupService;
   private IOptionsMonitor<WarmupOptions> _warmupOptions;
}
