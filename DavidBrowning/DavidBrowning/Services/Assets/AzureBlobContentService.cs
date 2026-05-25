// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Models.ViewModels;

namespace DavidBrowning.Services.Assets
{
   public sealed class AzureBlobContentService : IContentService
   {
      public Task<StoredContent> GetContentAsync(
         string assetKey,
         CancellationToken cancellationToken = default)
      {
         throw new System.NotImplementedException();
      }
   }
}
