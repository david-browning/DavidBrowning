// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

namespace DavidBrowning.Infrastructure.Assets;

public sealed class DummyContentStore : IContentStore
{
   public Task<StoredAsset> GetAssetAsync(
      string assetKey,
      CancellationToken cancellationToken = default)
   {
      var now = DateTimeOffset.UtcNow;

      StoredAsset ret = new()
      {
         AssetKey = assetKey,
         ContentType = "text/plain",
         Text = "Test Content",
         ContentLength = 12,
         EntityTag = AssetHelpers.GetEntityTag(assetKey, now, 12),
         LastModifiedUtc = now,
      };

      return Task.FromResult(ret);
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