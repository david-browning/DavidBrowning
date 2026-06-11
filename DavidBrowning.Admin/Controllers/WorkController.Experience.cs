// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Admin.Extensions;
using DavidBrowning.Admin.ViewModels;
using DavidBrowning.Admin.ViewModels.Work.Experience;
using DavidBrowning.Models.Work;
using Microsoft.AspNetCore.Mvc;

namespace DavidBrowning.Admin.Controllers;

public partial class WorkController
{
   [HttpGet]
   public async Task<IActionResult> ExperienceIndex(
      CancellationToken cancellationToken)
   {
      return View(
         await GetExperienceIndexViewModelAsync(null, cancellationToken));
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> ExperienceCreate(
      EditViewModel model,
      CancellationToken cancellationToken)
   {
      if (!ModelState.IsValid)
      {
         return PartialView(nameof(ExperienceEdit), model);
      }

      var exp = model.ToExperience();
      await _workStore.InsertExperienceAsync(exp, cancellationToken);
      ModelState.Clear();
      return PartialView(
         "ExperienceCreateRefresh",
         await GetExperienceIndexViewModelAsync(null, cancellationToken));
   }

   [HttpGet]
   public async Task<IActionResult> ExperienceEdit(
      int id,
      CancellationToken cancellationToken)
   {
      var exp = await _workStore.GetExperienceAsync(id, cancellationToken);
      if (exp is null)
      {
         return NotFound();
      }

      var roles = await _workStore.GetExperienceRolesAsync(
         id, cancellationToken);
      var model = new EditViewModel(
         exp, 
         await GetRolesListModelAsync(roles, cancellationToken));
      return PartialView(nameof(ExperienceEdit), model);
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> ExperienceEdit(
      EditViewModel model,
      CancellationToken cancellationToken)
   {
      if (!ModelState.IsValid)
      {
         return PartialView(nameof(ExperienceEdit), model);
      }

      var exp = model.ToExperience();
      bool updated = await _workStore.UpdateExperienceAsync(
         exp, cancellationToken);
      if (!updated)
      {
         return NotFound();
      }

      Response.TriggerAdminOffcanvasClose(
         ViewModels.Work.WorkAdminIds.ExperienceEditOffcanvas);
      return PartialView(
         "ExperienceListRefresh",
         await GetExperienceListViewModelAsync(
            await _workStore.GetExperienceAsync(cancellationToken), 
            cancellationToken));
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> ExperienceDelete(
      int id,
      CancellationToken cancellationToken)
   {
      var exp = await _workStore.GetExperienceAsync(id, cancellationToken);
      if (exp is null)
      {
         return NotFound();
      }

      await _workStore.DeleteExperienceAsync(id, cancellationToken);
      return RedirectToAction(nameof(ExperienceIndex));
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> ExperienceReorder(
      ReorderListRequestViewModel model,
      CancellationToken cancellationToken)
   {
      if (!ModelState.IsValid)
      {
         return BadRequest();
      }

      var idsInDisplayOrder = model.Items
         .OrderBy(item => item.SortOrder)
         .Select(item => item.Id)
         .ToList();

      try
      {
         await _workStore.ReorderExperienceAsync(
            idsInDisplayOrder, cancellationToken);
      }
      catch (ArgumentException)
      {
         return BadRequest();
      }

      return RedirectToAction(nameof(ExperienceIndex));
   }


   [HttpPost]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> ExperienceRoleReorder(
      ReorderListRequestViewModel model,
      CancellationToken cancellationToken)
   { 
      throw new NotImplementedException();
   }

   private async Task<IndexViewModel> GetExperienceIndexViewModelAsync(
      EditViewModel? existingCreateModel,
      CancellationToken cancellationToken)
   {
      var workExperience = await _workStore.GetExperienceAsync(
         cancellationToken);
      existingCreateModel ??= new EditViewModel();
      return new IndexViewModel()
      {
         Create = existingCreateModel,
         List = await GetExperienceListViewModelAsync(
            workExperience, cancellationToken),
         EditOffcanvas = new AdminOffcanvasViewModel()
         {
            Id = ViewModels.Work.WorkAdminIds.ExperienceEditOffcanvas,
            Title = "Edit work experience",
         },
      };
   }

   private async Task<ReorderListViewModel> GetRolesListModelAsync(
      IReadOnlyList<ExperienceRole> roles,
      CancellationToken cancellationToken)
   {
      return new ReorderListViewModel()
      {
         Title = "Roles",
         Items = roles.Select(role => new ReorderListItemViewModel()
         {
            Id = role.Id,
            DisplayName = role.Title,
            SecondaryText = role.DateDisplayText,
            SortOrder = role.SortOrder,
            IsActive = role.IsActive,
            DeleteAction = "Work",
            DeleteController = "",
            EditAction = "Work",
            EditController = "",
         }).ToList(),
      };
   }

   private async Task<ListViewModel> GetExperienceListViewModelAsync(
      IReadOnlyList<Experience> workExperience,
      CancellationToken cancellationToken)
   {
      var items = workExperience.Select(e => new ReorderListItemViewModel()
      {
         Id = e.Id,
         DisplayName = e.CompanyName,
         SecondaryText = $"{e.Roles.Count} roles",
         IsActive = e.IsActive,
         SortOrder = e.SortOrder,
         EditController = "Work",
         EditAction = nameof(ExperienceEdit),
         DeleteController = "Work",
         DeleteAction = nameof(ExperienceDelete),
      }).ToList();

      return new ListViewModel()
      {
         ReorderList = new ReorderListViewModel()
         {
            Title = "Work experience",
            ReoderParameters = new ReoderParameters()
            {
               ReorderController = "Work",
               ReorderAction = nameof(ExperienceReorder),
            },
            EditOffcanvasId = ViewModels.Work.WorkAdminIds.ExperienceEditOffcanvas,
            Items = items,
         }
      };
   }
}
