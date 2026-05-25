// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Models.ViewModels;
using DavidBrowning.Services.Assets;

namespace DavidBrowning.Services.Rendering
{
   public interface IContentRenderer
   {
      /// <summary>
      /// Renders authored source content into displayable HTML content.
      /// </summary>
      /// <param name="content">
      /// The source content to render.
      /// </param>
      /// <param name="cancellationToken">
      /// A token used to cancel the render operation.
      /// </param>
      /// <returns>
      /// The rendered content.
      /// </returns>
      Task<RenderedContent> RenderAsync(
         StoredContent content,
         CancellationToken cancellationToken = default);
   }
}
