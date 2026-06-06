// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Models;
using DavidBrowning.Services.Assets;

namespace DavidBrowning.Services.Rendering;

public class BasicContentPipeline : IContentPipeline
{
   public BasicContentPipeline(
      IContentStore contentService,
      IContentRenderer contentRenderer)
   {
      _contentRenderer = contentRenderer;
      _contentStore = contentService;
   }

   public async Task<RenderedContent> GetRenderedContentAsync(
      string assetKey,
      ContentRenderOptions? options = null,
      CancellationToken cancellationToken = default)
   {
      var asset = await _contentStore.GetAssetAsync(
         assetKey, cancellationToken);
      var rendered = await _contentRenderer.RenderAsync(
         asset, options, cancellationToken);
      return rendered;
   }

   private readonly IContentStore _contentStore;
   private readonly IContentRenderer _contentRenderer;
}