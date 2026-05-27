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
      IAsyncCache asyncCache)
   {
      _logger = logger;
      _pipeline = pipeline;
      _asyncCache = asyncCache;
   }

   public async Task<RenderedContent> GetRenderedContentAsync(
      string assetKey,
      ContentRenderOptions? options = null,
      CancellationToken cancellationToken = default)
   {
      var cacheKey = GetCacheKey(assetKey);
      return await _asyncCache.GetOrCreateAsync(
         cacheKey,
         token => _pipeline.GetRenderedContentAsync(
         assetKey, options, cancellationToken),
         cancellationToken);
   }

   public async Task<T> GetJsonFileContentAsync<T>(
      string assetKey,
      CancellationToken cancellationToken = default)
   {
      var cacheKey = GetJsonCacheKey<T>(assetKey);
      return await _asyncCache.GetOrCreateAsync(
         cacheKey,
         token => _pipeline.GetJsonFileContentAsync<T>(
            assetKey, cancellationToken),
         cancellationToken);
   }

   private string GetCacheKey(string assetKey)
   {
      return $"content-asset:{assetKey}";
   }

   private string GetJsonCacheKey<T>(string assetKey)
   {
      return $"json-content:{typeof(T).FullName}:{assetKey}";
   }

   private readonly ILogger<CachedContentPipeline> _logger;
   private readonly IContentPipeline _pipeline;
   private readonly IAsyncCache _asyncCache;

}
