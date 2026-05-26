// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Data.Stores.Error;
using DavidBrowning.Data.Stores.Writing;
using DavidBrowning.Diagnostics;
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
      ISlugService slugs)
   {
      _logger = logger;
      _clock = clock;
      _errorLogStore = errorLogStore;
      _options = options.Value;
      _webHostEnvironment = environment;
      _configuration = configuration;

      _writingStore = writingStore;
      _slugService = slugs;
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
   public async Task<IActionResult> Tag(string slug, CancellationToken cancellationToken)
   {
      if (string.IsNullOrWhiteSpace(slug))
      {
         return NotFound();
      }

      await Task.CompletedTask;

      return View();
   }

   /// <summary>
   /// Render one published post.
   /// </summary>
   /// <param name="slug"></param>
   /// <param name="cancellationToken"></param>
   /// <returns></returns>
   [HttpGet("{slug}")]
   public async Task<IActionResult> Details(string slug, CancellationToken cancellationToken)
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
}
