// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using DavidBrowning.Models.ViewModels;

namespace DavidBrowning.Services.Assets
{
   /// <summary>
   /// Represents a piece of content (Like text, an image, markdown) stored
   /// "somewhere".
   /// </summary>
   public sealed class StoredContent
   {
      public required string AssetKey { get; init; }

      public required ContentSourceFormat SourceFormat { get; init; }

      public required string SourceText { get; init; }
   }
}
