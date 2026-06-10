// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Admin.Extensions;
using DavidBrowning.Admin.ViewModels.Work.Resume;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DavidBrowning.Admin.Controllers;

public partial class WorkController
{

   [HttpGet]
   public async Task<IActionResult> ResumeIndex(
      CancellationToken cancellationToken)
   {
      return View(await GetResumeIndexViewModelAsync(cancellationToken));
   }

   [HttpPost]
   public async Task<IActionResult> ResumeUpload(
      IndexViewModel model,
      CancellationToken cancellationToken)
   {
      LoadAndValidateUploadResume(model, cancellationToken);
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

   private void LoadAndValidateUploadResume(
      IndexViewModel model,
      CancellationToken cancellationToken)
   {
      IFormFile? upload = model.UploadedFile;
      if (upload is null || upload.Length == 0)
      {
         ModelState.AddModelError(
            nameof(model.UploadedFile), "Select a file to upload.");
         return;
      }

      if(upload.ContentType != "application/pdf")
      {
         ModelState.AddModelError(
            nameof(model.UploadedFile), "File is not a PDF");
      }

      model.ContentType = upload.ContentType;
      model.OriginalFileName = upload.FileName;
      model.SizeBytes = upload.Length;
   }
}
