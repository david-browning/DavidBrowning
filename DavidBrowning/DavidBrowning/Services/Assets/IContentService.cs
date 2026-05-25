// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Threading;
using System.Threading.Tasks;

namespace DavidBrowning.Services.Assets
{
   public interface IContentService
   {
      /// <summary>
      /// Gets authored source content from the configured content store.
      /// </summary>
      /// <param name="assetKey">
      /// A root-relative logical key. Asset keys do not begin with a slash.
      /// </param>
      /// <param name="cancellationToken">
      /// A token used to cancel the content lookup operation.
      /// </param>
      /// <returns>
      /// The source content associated with the requested asset key.
      /// </returns>
      Task<StoredContent> GetContentAsync(
         string assetKey,
         CancellationToken cancellationToken = default);
   }
}
