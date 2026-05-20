// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using DavidBrowning.Data.Stores.Error;
using DavidBrowning.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace DavidBrowning.Controllers
{
   public class ErrorController : Controller
   {
      public ErrorController(
         IErrorStore errorLogStore,
         IWebHostEnvironment environment,
         IOptions<ErrorControllerDiagnosticsOptions> options) 
      {
         _errorLogStore = errorLogStore;
         _environment = environment;
         _options = options.Value;
      }

      public IActionResult Index()
      {
         return View();
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
         if (_options.Enabled)
         {
            return null;
         }

         if (!Request.Headers.TryGetValue("X-Diagnostic-Key", out var suppliedToken))
         {
            return NotFound();
         }

         return NotFound();
      }

      private readonly IErrorStore _errorLogStore;
      private readonly IWebHostEnvironment _environment;
      private readonly ErrorControllerDiagnosticsOptions _options;
   }
}
