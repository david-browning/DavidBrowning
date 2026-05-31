// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

namespace DavidBrowning.Models.ViewModels;

public sealed class RenderedContent
{
   /// <summary>
   /// Asset keys are root-relative logical keys, but they do not begin with /.
   /// </summary>
   public required string AssetKey { get; init; }

   /// <summary>
   /// MIME type of the original asset before rendering.
   /// </summary>
   public required string OriginalContentType { get; init; }

   public required string Html { get; init; }
}