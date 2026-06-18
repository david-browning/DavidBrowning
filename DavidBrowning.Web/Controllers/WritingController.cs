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
using DavidBrowning.Models;
using DavidBrowning.Models.Writing;
using DavidBrowning.ViewModels;
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
      UrlBuilder urlBuilder,
      IWritingStore writingStore,
      ISlugService slugs,
      ISlugLookupService<WritingTag> tagStore,
      StructuredDataBuilder jsonDataBuilder)
   {
      _pageSize = configurationManager.GetValue<int>("Content:PageSize");
      _jsonCache = jsonCache;
      _postRendered = postRendered;
      _writingStore = writingStore;
      _slugService = slugs;
      _tagLookup = tagStore;
      _urlBuilder = urlBuilder;
      _jsonDataBuilder = jsonDataBuilder;
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

      PostRevision? revision = post.CurrentRevision;
      RenderedContent body = revision is not null ?
         await _postRendered.RenderAsync(
            revision, revision.AssetLinks.ToList(), cancellationToken) :
         RenderedContent.Empty;
      SeoMetadataViewModel seo = new()
      {
         Title = post.Title,
         Description = post.Summary ?? post.Slug,
         CanonicalUrl = _urlBuilder.GetAbsoluteUrl($"/writing/{post.Slug}"),
         NoIndex = false,
         OpenGraphType = "article",
         StructuredData = _jsonDataBuilder.CreateWritingPostMetadata(post),
      };

      return View(new DetailsViewModel(post, body, seo));
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

      IReadOnlyList<Post> featuredPosts = page == 1 ?
         await _writingStore.GetFeaturedPostsAsync(cancellationToken) :
         Array.Empty<Post>();

      var hero = await _jsonCache.GetJsonFileContentAsync<HeroData>(
         "heros/writing.json", cancellationToken);
      ArgumentNullException.ThrowIfNullOrEmpty(hero.Title);
      ArgumentNullException.ThrowIfNullOrEmpty(hero.Subtitle);
      ArgumentNullException.ThrowIfNullOrEmpty(hero.Lede);

      return new IndexViewModel()
      {
         PageTitle = hero.Title,
         HeroTitle = hero.Subtitle,
         Lede = hero.Lede,
         Posts = pagedPosts.Items,
         FeaturedPosts = featuredPosts,
         Pager = new PagerViewModel(
            pagedPosts.Page,
            (int)Math.Ceiling((double)pagedPosts.TotalCount / (double)_pageSize),
            "Writing", nameof(Index), nameof(Page)),
         Seo = new()
         {
            Title = hero.Title,
            Description = hero.Subtitle,
            CanonicalUrl = _urlBuilder.GetAbsoluteUrl("/writing"),
            NoIndex = false,
         },
      };
   }

   private readonly int _pageSize;
   private readonly JsonCache _jsonCache;
   private readonly UrlBuilder _urlBuilder;
   private readonly IWritingStore _writingStore;
   private readonly ISlugService _slugService;
   private readonly ISlugLookupService<WritingTag> _tagLookup;
   private readonly MarkdownPostContentRenderer _postRendered;
   private readonly StructuredDataBuilder _jsonDataBuilder;
}
