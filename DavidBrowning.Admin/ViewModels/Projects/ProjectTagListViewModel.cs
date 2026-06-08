// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Collections.Generic;
using System.Linq;
using DavidBrowning.Models.Projects;

namespace DavidBrowning.Admin.ViewModels.Projects;

public class ProjectTagListViewModel : IListViewModel<ProjectTagEditViewModel>
{
   public ListModes ListMode { get; set; }

   public List<ProjectTagEditViewModel>? Items { get; set; }

   public ProjectTagListViewModel(
      IEnumerable<ProjectTag> tags, ListModes mode)
   {
      ListMode = mode;
      Items = tags.Select(t => new ProjectTagEditViewModel(t)).ToList();
   }
}
