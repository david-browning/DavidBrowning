// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

namespace DavidBrowning.Models.Published;

public sealed record PublishedProject
{
   public required string Slug { get; init; }

   public required string Name { get; init; }

   public string? Description { get; init; }

   public string? Role { get; init; }

   public string? ContributionSummary { get; init; }

   public bool IsFeatured { get; init; }

   public int SortOrder { get; init; }

   public DateOnly? StartDate { get; init; }

   public DateOnly? EndDate { get; init; }

   public string? DateDisplayText { get; init; }

   public DateTime UpdatedAtUtc { get; init; }

   public required PublishedLookup Status { get; init; }

   public required PublishedLookup Type { get; init; }

   public required PublishedLookup Origin { get; init; }

   public IReadOnlyList<PublishedLookup> Tags { get; init; } =
      Array.Empty<PublishedLookup>();

   public IReadOnlyList<PublishedLookup> StackTags { get; init; } =
      Array.Empty<PublishedLookup>();

   public IReadOnlyList<PublishedLink> Links { get; init; } =
      Array.Empty<PublishedLink>();

   public PublishedTextContent? DetailsContent { get; init; }

   public IReadOnlyList<string> RelatedWritingSlugs { get; init; } =
      Array.Empty<string>();
}