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
      return View(await GetExperienceIndexViewModelAsync(
         null, cancellationToken));
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

      var experience = model.ToExperience();

      await _workStore.InsertExperienceAsync(experience, cancellationToken);
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
      var model = await GetExperienceEditViewModelAsync(id, cancellationToken);
      if (model is null)
      {
         return NotFound();
      }

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
         model.EditMode = EditModes.Edit;
         model.Roles = await GetRoleListViewModelAsync(
            model.Id, cancellationToken);
         return PartialView(nameof(ExperienceEdit), model);
      }

      var experience = model.ToExperience();
      bool updated = await _workStore.UpdateExperienceAsync(
         experience, cancellationToken);
      if (!updated)
      {
         return NotFound();
      }

      Response.TriggerAdminOffcanvasClose(
         ViewModels.Work.WorkAdminIds.ExperienceEditOffcanvas);

      return PartialView(
         "ExperienceListRefresh",
         GetExperienceListViewModel(
            await _workStore.GetExperienceAsync(cancellationToken),
            cancellationToken));
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> ExperienceDelete(
      int id,
      CancellationToken cancellationToken)
   {
      var experience = await _workStore.GetExperienceAsync(id, cancellationToken);
      if (experience is null)
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
      catch (InvalidOperationException)
      {
         return BadRequest();
      }

      return RedirectToAction(nameof(ExperienceIndex));
   }

   private async Task<EditViewModel?> GetExperienceEditViewModelAsync(
      int experienceId,
      CancellationToken cancellationToken)
   {
      var experience = await _workStore.GetExperienceAsync(
         experienceId, cancellationToken);
      if (experience is null)
      {
         return null;
      }

      return new EditViewModel(experience,
         await GetRoleReorderListViewModelAsync(
            experienceId, cancellationToken));
   }

   private async Task<IndexViewModel> GetExperienceIndexViewModelAsync(
      EditViewModel? existingCreateModel,
      CancellationToken cancellationToken)
   {
      var workExperience = await _workStore.GetExperienceAsync(cancellationToken);
      existingCreateModel ??= new EditViewModel()
      {
         Roles = await GetRoleListViewModelAsync(null, cancellationToken),
      };

      return new IndexViewModel()
      {
         Create = existingCreateModel,
         List = GetExperienceListViewModel(
            workExperience, cancellationToken),
         EditOffcanvas = new AdminOffcanvasViewModel()
         {
            Id = ViewModels.Work.WorkAdminIds.ExperienceEditOffcanvas,
            Title = "Edit work experience",
            // Use a wider off canvas for this view.
            CssClass = "admin-offcanvas-wide",
         },
      };
   }

   private ListViewModel GetExperienceListViewModel(
      IReadOnlyList<Experience> workExperience,
      CancellationToken cancellationToken)
   {
      var items = workExperience.Select(
         experience => new ReorderListItemViewModel()
         {
            Id = experience.Id,
            DisplayName = experience.CompanyName,
            SecondaryText = $"{experience.Roles.Count} roles",
            IsActive = experience.IsActive,
            SortOrder = experience.SortOrder,
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

   private async Task<RoleEditListViewModel> GetRoleListViewModelAsync(
      int? experienceId,
      CancellationToken cancellationToken)
   {
      return new RoleEditListViewModel()
      {
         Create = new RoleEditViewModel()
         {
            ExperienceId = experienceId,
         },
         Roles = await GetRoleReorderListViewModelAsync(
            experienceId, cancellationToken),
      };
   }

   private async Task<ReorderListViewModel> GetRoleReorderListViewModelAsync(
      int? experienceId,
      CancellationToken cancellationToken)
   {
      var roles = experienceId is not null ?
         await _workStore.GetExperienceRolesAsync(
            experienceId.Value, cancellationToken) :
            new List<ExperienceRole>();

      return new ReorderListViewModel()
      {
         Title = "Roles",
         Description = "Arrange the display order for this company.",
         RenderCard = true,
         Compact = true,
         IconOnlyDelete = true,
         EditOffcanvasId = ViewModels.Work.WorkAdminIds.ExperienceEditOffcanvas,
         ReoderParameters = new ReoderParameters()
         {
            ReorderController = "Work",
            ReorderAction = nameof(RoleReorder),
         },

         Items = roles
            .OrderBy(role => role.SortOrder)
            .ThenBy(role => role.Title)
            .Select(role =>
               new ReorderListItemViewModel()
               {
                  Id = role.Id,
                  DisplayName = role.Title,
                  SecondaryText = $"{role.Bullets.Count} bullets",
                  SortOrder = role.SortOrder,
                  IsActive = role.IsActive,
                  DeleteController = "Work",
                  DeleteAction = nameof(RoleDelete),
                  EditController = "Work",
                  EditAction = nameof(RoleEdit),
               })
            .ToList(),
      };
   }
}
