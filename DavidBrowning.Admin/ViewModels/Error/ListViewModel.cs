// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DavidBrowning.Models.Error;

namespace DavidBrowning.Admin.ViewModels.Error;

public class ListViewModel : IListViewModel<WebsiteError>
{
   public required ListModes ListMode { get; set; }
   
   public required List<WebsiteError>? Items { get; set; }

   [SetsRequiredMembers]
   public ListViewModel(
      IEnumerable<WebsiteError> errors,
      ListModes mode = ListModes.Readonly)
   {
      Items = errors.ToList();
      ListMode = mode;
   }
}
