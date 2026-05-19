// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Threading.Tasks;
using DavidBrowning.Data.Stores.Error;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace DavidBrowning.Middleware
{
   public sealed class ErrorLoggingMiddleware
   {
      private readonly RequestDelegate _next;
      private readonly ILogger<ErrorLoggingMiddleware> _logger;

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
         catch (Exception ex)
         {
            //errorStore.InsertError()
         }
      }
   }
}
