// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Threading;
using System.Threading.Tasks;

using DavidBrowning.Admin.ViewModels.About;

using Microsoft.AspNetCore.Mvc;

namespace DavidBrowning.Admin.Controllers;

public class AboutController : Controller
{
   public Task<IActionResult> Index(
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   public Task<IActionResult> InterestList(
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpGet]
   public IActionResult InterestCreate()
   {
      throw new NotImplementedException();
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> InterestCreate(
      InterestEditViewModel model,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpGet]
   public Task<IActionResult> InterestEdit(
      int id,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> InterestEdit(
      int id,
      InterestEditViewModel model,
      CancellationToken cancellationToken)
   {
      if (model.Id != id)
      {
         return Task.FromResult<IActionResult>(BadRequest());
      }

      throw new NotImplementedException();
   }

   [HttpGet]
   public Task<IActionResult> InterestDelete(
      int id,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpPost]
   [ActionName(nameof(InterestDelete))]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> InterestDeleteConfirmed(
      InterestDeleteViewModel model,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }
}
