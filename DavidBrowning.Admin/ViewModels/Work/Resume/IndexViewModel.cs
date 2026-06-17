// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DavidBrowning.Admin.ViewModels.Work.Resume;

public class IndexViewModel
{
   public required string AssetKey { get; set; }

   public string? FileName { get; set; }

   public string? ContentType { get; set; }

   public long? SizeBytes { get; set; }

   public string? OriginalFileName { get; set; }

   public required bool CanDownload { get; set; }

   [Required(ErrorMessage = "Select a file to upload.")]
   public IFormFile? UploadedFile { get; set; }
}
