// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.ComponentModel.DataAnnotations;
using DavidBrowning.Models;

namespace DavidBrowning.Admin.ViewModels;

public class AssetLinkInputViewModel
{
   [Range(1, int.MaxValue)]
   public int SiteAssetId { get; set; }

   [Required]
   [StringLength(DataConstants.MaxSlugLength)]
   [RegularExpression(
      @"^[a-z0-9][a-z0-9-]*$",
      ErrorMessage = "Asset reference keys may contain lowercase letters, numbers, and hyphens.")]
   public required string ReferenceKey { get; set; }

   public string? Caption { get; set; }

   public string? AltTextOverride { get; set; }
}
