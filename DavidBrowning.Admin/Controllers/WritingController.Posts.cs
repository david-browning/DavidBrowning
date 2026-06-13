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
   public Task<IActionResult> PostCreate(
      PostMetadataViewModel model,
      CancellationToken cancellationToken)
   {
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
   public Task<IActionResult> PostRevisionCreate(
      PostRevisionContentViewModel model,
      CancellationToken cancellationToken)
   {
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
            Metadata = new PostMetadataViewModel()
            {
               PostStyleOptions = await GetPostStyleOptionsAsync(
                  cancellationToken),
               WritingTagOptions = await GetPostWritingTagOptionsAsync(
                  cancellationToken),
            },
            RevisionHistory = new PostRevisionHistoryViewModel()
            {
               PostId = 0,
            }
         };

      }

      return new IndexViewModel()
      {
         CurrentRevisionId = post.CurrentRevisionId,
         Metadata = await GetPostMetadataAsync(post, cancellationToken),
         RevisionHistory = await GetRevisionHistoryViewModelAsync(
            post, cancellationToken),
         RevisionContent = GetRevisionContentViewModel(post, selectedRevisionId),
      };
   }

   private async Task<PostMetadataViewModel> GetPostMetadataAsync(
      Post? post,
      CancellationToken cancellationToken)
   {
      return new PostMetadataViewModel()
      {
         Id = post?.Id ?? null,
         IsFeatured = post?.IsFeatured ?? false,
         PostStyleId = post?.PostStyleId ?? null,
         Slug = post?.Slug ?? null,
         Status = post?.Status ?? null,
         Title = post?.Title ?? null,
         Summary = post?.Summary ?? null,
         Subtitle = post?.Subtitle ?? null,
         PublishedDateUtc = post?.PublishedDateUtc ?? null,
         EditMode = post is null ?
            ViewModels.EditModes.Create : ViewModels.EditModes.Edit,
         PostStyleOptions = await GetPostStyleOptionsAsync(cancellationToken),
         WritingTagOptions = await GetPostWritingTagOptionsAsync(cancellationToken),
         WritingTagIds = post is null ?
            new List<int>() :
            post.Tags.Select(tag => tag.WritingTagId).ToList(),
      };
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

   private async Task<PostRevisionHistoryViewModel> GetRevisionHistoryViewModelAsync(
      Post post,
      CancellationToken cancellationToken)
   {
      return new PostRevisionHistoryViewModel()
      {
         PostId = post.Id,
         CurrentRevisionId = post.CurrentRevisionId,
         SelectedRevisionId = post.CurrentRevisionId,
         Items = post.Revisions.Select(
            rev => new PostRevisionListItemViewModel(rev)).ToList()
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
      CancellationToken cancellationToken)
   {
      var styles = await _writingStore.GetPostStylesAsync(cancellationToken);
      return styles.Select(style => new PostStyleOptionViewModel()
      {
         DisplayName = style.DisplayName,
         Id = style.Id,
         IsActive = style.IsActive,
      }).ToList();
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
}
