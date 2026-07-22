// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Admin.Extensions;
using DavidBrowning.Admin.ViewModels;
using DavidBrowning.Admin.ViewModels.Asset;
using DavidBrowning.Helpers;
using DavidBrowning.Infrastructure.Assets;
using DavidBrowning.Infrastructure.Data.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DavidBrowning.Admin.Controllers;

[Authorize]
public class AssetController : Controller
{
   public AssetController(
      IUncategorizedStore uncategorizedStore,
      IContentStore contentStore)
   {
      _uncategorizedStore = uncategorizedStore;
      _contentStore = contentStore;
   }

   [HttpGet]
   public async Task<IActionResult> Index(
      CancellationToken cancellationToken)
   {
      return View(await GetIndexModelAsync(null, cancellationToken));
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> Create(
      EditViewModel model,
      CancellationToken cancellationToken)
   {
      await LoadAndValidateContentTypePickerAsync(model, cancellationToken);
      LoadAndValidateUploadedFile(
         model, uploadRequired: true, cancellationToken);
      if (!ModelState.IsValid)
      {
         if (Request.IsHtmxRequest())
         {
            return PartialView(nameof(Edit), model);
         }

         return View(
            nameof(Index), await GetIndexModelAsync(model, cancellationToken));
      }

      // Asset key is required so the model state validation would have returned
      // if its not null.

      string assetKey = NormalizeAssetKey(model.AssetKey!);
      IFormFile upload = model.UploadFile!;
      await using Stream content = upload.OpenReadStream();
      await _contentStore.WriteAsync(assetKey, content, cancellationToken);
      await _uncategorizedStore.InsertAssetAsync(
         model.ToAsset(), cancellationToken);
      ModelState.Clear();

      if (Request.IsHtmxRequest())
      {
         return PartialView(
            "CreateResult", await GetIndexModelAsync(null, cancellationToken));
      }

      TempData["SuccessMessage"] = $"Asset uploaded";
      return RedirectToAction(nameof(Index));
   }

   [HttpGet]
   public async Task<IActionResult> Edit(
      int id,
      CancellationToken cancellationToken)
   {
      var asset = await _uncategorizedStore.GetAssetAsync(id, cancellationToken);
      if (asset is null)
      {
         return NotFound();
      }

      var contentTypePicker = await ContentTypePickerViewModel.LoadAsync(
            _contentStore, asset.ContentType, cancellationToken: cancellationToken);
      var model = new EditViewModel(asset)
      {
         ContentTypePicker = contentTypePicker,
      };

      if (Request.IsHtmxRequest())
      {
         return PartialView(nameof(Edit), model);
      }

      return View(nameof(Edit), model);
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> Edit(
      EditViewModel model,
      CancellationToken cancellationToken)
   {
      await LoadAndValidateContentTypePickerAsync(model, cancellationToken);
      LoadAndValidateUploadedFile(
         model, uploadRequired: false, cancellationToken);
      if (!ModelState.IsValid)
      {
         return PartialView(nameof(Edit), model);
      }

      var updated = await _uncategorizedStore.UpdateAssetAsync(
         model.ToAsset(), cancellationToken);
      if (!updated)
      {
         return NotFound();
      }

      if (model.UploadFile != null)
      {
         string assetKey = NormalizeAssetKey(model.AssetKey!);
         IFormFile upload = model.UploadFile!;
         await using Stream content = upload.OpenReadStream();
         var result = await _contentStore.WriteAsync(
            assetKey, content, cancellationToken);
      }

      if (Request.IsHtmxRequest())
      {
         Response.TriggerAdminOffcanvasClose(AssetAdminIds.AssetEditOffcanvas);
         return PartialView(
            "ListRefresh", await GetListViewModelAsync(cancellationToken));
      }

      return RedirectToAction(nameof(Index));
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> Delete(
      int id,
      CancellationToken cancellationToken)
   {
      var asset = await _uncategorizedStore.GetAssetAsync(id, cancellationToken);
      if (asset is null)
      {
         return NotFound();
      }

      await _uncategorizedStore.DeleteAssetAsync(id, cancellationToken);
      await _contentStore.DeleteFileAsync(asset.AssetKey!, cancellationToken);
      TempData["SuccessMessage"] = $"Deleted asset \"{asset.AssetKey}\".";
      return RedirectToAction(nameof(Index));
   }

   [HttpGet]
   public async Task<IActionResult> Download(
      int id,
      CancellationToken cancellationToken)
   {
      var asset = await _uncategorizedStore.GetAssetAsync(id, cancellationToken);
      if (asset is null)
      {
         return NotFound();
      }

      var content = await _contentStore.OpenReadAsync(
         asset.AssetKey, cancellationToken);
      return File(content, asset.ContentType, asset.OriginalFileName);
   }

   [HttpGet]
   public async Task<IActionResult> TestBlobContentStore(
      [FromServices] IContentStore contentStore,
      CancellationToken cancellationToken)
   {
      const string assetKey = "tests/blob-store-smoke-test.txt";
      await using var stream = new MemoryStream(
         System.Text.Encoding.UTF8.GetBytes("Hello from AzureBlobContentStore."));
      ContentWriteResults writeResult = await contentStore.WriteAsync(
         assetKey, stream, cancellationToken);
      StoredAsset asset = await contentStore.GetAssetAsync(
         assetKey, cancellationToken);

      return Content(
         $"Write result: {writeResult}\n" +
         $"Asset key: {asset.AssetKey}\n" +
         $"Content type: {asset.ContentType}\n" +
         $"Content length: {asset.ContentLength}\n" +
         $"Text: {asset.Text}",
         "text/plain");
   }

   private async Task<IndexViewModel> GetIndexModelAsync(
      EditViewModel? existingCreateModel,
      CancellationToken cancellationToken)
   {
      var contentTypePicker = await ContentTypePickerViewModel.LoadAsync(
         _contentStore, cancellationToken: cancellationToken);

      existingCreateModel ??= new EditViewModel()
      {
         ContentTypePicker = contentTypePicker,
         ContentType = string.Empty,
      };

      existingCreateModel.EditMode = EditModes.Create;

      return new IndexViewModel()
      {
         Create = existingCreateModel,
         List = await GetListViewModelAsync(cancellationToken),
         EditOffcanvas = new AdminOffcanvasViewModel()
         {
            Id = AssetAdminIds.AssetEditOffcanvas,
            Title = "Edit Asset",
            Placeholder = "Select an asset to edit",
            LoadingText = "Loading asset..."
         },
      };
   }

   private async Task<ListViewModel> GetListViewModelAsync(
      CancellationToken cancellationToken)
   {
      var assets = await _uncategorizedStore.GetSiteAssetsAsync(cancellationToken);
      return new ListViewModel()
      {
         ReorderList = new ReorderListViewModel()
         {
            Title = "Assets",
            EditOffcanvasId = AssetAdminIds.AssetEditOffcanvas,
            Items = assets.Select(asset => new ReorderListItemViewModel()
            {
               Id = asset.Id,
               DisplayName = asset.AssetKey,
               SecondaryText = asset.OriginalFileName,
               DeleteAction = "Delete",
               DeleteController = "Asset",
               EditAction = "Edit",
               EditController = "Asset"
            })
            .ToList()
         }
      };
   }

   private async Task LoadAndValidateContentTypePickerAsync(
      EditViewModel model,
      CancellationToken cancellationToken)
   {
      model.ContentTypePicker = await ContentTypePickerViewModel.LoadAsync(
         _contentStore, model.ContentType, cancellationToken: cancellationToken);

      if (!model.ContentTypePicker.Supports(model.ContentType))
      {
         ModelState.AddModelError(
            nameof(model.ContentType), "Select a supported content type.");
      }
   }

   private void LoadAndValidateUploadedFile(
      EditViewModel model,
      bool uploadRequired,
      CancellationToken cancellationToken)
   {
      IFormFile? upload = model.UploadFile;
      if (upload is null || upload.Length == 0)
      {
         if (uploadRequired)
         {
            ModelState.AddModelError(
               nameof(model.UploadFile), "Select a file to upload.");
         }

         return;
      }

      string originalFileName = Path.GetFileName(upload.FileName);
      if (string.IsNullOrWhiteSpace(originalFileName))
      {
         ModelState.AddModelError(
            nameof(model.UploadFile), "The selected file does not have a valid file name.");
         return;
      }

      if (model.SizeBytes is null)
      {
         ModelState.AddModelError(nameof(model.SizeBytes), "File size is not included.");
      }

      model.OriginalFileName = originalFileName;
      model.SizeBytes = upload.Length;
      if (!model.ContentTypePicker.Options.Select(o => o.ContentType)
         .Contains(upload.ContentType))
      {
         ModelState.AddModelError(
            nameof(model.UploadFile), "The file is not an approved MIME type.");
         return;
      }

      if (ContentTypeHelpers.IsImageContentType(upload.ContentType) &&
         (model.WidthPixels is null || model.HeightPixels is null))
      {
         ModelState.AddModelError(nameof(model.UploadFile),
            "The uploaded file is an image but the width or height are not specified.");
      }

      return;
   }

   private static string NormalizeAssetKey(string assetKey)
   {
      string normalized = assetKey.Trim().Replace('\\', '/').ToLowerInvariant();

      if (string.IsNullOrWhiteSpace(normalized))
      {
         throw new InvalidOperationException("Asset key cannot be empty.");
      }

      if (normalized.StartsWith('/') ||
          normalized.Contains("..", StringComparison.Ordinal) ||
          Path.IsPathRooted(normalized))
      {
         throw new InvalidOperationException($"Invalid asset key: '{assetKey}'.");
      }

      return normalized;
   }

   private readonly IUncategorizedStore _uncategorizedStore;
   private readonly IContentStore _contentStore;
}
