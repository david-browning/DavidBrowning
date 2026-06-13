// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Threading;
using System.Threading.Tasks;

using DavidBrowning.Admin.ViewModels.Projects;

using Microsoft.AspNetCore.Mvc;

namespace DavidBrowning.Admin.Controllers;

public class ProjectsController : Controller
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
      ProjectEditViewModel model,
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
      ProjectEditViewModel model,
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
      ProjectDeleteViewModel model,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   public Task<IActionResult> LinkList(
      int projectId,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpGet]
   public IActionResult LinkCreate(
      int projectId)
   {
      throw new NotImplementedException();
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> LinkCreate(
      ProjectLinkEditViewModel model,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpGet]
   public Task<IActionResult> LinkEdit(
      int id,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> LinkEdit(
      int id,
      ProjectLinkEditViewModel model,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> LinkDelete(
      int id,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   public Task<IActionResult> OriginList(
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpGet]
   public IActionResult OriginCreate()
   {
      throw new NotImplementedException();
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> OriginCreate(
      ProjectOriginEditViewModel model,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpGet]
   public Task<IActionResult> OriginEdit(
      int id,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> OriginEdit(
      int id,
      ProjectOriginEditViewModel model,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> OriginDelete(
      int id,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   public Task<IActionResult> StackList(
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpGet]
   public IActionResult StackCreate()
   {
      throw new NotImplementedException();
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> StackCreate(
      ProjectStackEditViewModel model,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpGet]
   public Task<IActionResult> StackEdit(
      int id,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> StackEdit(
      int id,
      ProjectStackEditViewModel model,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> StackDelete(
      int id,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   public Task<IActionResult> TagList(
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpGet]
   public IActionResult TagCreate()
   {
      throw new NotImplementedException();
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> TagCreate(
      ProjectTagEditViewModel model,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpGet]
   public Task<IActionResult> TagEdit(
      int id,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> TagEdit(
      int id,
      ProjectTagEditViewModel model,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   public Task<IActionResult> TypeList(
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   public Task<IActionResult> StatusList(
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   public Task<IActionResult> VisibilityList(
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }
}
