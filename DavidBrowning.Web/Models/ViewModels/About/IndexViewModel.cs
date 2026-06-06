// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Collections.Generic;

namespace DavidBrowning.Models.ViewModels.About;

public class IndexViewModel
{
   public required string PageTitle { get; init; }

   public required string HeroTitle { get; init; }

   public required string HeroSubtitle { get; init; }

   public required string AboutMe { get; init; }

   public required RenderedContent MeImage { get; init; }

   public required IReadOnlyList<InterestCardViewModel> Interests { get; init; }
}
