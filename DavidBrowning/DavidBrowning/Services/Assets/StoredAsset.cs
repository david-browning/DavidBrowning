// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using DavidBrowning.Models.ViewModels;

namespace DavidBrowning.Services.Assets;

/// <summary>
/// Represents a piece of content (Like text, an image, markdown) stored
/// "somewhere".
/// </summary>
public sealed class StoredAsset
{
   public required string AssetKey { get; init; }

   public required ContentSourceFormat SourceFormat { get; init; }

   /// <summary>
   /// Size in bytes of the content.
   /// </summary>
   public long? ContentLength { get; init; }

   /// <summary>
   /// File text. If this is a binary blob like an image, then this will
   /// be empty.
   /// </summary>
   public string? Text { get; init; }

   public required string EntityTag { get; init; }

   /// <summary>
   /// When the asset was last modified.
   /// </summary>
   public required DateTimeOffset LastModifiedUtc { get; init; }
}
