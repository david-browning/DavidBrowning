// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
      JsonCache jsonCache)
   {
      _workStore = workStore;
      _projectStore = projectStore;
      _jsonCache = jsonCache;
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
      var heroData = await _jsonCache.GetJsonFileContentAsync<HeroData>(
         "Heros/Work.json", cancellationToken);
      var exp = await _workStore.GetExperienceAsync(cancellationToken);
      var cred = await _workStore.GetCredentialsAsync(cancellationToken);
      var projects = await _projectStore.GetFeaturedWorkProjectsAsync(
         cancellationToken);
      return new()
      {
         PageTitle = heroData.Title ?? "Missing Data",
         HeroTitle = heroData.Subtitle ?? "Missing Data",
         Lede = heroData.Lede ?? "Missing Data",
         Experience = exp.Select(e => new ExperienceViewModel(e)).ToList(),
         Credentials = cred.Select(c => new CredentialViewModel(c)).ToList(),
         FeaturedWorkProjects = projects,
      };
   }

   private const string _resumeAssetKey = "documents/resume.pdf";
   private readonly IWorkStore _workStore;
   private readonly IProjectStore _projectStore;
   private readonly JsonCache _jsonCache;
}
