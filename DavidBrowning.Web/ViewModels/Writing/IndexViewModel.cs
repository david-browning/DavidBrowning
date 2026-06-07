// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Collections.Generic;
using DavidBrowning.Models.Writing;

namespace DavidBrowning.Web.ViewModels.Writing;

public class IndexViewModel
{
   public required string PageTitle { get; init; }

   public required string HeroTitle { get; init; }

   public required string Lede { get; init; }

   public required IReadOnlyList<Post> Posts { get; init; }

   public required IReadOnlyList<Post> FeaturedPosts { get; init; }

   public required PagerViewModel Pager { get; init; }

   public bool ShowFeaturedPosts =>
      Pager.CurrentPage == 1 && FeaturedPosts.Count > 0;

   public required SeoMetadataViewModel Seo { get; init; }
}
