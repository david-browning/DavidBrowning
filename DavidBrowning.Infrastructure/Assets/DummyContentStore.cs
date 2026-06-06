// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DavidBrowning.Services.Assets;

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

   public Task<Stream> OpenReadAsync(
      string assetKey,
      CancellationToken cancellationToken = default)
   {
      throw new NotImplementedException();
   }
}