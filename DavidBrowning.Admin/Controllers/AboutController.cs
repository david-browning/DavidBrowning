// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Admin.ViewModels;
using DavidBrowning.Admin.ViewModels.About;
using DavidBrowning.Infrastructure.Assets;
using DavidBrowning.Infrastructure.Data.Stores;
using DavidBrowning.Models;
using Microsoft.AspNetCore.Mvc;

namespace DavidBrowning.Admin.Controllers;

public sealed class AboutController : Controller
{
   public AboutController(
      IContentStore contentStore,
      IUncategorizedStore uncategorizedStore)
   {
      _contentStore = contentStore;
      _uncategorizedStore = uncategorizedStore;
   }

   [HttpGet]
   public async Task<IActionResult> Index(
      CancellationToken cancellationToken)
   {
      return View(await GetIndexModelAsync(createModel: null, cancellationToken));
   }

   /*
    * This action is useful if you later reload only the list partial
    * through JavaScript. The normal Index view can also render the same
    * partial during the initial page load.
    */
   [HttpGet]
   public async Task<IActionResult> InterestList(
      CancellationToken cancellationToken)
   {
      var interests = await _uncategorizedStore.GetInterestsAsync(
         cancellationToken);

      return PartialView(nameof(InterestList), new InterestListViewModel(interests));
   }

