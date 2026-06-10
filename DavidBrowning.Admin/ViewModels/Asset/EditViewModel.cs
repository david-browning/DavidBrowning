// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using System;
using System.ComponentModel.DataAnnotations;

using DavidBrowning.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

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

   // Display-only metadata. Derive these values from the stored file.
   public string? ContentType { get; set; }

   public string? OriginalFileName { get; set; }

   public long? SizeBytes { get; set; }

   public int? WidthPixels { get; set; }

   public int? HeightPixels { get; set; }

   /// <summary>
   /// New file selected by the user. Required when creating an asset.
   /// Optional when editing metadata for an existing asset.
   /// </summary>
   public IFormFile? UploadFile { get; set; }


   [BindNever]
   [ValidateNever]
   public required ContentTypePickerViewModel ContentTypePicker
   {
      get;
      set;
   }

   public bool CanDownload
   {
      get
      {
         return EditMode == EditModes.Edit &&
            !string.IsNullOrWhiteSpace(AssetKey);
      }
   }

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

   public SiteAsset ToAsset()
   {
      ArgumentException.ThrowIfNullOrEmpty(AssetKey);
      ArgumentException.ThrowIfNullOrEmpty(ContentType);
      if(SizeBytes is null)
      {
         throw new NullReferenceException(nameof(SizeBytes));
      }

      return new SiteAsset()
      {
         Id = Id ?? 0,
         AssetKey = AssetKey,
         AltText = AltText,
         ContentType = ContentType,
         OriginalFileName = OriginalFileName,
         SizeBytes = SizeBytes.Value,
         WidthPixels = WidthPixels,
         HeightPixels = HeightPixels,
         // Date is populated by the EF core callbacks.
      };
   }
}
