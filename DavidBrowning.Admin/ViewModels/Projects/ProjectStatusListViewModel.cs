// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Collections.Generic;
using System.Linq;
using DavidBrowning.Models.Projects;

namespace DavidBrowning.Admin.ViewModels.Projects;

public class ProjectStatusListViewModel
{
   public IReadOnlyList<ProjectStatus>? Items { get; set; }

   public ProjectStatusListViewModel(IEnumerable<ProjectStatus> statuses)
   {
      Items = statuses.ToList();
   }
}
