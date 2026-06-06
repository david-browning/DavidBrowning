// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using DavidBrowning.Infrastructure.Rendering;
using DavidBrowning.Models;

namespace DavidBrowning.Infrastructure.Assets;

/// <summary>
/// A content pipeline handles the entire process of fetching content,
/// rendering it, and caching it.
/// </summary>
public interface IContentPipeline
{
   Task<RenderedContent> GetRenderedContentAsync(
      string assetKey,
      ContentRenderOptions? options = null,
      CancellationToken cancellationToken = default);
}
