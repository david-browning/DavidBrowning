// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DavidBrowning.Services.Assets;

public interface IContentStore
{
   /// <summary>
   /// Gets asset metadata and text content, when applicable.
   /// </summary>
   /// <param name="assetKey">
   /// A root-relative logical key. Asset keys do not begin with a slash.
   /// </param>
   /// <param name="cancellationToken">
   /// A token used to cancel the content lookup operation.
   /// </param>
   /// <returns>
   /// Metadata and optional text content for the requested asset.
   /// </returns>
   Task<StoredAsset> GetAssetAsync(
      string assetKey,
      CancellationToken cancellationToken = default);

   /// <summary>
   /// Opens a readable stream for an asset.
   /// </summary>
   Task<Stream> OpenReadAsync(
      string assetKey,
      CancellationToken cancellationToken = default);
}