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
      if(!ModelState.IsValid)
      {
         return PartialView(nameof(RoleEdit), model);
      }

      var role = model.ToRole();
      await _workStore.InsertRoleAsync(role, cancellationToken);
      ModelState.Clear();

      throw new NotImplementedException();
   }

   [HttpGet]
   public async Task<IActionResult> RoleEdit(
      int id,
      CancellationToken cancellationToken)
   {
      var role = await _workStore.GetRoleAsync(id, cancellationToken);
      if (role is null)
      {
         return NotFound();
      }

      var model = new RoleEditViewModel(role);
      return PartialView(nameof(RoleEdit), model);
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> RoleEdit(
      RoleEditViewModel model,
      CancellationToken cancellationToken)
   {
      if(!ModelState.IsValid)
      {
         return PartialView(nameof(RoleEdit), model);
      }

      var role = model.ToRole();
      bool updated = await _workStore.UpdateRoleAsync(role, cancellationToken);
      if(!updated)
      {
         return NotFound();
      }

      throw new NotImplementedException();
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> RoleDelete(
      int id,
      CancellationToken cancellationToken)
   {
      var role = await _workStore.GetRoleAsync(id, cancellationToken);
      if(role is null)
      {
         return NotFound();
      }

      await _workStore.DeleteRoleAsync(id, cancellationToken);
      throw new NotImplementedException();
   }

   [HttpPost]
   [ValidateAntiForgeryToken] 
   public async Task<IActionResult> RoleReorder(
      ReorderListRequestViewModel model,
      CancellationToken cancellationToken)
   {
      if(!ModelState.IsValid)
      {
         return BadRequest();
      }

      var idsInDisplayOrder = model.Items
         .OrderBy(item => item.SortOrder)
         .Select(item => item.Id)   
         .ToList();

      try
      {
         await _workStore.ReorderExperienceRolesAsync(
            idsInDisplayOrder, cancellationToken);
      }
      catch(ArgumentException)
      {
         return BadRequest();
      }

      throw new NotImplementedException();
   }
}
