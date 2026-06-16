// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using System;
using System.Collections.Generic;

namespace DavidBrowning.Admin.ViewModels.Projects;

public sealed class ProjectLookupPanelViewModel
{
   public required string Title { get; set; }

   public required string Description { get; set; }

   public required string CreateAction { get; set; }

   public required string EditAction { get; set; }

   public required string RegionId { get; set; }

   public IReadOnlyList<ProjectLookupItemViewModel> Items { get; set; } =
      Array.Empty<ProjectLookupItemViewModel>();
}