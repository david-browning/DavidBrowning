using System;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Admin.ViewModels.Writing.Posts;
using Microsoft.AspNetCore.Mvc;

namespace DavidBrowning.Admin.Controllers;

public partial class WritingController
{
   [HttpGet]
   public async Task<IActionResult> PostIndex(
      CancellationToken cancellationToken)
   {
      return View(await GetPostIndexViewModelAsync(
         null, cancellationToken));
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
      PostEditViewModel model,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   private async Task<IndexViewModel> GetPostIndexViewModelAsync(
      PostEditViewModel? existingCreateModel,
      CancellationToken cancellationToken)
   {
      return new IndexViewModel()
      {
         Create = existingCreateModel ?? new PostEditViewModel(),
      };
   }
}
