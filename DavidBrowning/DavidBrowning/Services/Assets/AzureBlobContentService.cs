// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DavidBrowning.Services.Assets;

public sealed class AzureBlobContentService : IContentService
{
   public Task<StoredAsset> GetAssetAsync(
      string assetKey,
      CancellationToken cancellationToken = default)
   {
      throw new System.NotImplementedException();
   }

   public Task<Stream> OpenReadAsync(
      string assetKey,
      CancellationToken cancellationToken = default)
   {
      throw new System.NotImplementedException();
   }

   public string GetAssetFileType(string assetKey)
   {
      return AssetHelpers.GetContentType(assetKey);
   }
}
