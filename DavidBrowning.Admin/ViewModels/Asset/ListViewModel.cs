// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Collections.Generic;
using System.Linq;
using DavidBrowning.Models;

namespace DavidBrowning.Admin.ViewModels.Asset;

public class ListViewModel : IListViewModel<EditViewModel>
{
   public required ListModes ListMode { get; set; }

   public List<EditViewModel>? Items { get; set; }

   public ListViewModel(
      IEnumerable<SiteAsset> assets,
      ListModes mode)
   {
      ListMode = mode;
      Items = assets.Select(a => new EditViewModel(a)).ToList();
   }
}
