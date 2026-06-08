// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Collections.Generic;
using System.Linq;
using DavidBrowning.Models.Projects;

namespace DavidBrowning.Admin.ViewModels.Projects;

public class ProjectStatusListViewModel : IListViewModel<ProjectStatus>
{
   public ListModes ListMode { get;set; }
   
   public List<ProjectStatus>? Items { get; set; }

   public ProjectStatusListViewModel(IEnumerable<ProjectStatus> statuses)
   {
      ListMode = ListModes.Readonly;
      Items = statuses.ToList();
   }
}
