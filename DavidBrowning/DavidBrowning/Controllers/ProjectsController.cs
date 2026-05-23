// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Data.Stores.Error;
using DavidBrowning.Data.Stores.Projects;
using DavidBrowning.Diagnostics;
using DavidBrowning.Models.Projects;
using DavidBrowning.Models.ViewModels.Projects;
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
         return View(await GetIndexModelAsync(cancellationToken));
      }

      /// <summary>
      /// Returns a page with all projects that have the given slug as a 
      /// technology stack tag.
      /// </summary>
      /// <param name="slug"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      [HttpGet("stacks/{slug}")]
      public IActionResult Stacks(string slug, CancellationToken cancellationToken)
      {
         if (string.IsNullOrWhiteSpace(slug))
         {
            return NotFound();
         }

         return View();
      }

      /// <summary>
      /// Returns a page with all projects that have the given slug as a 
      /// status.
      /// </summary>
      /// <param name="slug"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      [HttpGet("/statuses/{slug}")]
      public IActionResult Statuses(string slug, CancellationToken cancellationToken)
      {
         if (string.IsNullOrWhiteSpace(slug))
         {
            return NotFound();
         }

         return View();
      }

      /// <summary>
      /// Returns a page with all projects that have the given slug as an 
      /// origin.
      /// </summary>
      /// <param name="slug"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      [HttpGet("/origins/{slug}")]
      public IActionResult Origins(string slug, CancellationToken cancellationToken)
      {
         if (string.IsNullOrWhiteSpace(slug))
         {
            return NotFound();
         }

         return View();
      }
      /// <summary>
      /// Returns a page with all projects that have the given slug as a 
      /// project type.
      /// </summary>
      /// <param name="slug"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      [HttpGet("/types/{slug}")]
      public IActionResult Types(string slug, CancellationToken cancellationToken)
      {
         if (string.IsNullOrWhiteSpace(slug))
         {
            return NotFound();
         }

         return View();
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
         if (string.IsNullOrWhiteSpace(slug))
         {
            return NotFound();
         }

         return View();
      }

      private async Task<IndexViewModel> GetIndexModelAsync(
         CancellationToken cancellationToken)
      {
         var featured = await _projectStore.GetFeaturedProjectsAsync(cancellationToken);
         var all = await _projectStore.GetPublishedProjectsAsync(cancellationToken);
         return new IndexViewModel()
         {
            AllProjects = all,
            FeaturedProjects = featured,
         };
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
