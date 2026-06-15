// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DavidBrowning.Admin.ViewModels;

namespace DavidBrowning.Admin.ViewModels.Projects;

public sealed class ProjectContentEditViewModel
{
   [Range(1, int.MaxValue)]
   public required int ProjectId { get; set; }

   public int? ContentAssetId { get; set; }

   public string? ContentAssetKey { get; set; }

   public string? Content { get; set; }

   public List<AssetLinkInputViewModel> AssetLinks { get; set; } = new();
}