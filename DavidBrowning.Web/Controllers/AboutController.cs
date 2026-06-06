// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
      IContentPipeline contentPipeline,
      IUncategorizedStore uncategorizedStore)
   {
      _jsonCache = cache;
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
      var heroData = await _jsonCache.GetJsonFileContentAsync<HeroData>(
         "Heros/About.json", cancellationToken);
      var profileImage = await _contentPipeline.GetRenderedContentAsync(
         "Images/Me.jpg",
         new ContentRenderOptions()
         {
            AltText = "David Browning",
            CssClass = "wb-about-profile-image",
         },
         cancellationToken);

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
         PageTitle = "About",
         HeroTitle = heroData.Title ?? "Missing Data",
         HeroSubtitle = heroData.Subtitle ?? "Missing Data",
         AboutMe = heroData.Lede ?? "Missing Data",
         MeImage = profileImage,
         Interests = interestCards,
      };
   }

   private readonly JsonCache _jsonCache;
   private readonly IUncategorizedStore _uncategorizedStore;
   private readonly IContentPipeline _contentPipeline;
}
