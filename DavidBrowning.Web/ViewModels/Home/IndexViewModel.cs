// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using DavidBrowning.Models.Publishing;

namespace DavidBrowning.Web.ViewModels.Home;

public sealed class IndexViewModel
{
   public required string PageTitle { get; init; }

   public required string HeroTitle { get; init; }

   public required string Lede { get; init; }

   public required PublishedProject FeaturedProject { get; init; }

   public required PublishedWriting FeaturedPost { get; init; }

   public InterestCardViewModel? WorkbenchInterest { get; init; }

   public required SeoMetadataViewModel Seo { get; init; }
}