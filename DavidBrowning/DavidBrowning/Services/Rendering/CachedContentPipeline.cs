// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Models.ViewModels;
using DavidBrowning.Services.Assets;
using DavidBrowning.Services.Cache;

namespace DavidBrowning.Services.Rendering;

public sealed class CachedContentPipeline : IContentPipeline
{
   public CachedContentPipeline(
      IContentPipeline pipeline,
      RenderedContentMemoryCache cache)
   {
      _pipeline = pipeline;
      _cache = cache;
   }

   public Task<RenderedContent?> GetRenderedContentAsync(
      string assetKey,
      ContentRenderOptions? options = null,
      CancellationToken cancellationToken = default)
   {
      var cacheKey = GetCacheKey(assetKey, options);

      return _cache.GetOrCreateAsync(
         cacheKey,
         token => _pipeline.GetRenderedContentAsync(
            assetKey, options, token),
         cancellationToken);
   }

   private static string GetCacheKey(
      string assetKey,
      ContentRenderOptions? options)
   {
      var altText = options?.AltText ?? string.Empty;
      var cssClass = options?.CssClass ?? string.Empty;

      return
         $"content-asset:{assetKey}:" +
         $"alt:{altText.Length}:{altText}:" +
         $"class:{cssClass.Length}:{cssClass}";
   }

   private readonly IContentPipeline _pipeline;
   private readonly RenderedContentMemoryCache _cache;
}