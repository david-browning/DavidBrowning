// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Threading;
using System.Threading.Tasks;

using DavidBrowning.Admin.ViewModels.Writing;

using Microsoft.AspNetCore.Mvc;

namespace DavidBrowning.Admin.Controllers;

public class WritingController : Controller
{
   public Task<IActionResult> Index(
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   public Task<IActionResult> PostList(
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpGet]
   public IActionResult PostCreate()
   {
      throw new NotImplementedException();
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> PostCreate(
      PostEditViewModel model,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpGet]
   public Task<IActionResult> PostEdit(
      int id,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> PostEdit(
      int id,
      PostEditViewModel model,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpGet]
   public Task<IActionResult> PostDelete(
      int id,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpPost]
   [ActionName(nameof(PostDelete))]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> PostDeleteConfirmed(
      PostDeleteViewModel model,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   public Task<IActionResult> RevisionList(
      int postId,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpGet]
   public IActionResult RevisionCreate(
      int postId)
   {
      throw new NotImplementedException();
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> RevisionCreate(
      PostRevisionEditViewModel model,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpGet]
   public Task<IActionResult> RevisionEdit(
      int id,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> RevisionEdit(
      int id,
      PostRevisionEditViewModel model,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpGet]
   public Task<IActionResult> RevisionDelete(
      int id,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpPost]
   [ActionName(nameof(RevisionDelete))]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> RevisionDeleteConfirmed(
      PostRevisionDeleteModel model,
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
      WritingTagEditViewModel model,
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
      WritingTagEditViewModel model,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> TagDelete(
      int id,
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
