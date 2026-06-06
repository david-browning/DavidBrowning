// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

namespace DavidBrowning.Infrastructure.Assets;

/// <summary>
/// Represents content stored by an IContentStore.
/// </summary>
public sealed class StoredAsset
{
   public required string AssetKey { get; init; }

   public required string ContentType { get; init; }

   /// <summary>
   /// Size in bytes of the content.
   /// </summary>
   public long? ContentLength { get; init; }

   /// <summary>
   /// File text. Null for binary assets such as images.
   /// </summary>
   public string? Text { get; init; }

   public required string EntityTag { get; init; }

   /// <summary>
   /// When the asset was last modified.
   /// </summary>
   public required DateTimeOffset LastModifiedUtc { get; init; }
}