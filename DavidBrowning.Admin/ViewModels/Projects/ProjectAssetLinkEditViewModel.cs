// Copyright © 2026 David Browning. All rights reserved.
//
// Source-available for viewing only. No license granted.

using System.ComponentModel.DataAnnotations;

using DavidBrowning.Models;
using DavidBrowning.Models.Projects;

namespace DavidBrowning.Admin.ViewModels.Projects;

// ProjectAssetLink uses a composite key:
// (ProjectId, SiteAssetId, ProjectAssetRoleId).
//
// Treat those key fields as immutable while editing. To move an asset link to
// a different asset or role, delete the old link and create a new one.
public sealed class ProjectAssetLinkEditViewModel
{
   public EditModes EditMode { get; set; } = EditModes.Create;

   [Range(1, int.MaxValue)]
   public int ProjectId { get; set; }

   [Range(1, int.MaxValue)]
   public int SiteAssetId { get; set; }

   [Range(1, int.MaxValue)]
   public int ProjectAssetRoleId { get; set; }

   [StringLength(DataConstants.MaxMetadataLength)]
   public string? Caption { get; set; }

   [StringLength(DataConstants.MaxMetadataLength)]
   public string? AltTextOverride { get; set; }

   [StringLength(DataConstants.MaxSlugLength)]
   [RegularExpression(
      DataConstants.SlugRegex,
      ErrorMessage = DataConstants.SlugRegexError)]
   public string? ReferenceKey { get; set; }

   [Range(0, int.MaxValue)]
   public int SortOrder { get; set; }

   public ProjectAssetLinkEditViewModel()
   {
   }

   public ProjectAssetLinkEditViewModel(ProjectAssetLink link)
   {
      EditMode = EditModes.Edit;
      ProjectId = link.ProjectId;
      SiteAssetId = link.SiteAssetId;
      ProjectAssetRoleId = link.ProjectAssetRoleId;
      Caption = link.Caption;
      AltTextOverride = link.AltTextOverride;
      ReferenceKey = link.ReferenceKey;
      SortOrder = link.SortOrder;
   }
}
