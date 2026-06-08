// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using System;
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
      if (!ModelState.IsValid)
      {
         return View(nameof(Index), await GetIndexModelAsync(
            model, cancellationToken));
      }

      var interest = new Interest
      {
         Slug = model.Slug!,
         DisplayName = model.DisplayName!,
         Summary = model.Summary!,
         IsActive = model.IsActive,
         IconCssClass = model.SelectedIconCssClass,
      };

      await _uncategorizedStore.InsertInterestAsync(
         interest, cancellationToken);

      TempData["SuccessMessage"] = $"Created interest \"{interest.DisplayName}\".";

      return RedirectToAction(nameof(Index));
   }

   [HttpGet]
   public async Task<IActionResult> InterestEdit(
      int id,
      CancellationToken cancellationToken)
   {
      var interest = await _uncategorizedStore.GetInterestAsync(
         id, cancellationToken);

      if (interest is null)
      {
         return NotFound();
      }

      var iconPicker = await LoadIconPickerAsync(cancellationToken);
      return View(
         nameof(InterestEdit), new InterestEditViewModel(interest, iconPicker));
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

      if (!ModelState.IsValid)
      {
         model.IconPicker = await LoadIconPickerAsync(cancellationToken);
         return View(nameof(InterestEdit), model);
      }

      var interest = await _uncategorizedStore.GetInterestAsync(
         id, cancellationToken);

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

   private async Task<IndexViewModel> GetIndexModelAsync(
      InterestEditViewModel? createModel,
      CancellationToken cancellationToken)
   {
      var iconPicker = await LoadIconPickerAsync(cancellationToken);
      var interests = await _uncategorizedStore.GetInterestsAsync(
         cancellationToken);

      createModel ??= new InterestEditViewModel()
      {
         IconPicker = iconPicker,
      };

      createModel.EditMode = EditModes.Create;
      createModel.IconPicker = iconPicker;

      return new IndexViewModel
      {
         Create = createModel,
         List = new InterestListViewModel(interests),
      };
   }

   private Task<FontAwesomeIconPickerViewModel> LoadIconPickerAsync(
      CancellationToken cancellationToken)
   {
      return FontAwesomeIconPickerViewModel.LoadAsync(
         _contentStore, cancellationToken: cancellationToken);
   }

   private readonly IUncategorizedStore _uncategorizedStore;
   private readonly IContentStore _contentStore;
}