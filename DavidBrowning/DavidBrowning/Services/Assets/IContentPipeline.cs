// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Models.ViewModels;
using DavidBrowning.Services.Rendering;

namespace DavidBrowning.Services.Assets
{
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

      Task<T> GetJsonFileContentAsync<T>(
         string assetKey,
         CancellationToken cancellationToken = default);
   }
}
