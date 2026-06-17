// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

namespace DavidBrowning.Infrastructure.Assets;

public sealed class AzureBlobContentStore : IContentStore
{
   public Task<StoredAsset> GetAssetAsync(
      string assetKey,
      CancellationToken cancellationToken = default)
   {
      throw new NotImplementedException();
   }

   public Task<bool> AssetExists(
      string assetKey,
      CancellationToken cancellationToken = default)
   {
      throw new NotImplementedException();
   }

   public Task<Stream> OpenReadAsync(
      string assetKey,
      CancellationToken cancellationToken = default)
   {
      throw new NotImplementedException();
   }

   public Task<ContentWriteResults> WriteAsync(string assetKey,
      Stream
      contentStream,
      CancellationToken cancellationToken = default)
   {
      throw new NotImplementedException();
   }

   public Task DeleteFileAsync(
      string assetKey,
      CancellationToken cancellationToken = default)
   {
      throw new NotImplementedException();
   }
}