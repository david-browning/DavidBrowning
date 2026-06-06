// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Diagnostics;
using DavidBrowning.Infrastructure.Data.Stores;
using DavidBrowning.Web.Diagnostics;
using DavidBrowning.Web.ViewModels.Error;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DavidBrowning.Web.Controllers;

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

   [Route("Error/StatusCode/{statusCode}")]
   public IActionResult StatusCodePage(int statusCode)
   {
      var model = new StatusCodeViewModel()
      {
         HttpError = statusCode,
         RequestId = Activity.Current?.Id ??
            HttpContext.TraceIdentifier,
      };

      return statusCode switch
      {
         StatusCodes.Status400BadRequest => View("BadRequest", model),
         StatusCodes.Status401Unauthorized => View("Unauthorized", model),
         StatusCodes.Status403Forbidden => View("Forbidden", model),
         StatusCodes.Status404NotFound => View("NotFound", model),
         StatusCodes.Status500InternalServerError => View("ServerError", model),
         _ => View("StatusCode", model),
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

   [HttpGet("test-bad-request")]
   public IActionResult ReturnTestBadRequest()
   {
      return ReturnTestStatusCode(StatusCodes.Status400BadRequest);
   }

   [HttpGet("test-unauthorized")]
   public IActionResult ReturnTestUnauthorized()
   {
      return ReturnTestStatusCode(StatusCodes.Status401Unauthorized);
   }

   [HttpGet("test-forbidden")]
   public IActionResult ReturnTestForbidden()
   {
      return ReturnTestStatusCode(StatusCodes.Status403Forbidden);
   }

   [HttpGet("test-not-found")]
   public IActionResult ReturnTestNotFound()
   {
      return ReturnTestStatusCode(StatusCodes.Status404NotFound);
   }

   [HttpGet("test-server-error")]
   public IActionResult ReturnTestServerError()
   {
      return ReturnTestStatusCode(StatusCodes.Status500InternalServerError);
   }

   [HttpGet("test-generic-status")]
   public IActionResult ReturnTestGenericStatus()
   {
      return ReturnTestStatusCode(StatusCodes.Status418ImATeapot);
   }

   /// <summary>
   /// Returns an empty HTTP error response so the status-code middleware can
   /// re-execute the matching error-page endpoint.
   /// </summary>
   /// <param name="statusCode">
   /// HTTP error status code to return. Only 4xx and 5xx codes are supported.
   /// </param>
   /// <returns></returns>
   [HttpGet("test-status/{statusCode:int}")]
   public IActionResult ReturnTestStatusCode(int statusCode)
   {
      IActionResult? guardResult = GuardDiagnosticsAccess();

      if (guardResult is not null)
      {
         return guardResult;
      }

      if (statusCode < 400 || statusCode > 599)
      {
         return BadRequest(
            "The test status code must be between 400 and 599.");
      }

      return StatusCode(statusCode);
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
