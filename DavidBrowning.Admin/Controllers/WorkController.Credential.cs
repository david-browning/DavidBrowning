// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Admin.ViewModels;
using DavidBrowning.Admin.ViewModels.Work.Credentials;
using Microsoft.AspNetCore.Mvc;

namespace DavidBrowning.Admin.Controllers;

public partial class WorkController
{
   [HttpGet]
   public async Task<IActionResult> CredentialIndex(
      CancellationToken cancellationToken)
   {
      return View(
         await GetCredentialIndexViewModelAsync(null, cancellationToken));
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> CredentialCreate(
      EditViewModel model,
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
      EditViewModel model,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> CredentialDelete(
      int id,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> CredentialReorder(
      ReorderListRequestViewModel model,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   private Task<IndexViewModel> GetCredentialIndexViewModelAsync(
      EditViewModel? existingCreateModel,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   private Task<ListViewModel> GetCredentialListViewModelAsync(
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }
}
