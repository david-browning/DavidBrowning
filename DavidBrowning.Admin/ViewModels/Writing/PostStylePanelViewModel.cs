// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Collections.Generic;
namespace DavidBrowning.Admin.ViewModels.Writing;

public sealed class PostStylePanelViewModel
{
   public PostStyleEditViewModel Create { get; set; } = new();

   public IReadOnlyList<PostStyleEditViewModel> Items { get; set; } =
      Array.Empty<PostStyleEditViewModel>();
}
