// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Admin.ViewModels.Work.Resume;
using DavidBrowning.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace DavidBrowning.Admin.Controllers;

public partial class WorkController
{

   //[HttpGet]
   //public async Task<IActionResult> ResumeIndex(
   //   CancellationToken cancellationToken)
   //{
   //   return View(await GetResumeIndexViewModelAsync(cancellationToken));
   //}

   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> ResumeUpload(
      IndexViewModel model,
      CancellationToken cancellationToken)
   {
      await LoadAndValidateUploadResumeAsync(model, cancellationToken);
      if (!ModelState.IsValid)
      {
         return PartialView("ResumePreview", model);
      }

      // The validation already returned if the upload file is null.
      await using Stream content = model.UploadedFile!.OpenReadStream();
      var result = await _contentStore.WriteAsync(
         FixedAssetKeys.ResumePDFAssetKey, content, cancellationToken);

      return PartialView("ResumePreview",
         await GetResumeIndexViewModelAsync(cancellationToken));
   }

   [HttpGet]
   public async Task<IActionResult> DownloadResume(CancellationToken cancellationToken)
   {
      var asset = await _contentStore.GetAssetAsync(
         FixedAssetKeys.ResumePDFAssetKey, cancellationToken);
      if (asset is null)
      {
         return NotFound();
      }
      var content = await _contentStore.OpenReadAsync(
         asset.AssetKey, cancellationToken);
      return File(content, asset.ContentType, "DavidBrowning-Resume.pdf");
   }

   private async Task<IndexViewModel> GetResumeIndexViewModelAsync(
      CancellationToken cancellationToken)
   {
      if (await _contentStore.AssetExists(FixedAssetKeys.ResumePDFAssetKey))
      {
         var resumeFile = await _contentStore.GetAssetAsync(
            FixedAssetKeys.ResumePDFAssetKey, cancellationToken);
         return new IndexViewModel()
         {
            AssetKey = FixedAssetKeys.ResumePDFAssetKey,
            ContentType = resumeFile?.ContentType,
            SizeBytes = resumeFile?.ContentLength,
            FileName = "Uploaded",
            CanDownload = true,
         };
      }
      else
      {
         return new IndexViewModel()
         {
            AssetKey = FixedAssetKeys.ResumePDFAssetKey,
            CanDownload = false,
         };
      }
   }

   private async Task LoadAndValidateUploadResumeAsync(
      IndexViewModel model,
      CancellationToken cancellationToken)
   {
      if (model.UploadedFile is null || model.UploadedFile.Length == 0)
      {
         ModelState.AddModelError(nameof(model.UploadedFile), 
            "Select a PDF file to upload.");
         return;
      }
      else
      {
         const long maximumResumeSizeBytes = 5 * 1024 * 1024;

         if (model.UploadedFile.Length >
             maximumResumeSizeBytes)
         {
            ModelState.AddModelError(nameof(model.UploadedFile), 
               "The resume must be 5 MB or smaller.");
            return;
         }

         if (!string.Equals(Path.GetExtension(model.UploadedFile.FileName), 
            ".pdf", StringComparison.OrdinalIgnoreCase))
         {
            ModelState.AddModelError(nameof(model.UploadedFile), 
               "The selected file must have a .pdf extension.");
            return;
         }

         if (!string.Equals(model.UploadedFile.ContentType, "application/pdf", 
            StringComparison.OrdinalIgnoreCase))
         {
            ModelState.AddModelError(nameof(model.UploadedFile), 
               "The selected file must have the PDF content type.");
            return;
         }

         if (!await PdfFileValidator.HasPdfSignatureAsync(
            model.UploadedFile, cancellationToken))
         {
            ModelState.AddModelError(nameof(model.UploadedFile), 
               "The selected file does not contain a valid PDF signature.");
            return;
         }
      }

      model.ContentType = model.UploadedFile!.ContentType;
      model.OriginalFileName = model.UploadedFile!.FileName;
      model.SizeBytes = model.UploadedFile!.Length;
   }
}
