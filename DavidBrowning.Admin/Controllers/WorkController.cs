// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Threading;
using System.Threading.Tasks;

using DavidBrowning.Admin.ViewModels.Work;

using Microsoft.AspNetCore.Mvc;

namespace DavidBrowning.Admin.Controllers;

public class WorkController : Controller
{
   public Task<IActionResult> Index(
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   public Task<IActionResult> CredentialList(
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpGet]
   public IActionResult CredentialCreate()
   {
      throw new NotImplementedException();
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> CredentialCreate(
      CredentialEditViewModel model,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpGet]
   public Task<IActionResult> CredentialEdit(
      int id,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> CredentialEdit(
      int id,
      CredentialEditViewModel model,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpGet]
   public Task<IActionResult> CredentialDelete(
      int id,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpPost]
   [ActionName(nameof(CredentialDelete))]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> CredentialDeleteConfirmed(
      CredentialDeleteViewModel model,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   public Task<IActionResult> ExperienceList(
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpGet]
   public IActionResult ExperienceCreate()
   {
      throw new NotImplementedException();
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> ExperienceCreate(
      ExperienceEditViewModel model,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpGet]
   public Task<IActionResult> ExperienceEdit(
      int id,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> ExperienceEdit(
      int id,
      ExperienceEditViewModel model,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpGet]
   public Task<IActionResult> ExperienceDelete(
      int id,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpPost]
   [ActionName(nameof(ExperienceDelete))]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> ExperienceDeleteConfirmed(
      ExperienceDeleteViewModel model,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   public Task<IActionResult> ExperienceRoleList(
      int experienceId,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpGet]
   public IActionResult ExperienceRoleCreate(
      int experienceId)
   {
      throw new NotImplementedException();
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> ExperienceRoleCreate(
      ExperienceRoleEditViewModel model,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpGet]
   public Task<IActionResult> ExperienceRoleEdit(
      int id,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> ExperienceRoleEdit(
      int id,
      ExperienceRoleEditViewModel model,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpGet]
   public Task<IActionResult> ExperienceRoleDelete(
      int id,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpPost]
   [ActionName(nameof(ExperienceRoleDelete))]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> ExperienceRoleDeleteConfirmed(
      ExperienceRoleDeleteViewModel model,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   public Task<IActionResult> ExperienceRoleBulletList(
      int experienceRoleId,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpGet]
   public IActionResult ExperienceRoleBulletCreate(
      int experienceRoleId)
   {
      throw new NotImplementedException();
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> ExperienceRoleBulletCreate(
      ExperienceRoleBulletEditViewModel model,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpGet]
   public Task<IActionResult> ExperienceRoleBulletEdit(
      int id,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> ExperienceRoleBulletEdit(
      int id,
      ExperienceRoleBulletEditViewModel model,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpGet]
   public Task<IActionResult> ExperienceRoleBulletDelete(
      int id,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpPost]
   [ActionName(nameof(ExperienceRoleBulletDelete))]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> ExperienceRoleBulletDeleteConfirmed(
      ExperienceRoleBulletDeleteViewModel model,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   public Task<IActionResult> StyleList(
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }
}
