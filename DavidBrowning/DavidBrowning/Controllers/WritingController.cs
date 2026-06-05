// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Data.Stores.Writing;
using DavidBrowning.Models;
using DavidBrowning.Models.ViewModels;
using DavidBrowning.Models.ViewModels.Writing;
using DavidBrowning.Models.Writing;
using DavidBrowning.Services.Cache;
using DavidBrowning.Services.Rendering;
using DavidBrowning.Services.Slugs;
using Microsoft.AspNetCore.Mvc;

namespace DavidBrowning.Controllers;

[Route("writing")]
public class WritingController : Controller
{
   public WritingController(
      JsonCache jsonCache,
      MarkdownPostContentRenderer postRendered,
      IWritingStore writingStore,
      ISlugService slugs,
      ISlugLookupService<WritingTag> tagStore)
   {
      _jsonCache = jsonCache;
      _postRendered = postRendered;
      _writingStore = writingStore;
      _slugService = slugs;
      _tagLookup = tagStore;
   }

   public async Task<IActionResult> Index(CancellationToken cancellationToken)
   {
      return View(await GetIndexModelAsync(cancellationToken));
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

   private async Task<IndexViewModel> GetIndexModelAsync(
      CancellationToken cancellationToken)
   {
      var featured = await _writingStore.GetFeaturedPostsAsync(
         cancellationToken);
      var all = await _writingStore.GetPublishedPostsAsync(cancellationToken);
      var heroData = await _jsonCache.GetJsonFileContentAsync<HeroData>(
         "Heros/Writing.json", cancellationToken);
      if (heroData == null)
      {
         throw new FileNotFoundException("The hero data could not be parsed.");
      }

      return new IndexViewModel()
      {
         PageTitle = "Writings",
         HeroTitle = heroData.Title ?? "Missing Data",
         HeroSubtitle = heroData.Subtitle ?? "Missing Data",
         AllPosts = all,
         FeaturedPosts = featured,
      };
   }

   private readonly JsonCache _jsonCache;
   private readonly IWritingStore _writingStore;
   private readonly ISlugService _slugService;
   private readonly ISlugLookupService<WritingTag> _tagLookup;
   private readonly MarkdownPostContentRenderer _postRendered;
}
