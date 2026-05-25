// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Models.ViewModels;

namespace DavidBrowning.Services.Assets
{
   public class DummyContentService : IContentService
   {
      public Task<StoredContent> GetContentAsync(
         string assetKey, 
         CancellationToken cancellationToken = default)
      {
         var ret = new StoredContent()
         {
            AssetKey = assetKey,
            SourceFormat = ContentSourceFormat.PlainText,
            SourceText = "Test Content",
         };

         return Task.FromResult(ret);
      }
   }
}
