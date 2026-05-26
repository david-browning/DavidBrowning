// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Data.Stores.Error;
using DavidBrowning.Data.Stores.Projects;
using DavidBrowning.Diagnostics;
using DavidBrowning.Services.Assets;
using DavidBrowning.Services.Rendering;
using DavidBrowning.Services.Time;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace DavidBrowning.Controllers;

[Route("content")]
public class ContentController : Controller
{
   public ContentController(
      ILogger<ContentController> logger,
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
   }

   [HttpGet("{**assetKey}")]
   public async Task<IActionResult> GetAsync(
      string assetKey,
      CancellationToken cancellationToken)
   {
      if (_webHostEnvironment.IsDevelopment())
      {
         Response.Headers.CacheControl = "no-cache";
      }
      else
      {
         Response.Headers.CacheControl = "public, max-age=3600";
      }

      var asset = await _contentService.GetAssetAsync(
         assetKey, cancellationToken);
      var stream = await _contentService.OpenReadAsync(
         assetKey, cancellationToken);
      return File(
         fileStream: stream,
         contentType: _contentService.GetAssetFileType(assetKey),
         lastModified: asset.LastModifiedUtc,
         entityTag: new EntityTagHeaderValue(asset.EntityTag));
   }

   private readonly ILogger<ContentController> _logger;
   private readonly ISystemClock _clock;
   private readonly IErrorStore _errorLogStore;
   private readonly DiagnosticsOptions _options;
   private readonly IWebHostEnvironment _webHostEnvironment;
   private readonly IConfiguration _configuration;

   private readonly IContentService _contentService;
}