   /*
    * The create form already appears on the About index page.
    * Keep this endpoint as a convenience redirect rather than rendering
    * a second standalone create page.
    */
   [HttpGet]
   public IActionResult InterestCreate()
   {
      return RedirectToAction(nameof(Index));
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> InterestCreate(
      InterestEditViewModel model,
      CancellationToken cancellationToken)
   {
      await LoadAndValidateIconPickerAsync(model, cancellationToken);

      if (!ModelState.IsValid)
      {
         if (IsHtmxRequest())
         {
            return PartialView(nameof(InterestEdit), model);
         }

         return View(
            nameof(Index),
            await GetIndexModelAsync(model, cancellationToken));
      }

      var interest = new Interest()
      {
         Slug = model.Slug!,
         DisplayName = model.DisplayName!,
         Summary = model.Summary!,
         IsActive = model.IsActive,
         IconCssClass = model.SelectedIconCssClass,
      };

      await _uncategorizedStore.InsertInterestAsync(interest, cancellationToken);

      if (IsHtmxRequest())
      {
         return PartialView(
            "InterestCreateResult",
            await GetIndexModelAsync(createModel: null, cancellationToken));
      }

      TempData["SuccessMessage"] = $"Created interest \"{interest.DisplayName}\".";
      return RedirectToAction(nameof(Index));
   }

   [HttpGet]
   public async Task<IActionResult> InterestEdit(
      int id,
      CancellationToken cancellationToken)
   {
      Interest? interest = await _uncategorizedStore.GetInterestAsync(
         id, cancellationToken);
      if (interest is null)
      {
         return NotFound();
      }

      FontAwesomeIconPickerViewModel iconPicker =
         await LoadIconPickerAsync(interest.IconCssClass, cancellationToken);
      return View(nameof(InterestEdit), new InterestEditViewModel(interest, iconPicker));
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> InterestEdit(
      int id,
      InterestEditViewModel model,
      CancellationToken cancellationToken)
   {
      if (model.Id != id)
      {
         return BadRequest();
      }

      await LoadAndValidateIconPickerAsync(model, cancellationToken);

      if (!ModelState.IsValid)
      {
         return View(nameof(Index), await GetIndexModelAsync(model, cancellationToken));
      }

      var interest = await _uncategorizedStore.GetInterestAsync(id, cancellationToken);
      if (interest is null)
      {
         return NotFound();
      }

      /*
       * Map only editable fields.
       *
       * Do not trust SortOrder from the edit form. Ordering belongs to
       * the reorderable list UI, not to the single-item editor.
       */
      interest.Slug = model.Slug!;
      interest.DisplayName = model.DisplayName!;
      interest.Summary = model.Summary!;
      interest.IsActive = model.IsActive;
      interest.IconCssClass = model.SelectedIconCssClass;

      await _uncategorizedStore.UpdateInterestAsync(
         interest, cancellationToken);

      TempData["SuccessMessage"] = $"Updated interest \"{interest.DisplayName}\".";

      return RedirectToAction(nameof(Index));
   }

   [HttpGet]
   public async Task<IActionResult> InterestDelete(
      int id,
      CancellationToken cancellationToken)
   {
      var interest = await _uncategorizedStore.GetInterestAsync(
         id, cancellationToken);

      if (interest is null)
      {
         return NotFound();
      }

      return View(nameof(InterestDelete),
         new InterestDeleteViewModel
         {
            Id = interest.Id,
            DisplayName = interest.DisplayName,
            Slug = interest.Slug,
         });
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> InterestDeleteConfirmed(
      int id,
      CancellationToken cancellationToken)
   {
      var interest = await _uncategorizedStore.GetInterestAsync(
         id, cancellationToken);

      if (interest is null)
      {
         return NotFound();
      }

      await _uncategorizedStore.DeleteInterestAsync(id, cancellationToken);
      TempData["SuccessMessage"] = $"Deleted interest \"{interest.DisplayName}\".";
      return RedirectToAction(nameof(Index));
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> InterestReorder(
      ReorderListRequestViewModel model,
      CancellationToken cancellationToken)
   {
      if (!ModelState.IsValid)
      {
         return BadRequest(ModelState);
      }

      var idsInDisplayOrder = model.Items
         .OrderBy(item => item.SortOrder)
         .Select(item => item.Id)
         .ToList();

      try
      {
         await _uncategorizedStore.ReorderInterestsAsync(
            idsInDisplayOrder, cancellationToken);
      }
      catch (ArgumentException)
      {
         /*
          * A malformed or stale request should not silently produce a
          * partially reordered list.
          */
         return BadRequest();
      }

      TempData["SuccessMessage"] = "Saved interest display order.";

      return RedirectToAction(nameof(Index));
   }

   private bool IsHtmxRequest()
   {
      return string.Equals(
         Request.Headers["HX-Request"],
         "true",
         StringComparison.OrdinalIgnoreCase);
   }

   private async Task<IndexViewModel> GetIndexModelAsync(
      InterestEditViewModel? createModel,
      CancellationToken cancellationToken)
   {
      string? selectedIconCssClass = createModel?.SelectedIconCssClass;

      FontAwesomeIconPickerViewModel iconPicker =
         await LoadIconPickerAsync(selectedIconCssClass, cancellationToken);

      IReadOnlyList<Interest> interests =
         await _uncategorizedStore.GetInterestsAsync(cancellationToken);

      createModel ??= new InterestEditViewModel()
      {
         IconPicker = iconPicker,
      };

      createModel.EditMode = EditModes.Create;
      createModel.IconPicker = iconPicker;

      return new IndexViewModel()
      {
         Create = createModel,
         List = new InterestListViewModel(interests),
      };
   }

   private async Task<FontAwesomeIconPickerViewModel> LoadAndValidateIconPickerAsync(
      InterestEditViewModel model,
      CancellationToken cancellationToken)
   {
      FontAwesomeIconPickerViewModel iconPicker =
         await LoadIconPickerAsync(model.SelectedIconCssClass, cancellationToken);

      if (!iconPicker.Supports(model.SelectedIconCssClass))
      {
         ModelState.AddModelError(nameof(model.SelectedIconCssClass), "Select a supported icon.");
      }

      model.IconPicker = iconPicker;
      return iconPicker;
   }

   private Task<FontAwesomeIconPickerViewModel> LoadIconPickerAsync(
      string? selectedIconCssClass,
      CancellationToken cancellationToken)
   {
      return FontAwesomeIconPickerViewModel.LoadAsync(
         _contentStore,
         selectedIconCssClass,
         cancellationToken: cancellationToken);
   }

   private readonly IUncategorizedStore _uncategorizedStore;
   private readonly IContentStore _contentStore;
}