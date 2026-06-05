// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Data.Stores.Projects;
using DavidBrowning.Data.Stores.Uncategorized;
using DavidBrowning.Data.Stores.Writing;
using DavidBrowning.Models;
using DavidBrowning.Models.ViewModels.Home;
using DavidBrowning.Services.Cache;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DavidBrowning.Controllers;

public class HomeController : Controller
{
   public HomeController(
      IConfiguration configuration,
      IUncategorizedStore uncategorizedStore,
      IProjectStore projectStore,
      IWritingStore writingStore,
      JsonCache jsonCache)
   {
      _configuration = configuration;
      _uncategorizedStore = uncategorizedStore;
      _projectStore = projectStore;
      _writingStore = writingStore;
      _jsonCache = jsonCache;
   }

   public async Task<IActionResult> Index(CancellationToken cancellationToken)
   {
      return View(await GetIndexModelAsync(cancellationToken));
   }

   public async Task<IActionResult> Privacy(CancellationToken cancellationToken)
   {
      return View(await GetPrivacyModelAsync(cancellationToken));
   }

   private async Task<IndexViewModel> GetIndexModelAsync(
      CancellationToken cancellationToken)
   {
      var hero = await _jsonCache.GetJsonFileContentAsync<HeroData>(
         "Heros/Home.json", cancellationToken);
      var interests = await _uncategorizedStore.GetInterestsAsync(
         cancellationToken);
      var index = DateOnly.FromDateTime(DateTime.UtcNow).DayNumber %
         interests.Count;

      var projectSlug = _configuration.GetValue<string>("FeaturedProjectSlug");
      if (string.IsNullOrEmpty(projectSlug))
      {
         throw new ArgumentNullException(
            "The FeaturedProjectSlug is not set in the configuration.");
      }
      var project = await _projectStore.GetPublishedProjectBySlugAsync(
         projectSlug, cancellationToken);
      if (project == null) {
         throw new InvalidOperationException(
            $"Could not find a project with the slug {projectSlug}");
      }

         var postSlug = _configuration.GetValue<string>("FeaturePostSlug");
      if (string.IsNullOrEmpty(postSlug))
      {
         throw new ArgumentNullException(
            "The FeaturePostSlug is not set in the configuration.");
      }
      var post = await _writingStore.GetPublishedPostBySlugAsync(
         postSlug, cancellationToken);
      if(post == null)
      {
         throw new InvalidOperationException(
            $"Could not find a post with the slug {postSlug}");
      }

      return new()
      {
         PageTitle = hero.Title ?? "Missing Data",
         HeroTitle = hero.Subtitle ?? "Missing Data",
         Lede = hero.Lede ?? "Missing Data",
         FeaturedPost = post,
         FeaturedProject = project,
         WorkbenchInterest = new Models.ViewModels.InterestCardViewModel(
            interests[index]),
      };
   }

   private async Task<PrivacyViewModel> GetPrivacyModelAsync(
      CancellationToken cancellationToken)
   {
      var data = await _jsonCache.GetJsonFileContentAsync<PrivacyViewModel>(
         "Heros/Privacy.json", cancellationToken);
      if (data == null)
      {
         throw new FileNotFoundException("Missing privacy data");
      }

      return data;
   }

   private readonly IConfiguration _configuration;
   private readonly IUncategorizedStore _uncategorizedStore;
   private readonly IProjectStore _projectStore;
   private readonly IWritingStore _writingStore;
   private readonly JsonCache _jsonCache;
}
