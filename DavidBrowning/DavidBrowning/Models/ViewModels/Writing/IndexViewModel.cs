// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Collections.Generic;
using DavidBrowning.Models.Writing;

namespace DavidBrowning.Models.ViewModels.Writing;

public class IndexViewModel
{
   public required string PageTitle { get; init; }

   public required string HeroTitle { get; init; }

   public required string HeroSubtitle { get; init; }

   public required IReadOnlyList<Post> AllPosts { get; init; }

   public required IReadOnlyList<Post> FeaturedPosts { get; init; }
}
