// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using System.Diagnostics.CodeAnalysis;

namespace DavidBrowning.Models.Published;

public sealed record PublishedInterest
{
   public required string Name { get; init; }

   public required string Description { get; init; }

   public string? IconClass { get; init; }

   public int SortOrder { get; init; }

   public PublishedInterest()
   {

   }

   [SetsRequiredMembers]
   public PublishedInterest(Interest i)
   {
      Name = i.DisplayName;
      Description = i.Summary;
      IconClass = i.IconCssClass;
      SortOrder = i.SortOrder;
   }
}