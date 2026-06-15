// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

namespace DavidBrowning.Admin.ViewModels.Projects;

public sealed class LookupOptionViewModel
{
   public required int Id { get; set; }

   public required string DisplayName { get; set; }

   public bool IsActive { get; set; } = true;
}