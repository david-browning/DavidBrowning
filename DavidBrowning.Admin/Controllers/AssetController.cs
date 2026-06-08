// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Threading;
using System.Threading.Tasks;

using DavidBrowning.Admin.ViewModels.Asset;

using Microsoft.AspNetCore.Mvc;

namespace DavidBrowning.Admin.Controllers;

public class AssetController : Controller
{
   public Task<IActionResult> Index(
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   public Task<IActionResult> List(
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpGet]
   public IActionResult Create()
   {
      throw new NotImplementedException();
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> Create(
      EditViewModel model,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpGet]
   public Task<IActionResult> Edit(
      int id,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> Edit(
      int id,
      EditViewModel model,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpGet]
   public Task<IActionResult> Delete(
      int id,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpPost]
   [ActionName(nameof(Delete))]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> DeleteConfirmed(
      DeleteViewModel model,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpGet]
   public Task<IActionResult> Download(
      string assetKey,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }
}
