// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Models.ViewModels;
using DavidBrowning.Services.Assets;
using DavidBrowning.Services.Cache;
using Microsoft.Extensions.Logging;

namespace DavidBrowning.Services.Rendering;

public class CachedContentPipeline : IContentPipeline
{
   public CachedContentPipeline(
      ILogger<CachedContentPipeline> logger,
      IContentPipeline pipeline,
      RenderedContentMemoryCache cache)
   {
      _logger = logger;
      _pipeline = pipeline;
      _cache = cache;
   }

   public async Task<RenderedContent?> GetRenderedContentAsync(
      string assetKey,
      ContentRenderOptions? options = null,
      CancellationToken cancellationToken = default)
   {
      var cacheKey = GetCacheKey(assetKey);
      return await _cache.GetOrCreateAsync(
         cacheKey,
         token => _pipeline.GetRenderedContentAsync(
            assetKey, options, token),
            cancellationToken);
   }

   private string GetCacheKey(string assetKey)
   {
      return $"content-asset:{assetKey}";
   }

   private readonly ILogger<CachedContentPipeline> _logger;
   private readonly IContentPipeline _pipeline;
   private readonly RenderedContentMemoryCache _cache;

}
