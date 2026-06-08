// Copyright © 2026 David Browning. All rights reserved.
//
// Source-available for viewing only. No license granted.

using System.ComponentModel.DataAnnotations;

using DavidBrowning.Models;

namespace DavidBrowning.Admin.ViewModels.Asset;

public sealed class DeleteViewModel
{
   [Range(1, int.MaxValue)]
   public int Id { get; set; }

   [Required]
   [StringLength(DataConstants.MaxAssetKeyLength)]
   public string? AssetKey { get; set; }

   public string? ContentType { get; set; }

   public long SizeBytes { get; set; }

   public int ProjectLinkCount { get; set; }

   public int PostRevisionLinkCount { get; set; }
}
