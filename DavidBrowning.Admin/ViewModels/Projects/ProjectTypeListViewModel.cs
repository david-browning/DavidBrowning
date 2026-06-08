// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Collections.Generic;
using System.Linq;
using DavidBrowning.Models.Projects;

namespace DavidBrowning.Admin.ViewModels.Projects;

public class ProjectTypeListViewModel : IReadonlyListViewModel<ProjectType>
{
   public IReadOnlyList<ProjectType>? Items {  get; set; }

   public ProjectTypeListViewModel(IEnumerable<ProjectType> types)
   {
      Items = types.ToList();
   }
}
