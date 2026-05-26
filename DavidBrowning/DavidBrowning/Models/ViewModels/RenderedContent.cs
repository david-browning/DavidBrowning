// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
namespace DavidBrowning.Models.ViewModels
{
   public enum ContentSourceFormat
   {
      Unknown = 0,
      Markdown = 1,
      Html = 2,
      PlainText = 3,
      Image = 4,
      Json = 5,
   }

   public sealed class RenderedContent
   {
      /// <summary>
      /// Asset keys are root-relative logical keys, but they do not begin with /
      /// </summary>
      public required string AssetKey { get; init; }

      /// <summary>
      /// The format of the authored/source content before rendering.
      /// </summary>
      public required ContentSourceFormat OriginalSourceFormat { get; init; }

      public required string Html { get; init; }
   }
}
