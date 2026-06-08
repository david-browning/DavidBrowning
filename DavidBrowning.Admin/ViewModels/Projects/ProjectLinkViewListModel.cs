// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Collections.Generic;
using System.Linq;
using DavidBrowning.Models.Projects;

namespace DavidBrowning.Admin.ViewModels.Projects;

public class ProjectLinkViewListModel : IListViewModel<ProjectLinkEditViewModel>
{
   public ListModes ListMode { get; set; }

   public List<ProjectLinkEditViewModel>? Items { get; set; }

   public ProjectLinkViewListModel(IEnumerable<ProjectLink> links, ListModes mode)
   {
      ListMode = mode;
      Items = links.Select(l => new ProjectLinkEditViewModel(l)).ToList();
   }
}
