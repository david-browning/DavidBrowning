// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Admin.ViewModels;
using DavidBrowning.Admin.ViewModels.Work.Experience;
using Microsoft.AspNetCore.Mvc;

namespace DavidBrowning.Admin.Controllers;

public partial class WorkController
{
   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> RoleCreate(
      RoleEditViewModel model,
      CancellationToken cancellationToken)
   {
      if (!ModelState.IsValid)
      {
         return PartialView(nameof(RoleCreate), model);
      }

      int experienceId = model.ExperienceId!.Value;

      var experience = await _workStore.GetExperienceAsync(
         experienceId, cancellationToken);

      if (experience is null)
      {
         return NotFound();
      }

      var role = model.ToRole();
      await _workStore.InsertRoleAsync(role, cancellationToken);
      ModelState.Clear();
      return PartialView(
         "RoleCreateRefresh",
         await GetRoleListViewModelAsync(experienceId, cancellationToken));
   }

   [HttpGet]
   public async Task<IActionResult> RoleEdit(
      int id,
      CancellationToken cancellationToken)
   {
      var model = await GetRoleEditViewModelAsync(id, cancellationToken);
      if (model is null)
      {
         return NotFound();
      }

      return View(nameof(RoleEdit), model);
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> RoleEdit(
      RoleEditViewModel model,
      CancellationToken cancellationToken)
   {
      model.EditMode = EditModes.Edit;

      if (!ModelState.IsValid)
      {
         return View(nameof(RoleEdit), model);
      }

      if (model.Id is null || model.ExperienceId is null)
      {
         return BadRequest();
      }

      var storedRole = await _workStore.GetRoleAsync(
         model.Id.Value, cancellationToken);
      if (storedRole is null)
      {
         return NotFound();
      }

      if (storedRole.ExperienceId != model.ExperienceId.Value)
      {
         return BadRequest();
      }

      var role = model.ToRole();
      bool updated = await _workStore.UpdateRoleAsync(role, cancellationToken);
      if (!updated)
      {
         return NotFound();
      }

      return RedirectToAction(nameof(RoleEdit), new { id = model.Id, });
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> RoleDelete(
      int id,
      CancellationToken cancellationToken)
   {
      var role = await _workStore.GetRoleAsync(id, cancellationToken);
      if (role is null)
      {
         return NotFound();
      }

      bool deleted = await _workStore.DeleteRoleAsync(id, cancellationToken);
      if (!deleted)
      {
         return NotFound();
      }

      return PartialView(
         "_ReorderList",
         await GetRoleReorderListViewModelAsync(
            role.ExperienceId, cancellationToken));
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> RoleReorder(
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
      if (idsInDisplayOrder.Count == 0)
      {
         return BadRequest();
      }

      var firstRole = await _workStore.GetRoleAsync(
         idsInDisplayOrder[0], cancellationToken);
      if (firstRole is null)
      {
         return NotFound();
      }

      try
      {
         await _workStore.ReorderExperienceRolesAsync(
            firstRole.ExperienceId, idsInDisplayOrder, cancellationToken);
      }
      catch (InvalidOperationException)
      {
         return BadRequest();
      }

      return PartialView(
         "_ReorderList",
         await GetRoleReorderListViewModelAsync(
            firstRole.ExperienceId, cancellationToken));
   }
}
