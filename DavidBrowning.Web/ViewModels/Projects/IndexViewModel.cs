// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Collections.Generic;
using DavidBrowning.Models.Publishing;

namespace DavidBrowning.Web.ViewModels.Projects;

public class IndexViewModel
{
   public required string PageTitle { get; init; }

   public required string HeroTitle { get; init; }

   public required string Lede { get; init; }

   public required IReadOnlyList<PublishedProject> AllProjects { get; set; }

   public required IReadOnlyList<PublishedProject> FeaturedProjects { get; set; }

   public required SeoMetadataViewModel Seo { get; init; }
}
