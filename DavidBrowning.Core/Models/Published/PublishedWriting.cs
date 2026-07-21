// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

namespace DavidBrowning.Models.Published;

public sealed record PublishedWriting
{
   public required string Slug { get; init; }

   public required string Title { get; init; }

   public string? Summary { get; init; }

   public bool IsFeatured { get; init; }

   public DateTime? PublishedAtUtc { get; init; }

   public DateTime LastUpdatedAtUtc { get; init; }

   public PublishedLookup? Style { get; init; }

   public IReadOnlyList<PublishedLookup> Tags { get; init; } =
      Array.Empty<PublishedLookup>();

   public required PublishedTextContent Content { get; init; }

   public IReadOnlyList<string> RelatedProjectSlugs { get; init; } =
      Array.Empty<string>();
}