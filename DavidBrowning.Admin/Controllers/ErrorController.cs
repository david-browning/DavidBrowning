// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Admin.ViewModels;
using DavidBrowning.Admin.ViewModels.Error;
using DavidBrowning.Diagnostics;
using DavidBrowning.Infrastructure.Data.Stores;
using DavidBrowning.ViewModels;
using DavidBrowning.ViewModels.Error;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DavidBrowning.Admin.Controllers;

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
   public Task<IActionResult> Index(CancellationToken cancellationToken)
   {
      return GetPageAsync(1, cancellationToken);
   }

   [HttpGet("page/{page:int:min(1)}")]
   public async Task<IActionResult> Page(
      int page,
      CancellationToken cancellationToken)
   {
      if (page == 1)
      {
         return RedirectToAction(nameof(Index));
      }

      return await GetPageAsync(page, cancellationToken);
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

   [HttpGet]
   public async Task<IActionResult> Details(
      int id,
      CancellationToken cancellationToken)
   {
      var error = await _errorStore.GetErrorAsync(id, cancellationToken);
      if (error is null)
      {
         return NotFound();
      }

      return PartialView("Details", error);
   }

   private async Task<IActionResult> GetPageAsync(
   int page,
   CancellationToken cancellationToken)
   {
      var model = await GetIndexModelAsync(page, cancellationToken);
      if (page > Math.Max(model.Pager.TotalPages, 1))
      {
         return NotFound();
      }

      return View("Index", model);
   }

   private async Task<IndexViewModel> GetIndexModelAsync(
      int page,
      CancellationToken cancellationToken)
   {
      var pagedErrors = await _errorStore.GetPagedErrorsAsync(
         page, _pageSize, cancellationToken);
      return new IndexViewModel()
      {
         Errors = pagedErrors.Items,
         Pager = new PagerViewModel(
            page,
            (int)Math.Ceiling((double)pagedErrors.TotalCount / (double)_pageSize),
            "Error",
            nameof(Index), nameof(Page)),
         DetailsOffcanvas = new AdminOffcanvasViewModel()
         {
            Id = ErrorAdminIds.ErrorDetailsOffcanvas,
            Title = "Error details",
            Placeholder = "Select an error to view details.",
            LoadingText = "Loading error details...",
            CssClass = "admin-offcanvas-wide",
         },
      };
   }

   private readonly int _pageSize = 25;
   private readonly ILogger<ErrorController> _logger;
   private readonly DiagnosticsOptions _options;
   private readonly IWebHostEnvironment _webHostEnvironment;
   private readonly IConfiguration _configuration;
   private readonly IErrorStore _errorStore;
}
