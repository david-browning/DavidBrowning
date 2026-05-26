// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Models.ViewModels;

namespace DavidBrowning.Services.Assets;

public class DummyContentService : IContentService
{
   public Task<StoredAsset> GetAssetAsync(
      string assetKey,
      CancellationToken cancellationToken = default)
   {
      var now = DateTimeOffset.UtcNow;
      var ret = new StoredAsset()
      {
         AssetKey = assetKey,
         SourceFormat = ContentSourceFormat.PlainText,
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
      throw new System.NotImplementedException();
   }

   public string GetAssetFileType(string assetKey)
   {
      return AssetHelpers.GetContentType(assetKey);
   }
}