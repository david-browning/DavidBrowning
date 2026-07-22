// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Admin.Extensions;
using DavidBrowning.Admin.ViewModels;
using DavidBrowning.Admin.ViewModels.Writing;
using DavidBrowning.Infrastructure;
using DavidBrowning.Infrastructure.Data;
using DavidBrowning.Infrastructure.Data.Stores;
using DavidBrowning.Infrastructure.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DavidBrowning.Admin.Controllers;

[Authorize]
public partial class WritingController : Controller
{
   public WritingController(
      ISlugService slugService,
      IUncategorizedStore uncategorizedStore,
      IWritingStore writingStore,
      MarkdownPostContentRenderer postRendered)
   {
      _slugService = slugService;
      _uncategorizedStore = uncategorizedStore;
      _writingStore = writingStore;
      _postRendered = postRendered;
   }

   public async Task<IActionResult> Index(
      CancellationToken cancellationToken)
   {
      return View(await GetIndexModelAsync(null, null, cancellationToken));
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> StyleCreate(
      PostStyleEditViewModel model,
      CancellationToken cancellationToken)
   {
      if (!ModelState.IsValid)
      {
         return PartialView(nameof(StyleEdit), model);
      }

      try
      {
         await _writingStore.InsertPostStyleAsync(
            model.ToPostStyle(), cancellationToken);
      }
      catch (DuplicateSlugException)
      {
         ModelState.AddModelError(nameof(model.Slug),
            "Another post style already uses this slug.");
         return PartialView(nameof(StyleEdit), model);
      }

      ModelState.Clear();
      return PartialView("StyleCreateRefresh",
         await GetStylePanelViewModelAsync(null, cancellationToken));
   }

   [HttpGet]
   public async Task<IActionResult> StyleEdit(
      int id,
      CancellationToken cancellationToken)
   {
      var style = await _writingStore.GetPostStyleAsync(id, cancellationToken);
      if (style is null)
      {
         return NotFound();
      }

      return PartialView(nameof(StyleEdit), new PostStyleEditViewModel(style));
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> StyleEdit(
      PostStyleEditViewModel model,
      CancellationToken cancellationToken)
   {
      if (!ModelState.IsValid)
      {
         return PartialView(nameof(StyleEdit), model);
      }

      try
      {
         var updated = await _writingStore.UpdatePostStyleAsync(
            model.ToPostStyle(), cancellationToken);

         if (!updated)
         {
            return NotFound();
         }
      }
      catch (DuplicateSlugException)
      {
         ModelState.AddModelError(nameof(model.Slug),
            "Another post style already uses this slug.");
         return PartialView(nameof(StyleEdit), model);
      }

      Response.TriggerAdminOffcanvasClose(WritingAdminIds.StyleEditOffcanvas);
      return PartialView("StyleListRefresh",
         await GetStyleListAsync(cancellationToken));
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> TagCreate(
      WritingTagEditViewModel model,
      CancellationToken cancellationToken)
   {
      if (!ModelState.IsValid)
      {
         return PartialView(nameof(TagEdit), model);
      }

      try
      {
         await _writingStore.InsertTagAsync(model.ToTag(), cancellationToken);
      }
      catch (DuplicateSlugException)
      {
         ModelState.AddModelError(nameof(model.Slug),
            "Another post tag already uses this slug.");
         return PartialView(nameof(TagEdit), model);
      }

      ModelState.Clear();
      return PartialView("TagCreateRefresh",
         await GetTagPanelViewModelAsync(null, cancellationToken));
   }

   [HttpGet]
   public async Task<IActionResult> TagEdit(
      int id,
      CancellationToken cancellationToken)
   {
      var tag = await _writingStore.GetTagAsync(id, cancellationToken);
      if (tag is null)
      {
         return NotFound();
      }

      return PartialView(nameof(TagEdit), new WritingTagEditViewModel(tag));
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> TagEdit(
      WritingTagEditViewModel model,
      CancellationToken cancellationToken)
   {
      if (!ModelState.IsValid)
      {
         return PartialView(nameof(TagEdit), model);
      }

      try
      {
         var updated = await _writingStore.UpdateTagAsync(
            model.ToTag(), cancellationToken);

         if (!updated)
         {
            return NotFound();
         }
      }
      catch (DuplicateSlugException)
      {
         ModelState.AddModelError(nameof(model.Slug),
            "Another post tag already uses this slug.");
         return PartialView(nameof(TagEdit), model);
      }

      Response.TriggerAdminOffcanvasClose(WritingAdminIds.TagEditOffcanvas);
      return PartialView("TagListRefresh",
         await GetTagListViewModelAsync(cancellationToken));
   }

   private async Task<IndexViewModel> GetIndexModelAsync(
      PostStyleEditViewModel? existingPostStyleModel,
      WritingTagEditViewModel? existingTagModel,
      CancellationToken cancellationToken)
   {
      return new IndexViewModel()
      {
         Styles = await GetStylePanelViewModelAsync(
            existingPostStyleModel, cancellationToken),
         Tags = await GetTagPanelViewModelAsync(
            existingTagModel, cancellationToken),
         Posts = await GetPostListViewModelAsync(cancellationToken),
         TagEditOffcanvas = new AdminOffcanvasViewModel()
         {
            Id = WritingAdminIds.TagEditOffcanvas,
            Title = "Edit writing tag",
         },
         StyleEditOffcanvas = new AdminOffcanvasViewModel()
         {
            Id = WritingAdminIds.StyleEditOffcanvas,
            Title = "Edit post style",
         },
      };
   }

   private async Task<WritingTagPanelViewModel> GetTagPanelViewModelAsync(
      WritingTagEditViewModel? existingTagModel,
      CancellationToken cancellationToken)
   {
      return new WritingTagPanelViewModel()
      {
         Create = existingTagModel ?? new WritingTagEditViewModel(),
         Items = await GetTagListViewModelAsync(cancellationToken),
      };
   }

   private async Task<IReadOnlyList<WritingTagEditViewModel>> GetTagListViewModelAsync(
      CancellationToken cancellationToken)
   {
      var tags = await _writingStore.GetTagsAsync(cancellationToken);
      return tags.Select(tag => new WritingTagEditViewModel(tag)).ToList();
   }

   private async Task<PostStylePanelViewModel> GetStylePanelViewModelAsync(
      PostStyleEditViewModel? existingModel,
      CancellationToken cancellationToken)
   {
      return new PostStylePanelViewModel()
      {
         Create = existingModel ?? new PostStyleEditViewModel(),
         Items = await GetStyleListAsync(cancellationToken),
      };
   }

   private async Task<IReadOnlyList<PostStyleEditViewModel>> GetStyleListAsync(
      CancellationToken cancellationToken)
   {
      var styles = await _writingStore.GetPostStylesAsync(cancellationToken);
      return styles.Select(style => new PostStyleEditViewModel(style)).ToList();
   }

   private readonly IWritingStore _writingStore;
   private readonly IUncategorizedStore _uncategorizedStore;
   private readonly ISlugService _slugService;
   private readonly MarkdownPostContentRenderer _postRendered;
}
