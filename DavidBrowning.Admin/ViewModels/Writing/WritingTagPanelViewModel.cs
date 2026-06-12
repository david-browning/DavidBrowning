// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Collections.Generic;
namespace DavidBrowning.Admin.ViewModels.Writing;

public sealed class WritingTagPanelViewModel
{
   public WritingTagEditViewModel Create { get; set; } = new();

   public IReadOnlyList<WritingTagEditViewModel> Items { get; set; } =
      Array.Empty<WritingTagEditViewModel>();
}
