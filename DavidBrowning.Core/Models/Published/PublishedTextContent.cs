// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

namespace DavidBrowning.Models.Published;

public sealed record PublishedLookup
{
   public required string Slug { get; init; }

   public required string DisplayName { get; init; }

   public string? Description { get; init; }
}

public sealed record PublishedLink
{
   public required string LinkType { get; init; }

   public required string DisplayName { get; init; }

   public required string Url { get; init; }

   public int SortOrder { get; init; }
}

public sealed record PublishedAssetReference
{
   public required string ReferenceKey { get; init; }

   public required string AssetKey { get; init; }

   public string? AltText { get; init; }

   public string? Caption { get; init; }
}

public enum PublishedContentFormat
{
   Markdown,
   PlainText,
}

public sealed record PublishedTextContent
{
   /// <summary>
   /// A stable rendering-cache identity.
   /// </summary>
   public required string CacheKey { get; init; }

   public PublishedContentFormat Format { get; init; }

   public required string Source { get; init; }

   public IReadOnlyList<PublishedAssetReference> AssetReferences { get; init; } =
      Array.Empty<PublishedAssetReference>();
}