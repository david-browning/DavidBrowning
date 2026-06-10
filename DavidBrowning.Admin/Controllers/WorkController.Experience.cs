// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Admin.ViewModels;
using DavidBrowning.Admin.ViewModels.Work.Experience;
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
   public Task<IActionResult> ExperienceCreate(
      EditViewModel model,
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
      EditViewModel model,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> ExperienceDelete(
      int id,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> ExperienceReorder(
      ReorderListRequestViewModel model,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   private Task<IndexViewModel> GetExperienceIndexViewModelAsync(
      EditViewModel? existingCreateModel,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   private Task<ListViewModel> GetExperienceListViewModel(
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }
}
