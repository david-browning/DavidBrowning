// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Admin.Extensions;
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
      return View(await GetIndexModelAsync(null, cancellationToken));
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> InterestCreate(
      InterestEditViewModel model,
      CancellationToken cancellationToken)
   {
      await FontAwesomeIconPickerViewModel.LoadAndValidateIconPickerAsync(
         _contentStore, model, ModelState, cancellationToken);

      if (!ModelState.IsValid)
      {
         if (Request.IsHtmxRequest())
         {
            return PartialView(nameof(InterestEdit), model);
         }

         return View(
            nameof(Index), await GetIndexModelAsync(model, cancellationToken));
      }

      var interest = model.ToInterest();
      await _uncategorizedStore.InsertInterestAsync(interest, cancellationToken);
      ModelState.Clear();

      if (Request.IsHtmxRequest())
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
         await FontAwesomeIconPickerViewModel.LoadIconPickerAsync(
            _contentStore, interest.IconCssClass, cancellationToken);
      var model = new InterestEditViewModel(interest, iconPicker);
      if (Request.IsHtmxRequest())
      {
         return PartialView(nameof(InterestEdit), model);
      }

      return View(nameof(InterestEdit), model);
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> InterestEdit(
      InterestEditViewModel model,
      CancellationToken cancellationToken)
   {
      model.IconPicker = await FontAwesomeIconPickerViewModel.LoadIconPickerAsync(
            _contentStore, model.SelectedIconCssClass, cancellationToken);

      if (!model.IconPicker.Supports(model.SelectedIconCssClass))
      {
         ModelState.AddModelError(
            nameof(model.SelectedIconCssClass), "Select a supported icon.");
      }

      if (!ModelState.IsValid)
      {
         return PartialView(nameof(InterestEdit), model);
      }

      var updatedInterest = model.ToInterest();
      bool updated = await _uncategorizedStore.UpdateInterestAsync(
         updatedInterest, cancellationToken);
      if (!updated)
      {
         return NotFound();
      }

      if (Request.IsHtmxRequest())
      {
         Response.TriggerAdminOffcanvasClose(
            AboutAdminIds.InterestEditOffcanvas);

         return PartialView(
            "_InterestListRefresh",
            await GetListViewModelAsync(cancellationToken));
      }

      TempData["SuccessMessage"] =
         $"Updated interest \"{updatedInterest.DisplayName}\".";

      return RedirectToAction(nameof(Index));
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
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
      string? selectedIconCssClass = createModel?.SelectedIconCssClass;

      var iconPicker =
         await FontAwesomeIconPickerViewModel.LoadIconPickerAsync(
            _contentStore, selectedIconCssClass, cancellationToken);


      createModel ??= new InterestEditViewModel()
      {
         IconPicker = iconPicker,
      };

      createModel.EditMode = EditModes.Create;
      createModel.IconPicker = iconPicker;

      return new IndexViewModel()
      {
         Create = createModel,
         List = await GetListViewModelAsync(cancellationToken),
         EditOffcanvas = new AdminOffcanvasViewModel()
         {
            Id = AboutAdminIds.InterestEditOffcanvas,
            Title = "Edit interest",
            Placeholder = "Select an interest to edit.",
            LoadingText = "Loading interest...",
         },
      };
   }

   private async Task<InterestListViewModel> GetListViewModelAsync(
      CancellationToken cancellationToken)
   {
      IReadOnlyList<Interest> interests =
         await _uncategorizedStore.GetInterestsAsync(cancellationToken);
      return new InterestListViewModel()
      {
         ReorderList = new ReorderListViewModel
         {
            Title = "Interests",
            Description =
               "Edit About-page interests or change their display order.",
            EmptyMessage =
               "No interests have been created yet.",
            ReoderParameters = new ReoderParameters()
            {
               ReorderController = "About",
               ReorderAction = "InterestReorder",
            },
            EditOffcanvasId = AboutAdminIds.InterestEditOffcanvas,

            Items = interests
               .OrderBy(interest => interest.SortOrder)
               .ThenBy(interest => interest.DisplayName)
               .Select(interest => new ReorderListItemViewModel
               {
                  Id = interest.Id,
                  DisplayName = interest.DisplayName,
                  SecondaryText = interest.Slug,
                  IconCssClass = interest.IconCssClass,
                  IsActive = interest.IsActive,
                  SortOrder = interest.SortOrder,
                  EditController = "About",
                  EditAction = "InterestEdit",
                  DeleteController = "About",
                  DeleteAction = "InterestDelete",
               })
               .ToList(),
         }
      };
   }
   
   private readonly IUncategorizedStore _uncategorizedStore;
   private readonly IContentStore _contentStore;
}