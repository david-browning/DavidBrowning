// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Collections.Generic;
using DavidBrowning.Models.Projects;

namespace DavidBrowning.Models.ViewModels.Projects
{
   public class IndexViewModel
   {
      public required IReadOnlyList<Project> AllProjects { get; set; }

      public required IReadOnlyList<Project> FeaturedProjects { get; set; }
   }
}
