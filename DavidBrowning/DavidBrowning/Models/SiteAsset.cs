// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DavidBrowning.Models.Projects;
using Microsoft.EntityFrameworkCore;

namespace DavidBrowning.Models
{
   /// <summary>
   /// Maps to db_SiteAssets.
   /// Reusable blob-backed asset that can be referenced by posts, projects, or future site content.
   /// </summary>
   [PrimaryKey(nameof(Id))]
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
      /// Azure blob container containing the asset.
      /// </summary>
      [Required]
      [StringLength(DataConstants.MaxAzureAssetLength)]
      public required string BlobContainer { get; set; }

      /// <summary>
      /// Name/path of the blob inside the container.
      /// </summary>
      [Required]
      [StringLength(DataConstants.MaxAzureAssetLength)]
      public required string BlobName { get; set; }

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

      public ICollection<SiteAssetLink> PostLinks { get; } = new List<SiteAssetLink>();

      public ICollection<ProjectAssetLink> ProjectLinks { get; } = new List<ProjectAssetLink>();
   }
}
