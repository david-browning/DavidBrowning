// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Models.ViewModels;
using DavidBrowning.Services.Rendering;
using Microsoft.Extensions.Logging;

namespace DavidBrowning.Services.Assets;

/// <summary>
/// A content pipeline that caches data in memory.
/// </summary>
public class MemoryCacheContentPipeline : IContentPipeline
{
   public MemoryCacheContentPipeline(
      ILogger<MemoryCacheContentPipeline> logger,
      IContentService contentService,
      IContentRenderer contentRenderer)
   {
      _logger = logger;
      _contentRenderer = contentRenderer;
      _contentStore = contentService;
   }

   public Task<RenderedContent> GetRenderedContentAsync(
      string assetKey,
      ContentRenderOptions? options = null,
      CancellationToken cancellationToken = default)
   {
      throw new System.NotImplementedException();
   }

   public Task<T> GetJsonFileContentAsync<T>(
      string assetKey,
      CancellationToken cancellationToken = default)
   {
      throw new System.NotImplementedException();
   }

   private readonly ILogger<MemoryCacheContentPipeline> _logger;
   private readonly IContentService _contentStore;
   private readonly IContentRenderer _contentRenderer;
}
