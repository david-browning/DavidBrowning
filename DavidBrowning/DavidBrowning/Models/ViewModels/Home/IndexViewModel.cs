// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using DavidBrowning.Models.Projects;
using DavidBrowning.Models.Writing;

namespace DavidBrowning.Models.ViewModels.Home;

public sealed class IndexViewModel
{
   public required string PageTitle { get; init; }

   public required string HeroTitle { get; init; }

   public required string Lede { get; init; }

   public required Project FeaturedProject { get; init; }

   public required Post FeaturedPost { get; init; }

   public InterestCardViewModel? WorkbenchInterest { get; init; }
}