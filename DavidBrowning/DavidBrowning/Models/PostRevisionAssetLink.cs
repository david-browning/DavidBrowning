// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using System.ComponentModel.DataAnnotations;
using DavidBrowning.Models.Writing;

namespace DavidBrowning.Models;

/// <summary>
/// Maps to db_PostRevisionAssetLinks.
/// Connects a writing post to a reusable site asset.
/// </summary>
public sealed class PostRevisionAssetLink
{
   public int PostRevisionId { get; set; }

   public PostRevision? PostRevision { get; set; }

   public int SiteAssetId { get; set; }

   public SiteAsset? SiteAsset { get; set; }

   [StringLength(DataConstants.MaxSlugLength)]
   public required string ReferenceKey { get; set; }

   [StringLength(DataConstants.MaxMetadataLength)]
   public string? Caption { get; set; }

   [StringLength(DataConstants.MaxMetadataLength)]
   public string? AltTextOverride { get; set; }
}