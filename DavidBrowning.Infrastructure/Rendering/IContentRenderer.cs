// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using DavidBrowning.Infrastructure.Assets;
using DavidBrowning.Models;

namespace DavidBrowning.Infrastructure.Rendering;

public interface IContentRenderer
{
   /// <summary>
   /// Renders a stored asset into displayable HTML content.
   /// </summary>
   /// <param name="asset">
   /// The stored asset to render.
   /// </param>
   /// <param name="cancellationToken">
   /// A token used to cancel the render operation.
   /// </param>
   /// <returns>
   /// The rendered content.
   /// </returns>
   Task<RenderedContent> RenderAsync(
      StoredAsset content,
      ContentRenderOptions? options = null,
      CancellationToken cancellationToken = default);
}
