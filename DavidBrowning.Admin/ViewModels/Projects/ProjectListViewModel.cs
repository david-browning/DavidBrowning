// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Collections.Generic;
using System.Linq;
using DavidBrowning.Models.Projects;

namespace DavidBrowning.Admin.ViewModels.Projects;

public class ProjectListViewModel : IListViewModel<ProjectEditViewModel>
{
   public ListModes ListMode { get; set; }

   public List<ProjectEditViewModel>? Items { get; set; }

   public ProjectListViewModel(IEnumerable<Project> projects, ListModes mode)
   {
      ListMode = mode;
      Items = projects.Select(p => new ProjectEditViewModel(p)).ToList();
   }
}
