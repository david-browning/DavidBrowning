// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Models;

namespace DavidBrowning.Services.Assets
{
   public class DummySiteAssetService : ISiteAssetService
   {
      public Task<SiteAssetResult?> ResolveAsync(
         SiteAsset asset, 
         CancellationToken cancellationToken = default)
      {
         throw new System.NotImplementedException();
      }

      public Task<SiteAssetResult?> ResolveAsync(
         SiteAssetReference reference, 
         CancellationToken cancellationToken = default)
      {
         throw new System.NotImplementedException();
      }
   }
}
