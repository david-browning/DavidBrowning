// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DavidBrowning.Models;

namespace DavidBrowning.Admin.ViewModels.About;

public class InterestListViewModel : IListViewModel<InterestEditViewModel>
{
   public required ListModes ListMode { get; set; }

   public List<InterestEditViewModel>? Items { get; set; }

   [SetsRequiredMembers]
   public InterestListViewModel(
      IEnumerable<Interest> interests,
      ListModes mode)
   {
      Items = interests.Select(i => new InterestEditViewModel(i)).ToList();
      ListMode = mode;
   }
}
