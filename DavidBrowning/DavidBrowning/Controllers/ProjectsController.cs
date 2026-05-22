// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Data.Stores.Error;
using DavidBrowning.Data.Stores.Projects;
using DavidBrowning.Diagnostics;
using DavidBrowning.Models.Projects;
using DavidBrowning.Services.Assets;
using DavidBrowning.Services.Cache;
using DavidBrowning.Services.Slugs;
using DavidBrowning.Services.Time;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DavidBrowning.Controllers
{
   [Route("projects")]
   public class ProjectsController : Controller
   {
      public ProjectsController(
         ILogger<ProjectsController> logger,
         ISystemClock clock,
         IErrorStore errorLogStore,
         IOptions<DiagnosticsOptions> options,
         ISiteAssetService assetService,
         IWebHostEnvironment environment,
         IConfiguration configuration,

         IProjectStore project,
         ISlugService slugService,
         ISlugLookupService<ProjectVisibility> projectLookup)
      {
         _logger = logger;
         _clock = clock;
         _errorLogStore = errorLogStore;
         _options = options.Value;
         _assetService = assetService;
         _webHostEnvironment = environment;
         _configuration = configuration;

         _projectStore = project;
         _slugService = slugService;
         _projectVisibilityLookup = projectLookup;
      }

      public async Task<IActionResult> Index(CancellationToken cancellationToken)
      {
         int? publishedVisibilityId = await _projectVisibilityLookup.GetIdBySlugAsync("private");
         return View();
      }

      /// <summary>
      /// A page of featured projects.
      /// </summary>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      [HttpGet("featured")]
      public IActionResult Featured(CancellationToken cancellationToken)
      {
         return View();
      }

      /// <summary>
      /// Gets a partial view with cards of the featured projects.
      /// </summary>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      [HttpGet("featured-cards")]
      public IActionResult FeaturedCards(CancellationToken cancellationToken)
      {
         var numCards = _configuration.GetValue<int>("Content:CardCollectionLength");
         return PartialView();
      }

      /// <summary>
      /// Returns a partial view of a card with project highlights
      /// </summary>
      /// <param name="slug"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      [HttpGet("card/{slug}")]
      public IActionResult Card(string slug, CancellationToken cancellationToken)
      {
         return PartialView();
      }

      /// <summary>
      /// Gets a page with project details.
      /// </summary>
      /// <param name="slug"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      [HttpGet("{slug}")]
      public IActionResult Details(string slug, CancellationToken cancellationToken)
      {
         return View();
      }


      private readonly ILogger<ProjectsController> _logger;
      private readonly ISystemClock _clock;
      private readonly IErrorStore _errorLogStore;
      private readonly DiagnosticsOptions _options;
      private readonly ISiteAssetService _assetService;
      private readonly IWebHostEnvironment _webHostEnvironment;
      private readonly IConfiguration _configuration;

      private readonly IProjectStore _projectStore;
      private readonly ISlugService _slugService;
      private readonly ISlugLookupService<ProjectVisibility> _projectVisibilityLookup;
   }
}
