// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Data.Stores.Error;
using DavidBrowning.Data.Stores.Writing;
using DavidBrowning.Diagnostics;
using DavidBrowning.Models.ViewModels;
using DavidBrowning.Models.Writing;
using DavidBrowning.Services.Cache;
using DavidBrowning.Services.Slugs;
using DavidBrowning.Services.Time;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DavidBrowning.Controllers;

[Route("writing")]
public class WritingController : Controller
{
   public WritingController(
      ILogger<WritingController> logger,
      ISystemClock clock,
      IErrorStore errorLogStore,
      IOptions<DiagnosticsOptions> options,
      IWebHostEnvironment environment,
      IConfiguration configuration,

      IWritingStore writingStore,
      ISlugService slugs,
      ISlugLookupService<WritingTag> tagStore)
   {
      _logger = logger;
      _clock = clock;
      _errorLogStore = errorLogStore;
      _options = options.Value;
      _webHostEnvironment = environment;
      _configuration = configuration;

      _writingStore = writingStore;
      _slugService = slugs;
      _tagLookup = tagStore;
   }

   public IActionResult Index()
   {
      return View();
   }

   /// <summary>
   /// Gets Published posts by writing tag.
   /// </summary>
   /// <param name="slug"></param>
   /// <param name="cancellationToken"></param>
   /// <returns></returns>
   [HttpGet("tags/{slug}")]
   public async Task<IActionResult> Tags(
      string slug,
      CancellationToken cancellationToken)
   {
      if (string.IsNullOrWhiteSpace(slug))
      {
         return NotFound();
      }

      var normalizedSlug = _slugService.CleanSlug(slug);
      var tag = await _tagLookup.GetBySlugAsync(
         normalizedSlug, cancellationToken);
      if (tag == null)
      {
         return NotFound();
      }

      var results = await _writingStore.GetPublishedPostsByTagSlugAsync(
            normalizedSlug, cancellationToken);
      FilteredResultsViewModel model = new()
      {
         PageTitle = $"{tag.DisplayName} Posts",
         FilterName = tag.DisplayName,
         FilterSlug = normalizedSlug,
         Results = results,
         ResultPartialName = "_PostCard"
      };

      return View("_FilteredResults", model);
   }

   /// <summary>
   /// Render one published post.
   /// </summary>
   /// <param name="slug"></param>
   /// <param name="cancellationToken"></param>
   /// <returns></returns>
   [HttpGet("details/{slug}")]
   public async Task<IActionResult> Details(
      string slug, 
      CancellationToken cancellationToken)
   {
      if (string.IsNullOrWhiteSpace(slug))
      {
         return NotFound();
      }

      await Task.CompletedTask;

      return View();
   }

   private readonly ILogger<WritingController> _logger;
   private readonly ISystemClock _clock;
   private readonly IErrorStore _errorLogStore;
   private readonly DiagnosticsOptions _options;
   private readonly IWebHostEnvironment _webHostEnvironment;
   private readonly IConfiguration _configuration;

   private readonly IWritingStore _writingStore;
   private readonly ISlugService _slugService;
   private readonly ISlugLookupService<WritingTag> _tagLookup;
}
