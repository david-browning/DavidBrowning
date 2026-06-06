// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Infrastructure;
using DavidBrowning.Infrastructure.Cache;
using DavidBrowning.Infrastructure.Data.Stores;
using DavidBrowning.Infrastructure.Rendering;
using DavidBrowning.Models.Writing;
using DavidBrowning.Web.ViewModels;
using DavidBrowning.Web.ViewModels.Writing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DavidBrowning.Web.Controllers;

[Route("writing")]
public class WritingController : Controller
{
   public WritingController(
      IConfiguration configurationManager,
      JsonCache jsonCache,
      MarkdownPostContentRenderer postRendered,
      IWritingStore writingStore,
      ISlugService slugs,
      ISlugLookupService<WritingTag> tagStore)
   {
      _pageSize = configurationManager.GetValue<int>("Content:PageSize");
      _jsonCache = jsonCache;
      _postRendered = postRendered;
      _writingStore = writingStore;
      _slugService = slugs;
      _tagLookup = tagStore;
   }

   public Task<IActionResult> Index(CancellationToken cancellationToken)
   {
      return GetPageAsync(1, cancellationToken);
   }

   [HttpGet("page/{page:int:min(1)}")]
   public async Task<IActionResult> Page(
      int page,
      CancellationToken cancellationToken)
   {
      if (page == 1)
      {
         return RedirectToAction(nameof(Index));
      }

      return await GetPageAsync(page, cancellationToken);
   }

   /// <summary>
   /// Gets Published posts by writing tag.
   /// </summary>
   /// <param name="slug"></param>
   /// <param name="cancellationToken"></param>
   /// <returns></returns>
   [HttpGet("tags/{slug}")]
   public async Task<IActionResult> Tags(
      string slug,
      CancellationToken cancellationToken)
   {
      if (string.IsNullOrWhiteSpace(slug))
      {
         return NotFound();
      }

      var normalizedSlug = _slugService.CleanSlug(slug);
      var tag = await _tagLookup.GetBySlugAsync(
         normalizedSlug, cancellationToken);
      if (tag == null)
      {
         return NotFound();
      }

      var results = await _writingStore.GetPublishedPostsByTagSlugAsync(
         normalizedSlug, cancellationToken);
      FilteredResultsViewModel model = new()
      {
         PageTitle = $"{tag.DisplayName} Posts",
         FilterName = tag.DisplayName,
         FilterSlug = normalizedSlug,
         Results = results,
         ResultPartialName = "_PostCard"
      };

      return View("_FilteredResults", model);
   }

   /// <summary>
   /// Render one published post.
   /// </summary>
   /// <param name="slug"></param>
   /// <param name="cancellationToken"></param>
   /// <returns></returns>
   [HttpGet("details/{slug}")]
   public async Task<IActionResult> Details(
      string slug,
      CancellationToken cancellationToken)
   {
      if (string.IsNullOrWhiteSpace(slug))
      {
         return NotFound();
      }

      var post = await _writingStore.GetPublishedPostBySlugAsync(
         slug, cancellationToken);

      if (post is null)
      {
         return NotFound();
      }

      var revision = post.CurrentRevision ??
         throw new InvalidOperationException(
            "Published post is missing its current revision.");
      var body = await _postRendered.RenderAsync(
         revision, revision.AssetLinks.ToList(), cancellationToken);
      return View(new DetailsViewModel(post, body));
   }

   private async Task<IActionResult> GetPageAsync(
      int page,
      CancellationToken cancellationToken)
   {
      var model = await GetIndexModelAsync(page, cancellationToken);
      if (page > Math.Max(model.Pager.TotalPages, 1))
      {
         return NotFound();
      }

      return View("Index", model);
   }

   private async Task<IndexViewModel> GetIndexModelAsync(
      int page,
      CancellationToken cancellationToken)
   {
      var pagedPosts = await _writingStore.GetPagedPublishedPostsAsync(
         page, _pageSize, cancellationToken);

      IReadOnlyList<Post> featuredPosts =
         page == 1
            ? await _writingStore.GetFeaturedPostsAsync(cancellationToken)
            : Array.Empty<Post>();

      var heroData = await _jsonCache.GetJsonFileContentAsync<HeroData>(
         "Heros/Writing.json", cancellationToken);

      return new IndexViewModel()
      {
         PageTitle = "Writing",
         HeroTitle = heroData.Title ?? "Missing Data",
         HeroSubtitle = heroData.Subtitle ?? "Missing Data",
         Posts = pagedPosts.Items,
         FeaturedPosts = featuredPosts,
         Pager = new PagerViewModel(
            currentPage: pagedPosts.Page,
            totalPages: pagedPosts.TotalPages,
            controller: "Writing",
            indexAction: nameof(Index),
            pageAction: nameof(Page)),
      };
   }

   private readonly int _pageSize;
   private readonly JsonCache _jsonCache;
   private readonly IWritingStore _writingStore;
   private readonly ISlugService _slugService;
   private readonly ISlugLookupService<WritingTag> _tagLookup;
   private readonly MarkdownPostContentRenderer _postRendered;
}
