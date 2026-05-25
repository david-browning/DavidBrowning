// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Data.Stores.Error;
using DavidBrowning.Data.Stores.Projects;
using DavidBrowning.Diagnostics;
using DavidBrowning.Models.ViewModels.About;
using DavidBrowning.Services.Assets;
using DavidBrowning.Services.Rendering;
using DavidBrowning.Services.Time;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DavidBrowning.Controllers
{
   [Route("about")]
   public class AboutController : Controller
   {
      public AboutController(
         ILogger<WorkController> logger,
         ISystemClock clock,
         IErrorStore errorLogStore,
         IOptions<DiagnosticsOptions> options,
         IWebHostEnvironment environment,
         IConfiguration configuration,

         IContentService contentService,
         IContentRenderer contentRenderer,
         IProjectStore projectStore)
      {
         _logger = logger;
         _clock = clock;
         _errorLogStore = errorLogStore;
         _options = options.Value;
         _webHostEnvironment = environment;
         _configuration = configuration;

         _contentService = contentService;
         _renderer = contentRenderer;
         _projectStore = projectStore;
      }

      public async Task<IActionResult> Index(CancellationToken cancellationToken)
      {
         return View(await GetIndexModelAsync(cancellationToken));
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

      private async Task<IndexViewModel> GetIndexModelAsync(
         CancellationToken cancellationToken)
      {
         var heroSubtitleContent = await _contentService.GetContentAsync(
            "Blurbs/About.md",
            cancellationToken);
         var heroSubtitleRendered = await _renderer.RenderAsync(
            heroSubtitleContent, cancellationToken);

         return new IndexViewModel()
         {
            PageTitle = "About",
            HeroTitle = "About me.",
            HeroSubtitle = heroSubtitleRendered,
         };
      }

      private readonly ILogger<WorkController> _logger;
      private readonly ISystemClock _clock;
      private readonly IErrorStore _errorLogStore;
      private readonly DiagnosticsOptions _options;
      private readonly IWebHostEnvironment _webHostEnvironment;
      private readonly IConfiguration _configuration;

      private readonly IProjectStore _projectStore;
      private readonly IContentService _contentService;
      private readonly IContentRenderer _renderer;
   }
}
