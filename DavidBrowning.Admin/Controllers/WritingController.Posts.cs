// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Admin.ViewModels.Writing.Posts;
using DavidBrowning.Models;
using DavidBrowning.Models.Writing;
using Microsoft.AspNetCore.Mvc;

namespace DavidBrowning.Admin.Controllers;

public partial class WritingController
{
   [HttpGet]
   public async Task<IActionResult> PostIndex(
      CancellationToken cancellationToken)
   {
      return View(await GetPostIndexViewModelAsync(
         null, null, cancellationToken));
   }

   [HttpGet]
   public Task<IActionResult> PostMetadataEdit(
      PostMetadataViewModel model,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpGet]
   public async Task<IActionResult> PostEdit(
      int id,
      CancellationToken cancellationToken)
   {
      var post = await _writingStore.GetPostAsync(id, cancellationToken);
      if (post is null)
      {
         return NotFound();
      }

      return View(await GetPostIndexViewModelAsync(
         post, post.CurrentRevisionId, cancellationToken));
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> PostCreate(
      PostMetadataViewModel model,
      CancellationToken cancellationToken)
   {
      if (!ModelState.IsValid)
      {
         await PopulatePostMetadataOptionsAsync(model, cancellationToken);
         return PartialView(nameof(PostMetadataEdit), model);
      }


      throw new NotImplementedException();
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> PostEdit(
      PostMetadataViewModel model,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpGet]
   public Task<IActionResult> PostRevisionEdit(
      int postId,
      int? revisionId,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> PostRevisionCreate(
      PostRevisionContentViewModel model,
      CancellationToken cancellationToken)
   {
      string createdBy = "David Browning";
      await _writingStore.InsertPostRevisionAsync(
         model.PostId,
         model.ContentFormat!.Value,
         model.Content,
         createdBy,
         cancellationToken);


      throw new NotImplementedException();
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> SetCurrentRevision(
      int postId,
      int revisionId,
      CancellationToken cancellationToken)
   {
      throw new NotImplementedException();
   }

   private async Task<IndexViewModel> GetPostIndexViewModelAsync(
      Post? post,
      int? selectedRevisionId,
      CancellationToken cancellationToken)
   {
      if (post is null)
      {
         return new IndexViewModel()
         {
            Metadata = await GetPostMetadataAsync(null, cancellationToken),
            RevisionHistory = new PostRevisionHistoryViewModel()
            {
               PostId = 0,
            }
         };

      }

      return new IndexViewModel()
      {
         Metadata = await GetPostMetadataAsync(post, cancellationToken),
         RevisionHistory = GetRevisionHistoryViewModel(post, selectedRevisionId),
         RevisionContent = GetRevisionContentViewModel(post, selectedRevisionId),
      };
   }

   private async Task<PostMetadataViewModel> GetPostMetadataAsync(
      Post? post,
      CancellationToken cancellationToken)
   {
      var styles = await GetPostStyleOptionsAsync(
         post?.PostStyleId,
         cancellationToken);

      var tags = await GetPostWritingTagOptionsAsync(cancellationToken);
      if (post is null)
      {
         return new PostMetadataViewModel()
         {
            PostStyleOptions = styles,
            WritingTagOptions = tags,
         };
      }

      return new PostMetadataViewModel(post, styles, tags);
   }

   private PostRevisionContentViewModel GetRevisionContentViewModel(
      Post post,
      int? revisionId)
   {
      if (revisionId is null)
      {
         return new PostRevisionContentViewModel()
         {
            PostId = post.Id,
            ContentFormat = ContentFormat.Markdown,
         };
      }

      var revision = post.Revisions.SingleOrDefault(
         revision => revision.Id == revisionId.Value);

      if (revision is null)
      {
         throw new InvalidOperationException(
            "The selected revision does not belong to this post.");
      }

      return new PostRevisionContentViewModel(revision, post.CurrentRevisionId);
   }

   private PostRevisionHistoryViewModel GetRevisionHistoryViewModel(
      Post post,
      int? selectedRevisionId)
   {
      int? resolvedSelectedRevisionId =
         selectedRevisionId ?? post.CurrentRevisionId;

      return new PostRevisionHistoryViewModel()
      {
         PostId = post.Id,
         CurrentRevisionId = post.CurrentRevisionId,
         SelectedRevisionId = resolvedSelectedRevisionId,
         Items = post.Revisions.Select(revision =>
            new PostRevisionListItemViewModel()
            {
               Id = revision.Id,
               RevisionNumber = revision.RevisionNumber,
               ContentFormat = revision.ContentFormat,
               CreatedBy = revision.CreatedBy,
               CreatedAtUtc = revision.CreatedAtUtc,
               IsCurrentRevision = revision.Id == post.CurrentRevisionId,
               IsSelectedRevision = revision.Id == resolvedSelectedRevisionId,
            }).ToList(),
      };
   }

   private async Task<PostListViewModel> GetPostListViewModelAsync(
      CancellationToken cancellationToken)
   {
      var posts = await _writingStore.GetAllPostsAsync(cancellationToken);
      return new PostListViewModel()
      {
         Items = posts.Select(post => new PostListItemViewModel(post)).ToList(),
      };
   }

   private async Task<IReadOnlyList<PostStyleOptionViewModel>> GetPostStyleOptionsAsync(
      int? selectedStyleId,
      CancellationToken cancellationToken)
   {
      var styles = await _writingStore.GetPostStylesAsync(cancellationToken);

      return styles
         .Where(style => style.IsActive || style.Id == selectedStyleId)
         .Select(style => new PostStyleOptionViewModel()
         {
            Id = style.Id,
            DisplayName = style.DisplayName,
            IsActive = style.IsActive,
         })
         .ToList();
   }

   private async Task<IReadOnlyList<WritingTagOptionViewModel>> GetPostWritingTagOptionsAsync(
      CancellationToken cancellationToken)
   {
      var tags = await _writingStore.GetTagsAsync(cancellationToken);
      return tags.Select(tag => new WritingTagOptionViewModel()
      {
         DisplayName = tag.DisplayName,
         Id = tag.Id,
         Slug = tag.Slug,
      }).ToList();
   }

   private async Task PopulatePostMetadataOptionsAsync(
      PostMetadataViewModel model,
      CancellationToken cancellationToken)
   {
      model.PostStyleOptions = await GetPostStyleOptionsAsync(
         model.PostStyleId, cancellationToken);
      model.WritingTagOptions = await GetPostWritingTagOptionsAsync(
         cancellationToken);
   }
}
