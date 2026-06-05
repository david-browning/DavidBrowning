// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Data.Stores.Projects;
using DavidBrowning.Models;
using DavidBrowning.Models.Projects;
using DavidBrowning.Models.ViewModels;
using DavidBrowning.Models.ViewModels.Projects;
using DavidBrowning.Services.Assets;
using DavidBrowning.Services.Cache;
using DavidBrowning.Services.Rendering;
using DavidBrowning.Services.Slugs;
using Microsoft.AspNetCore.Mvc;

namespace DavidBrowning.Controllers;

[Route("projects")]
public class ProjectsController : Controller
{
   public ProjectsController(
      JsonCache jsonCache,
      IContentPipeline contentPipeline,
      IProjectStore project,
      ISlugService slugService,
      MarkdownProjectContentRenderer projectRenderer,
      ISlugLookupService<ProjectStackTag> stackLookup,
      ISlugLookupService<ProjectOrigin> originLookup,
      ISlugLookupService<ProjectType> typeLookup,
      ISlugLookupService<ProjectStatus> statusLookup,
      ISlugLookupService<ProjectTag> tagLookup)
   {
      _jsonCache = jsonCache;
      _contentPipeline = contentPipeline;
      _projectStore = project;
      _slugService = slugService;
      _stackLookup = stackLookup;
      _originLookup = originLookup;
      _typeLookup = typeLookup;
      _statusLookup = statusLookup;
      _tagLookup = tagLookup;
      _projectContentRenderer = projectRenderer;
   }

   public async Task<IActionResult> Index(CancellationToken cancellationToken)
   {
      return View(await GetIndexModelAsync(cancellationToken));
   }

   /// <summary>
   /// Returns a page with all projects that have the given slug as a 
   /// technology stack tag.
   /// </summary>
   /// <param name="slug"></param>
   /// <param name="cancellationToken"></param>
   /// <returns></returns>
   [HttpGet("stacks/{slug}")]
   public async Task<IActionResult> Stacks(
      string slug,
      CancellationToken cancellationToken)
   {
      if (string.IsNullOrWhiteSpace(slug))
      {
         return NotFound();
      }

      var normalizedSlug = _slugService.CleanSlug(slug);
      var stack = await _stackLookup.GetBySlugAsync(
         normalizedSlug, cancellationToken);
      if (stack == null)
      {
         return NotFound();
      }

      var projects = await _projectStore.GetPublishedProjectsByStackTagSlugAsync(
         normalizedSlug, cancellationToken);
      FilteredResultsViewModel model = new()
      {
         PageTitle = $"Projects built with {stack.DisplayName}",
         FilterName = stack.DisplayName,
         Description = stack.Description,
         FilterSlug = normalizedSlug,
         Results = projects,
         ResultPartialName = "_ProjectCard",
      };

      return View("_FilteredResults", model);
   }

   /// <summary>
   /// Returns a page with all projects that have the given slug as a 
   /// status.
   /// </summary>
   /// <param name="slug"></param>
   /// <param name="cancellationToken"></param>
   /// <returns></returns>
   [HttpGet("statuses/{slug}")]
   public async Task<IActionResult> Statuses(
      string slug,
      CancellationToken cancellationToken)
   {
      if (string.IsNullOrWhiteSpace(slug))
      {
         return NotFound();
      }

      var normalizedSlug = _slugService.CleanSlug(slug);
      var status = await _statusLookup.GetBySlugAsync(
         normalizedSlug, cancellationToken);
      if (status == null)
      {
         return NotFound();
      }

      var projects = await _projectStore.GetPublishedProjectsByStatusSlugAsync(
         normalizedSlug, cancellationToken);
      FilteredResultsViewModel model = new()
      {
         PageTitle = $"{status.DisplayName} Projects",
         FilterName = status.DisplayName,
         Description = status.Description,
         FilterSlug = normalizedSlug,
         Results = projects,
         ResultPartialName = "_ProjectCard",
      };

      return View("_FilteredResults", model);
   }

