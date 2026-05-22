// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using DavidBrowning.Data.Stores.Error;
using DavidBrowning.Data.Stores.Projects;
using DavidBrowning.Diagnostics;
using DavidBrowning.Services.Assets;
using DavidBrowning.Services.Time;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DavidBrowning.Controllers
{
   public class AboutController : Controller
   {
      public AboutController(
         ILogger<WorkController> logger,
         ISystemClock clock,
         IErrorStore errorLogStore,
         IOptions<DiagnosticsOptions> options,
         ISiteAssetService assetService, 
         IWebHostEnvironment environment,
         IConfiguration configuration,

         IProjectStore projectStore)
      {
         _logger = logger;
         _clock = clock;
         _errorLogStore = errorLogStore;
         _options = options.Value;
         _assetService = assetService;
         _webHostEnvironment = environment;
         _configuration = configuration;

         _projectStore = projectStore;
      }

      [HttpGet]
      public IActionResult Index()
      {
         return View();
      }

      /// <summary>
      /// Returns a page with information about the site.
      /// </summary>
      /// <returns></returns>
      [HttpGet("this")]
      public IActionResult This()
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
   }
}
