// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

namespace DavidBrowning.Services.Assets
{
   /// <summary>
   /// This is returned when an asset is looked up from a blob storage.
   /// </summary>
   public sealed class SiteAssetResult
   {
      public required string Url { get; init; }

      public string? ContentType { get; init; }

      public string? AltText { get; init; }

      public long? SizeBytes { get; init; }
   }
}