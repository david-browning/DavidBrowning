// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DavidBrowning.Models.Writing;

namespace DavidBrowning.Admin.ViewModels.Writing;

public class WritingTagListViewModel : IListViewModel<WritingTagEditViewModel>
{
   public ListModes ListMode { get; set; }

   public List<WritingTagEditViewModel>? Items { get; set; }

   [SetsRequiredMembers]
   public WritingTagListViewModel(
      IEnumerable<WritingTag> tags,
      ListModes mode)
   {
      Items = tags.Select(t => new WritingTagEditViewModel(t)).ToList();
      ListMode = mode;
   }
}
