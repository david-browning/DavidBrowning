// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Text.Json;
using System.Threading.Tasks;
using DavidBrowning.Data.Stores.Error;
using DavidBrowning.Models.Error;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace DavidBrowning.Middleware
{
   internal sealed class ErrorLoggingMiddleware
   {
      public ErrorLoggingMiddleware(
         RequestDelegate next,
         ILogger<ErrorLoggingMiddleware> logger)
      {
         _next = next;
         _logger = logger;
      }

      public async Task InvokeAsync(
        HttpContext context,
        IErrorStore errorStore,
        IWebHostEnvironment environment)
      {
         try
         {
            await _next(context);
         }
         catch (Exception exception)
         {
            var error = CreateErrorLogEntry(
               context,
               exception,
               environment);

            try
            {
               await errorStore.InsertErrorAsync(error, context.RequestAborted);
            }
            catch (Exception loggingException)
            {
               _logger.LogError(
                  loggingException,
                  "Failed to persist error log entry for TraceIdentifier {TraceIdentifier}.",
                  context.TraceIdentifier);
            }

            _logger.LogError(
               exception,
               "Unhandled exception for {Method} {Path}. TraceIdentifier: {TraceIdentifier}",
               context.Request.Method,
               context.Request.Path,
               context.TraceIdentifier);

            throw;
         }
      }

      private static WebsiteError CreateErrorLogEntry(
         HttpContext context,
         Exception exception,
         IWebHostEnvironment environment)
      {
         Endpoint? endpoint = context.GetEndpoint();

         string? routeValuesJson = null;

         if (context.Request.RouteValues.Count > 0)
         {
            routeValuesJson = JsonSerializer.Serialize(context.Request.RouteValues);
         }

         return new WebsiteError
         {
            OccurredAtUtc = DateTime.UtcNow,
            TraceIdentifier = context.TraceIdentifier,
            EnvironmentName = environment.EnvironmentName,
            ApplicationVersion = typeof(ErrorLoggingMiddleware).Assembly.GetName().Version?.ToString(),
            MachineName = Environment.MachineName,

            HttpMethod = context.Request.Method,
            Path = context.Request.Path.Value,
            QueryString = context.Request.QueryString.HasValue
               ? context.Request.QueryString.Value
               : null,
            EndpointName = endpoint?.DisplayName,
            RouteValuesJson = routeValuesJson,

            StatusCode = StatusCodes.Status500InternalServerError,

            ExceptionType = exception.GetType().FullName ?? exception.GetType().Name,
            ExceptionMessage = exception.Message,
            ExceptionSource = exception.Source,
            StackTrace = exception.StackTrace,

            InnerExceptionType = exception.InnerException?.GetType().FullName,
            InnerExceptionMessage = exception.InnerException?.Message,

            UserName = context.User.Identity?.IsAuthenticated == true
               ? context.User.Identity.Name
               : null,

            UserAgent = context.Request.Headers.UserAgent.ToString(),
            Referrer = context.Request.Headers.Referer.ToString(),
            RemoteIpAddress = context.Connection.RemoteIpAddress?.ToString(),

            IsHandled = true
         };
      }

      private readonly RequestDelegate _next;
      private readonly ILogger<ErrorLoggingMiddleware> _logger;
   }
}
