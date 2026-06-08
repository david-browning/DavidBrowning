// Copyright © 2026 David Browning. All rights reserved.
//
// Source-available for viewing only. No license granted.

using System.ComponentModel.DataAnnotations;

using DavidBrowning.Models;

namespace DavidBrowning.Admin.ViewModels.Writing;

// PostRevisionAssetLink uses a composite key:
// (PostRevisionId, SiteAssetId).
//
// Treat those key fields as immutable while editing. To use a different asset,
// delete the old link and create a new one.
public sealed class PostRevisionAssetLinkEditViewModel
{
   public EditModes EditMode { get; set; } = EditModes.Create;

   [Range(1, int.MaxValue)]
   public int PostRevisionId { get; set; }

   [Range(1, int.MaxValue)]
   public int SiteAssetId { get; set; }

   [Required]
   [StringLength(DataConstants.MaxSlugLength)]
   [RegularExpression(
      DataConstants.SlugRegex,
      ErrorMessage = DataConstants.SlugRegexError)]
   public string? ReferenceKey { get; set; }

   [StringLength(DataConstants.MaxMetadataLength)]
   public string? Caption { get; set; }

   [StringLength(DataConstants.MaxMetadataLength)]
   public string? AltTextOverride { get; set; }

   public PostRevisionAssetLinkEditViewModel()
   {
   }

   public PostRevisionAssetLinkEditViewModel(PostRevisionAssetLink link)
   {
      EditMode = EditModes.Edit;
      PostRevisionId = link.PostRevisionId;
      SiteAssetId = link.SiteAssetId;
      ReferenceKey = link.ReferenceKey;
      Caption = link.Caption;
      AltTextOverride = link.AltTextOverride;
   }
}
