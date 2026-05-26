// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.IO;
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
      Task<StoredAsset> GetAssetAsync(
         string assetKey,
         CancellationToken cancellationToken = default);

      /// <summary>
      /// Opens a readable stream for an asset.
      /// </summary>
      /// <param name="assetKey">
      /// A root-relative logical key. Asset keys do not begin with a slash.
      /// </param>
      /// <param name="cancellationToken">
      /// A token used to cancel the stream open operation.
      /// </param>
      /// <returns>
      /// A readable stream for the requested asset.
      /// </returns>
      Task<Stream> OpenReadAsync(
         string assetKey,
         CancellationToken cancellationToken = default);

      /// <summary>
      /// Returning File() from a controller requires the type of file to be
      /// included. This looks up what type of file to pass to the controller.
      /// </summary>
      /// <param name="asset"></param>
      /// <returns></returns>
      string GetAssetFileType(string assetKey);
   }
}
