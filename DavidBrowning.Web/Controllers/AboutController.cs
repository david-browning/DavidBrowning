// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Infrastructure;
using DavidBrowning.Infrastructure.Assets;
using DavidBrowning.Infrastructure.Cache;
using DavidBrowning.Infrastructure.Data.Stores;
using DavidBrowning.Infrastructure.Rendering;
using DavidBrowning.Web.ViewModels;
using DavidBrowning.Web.ViewModels.About;
using Microsoft.AspNetCore.Mvc;

namespace DavidBrowning.Web.Controllers;

[Route("about")]
public class AboutController : Controller
{
   public AboutController(
      JsonCache cache,
      UrlBuilder urlBuilder,
      IContentPipeline contentPipeline,
      IUncategorizedStore uncategorizedStore)
   {
      _jsonCache = cache;
      _urlBuilder = urlBuilder;
      _contentPipeline = contentPipeline;
      _uncategorizedStore = uncategorizedStore;
   }

   public async Task<IActionResult> Index(CancellationToken cancellationToken)
   {
      return View(await GetIndexModelAsync(cancellationToken));
   }

   private async Task<IndexViewModel> GetIndexModelAsync(
      CancellationToken cancellationToken)
   {
      var hero = await _jsonCache.GetJsonFileContentAsync<HeroData>(
         "Heros/About.json", cancellationToken);
      ArgumentNullException.ThrowIfNullOrEmpty(hero.Title);
      ArgumentNullException.ThrowIfNullOrEmpty(hero.Subtitle);
      ArgumentNullException.ThrowIfNullOrEmpty(hero.Lede);
      var profileImage = await _contentPipeline.GetRenderedContentAsync(
         "Images/Me.jpg",
         new ContentRenderOptions()
         {
            AltText = "David Browning",
            CssClass = "wb-about-profile-image",
         },
         cancellationToken);

      var aboutMe = await _contentPipeline.GetRenderedContentAsync(
         "Documents/About.txt", null, cancellationToken);

      var interests = await _uncategorizedStore.GetInterestsAsync(
         cancellationToken);
      List<InterestCardViewModel> interestCards = new();
      foreach (var interest in interests)
      {
         var card = new InterestCardViewModel(interest);
         interestCards.Add(card);
      }

      //var interestCardTasks = interests
      //   .Select(interest => GetInterestCardViewModel(
      //      interest,
      //      cancellationToken));
      //var interestCards = await Task.WhenAll(interestCardTasks);

      return new IndexViewModel()
      {
         PageTitle = hero.Title,
         HeroTitle = hero.Subtitle,
         Lede = hero.Lede,
         AboutMe = aboutMe,
         MeImage = profileImage,
         Interests = interestCards,
         Seo = new()
         {
            Title = hero.Title,
            Description = hero.Lede,
            CanonicalUrl = _urlBuilder.GetAbsoluteUrl("/about"),
            NoIndex = false,
         },
      };
   }

   private readonly JsonCache _jsonCache;
   private readonly UrlBuilder _urlBuilder;
   private readonly IUncategorizedStore _uncategorizedStore;
   private readonly IContentPipeline _contentPipeline;
}
