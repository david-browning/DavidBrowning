// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DavidBrowning.Models.Writing;

namespace DavidBrowning.Admin.ViewModels.Writing;

public class WritingTagListViewModel
{
   public List<WritingTagEditViewModel>? Items { get; set; }

   [SetsRequiredMembers]
   public WritingTagListViewModel(IEnumerable<WritingTag> tags)
   {
      Items = tags.Select(t => new WritingTagEditViewModel(t)).ToList();
   }
}
