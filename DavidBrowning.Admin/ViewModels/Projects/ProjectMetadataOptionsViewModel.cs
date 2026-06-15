// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using System;
using System.Collections.Generic;

namespace DavidBrowning.Admin.ViewModels.Projects;

public sealed class ProjectMetadataOptionsViewModel
{
   public IReadOnlyList<LookupOptionViewModel> Statuses { get; set; } =
      Array.Empty<LookupOptionViewModel>();

   public IReadOnlyList<LookupOptionViewModel> Types { get; set; } =
      Array.Empty<LookupOptionViewModel>();

   public IReadOnlyList<LookupOptionViewModel> Origins { get; set; } =
      Array.Empty<LookupOptionViewModel>();

   public IReadOnlyList<LookupOptionViewModel> Visibilities { get; set; } =
      Array.Empty<LookupOptionViewModel>();

   public IReadOnlyList<LookupOptionViewModel> Tags { get; set; } =
      Array.Empty<LookupOptionViewModel>();

   public IReadOnlyList<LookupOptionViewModel> StackTags { get; set; } =
      Array.Empty<LookupOptionViewModel>();
}