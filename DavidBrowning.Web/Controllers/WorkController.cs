// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Infrastructure;
using DavidBrowning.Infrastructure.Cache;
using DavidBrowning.Infrastructure.Data.Stores;
using DavidBrowning.Models;
using DavidBrowning.Web.ViewModels.Work;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace DavidBrowning.Web.Controllers;

[Route("work")]
[OutputCache(PolicyName = PolicyNames.PublicPage)]
public class WorkController : Controller
{
   public WorkController(
      IPublishedSiteStore siteStore,
      JsonCache jsonCache,
      UrlBuilder urlBuilder)
   {
      _siteStore = siteStore;
      _jsonCache = jsonCache;
      _urlBuilder = urlBuilder;
   }

   public async Task<IActionResult> Index(CancellationToken cancellationToken)
   {
      return View(await GetIndexModelAsync(cancellationToken));
   }

   /// <summary>
   /// Returns a page with my resume.
   /// </summary>
   /// <returns></returns>
   [HttpGet("resume")]
   public IActionResult Resume()
   {
      return RedirectToRoute("GetContentAsset", new
      {
         assetKey = FixedAssetKeys.ResumePDFAssetKey,
      });
   }

   /// <summary>
   /// Returns a partial view with the highlights of my career.
   /// Useful for a page header or hero image.
   /// </summary>
   /// <returns></returns>
   [HttpGet("highlights")]
   public IActionResult Highlights()
   {
      return PartialView();
   }

   /// <summary>
   /// A page of the case studies I've written.
   /// </summary>
   /// <returns></returns>
   [HttpGet("case-studies")]
   public IActionResult CaseStudies()
   {
      return View();
   }

   /// <summary>
   /// Gets a page with the details of a case study.
   /// </summary>
   /// <param name="slug"></param>
   /// <returns></returns>
   [HttpGet("case-studies/{slug}")]
   public IActionResult CaseStudy(string slug)
   {
      return View();
   }

   private async Task<IndexViewModel> GetIndexModelAsync(
      CancellationToken cancellationToken)
   {
      var hero = await _jsonCache.GetJsonFileContentAsync<HeroData>(
         "heros/work.json", cancellationToken);
      ArgumentNullException.ThrowIfNullOrEmpty(hero.Title);
      ArgumentNullException.ThrowIfNullOrEmpty(hero.Subtitle);
      ArgumentNullException.ThrowIfNullOrEmpty(hero.Lede);
      var exp = await _siteStore.GetExperienceAsync(cancellationToken);
      var cred = await _siteStore.GetCredentialsAsync(cancellationToken);
      var projects = await _siteStore.GetFeaturedWorkProjectsAsync(
         cancellationToken);
      return new()
      {
         PageTitle = hero.Title,
         HeroTitle = hero.Subtitle,
         Lede = hero.Lede,
         Experience = exp.Select(e => new ExperienceViewModel(e)).ToList(),
         Credentials = cred.Select(c => new CredentialViewModel(c)).ToList(),
         FeaturedWorkProjects = projects,
         Seo = new()
         {
            Title = hero.Title,
            Description = hero.Subtitle,
            CanonicalUrl = _urlBuilder.GetAbsoluteUrl("/work"),
            NoIndex = false,
         }
      };
   }

   private readonly IPublishedSiteStore _siteStore;
   private readonly JsonCache _jsonCache;
   private readonly UrlBuilder _urlBuilder;
}
