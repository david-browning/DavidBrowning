// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Collections.Generic;
using System.Linq;
using DavidBrowning.Models.Projects;

namespace DavidBrowning.Admin.ViewModels.Projects;

public class ProjectOriginListViewModel
{
   public List<ProjectOriginEditViewModel>? Items { get; set; }

   public ProjectOriginListViewModel(      IEnumerable<ProjectOrigin> origins)
   {
      Items = origins.Select(o => new ProjectOriginEditViewModel(o)).ToList();
   }
}
