// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Infrastructure;
using DavidBrowning.Infrastructure.Cache;
using DavidBrowning.Infrastructure.Data.Stores;
using DavidBrowning.Web.ViewModels;
using DavidBrowning.Web.ViewModels.Work;
using Microsoft.AspNetCore.Mvc;

namespace DavidBrowning.Web.Controllers;

[Route("work")]
public class WorkController : Controller
{
   public WorkController(
      IWorkStore workStore,
      IProjectStore projectStore,
      JsonCache jsonCache,
      UrlBuilder urlBuilder)
   {
      _workStore = workStore;
      _projectStore = projectStore;
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
         assetKey = _resumeAssetKey,
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
         "Heros/Work.json", cancellationToken);
      ArgumentNullException.ThrowIfNullOrEmpty(hero.Title);
      ArgumentNullException.ThrowIfNullOrEmpty(hero.Subtitle);
      ArgumentNullException.ThrowIfNullOrEmpty(hero.Lede);
      var exp = await _workStore.GetExperienceAsync(cancellationToken);
      var cred = await _workStore.GetCredentialsAsync(cancellationToken);
      var projects = await _projectStore.GetFeaturedWorkProjectsAsync(
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

   private const string _resumeAssetKey = "documents/resume.pdf";
   private readonly IWorkStore _workStore;
   private readonly IProjectStore _projectStore;
   private readonly JsonCache _jsonCache;
   private readonly UrlBuilder _urlBuilder;
}