   /// <summary>
   /// Returns a page with all projects that have the given slug as an 
   /// origin.
   /// </summary>
   /// <param name="slug"></param>
   /// <param name="cancellationToken"></param>
   /// <returns></returns>
   [HttpGet("origins/{slug}")]
   public async Task<IActionResult> Origins(
      string slug,
      CancellationToken cancellationToken)
   {
      if (string.IsNullOrWhiteSpace(slug))
      {
         return NotFound();
      }

      var normalizedSlug = _slugService.CleanSlug(slug);
      var origin = await _originLookup.GetBySlugAsync(
         normalizedSlug, cancellationToken);
      if (origin == null)
      {
         return NotFound();
      }

      var projects = await _projectStore.GetPublishedProjectsByOriginSlugAsync(
         normalizedSlug, cancellationToken);
      FilteredResultsViewModel model = new()
      {
         PageTitle = $"Projects from {origin.DisplayName}",
         FilterName = origin.DisplayName,
         Description = origin.Description,
         FilterSlug = normalizedSlug,
         Results = projects,
         ResultPartialName = "_ProjectCard"
      };

      return View("_FilteredResults", model);
   }
   /// <summary>
   /// Returns a page with all projects that have the given slug as a 
   /// project type.
   /// </summary>
   /// <param name="slug"></param>
   /// <param name="cancellationToken"></param>
   /// <returns></returns>
   [HttpGet("types/{slug}")]
   public async Task<IActionResult> Types(
      string slug,
      CancellationToken cancellationToken)
   {
      if (string.IsNullOrWhiteSpace(slug))
      {
         return NotFound();
      }

      var normalizedSlug = _slugService.CleanSlug(slug);
      var type = await _typeLookup.GetBySlugAsync(
         normalizedSlug, cancellationToken);
      if (type == null)
      {
         return NotFound();
      }

      var projects = await _projectStore.GetPublishedProjectsByTypeSlugAsync(
         normalizedSlug, cancellationToken);
      FilteredResultsViewModel model = new()
      {
         PageTitle = $"{type.DisplayName} Projects",
         FilterName = type.DisplayName,
         Description = type.Description,
         FilterSlug = normalizedSlug,
         Results = projects,
         ResultPartialName = "_ProjectCard"
      };

      return View("_FilteredResults", model);
   }

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

      var projects = await _projectStore.GetPublishedProjectsByTagSlugAsync(
         normalizedSlug, cancellationToken);
      FilteredResultsViewModel model = new()
      {
         PageTitle = $"Projects tagged with \"{tag.DisplayName}\"",
         FilterName = tag.DisplayName,
         Description = tag.Description,
         FilterSlug = normalizedSlug,
         Results = projects,
         ResultPartialName = "_ProjectCard",
      };

      return View("_FilteredResults", model);
   }

   /// <summary>
   /// Gets a page with project details.
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

      var normalizedSlug = _slugService.CleanSlug(slug);
      var project = await _projectStore.GetPublishedProjectBySlugAsync(
         normalizedSlug, cancellationToken);
      if (project == null)
      {
         return NotFound();
      }

      var body = await _projectContentRenderer.RenderAsync(
         project, cancellationToken);

      return View(new DetailsViewModel(project, body));
   }

   private async Task<IndexViewModel> GetIndexModelAsync(
      CancellationToken cancellationToken)
   {
      var featured = await _projectStore.GetFeaturedProjectsAsync(
         cancellationToken);
      var all = await _projectStore.GetPublishedProjectsAsync(
         cancellationToken);
      var heroData = await _jsonCache.GetJsonFileContentAsync<HeroData>(
         "Heros/Projects.json", cancellationToken);
      if (heroData == null)
      {
         throw new FileNotFoundException("The hero data could not be parsed.");
      }

      return new IndexViewModel()
      {
         PageTitle = "Projects",
         HeroTitle = heroData.Title ?? "Missing Data",
         HeroSubtitle = heroData.Subtitle ?? "Missing Data",
         AllProjects = all,
         FeaturedProjects = featured,
      };
   }

   private readonly JsonCache _jsonCache;
   private readonly IContentPipeline _contentPipeline;
   private readonly IProjectStore _projectStore;
   private readonly ISlugService _slugService;
   private readonly MarkdownProjectContentRenderer _projectContentRenderer;
   private readonly ISlugLookupService<ProjectStackTag> _stackLookup;
   private readonly ISlugLookupService<ProjectOrigin> _originLookup;
   private readonly ISlugLookupService<ProjectType> _typeLookup;
   private readonly ISlugLookupService<ProjectStatus> _statusLookup;
   private readonly ISlugLookupService<ProjectTag> _tagLookup;
}
