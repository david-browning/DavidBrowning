// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DavidBrowning.Models.Projects;
using Microsoft.EntityFrameworkCore;

namespace DavidBrowning.Models;

/// <summary>
/// Maps to db_SiteAssets.
/// Reusable blob-backed asset that can be referenced by posts, projects, or future site content.
/// </summary>
[PrimaryKey(nameof(Id))]
[Index(nameof(AssetKey), IsUnique = true)]
[Index(nameof(BlobContainer), nameof(BlobName), IsUnique = true)]
public sealed class SiteAsset
{
   [Required, Key]
   public int Id { get; set; }

   /// <summary>
   /// The type of content the asset is.
   /// Example: image, PDF, code, binary.
   /// </summary>
   public required SiteAssetType AssetType { get; set; }

   /// <summary>
   /// Original uploaded filename, if available.
   /// </summary>
   [StringLength(DataConstants.MaxNameLength)]
   public string? OriginalFileName { get; set; }

   /// <summary>
   /// Root-relative logical key used to locate the asset through the content
   /// pipeline. Asset keys do not begin with a slash.
   /// </summary>
   [Required]
   [StringLength(DataConstants.MaxAzureAssetLength)]
   public required string AssetKey { get; set; }

   /// <summary>
   /// Azure blob container containing the asset.
   /// </summary>
   [Required]
   [StringLength(DataConstants.MaxAzureAssetLength)]
   public string? BlobContainer { get; set; }

   /// <summary>
   /// Name/path of the blob inside the container.
   /// </summary>
   [Required]
   [StringLength(DataConstants.MaxAzureAssetLength)]
   public string? BlobName { get; set; }

   /// <summary>
   /// MIME type used when serving the asset.
   /// Examples: image/png, image/jpeg, application/pdf.
   /// </summary>
   [Required]
   [StringLength(DataConstants.MaxContentTypeLength)]
   public required string ContentType { get; set; }

   /// <summary>
   /// Default alternate text for the asset.
   /// Content-specific links can override this.
   /// </summary>
   public string? AltText { get; set; }

   /// <summary>
   /// Size of the asset in bytes.
   /// </summary>
   public required long SizeBytes { get; set; }

   /// <summary>
   /// When this asset record was created.
   /// Stored in UTC.
   /// </summary>
   public required DateTime CreatedAtUtc { get; set; }

   /// <summary>
   /// Intrinsic width of an image asset in pixels.
   /// Null for non-image assets or when dimensions are unknown.
   /// </summary>
   public int? WidthPixels { get; set; }

   /// <summary>
   /// Intrinsic height of an image asset in pixels.
   /// Null for non-image assets or when dimensions are unknown.
   /// </summary>
   public int? HeightPixels { get; set; }

   public ICollection<SiteAssetLink> PostLinks { get; } = new List<SiteAssetLink>();

   public ICollection<ProjectAssetLink> ProjectLinks { get; } = new List<ProjectAssetLink>();
}
