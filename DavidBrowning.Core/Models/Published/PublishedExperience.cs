// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using System.Diagnostics.CodeAnalysis;
using DavidBrowning.Models.Work;

namespace DavidBrowning.Models.Published;

public sealed record PublishedExperience
{
   public required string Organization { get; init; }

   public string? Location { get; init; }

   public DateOnly? StartDate { get; init; }

   public DateOnly? EndDate { get; init; }

   public string? DateDisplayText { get; init; }

   public int SortOrder { get; init; }

   public IReadOnlyList<PublishedExperienceRole> Roles { get; init; } =
      Array.Empty<PublishedExperienceRole>();

   public PublishedExperience()
   {

   }

   [SetsRequiredMembers]
   public PublishedExperience(Experience e)
   {
      
   }
}

public sealed record PublishedExperienceRole
{
   public required string Title { get; init; }

   public string? Summary { get; init; }

   public int SortOrder { get; init; }

   public IReadOnlyList<string> Bullets { get; init; } =
      Array.Empty<string>();
}