// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Collections.Generic;
using System.Linq;
using DavidBrowning.Models.Projects;

namespace DavidBrowning.Admin.ViewModels.Projects;

public class ProjectOriginListViewModel : IListViewModel<ProjectOriginEditViewModel>
{
   public ListModes ListMode { get; set; }

   public List<ProjectOriginEditViewModel>? Items { get; set; }

   public ProjectOriginListViewModel(
      IEnumerable<ProjectOrigin> origins,
      ListModes mode)
   {
      ListMode = mode;
      Items = origins.Select(o => new ProjectOriginEditViewModel(o)).ToList();
   }
}
