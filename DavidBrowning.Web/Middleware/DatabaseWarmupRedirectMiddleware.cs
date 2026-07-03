// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Linq;
using System.Threading.Tasks;
using DavidBrowning.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace DavidBrowning.Web.Middleware;

public class DatabaseWarmupRedirectMiddleware
{
   public DatabaseWarmupRedirectMiddleware(
      RequestDelegate next,
      ILogger<DatabaseWarmupRedirectMiddleware> logger)
   {
      _next = next;
      _logger = logger;
   }

   public async Task InvokeAsync(HttpContext context)
   {
      try
      {
         await _next(context);
      }
      catch (Exception ex) when (ShouldRedirectToWarmup(context, ex))
      {
         _logger.LogInformation(
            ex, "Database appears unavailable or resuming. Redirecting to warming page.");

         var returnUrl =
            $"{context.Request.PathBase}{context.Request.Path}{context.Request.QueryString}";

         var warmingUrl =
            $"/system/warming-up?returnUrl={Uri.EscapeDataString(returnUrl)}";

         context.Response.Clear();
         context.Response.Headers["X-DavidBrowning-Warming-Up"] = "1";
         context.Response.Redirect(warmingUrl);
      }
   }

   private static bool ShouldRedirectToWarmup(HttpContext context, Exception ex)
   {
      // Only redirect to warm up on GETs
      if (!HttpMethods.IsGet(context.Request.Method) &&
         !HttpMethods.IsHead(context.Request.Method))
      {
         return false;
      }

      if (context.Response.HasStarted)
      {
         return false;
      }

      // Don't redirect if any of these paths:
      if (IsSkippedPath(context.Request.Path))
      {
         return false;
      }

      return SqlHelpers.IsWarmupRetryException(ex);
   }

   private static bool IsSkippedPath(PathString requestPath)
   {
      string path = requestPath.Value ?? string.Empty;

      if (SkippedExactPaths.Contains(path, StringComparer.OrdinalIgnoreCase))
      {
         return true;
      }

      return SkippedPathPrefixes.Any(prefix => PathStartsWithSegment(path, prefix));
   }

   private static bool PathStartsWithSegment(
      string path,
      string prefix)
   {
      if (!path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
      {
         return false;
      }

      return path.Length == prefix.Length || path[prefix.Length] == '/';
   }

   private readonly RequestDelegate _next;
   private readonly ILogger<DatabaseWarmupRedirectMiddleware> _logger;

   private static readonly string[] SkippedExactPaths =
   {
      "/favicon.ico",
   };

   private static readonly string[] SkippedPathPrefixes =
   {
      "/system/warming-up",
      "/api/warmup",
      "/css",
      "/js",
      "/lib",
   };
}
