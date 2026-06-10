// Copyright Â© 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Collections.Generic;
using System.Linq;
using DavidBrowning.Models.Projects;

namespace DavidBrowning.Admin.ViewModels.Projects;

public class ProjectVisibilityListViewModel
{
   public List<ProjectVisibility>? Items { get; set; }

   public ProjectVisibilityListViewModel(IEnumerable<ProjectVisibility> visibilities)
   {
      Items = visibilities.ToList();
   }
}
