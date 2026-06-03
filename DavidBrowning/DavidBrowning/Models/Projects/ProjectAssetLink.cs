// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DavidBrowning.Models.Projects;

/// <summary>
/// Maps to db_ProjectAssetLinks.
/// Join table connecting projects to reusable site assets.
/// </summary>
public sealed class ProjectAssetLink
{
   /// <summary>
   /// Foreign key to db_Projects.
   /// </summary>
   public int ProjectId { get; set; }

   [ForeignKey(nameof(ProjectId))]
   public Project? Project { get; set; }

   /// <summary>
   /// Foreign key to db_SiteAssets.
   /// </summary>
   public int SiteAssetId { get; set; }

   [ForeignKey(nameof(SiteAssetId))]
   public SiteAsset? SiteAsset { get; set; }

   /// <summary>
   /// Foreign key to db_ProjectAssetRoles.
   /// Describes how the project uses the asset.
   /// </summary>
   public int ProjectAssetRoleId { get; set; }

   [ForeignKey(nameof(ProjectAssetRoleId))]
   public ProjectAssetRole? ProjectAssetRole { get; set; }

   /// <summary>
   /// Optional project-specific caption for the asset.
   /// </summary>
   [StringLength(DataConstants.MaxMetadataLength)]
   public string? Caption { get; set; }

   /// <summary>
   /// Optional project-specific alt text override.
   /// If null, use the asset's default alt text.
   /// </summary>
   [StringLength(DataConstants.MaxMetadataLength)]
   public string? AltTextOverride { get; set; }

   /// <summary>
   /// Stable author-facing key used to reference the linked asset from project
   /// content. Example: architecture-diagram.
   /// </summary>
   [StringLength(DataConstants.MaxSlugLength)]
   public string? ReferenceKey { get; set; }

   /// <summary>
   /// Manual display ordering for assets within the same role.
   /// </summary>
   public int SortOrder { get; set; }
}