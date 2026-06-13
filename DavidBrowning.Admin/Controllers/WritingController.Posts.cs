using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Admin.ViewModels.Writing.Posts;
using DavidBrowning.Models.Writing;
using Microsoft.AspNetCore.Mvc;

namespace DavidBrowning.Admin.Controllers;

public partial class WritingController
{
   [HttpGet]
   public async Task<IActionResult> PostIndex(
      CancellationToken cancellationToken)
   {
      return View(GetPostIndexViewModel(null, null, cancellationToken));
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

      return View(GetPostIndexViewModel(
         post, post.CurrentRevisionId, cancellationToken));
   }

   private IndexViewModel GetPostIndexViewModel(
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
               
            },
            RevisionHistory = new List<PostRevision>(),
         };

      }

      return new IndexViewModel()
      {
         CurrentRevisionId = post.CurrentRevisionId,
         Metadata = GetPostMetadata(post, cancellationToken),
         RevisionHistory = post.Revisions.ToList(),
         RevisionContent = GetRevisionContentViewModel(post, selectedRevisionId),
      };
   }

   private PostMetadataViewModel GetPostMetadata(
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
      };
   }

   private PostRevisionContentViewModel GetRevisionContentViewModel(
      Post post,
      int? id)
   {
      var rev = post.Revisions.FirstOrDefault(revision => revision.Id == id);
      return new PostRevisionContentViewModel()
      {
         PostId = post.Id,
         Id = id,
         Content = rev?.Content,
         ContentFormat = rev?.ContentFormat,
         CreatedBy = rev?.CreatedBy,
         RevisionNumber = rev?.RevisionNumber
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
}
