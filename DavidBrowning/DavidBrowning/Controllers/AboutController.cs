// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Data.Stores.Error;
using DavidBrowning.Data.Stores.Projects;
using DavidBrowning.Data.Stores.Uncategorized;
using DavidBrowning.Diagnostics;
using DavidBrowning.Models;
using DavidBrowning.Models.ViewModels;
using DavidBrowning.Models.ViewModels.About;
using DavidBrowning.Services.Assets;
using DavidBrowning.Services.Cache;
using DavidBrowning.Services.Rendering;
using DavidBrowning.Services.Time;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DavidBrowning.Controllers;

[Route("about")]
public class AboutController : Controller
{
   public AboutController(
      ILogger<WorkController> logger,
      ISystemClock clock,
      IErrorStore errorLogStore,
      IOptions<DiagnosticsOptions> options,
      IWebHostEnvironment environment,
      IConfiguration configuration,

      JsonCache cache,
      IContentPipeline contentPipeline,
      IUncategorizedStore uncategorizedStore,
      IProjectStore projectStore)
   {
      _logger = logger;
      _clock = clock;
      _errorLogStore = errorLogStore;
      _options = options.Value;
      _webHostEnvironment = environment;
      _configuration = configuration;

      _jsonCache = cache;
      _contentPipeline = contentPipeline;
      _uncategorizedStore = uncategorizedStore;
      _projectStore = projectStore;
   }

   public async Task<IActionResult> Index(CancellationToken cancellationToken)
   {
      return View(await GetIndexModelAsync(cancellationToken));
   }

   /// <summary>
   /// Returns a page with information about the site.
   /// </summary>
   /// <returns></returns>
   [HttpGet("this")]
   public IActionResult This()
   {
      return View();
   }

   private async Task<IndexViewModel> GetIndexModelAsync(
      CancellationToken cancellationToken)
   {
      var heroData = await _jsonCache.GetJsonFileContentAsync<HeroData>(
         "Heros/About.json", cancellationToken);
      if (heroData == null)
      {
         throw new FileNotFoundException("The hero data could not be parsed.");
      }

      var profileImage = await _contentPipeline.GetRenderedContentAsync(
         "Images/Me.jpg",
         new ContentRenderOptions()
         {
            AltText = "David Browning",
            CssClass = "wb-about-profile-image",
         },
         cancellationToken);
      if (profileImage == null)
      {
         throw new FileNotFoundException("The image is not found");
      }

      var interests = await _uncategorizedStore.GetInterestsAsync();
      List<InterestCardViewModel> interestCards = new();
      foreach (var interest in interests)
      {
         var card = await GetInterestCardViewModelAsync(
            interest, cancellationToken);
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

   private async Task<InterestCardViewModel> GetInterestCardViewModelAsync(
      Interest interest,
      CancellationToken cancellationToken = default)
   {
      return new InterestCardViewModel()
      {
         Description = interest.Summary,
         Title = interest.DisplayName,
         IconCssClass = interest.IconCssClass,
         //Image = await _contentPipeline.GetRenderedContentAsync(),
      };
   }

   private readonly ILogger<WorkController> _logger;
   private readonly ISystemClock _clock;
   private readonly IErrorStore _errorLogStore;
   private readonly DiagnosticsOptions _options;
   private readonly IWebHostEnvironment _webHostEnvironment;
   private readonly IConfiguration _configuration;

   private readonly JsonCache _jsonCache;
   private readonly IProjectStore _projectStore;
   private readonly IUncategorizedStore _uncategorizedStore;
   private readonly IContentPipeline _contentPipeline;
}
