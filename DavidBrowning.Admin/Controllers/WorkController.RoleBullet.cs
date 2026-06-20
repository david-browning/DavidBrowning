// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Admin.ViewModels.Work.Experience;
using Microsoft.AspNetCore.Mvc;

namespace DavidBrowning.Admin.Controllers;

public partial class WorkController
{
   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> RoleBulletCreate(
      RoleBulletEditViewModel model,
      CancellationToken cancellationToken)
   {
      if (!ModelState.IsValid)
      {
         return PartialView(nameof(RoleBulletCreate), model);
      }

      if (model.ExperienceRoleId is null)
      {
         return BadRequest();
      }

      var role = await _workStore.GetRoleAsync(
         model.ExperienceRoleId.Value,
         cancellationToken);

      if (role is null)
      {
         return NotFound();
      }

      await _workStore.InsertRoleBulletAsync(
         model.ToBullet(),
         cancellationToken);

      if (!Request.Headers.ContainsKey("HX-Request"))
      {
         return RedirectToAction(nameof(RoleEdit), new
         {
            id = model.ExperienceRoleId,
         });
      }

      var roleModel = await GetRoleEditViewModelAsync(
         model.ExperienceRoleId.Value,
         cancellationToken);

      if (roleModel is null)
      {
         return NotFound();
      }

      return PartialView("RoleBulletCreateRefresh", roleModel);
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> RoleBulletDelete(
      int id,
      CancellationToken cancellationToken)
   {
      var bullet = await _workStore.GetRoleBulletAsync(id, cancellationToken);

      if (bullet is null)
      {
         return NotFound();
      }

      int roleId = bullet.ExperienceRoleId;

      bool deleted = await _workStore.DeleteRoleBulletAsync(
         id,
         cancellationToken);

      if (!deleted)
      {
         return NotFound();
      }

      if (!Request.Headers.ContainsKey("HX-Request"))
      {
         return RedirectToAction(nameof(RoleEdit), new
         {
            id = roleId,
         });
      }

      var roleModel = await GetRoleEditViewModelAsync(
         roleId,
         cancellationToken);

      if (roleModel is null)
      {
         return NotFound();
      }

      return PartialView("RoleBulletList", roleModel);
   }

   private async Task<RoleEditViewModel?> GetRoleEditViewModelAsync(
      int roleId,
      CancellationToken cancellationToken)
   {
      var role = await _workStore.GetRoleAsync(roleId, cancellationToken);

      return role is null
         ? null
         : new RoleEditViewModel(role);
   }
}