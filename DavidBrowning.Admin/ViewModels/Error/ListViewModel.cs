// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DavidBrowning.Models.Error;

namespace DavidBrowning.Admin.ViewModels.Error;

public class ListViewModel : IReadonlyListViewModel<WebsiteError>
{
   public required IReadOnlyList<WebsiteError>? Items { get; set; }

   [SetsRequiredMembers]
   public ListViewModel(IEnumerable<WebsiteError> errors)
   {
      Items = errors.ToList();
   }
}
