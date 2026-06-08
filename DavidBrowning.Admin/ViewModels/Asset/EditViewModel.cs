// Copyright © 2026 David Browning. All rights reserved.
//
// Source-available for viewing only. No license granted.

using System;
using System.ComponentModel.DataAnnotations;

using DavidBrowning.Models;

using Microsoft.AspNetCore.Http;

namespace DavidBrowning.Admin.ViewModels.Asset;

public sealed class EditViewModel
{
   public EditModes EditMode { get; set; } = EditModes.Create;

   public int? Id { get; set; }

   [Required]
   [StringLength(DataConstants.MaxAssetKeyLength)]
   public string? AssetKey { get; set; }

   [StringLength(DataConstants.MaxMetadataLength)]
   public string? AltText { get; set; }

   // Required for create, optional for edit. Enforce that conditional rule
   // in the controller or application service.
   public IFormFile? UploadedFile { get; set; }

   // Display-only metadata. Derive these values from the stored file.
   public string? ContentType { get; init; }

   public string? OriginalFileName { get; init; }

   public long? SizeBytes { get; init; }

   public int? WidthPixels { get; init; }

   public int? HeightPixels { get; init; }

   public EditViewModel()
   {
   }

   public EditViewModel(SiteAsset asset)
   {
      EditMode = EditModes.Edit;
      Id = asset.Id;
      AssetKey = asset.AssetKey;
      AltText = asset.AltText;
      ContentType = asset.ContentType;
      OriginalFileName = asset.OriginalFileName;
      SizeBytes = asset.SizeBytes;
      WidthPixels = asset.WidthPixels;
      HeightPixels = asset.HeightPixels;
   }
}
