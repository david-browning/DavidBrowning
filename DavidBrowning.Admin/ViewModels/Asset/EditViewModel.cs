// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using DavidBrowning.Models;

namespace DavidBrowning.Admin.ViewModels.Asset;

public class EditViewModel
{
   public required EditModes EditMode { get; init; }

   public int? Id { get; set; }

   /// <summary>
   /// Root-relative logical key used to locate the asset through the content
   /// pipeline. Asset keys do not begin with a slash.
   /// </summary>
   [Required]
   [StringLength(DataConstants.MaxAssetKeyLength)]
   public string? AssetKey { get; set; }

   /// <summary>
   /// MIME type used when serving the asset.
   /// Examples: image/png, image/jpeg, application/pdf.
   /// </summary>
   [Required]
   [StringLength(DataConstants.MaxContentTypeLength)]
   public string? ContentType { get; set; }

   /// <summary>
   /// Original uploaded filename, if available.
   /// </summary>
   [StringLength(DataConstants.MaxNameLength)]
   public string? OriginalFileName { get; set; }

   /// <summary>
   /// Default alternate text for the asset.
   /// Content-specific links can override this.
   /// </summary>
   [StringLength(DataConstants.MaxMetadataLength)]
   public string? AltText { get; set; }

   /// <summary>
   /// Size of the asset in bytes.
   /// </summary>
   [Required]
   [Range(0, int.MaxValue)]
   public long SizeBytes { get; set; } = 0;

   /// <summary>
   /// Intrinsic width of an image asset in pixels.
   /// Null for non-image assets or when dimensions are unknown.
   /// </summary>
   [Range(0, int.MaxValue)]
   public int? WidthPixels { get; set; }

   /// <summary>
   /// Intrinsic height of an image asset in pixels.
   /// Null for non-image assets or when dimensions are unknown.
   /// </summary>
   [Range(0, int.MaxValue)]
   public int? HeightPixels { get; set; }

   /// <summary>
   /// When this asset record was created.
   /// Stored in UTC.
   /// </summary>
   [Required]
   public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

   public EditViewModel()
   {
      EditMode = EditModes.Create;
   }

   [SetsRequiredMembers]
   public EditViewModel(SiteAsset asset)
   {
      EditMode = EditModes.Edit;
      Id = asset.Id;
      AssetKey = asset.AssetKey;
      ContentType = asset.ContentType;
      OriginalFileName = asset.OriginalFileName;
      AltText = asset.AltText;
      SizeBytes = asset.SizeBytes;
      WidthPixels = asset.WidthPixels;
      HeightPixels = asset.HeightPixels;
      CreatedAtUtc = asset.CreatedAtUtc;
   }
}
