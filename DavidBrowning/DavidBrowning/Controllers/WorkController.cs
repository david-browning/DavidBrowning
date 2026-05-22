// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using DavidBrowning.Data.Stores.Error;
using DavidBrowning.Data.Stores.Projects;
using DavidBrowning.Diagnostics;
using DavidBrowning.Services.Assets;
using DavidBrowning.Services.Slugs;
using DavidBrowning.Services.Time;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DavidBrowning.Controllers
{
   [Route("work")]
   public class WorkController : Controller
   {
      public WorkController(
         ILogger<WorkController> logger,
         ISystemClock clock,
         IErrorStore errorLogStore,
         IOptions<DiagnosticsOptions> options,
         ISiteAssetService assetService,
         IWebHostEnvironment environment,
         IConfiguration configuration,

         IProjectStore projectStore,
         ISlugService slugs)
      {
         _logger = logger;
         _clock = clock;
         _errorLogStore = errorLogStore;
         _options = options.Value;
         _assetService = assetService;
         _webHostEnvironment = environment;
         _configuration = configuration;

         _projectStore = projectStore;
         _slugService = slugs;
      }

      public IActionResult Index()
      {
         return View();
      }

      /// <summary>
      /// Returns a page with my resume.
      /// </summary>
      /// <returns></returns>
      [HttpGet("resume")]
      public IActionResult Resume()
      {
         return View();
      }

      /// <summary>
      /// Returns a partial view with the highlights of my career.
      /// Useful for a page header or hero image.
      /// </summary>
      /// <returns></returns>
      [HttpGet("highlights")]
      public IActionResult Highlights()
      {
         return PartialView();
      }

      /// <summary>
      /// A page of the case studies I've written.
      /// </summary>
      /// <returns></returns>
      [HttpGet("case-studies")]
      public IActionResult CaseStudies()
      {
         return View();
      }

      /// <summary>
      /// Gets a page with the details of a case study.
      /// </summary>
      /// <param name="slug"></param>
      /// <returns></returns>
      [HttpGet("case-studies/{slug}")]
      public IActionResult CaseStudy(string slug)
      {
         return View();
      }

      private readonly ILogger<WorkController> _logger;
      private readonly ISystemClock _clock;
      private readonly IErrorStore _errorLogStore;
      private readonly DiagnosticsOptions _options;
      private readonly ISiteAssetService _assetService;
      private readonly IWebHostEnvironment _webHostEnvironment;
      private readonly IConfiguration _configuration;

      private readonly IProjectStore _projectStore;
      private readonly ISlugService _slugService;
   }
}
