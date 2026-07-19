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
         _logger.LogInformation(ex, "Database appears unavailable or resuming. Redirecting to warming page.");
         WriteDatabaseWarmingupResponse(context);
      }
      catch (Exception ex) when (SqlHelpers.IsFreeAllowanceException(ex))
      {
         _logger.LogWarning(ex, "Azure SQL Database is paused because the monthly free allowance has been exhausted.");
         await WriteDatabaseAllowancePausedResponse(context);
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

   private static void WriteDatabaseWarmingupResponse(HttpContext context)
   {
      var returnUrl =
         $"{context.Request.PathBase}{context.Request.Path}{context.Request.QueryString}";

      var warmingUrl =
         $"/system/warming-up?returnUrl={Uri.EscapeDataString(returnUrl)}";

      context.Response.Clear();
      context.Response.Headers["X-DavidBrowning-Warming-Up"] = "1";
      context.Response.Redirect(warmingUrl);
   }

   private static async Task WriteDatabaseAllowancePausedResponse(
      HttpContext context)
   {
      if (context.Response.HasStarted)
      {
         throw new InvalidOperationException(
             "Cannot write database unavailable response because the HTTP response has already started.");
      }

      context.Response.Clear();
      context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
      context.Response.ContentType = "text/html; charset=utf-8";
      context.Response.Headers.CacheControl = "no-store";
      context.Response.Headers["X-Robots-Tag"] = "noindex";

      await context.Response.WriteAsync("""
            <!doctype html>
            <html lang="en">
            <head>
                <meta charset="utf-8">
                <meta name="viewport" content="width=device-width, initial-scale=1">
                <title>Site temporarily unavailable</title>
            </head>
            <body>
                <main>
                    <h1>Site temporarily unavailable</h1>
                    <p>The site database is temporarily unavailable. Please try again later.</p>
                </main>
            </body>
            </html>
            """);
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
