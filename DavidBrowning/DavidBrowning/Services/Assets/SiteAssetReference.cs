// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

namespace DavidBrowning.Services.Assets
{
   /// <summary>
   /// The necessary data to get an asset from a blob storage.
   /// Blob storage does not necessarily have to be Azure storage blobs, but 
   /// some abstract storage solution where you look up by "container" and 
   /// "name".
   /// </summary>
   public sealed class SiteAssetReference
   {
      public required string BlobContainer { get; init; }

      /// <summary>
      /// Name of the asset inside the container.
      /// </summary>
      public required string BlobName { get; init; }
   }
}