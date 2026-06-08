// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Infrastructure;
using DavidBrowning.Infrastructure.Cache;
using DavidBrowning.Infrastructure.Data.Stores;
using DavidBrowning.Models;
using DavidBrowning.Web.ViewModels;
using DavidBrowning.Web.ViewModels.Home;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DavidBrowning.Web.Controllers;

public class HomeController : Controller
{
   public HomeController(
      IConfiguration configuration,
      IUncategorizedStore uncategorizedStore,
      IProjectStore projectStore,
      IWritingStore writingStore,
      JsonCache jsonCache,
      UrlBuilder urlBuilder)
   {
      _configuration = configuration;
      _uncategorizedStore = uncategorizedStore;
      _projectStore = projectStore;
      _writingStore = writingStore;
      _jsonCache = jsonCache;
      _urlBuilder = urlBuilder;
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
      if (project == null)
      {
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
      if (post == null)
      {
         throw new InvalidOperationException(
            $"Could not find a post with the slug {postSlug}");
      }

      ArgumentNullException.ThrowIfNullOrEmpty(hero.Title);
      ArgumentNullException.ThrowIfNullOrEmpty(hero.Subtitle);
      ArgumentNullException.ThrowIfNullOrEmpty(hero.Lede);

      return new()
      {
         PageTitle = hero.Title,
         HeroTitle = hero.Subtitle,
         Lede = hero.Lede,
         FeaturedPost = post,
         FeaturedProject = project,
         WorkbenchInterest = new InterestCardViewModel(interests[index]),
         Seo = new()
         {
            Title = hero.Title,
            Description = hero.Lede,
            CanonicalUrl = _urlBuilder.GetAbsoluteUrl("/"),
            NoIndex = false,
         }
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
   private readonly UrlBuilder _urlBuilder;
}
