// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Admin.ViewModels.Writing;
using DavidBrowning.Infrastructure.Data.Stores;
using Microsoft.AspNetCore.Mvc;

namespace DavidBrowning.Admin.Controllers;

public class WritingController : Controller
{
   public WritingController(IWritingStore writingStore)
   {
      _writingStore = writingStore;
   }

   public async Task<IActionResult> Index(
      CancellationToken cancellationToken)
   {
      return View(await GetIndexModelAsync(null, null, cancellationToken));
   }

   [HttpGet]
   public Task<IActionResult> PostCreate(CancellationToken cancellationToken)
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
      PostEditViewModel model,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> StyleCreate(
      PostStyleEditViewModel model,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpGet]
   public Task<IActionResult> StyleEdit(
      int id,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> StyleEdit(
      PostStyleEditViewModel model,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> StyleDelete(
      int id,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   private async Task<IndexViewModel> GetIndexModelAsync(
      PostStyleEditViewModel? existingPostStyleModel,
      WritingTagEditViewModel? existingTagModel,
      CancellationToken cancellationToken)
   {
      var posts = await _writingStore.GetAllPostsAsync(cancellationToken);
      var postStyles = await _writingStore.GetPostStylesAsync(cancellationToken);
      var tags = await _writingStore.GetTagsAsync(cancellationToken);
      return new IndexViewModel()
      {
         Styles = new PostStylePanelViewModel()
         {
            Create = existingPostStyleModel ?? new PostStyleEditViewModel(),
            Items = postStyles.Select(
               style => new PostStyleEditViewModel(style)).ToList(),
         },
         Tags = new WritingTagPanelViewModel()
         {
            Create = existingTagModel ?? new WritingTagEditViewModel(),
            Items = tags.Select(tag => new WritingTagEditViewModel(tag)).ToList(),
         },
         Posts = new PostListViewModel()
         {
            Items = posts.Select(post => new PostListItemViewModel(post)).ToList(),
         }
      };
   }

   private readonly IWritingStore _writingStore;
}
