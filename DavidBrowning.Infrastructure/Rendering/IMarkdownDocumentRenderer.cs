// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using DavidBrowning.Models;

namespace DavidBrowning.Infrastructure.Rendering;

public interface IMarkdownDocumentRenderer
{
   Task<RenderedContent> RenderAsync(
      string documentKey,
      string markdown,
      IReadOnlyCollection<LinkedAssetReference> assetLinks,
      CancellationToken cancellationToken = default);
}