// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Models.ViewModels;

namespace DavidBrowning.Services.Rendering;

public interface IMarkdownDocumentRenderer
{
   Task<RenderedContent> RenderAsync(
      string documentKey,
      string markdown,
      IReadOnlyCollection<LinkedAssetReference> assetLinks,
      CancellationToken cancellationToken = default);
}