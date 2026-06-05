// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Diagnostics;
using DavidBrowning.Data.Stores.Error;
using DavidBrowning.Diagnostics;
using DavidBrowning.Models.ViewModels.Error;
using DavidBrowning.Services.Time;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DavidBrowning.Controllers;

public class ErrorController : Controller
{
   public ErrorController(
      ILogger<ErrorController> logger,
      IOptions<DiagnosticsOptions> options,
      IWebHostEnvironment environment,
      IConfiguration configuration,
      IErrorStore errorStore)
   {
      _logger = logger;
      _options = options.Value;
      _webHostEnvironment = environment;
      _configuration = configuration;
      _errorStore = errorStore;
   }

   [HttpGet]
   [Route("/Error/StatusCode/{statusCode:int}")]
   public IActionResult StatusCodePage(int statusCode)
   {
      var exceptionFeature =
         HttpContext.Features.Get<IExceptionHandlerPathFeature>();

      var statusCodeFeature =
         HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

      StatusCodeViewModel model = new()
      {
         HttpError = statusCode,
         RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
         OriginalPath =
            exceptionFeature?.Path ??
            statusCodeFeature?.OriginalPath ??
            HttpContext.Request.Path.Value,
         OriginalQueryString =
            statusCodeFeature?.OriginalQueryString ?? string.Empty,
      };

      Response.StatusCode = statusCode;

      return statusCode switch
      {
         StatusCodes.Status403Forbidden => View("Forbidden", model),
         StatusCodes.Status404NotFound => View("NotFound", model),
         _ => View("ServerError", model)
      };
   }

   [HttpGet("test-crash")]
   public IActionResult ThrowTestException()
   {
      IActionResult? guardResult = GuardDiagnosticsAccess();

      if (guardResult is not null)
      {
         return guardResult;
      }

      throw new InvalidOperationException(
         "This is a test exception thrown by ErrorController to test the error middleware.");
   }

   /// <summary>
   /// Returns null if diagnostics are enabled.
   /// Otherwise, returns a result that the calling controller can send back.
   /// </summary>
   /// <returns></returns>
   private IActionResult? GuardDiagnosticsAccess()
   {
      if (_options.EnableDebugEndpoints)
      {
         return null;
      }

      if (!Request.Headers.TryGetValue("X-Diagnostic-Key", out var suppliedToken))
      {
         return NotFound();
      }

      return NotFound();
   }

   private readonly ILogger<ErrorController> _logger;
   private readonly DiagnosticsOptions _options;
   private readonly IWebHostEnvironment _webHostEnvironment;
   private readonly IConfiguration _configuration;
   private readonly IErrorStore _errorStore;
}
