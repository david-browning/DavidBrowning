// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Collections.Generic;
using System.Linq;
using DavidBrowning.Models.Projects;

namespace DavidBrowning.Admin.ViewModels.Projects;

public class ProjectStackListViewModel : IListViewModel<ProjectStackEditViewModel>
{
   public ListModes ListMode { get; set; }
   
   public List<ProjectStackEditViewModel>? Items {  get; set; }

   public ProjectStackListViewModel(
      IEnumerable<ProjectStackTag> stacks, 
      ListModes mode)
   {
      ListMode = mode;
      Items = stacks.Select(s => new ProjectStackEditViewModel(s)).ToList();
   }
}
