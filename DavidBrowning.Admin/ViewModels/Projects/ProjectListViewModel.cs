// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using System;
using System.Collections.Generic;

namespace DavidBrowning.Admin.ViewModels.Projects;

public sealed class ProjectListViewModel
{
   public IReadOnlyList<ProjectListItemViewModel> Items { get; set; } =
      Array.Empty<ProjectListItemViewModel>();
}