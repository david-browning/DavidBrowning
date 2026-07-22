// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Admin.ViewModels;
using DavidBrowning.Admin.ViewModels.Writing.Posts;
using DavidBrowning.Infrastructure.Data;
using DavidBrowning.Models;
using DavidBrowning.Models.Writing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DavidBrowning.Admin.Controllers;

[Authorize]
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

      return View(nameof(PostIndex), await GetPostIndexViewModelAsync(
         post, post.CurrentRevisionId, cancellationToken));
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> PostCreate(
      PostMetadataViewModel model,
      CancellationToken cancellationToken)
   {
      model.EditMode = ViewModels.EditModes.Create;
      await PopulatePostMetadataOptionsAsync(model, cancellationToken);

      if (!ModelState.IsValid)
      {
         return PartialView("PostMetadataEdit", model);
      }

      int postId = await _writingStore.InsertPostAsync(
         model.ToPost(), model.WritingTagIds, cancellationToken);
      return RedirectToPostEdit(postId);
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> PostEdit(
      PostMetadataViewModel model,
      CancellationToken cancellationToken)
   {
      await PopulatePostMetadataOptionsAsync(model, cancellationToken);

      if (!ModelState.IsValid)
      {
         return PartialView("PostMetadataEdit", model);
      }

      try
      {
         model.PublishedDateUtc = DateTime.UtcNow;
         var result = await _writingStore.UpdatePostAsync(
            model.ToPost(), model.WritingTagIds, cancellationToken);
         if (!result)
         {
            return BadRequest();
         }
      }
      catch (DuplicateSlugException)
      {
         ModelState.AddModelError(nameof(model.Slug),
            "Another post already uses this slug.");
         return PartialView("PostMetadataEdit", model);
      }

      return PartialView("PostMetadataEdit", model);
   }

   [HttpGet]
   public async Task<IActionResult> PostRevisionEdit(
      int postId,
      int? revisionId,
      CancellationToken cancellationToken)
   {
      var post = await _writingStore.GetPostAsync(postId, cancellationToken);
      if (post is null)
      {
         return NotFound();
      }

      if (revisionId is null)
      {
         var createModel = new PostRevisionContentViewModel()
         {
            PostId = postId,
            ContentFormat = ContentFormat.Markdown,
         };

         var emptyPreviewModel = new IndexViewModel()
         {
            Metadata = await GetPostMetadataAsync(post, cancellationToken),
            RevisionHistory = GetRevisionHistoryViewModel(post, null),
            RevisionContent = createModel,
            AssetChooser = await GetAssetChooserViewModelAsync(cancellationToken),
            ContentPreview = null,
         };

         return PartialView("PostRevisionEditRefresh", emptyPreviewModel);
      }

      var revision = await _writingStore.GetPostRevisionAsync(
         postId, revisionId.Value, cancellationToken);

      if (revision is null)
      {
         return NotFound();
      }

      var viewModel = await GetPostIndexViewModelAsync(
         post, revision.Id, cancellationToken);

      return PartialView("PostRevisionEditRefresh", viewModel);
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> PostRevisionCreate(
      PostRevisionContentViewModel model,
      CancellationToken cancellationToken)
   {
      if (!ModelState.IsValid)
      {
         return PartialView(nameof(PostRevisionEdit), model);
      }

      string createdBy = "David Browning";

      var assetLinks = model.AssetLinks
         .Select(link => new PostRevisionAssetLink()
         {
            SiteAssetId = link.SiteAssetId,
            ReferenceKey = link.ReferenceKey,
            Caption = link.Caption,
            AltTextOverride = link.AltTextOverride,
         })
         .ToList();

      var revision = await _writingStore.InsertPostRevisionAsync(
         model.PostId, model.ContentFormat!.Value, model.Content,
         assetLinks, createdBy, cancellationToken);

      if (revision is null)
      {
         return NotFound();
      }

      var post = await _writingStore.GetPostAsync(
         model.PostId, cancellationToken);
      if (post is null)
      {
         return NotFound();
      }

      var viewModel = await GetPostIndexViewModelAsync(
         post, revision.Id, cancellationToken);
      return PartialView("PostRevisionCreateRefresh", viewModel);
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> SetCurrentRevision(
      int postId,
      int revisionId,
      CancellationToken cancellationToken)
   {
      var post = await _writingStore.GetPostAsync(postId, cancellationToken);
      if (post is null)
      {
         return NotFound();
      }

      var updated = await _writingStore.SetCurrentRevisionAsync(
         postId, revisionId, cancellationToken);
      if (!updated)
      {
         return BadRequest();
      }

      return RedirectToPostEdit(postId);
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> PostRevisionPreview(
      PostRevisionContentViewModel model,
      CancellationToken cancellationToken)
   {
      if (model.ContentFormat != ContentFormat.Markdown)
      {
         return PartialView(
            "PostRevisionPreviewError",
            "Preview is currently only supported for Markdown content.");
      }

      try
      {
         var revision = new PostRevision()
         {
            PostId = model.PostId,
            RevisionNumber = 0,
            ContentFormat = model.ContentFormat.Value,
            Content = model.Content ?? string.Empty,
            CreatedBy = "Preview",
         };

         var assetLinks = await GetPreviewAssetLinksAsync(
            model.AssetLinks,
            cancellationToken);

         var rendered = await _postRendered.RenderAsync(
            revision,
            assetLinks,
            cancellationToken);

         return PartialView("PostRevisionPreviewBody", rendered);
      }
      catch (InvalidOperationException ex)
      {
         return PartialView("PostRevisionPreviewError", ex.Message);
      }
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
            },
            AssetChooser = await GetAssetChooserViewModelAsync(cancellationToken),
            ContentPreview = null,
            RevisionContent = null,
         };
      }

      var revision = post.Revisions.SingleOrDefault(
        revision => revision.Id == (selectedRevisionId ?? 0));
      return new IndexViewModel()
      {
         Metadata = await GetPostMetadataAsync(post, cancellationToken),
         RevisionHistory = GetRevisionHistoryViewModel(post, selectedRevisionId),
         RevisionContent = GetRevisionContentViewModel(post, selectedRevisionId),
         AssetChooser = await GetAssetChooserViewModelAsync(cancellationToken),
         ContentPreview = revision is not null ?
            await _postRendered.RenderAsync(
               revision, revision.AssetLinks.ToList(), cancellationToken) :
            null,
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
         Items = post.Revisions
         .Select(revision => new PostRevisionListItemViewModel(
            revision, post.CurrentRevisionId, resolvedSelectedRevisionId))
         .ToList(),
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

   private async Task<AssetChooserViewModel> GetAssetChooserViewModelAsync(
      CancellationToken cancellationToken)
   {
      var assets = await _uncategorizedStore.GetSiteAssetsAsync(
         cancellationToken);
      return new AssetChooserViewModel()
      {
         Assets = GetAssetChooserItems(assets),
      };
   }

   private IReadOnlyList<AssetChooserItemViewModel> GetAssetChooserItems(
      IReadOnlyList<SiteAsset> assets)
   {
      var usedKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
      var items = new List<AssetChooserItemViewModel>();

      foreach (var asset in assets.OrderBy(asset => asset.AssetKey))
      {
         string baseKey = Path.GetFileNameWithoutExtension(asset.AssetKey);
         string referenceKey = _slugService.CreateSlug(baseKey);
         string uniqueReferenceKey = GetUniqueReferenceKey(referenceKey, usedKeys);

         items.Add(new AssetChooserItemViewModel()
         {
            Id = asset.Id,
            AssetKey = asset.AssetKey,
            ContentType = asset.ContentType,
            ReferenceKey = uniqueReferenceKey,
         });
      }

      return items;
   }

   private static string GetUniqueReferenceKey(
      string referenceKey,
      ISet<string> usedKeys)
   {
      if (usedKeys.Add(referenceKey))
      {
         return referenceKey;
      }

      for (int i = 2; ; i++)
      {
         string candidate = $"{referenceKey}-{i}";

         if (usedKeys.Add(candidate))
         {
            return candidate;
         }
      }
   }

   private async Task<IReadOnlyList<PostRevisionAssetLink>> GetPreviewAssetLinksAsync(
      IReadOnlyList<AssetLinkInputViewModel> inputLinks,
      CancellationToken cancellationToken)
   {
      var links = new List<PostRevisionAssetLink>();

      foreach (var inputLink in inputLinks
         .GroupBy(link => link.ReferenceKey, StringComparer.OrdinalIgnoreCase)
         .Select(group => group.First()))
      {
         var asset = await _uncategorizedStore.GetAssetAsync(
            inputLink.SiteAssetId,
            cancellationToken);

         if (asset is null)
         {
            throw new InvalidOperationException(
               $"The linked asset '{inputLink.ReferenceKey}' no longer exists.");
         }

         links.Add(new PostRevisionAssetLink()
         {
            SiteAssetId = asset.Id,
            SiteAsset = asset,
            ReferenceKey = inputLink.ReferenceKey,
            Caption = inputLink.Caption,
            AltTextOverride = inputLink.AltTextOverride,
         });
      }

      return links;
   }

   private IActionResult RedirectToPostEdit(int postId)
   {
      var url = Url.Action(nameof(PostEdit), new
      {
         id = postId,
      });

      if (string.IsNullOrWhiteSpace(url))
      {
         throw new InvalidOperationException(
            "Could not build the post edit URL.");
      }

      if (Request.Headers.ContainsKey("HX-Request"))
      {
         Response.Headers["HX-Redirect"] = url;
         return Ok();
      }

      return RedirectToAction(nameof(PostEdit), new
      {
         id = postId,
      });
   }
}
