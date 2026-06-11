// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Admin.Extensions;
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
   public async Task<IActionResult> CredentialCreate(
      EditViewModel model,
      CancellationToken cancellationToken)
   {
      if(!ModelState.IsValid)
      {
         return PartialView(nameof(CredentialEdit), model);
      }

      var cred = model.ToCredential();
      await _workStore.InsertCredentialAsync(cred, cancellationToken);
      return PartialView(
         "CredentialEdit", await GetEditViewModelAsync(cancellationToken));
   }

   [HttpGet]
   public async Task<IActionResult> CredentialEdit(
      int id,
      CancellationToken cancellationToken)
   {
      var cred = await _workStore.GetCredentialAsync(id, cancellationToken);
      if (cred is null)
      {
         return NotFound();
      }

      var model = new EditViewModel(cred);
      return PartialView(nameof(CredentialEdit), model);
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> CredentialEdit(
      EditViewModel model,
      CancellationToken cancellationToken)
   {
      if(!ModelState.IsValid)
      {
         return PartialView(nameof(CredentialEdit), model);
      }
      var cred = model.ToCredential();
      bool updated = await _workStore.UpdateCredentialAsync(
         cred, cancellationToken);
      if(!updated)
      {
         return NotFound();
      }

      Response.TriggerAdminOffcanvasClose(
         ViewModels.Work.WorkAdminIds.CredentialEditOffcanvas);
      return PartialView("CredentialListRefresh", 
         await GetCredentialListViewModelAsync(cancellationToken));
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> CredentialDelete(
      int id,
      CancellationToken cancellationToken)
   {
      var cred = await _workStore.GetCredentialAsync(id, cancellationToken);
      if(cred is null)
      {
         return NotFound();
      }

      await _workStore.DeleteCredentialAsync(id, cancellationToken);
      return RedirectToAction(nameof(CredentialIndex));
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> CredentialReorder(
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
         await _workStore.ReorderCredentialsAsync(
            idsInDisplayOrder, cancellationToken);
      }
      catch (ArgumentException)
      {
         return BadRequest();
      }

      return RedirectToAction(nameof(CredentialIndex));
   }

   private async Task<IndexViewModel> GetCredentialIndexViewModelAsync(
      EditViewModel? existingCreateModel,
      CancellationToken cancellationToken)
   {
      existingCreateModel ??= await GetEditViewModelAsync(cancellationToken);

      return new IndexViewModel()
      {
         Create = existingCreateModel,
         List = await GetCredentialListViewModelAsync(cancellationToken),
         EditOffcanvas = new AdminOffcanvasViewModel()
         {
            Id = ViewModels.Work.WorkAdminIds.CredentialEditOffcanvas,
            Title = "Edit credential",
         }
      };
   }

   private async Task<EditViewModel> GetEditViewModelAsync(
      CancellationToken  cancellationToken)
   {
      return new EditViewModel()
      {
         
      };
   }

   private async Task<ListViewModel> GetCredentialListViewModelAsync(
      CancellationToken cancellationToken)
   {
      var credentials = await _workStore.GetCredentialsAsync(cancellationToken);
      return new ListViewModel()
      {
         ReorderList = new ReorderListViewModel()
         {
            Title = "Credentials",
            ReoderParameters = new ReoderParameters()
            {
               ReorderController = "Work",
               ReorderAction = "CredentialReorder",
            },
            EditOffcanvasId =
               ViewModels.Work.WorkAdminIds.CredentialEditOffcanvas,

            Items = credentials
               .OrderBy(c => c.SortOrder)
               .ThenBy(c => c.IssuingOrganization)
               .Select(c => new ReorderListItemViewModel()
               {
                  Id = c.Id,
                  DisplayName = c.Name,
                  SecondaryText = c.IssuingOrganization,
                  IsActive = c.IsActive,
                  SortOrder = c.SortOrder,
                  EditController = "Work",
                  EditAction = "CredentialEdit",
                  DeleteController = "Work",
                  DeleteAction = "Delete",
               })
               .ToList()
         }
      };
   }
}
