// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Admin.ViewModels;
using DavidBrowning.Admin.ViewModels.Projects;
using DavidBrowning.Infrastructure;
using DavidBrowning.Infrastructure.Assets;
using DavidBrowning.Infrastructure.Data;
using DavidBrowning.Infrastructure.Data.Stores;
using DavidBrowning.Infrastructure.Rendering;
using DavidBrowning.Models;
using DavidBrowning.Models.Projects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DavidBrowning.Admin.Controllers;

public partial class ProjectsController : Controller
{
   public ProjectsController(
      ISlugService slugService,
      IContentStore contentStore,
      IProjectStore projectStore,
      IUncategorizedStore uncategorizedStore,
      IMarkdownDocumentRenderer markdownRenderer)
   {
      _slugService = slugService;
      _projectStore = projectStore;
      _contentStore = contentStore;
      _uncategorizedStore = uncategorizedStore;
      _markdownRenderer = markdownRenderer;
   }

   [HttpGet]
   public async Task<IActionResult> Index(
      CancellationToken cancellationToken)
   {
      return View(await GetIndexViewModelAsync(cancellationToken));
   }

   [HttpGet]
   public async Task<IActionResult> ProjectEdit(
      int id,
      CancellationToken cancellationToken)
   {
      var project = await _projectStore.GetProjectAsync(id, cancellationToken);
      if (project is null)
      {
         return NotFound();
      }

      return View("ProjectEdit", await GetEditPageViewModelAsync(
         project, cancellationToken));
   }

   [HttpGet]
   public async Task<IActionResult> ProjectCreate(
      CancellationToken cancellationToken)
   {
      var metadata = new ProjectMetadataViewModel()
      {
         EditMode = EditModes.Create,
      };

      await PopulateProjectMetadataOptionsAsync(metadata, cancellationToken);

      return View("ProjectEdit", new ProjectEditPageViewModel()
      {
         Metadata = metadata,
         Content = new ProjectContentEditViewModel()
         {
            ProjectId = 0,
         },
         AssetChooser = await GetAssetChooserViewModelAsync(cancellationToken),
         ContentPreview = null,
      });
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> ProjectCreate(
      ProjectMetadataViewModel model,
      CancellationToken cancellationToken)
   {
      model.EditMode = EditModes.Create;
      await PopulateProjectMetadataOptionsAsync(model, cancellationToken);

      if (!ModelState.IsValid)
      {
         return PartialView("ProjectMetadataEdit", model);
      }

      try
      {
         int projectId = await _projectStore.InsertProjectAsync(
            model.ToProject(), model.ProjectTagIds,
            model.ProjectStackTagIds, cancellationToken);

         return RedirectToProjectEdit(projectId);
      }
      catch (DuplicateSlugException)
      {
         ModelState.AddModelError(
            nameof(model.Slug), "Another project already uses this slug.");

         return PartialView("ProjectMetadataEdit", model);
      }
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> ProjectEdit(
      ProjectMetadataViewModel model,
      CancellationToken cancellationToken)
   {
      model.EditMode = EditModes.Edit;
      await PopulateProjectMetadataOptionsAsync(model, cancellationToken);

      if (!ModelState.IsValid)
      {
         return PartialView("ProjectMetadataEdit", model);
      }

      try
      {
         bool updated = await _projectStore.UpdateProjectAsync(
            model.ToProject(), model.ProjectTagIds,
            model.ProjectStackTagIds, cancellationToken);

         if (!updated)
         {
            return NotFound();
         }
      }
      catch (DuplicateSlugException)
      {
         ModelState.AddModelError(
            nameof(model.Slug),
            "Another project already uses this slug.");

         return PartialView("ProjectMetadataEdit", model);
      }

      return PartialView("ProjectMetadataEdit", model);
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> ProjectContentSave(
      ProjectContentEditViewModel model,
      CancellationToken cancellationToken)
   {
      if (!ModelState.IsValid)
      {
         return PartialView("ProjectContentEdit", model);
      }

      var contentData = await _projectStore.GetProjectContentAsync(
         model.ProjectId, cancellationToken);

      if (contentData is null)
      {
         return NotFound();
      }

      var assetLinks = await BuildProjectAssetLinksAsync(
         model.AssetLinks, cancellationToken);

      string content = model.Content ?? string.Empty;
      byte[] contentBytes = Encoding.UTF8.GetBytes(content);

      string contentAssetKey = contentData.ContentAssetKey ??
         CreateProjectDetailsAssetKey(contentData.ProjectSlug);

      await using (var stream = new MemoryStream(contentBytes, writable: false))
      {
         await _contentStore.WriteAsync(
            contentAssetKey, stream, cancellationToken);
      }

      bool updated = await _projectStore.UpdateProjectContentAsync(
         model.ProjectId, contentAssetKey, contentBytes.LongLength,
         assetLinks, cancellationToken);

      if (!updated)
      {
         return NotFound();
      }

      var project = await _projectStore.GetProjectAsync(
         model.ProjectId, cancellationToken);

      if (project is null)
      {
         return NotFound();
      }

      return PartialView(
         "ProjectContentRefresh",
         await GetEditPageViewModelAsync(project, cancellationToken));
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> ProjectContentPreview(
      ProjectContentEditViewModel model,
      CancellationToken cancellationToken)
   {
      try
      {
         var references = await BuildLinkedAssetReferencesAsync(
            model.AssetLinks, cancellationToken);

         var rendered = await _markdownRenderer.RenderAsync(
            $"project-preview:{model.ProjectId}",
            model.Content ?? string.Empty,
            references, cancellationToken);

         return PartialView("ProjectPreviewBody", rendered);
      }
      catch (InvalidOperationException ex)
      {
         return PartialView("ProjectPreviewError", ex.Message);
      }
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> ProjectDelete(
      int id,
      CancellationToken cancellationToken)
   {
      bool deleted = await _projectStore.DeleteProjectAsync(
         id, cancellationToken);

      if (!deleted)
      {
         return NotFound();
      }

      return RedirectToAction(nameof(Index));
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public async Task<IActionResult> ProjectReorder(
      ReorderListRequestViewModel model,
      CancellationToken cancellationToken)
   {
      if (!ModelState.IsValid)
      {
         return BadRequest();
      }

      var idsInDisplayOrder = model.Items
         .OrderBy(item => item.SortOrder)
         .Select(item => item.Id)
         .ToList();

      try
      {
         await _projectStore.ReorderProjectsAsync(
            idsInDisplayOrder, cancellationToken);
      }
      catch (InvalidOperationException)
      {
         return BadRequest();
      }

      return RedirectToAction(nameof(Index));
   }

   private async Task<ProjectAdminIndexViewModel> GetIndexViewModelAsync(
      CancellationToken cancellationToken)
   {
      return new ProjectAdminIndexViewModel()
      {
         Statuses = await GetStatusPanelAsync(cancellationToken),
         Origins = await GetOriginPanelAsync(cancellationToken),
         Types = await GetTypePanelAsync(cancellationToken),
         Visibilities = await GetVisibilityPanelAsync(cancellationToken),
         Tags = await GetTagPanelAsync(cancellationToken),
         StackTags = await GetStackTagPanelAsync(cancellationToken),
         Projects = await GetProjectReorderListViewModelAsync(cancellationToken),
         LookupEditorOffcanvas = new AdminOffcanvasViewModel()
         {
            Id = ProjectAdminIds.ProjectLookupEditorOffcanvas,
            Title = "Project lookup editor",
            Placeholder = "Select a project lookup value to edit.",
            LoadingText = "Loading project lookup editor...",
         },
      };
   }

   private async Task<ProjectEditPageViewModel> GetEditPageViewModelAsync(
      Project project,
      CancellationToken cancellationToken)
   {
      var metadataOptions = await GetMetadataOptionsAsync(cancellationToken);
      var content = await GetProjectContentViewModelAsync(
         project.Id, cancellationToken);

      return new ProjectEditPageViewModel()
      {
         Metadata = new ProjectMetadataViewModel(project, metadataOptions),
         Content = content,
         AssetChooser = await GetAssetChooserViewModelAsync(cancellationToken),
         ContentPreview = null,
      };
   }

   private async Task<ProjectContentEditViewModel> GetProjectContentViewModelAsync(
      int projectId,
      CancellationToken cancellationToken)
   {
      var contentData = await _projectStore.GetProjectContentAsync(
         projectId,
         cancellationToken);

      if (contentData is null)
      {
         return new ProjectContentEditViewModel()
         {
            ProjectId = projectId,
            Content = null,
         };
      }

      string? content = null;

      if (!string.IsNullOrWhiteSpace(contentData.ContentAssetKey))
      {
         var storedAsset = await _contentStore.GetAssetAsync(
            contentData.ContentAssetKey,
            cancellationToken);

         if (!storedAsset.ContentType.Equals(
            _detailsContentType,
            StringComparison.OrdinalIgnoreCase))
         {
            throw new InvalidOperationException(
               $"Project details asset '{contentData.ContentAssetKey}' must use " +
               $"content type '{_detailsContentType}'.");
         }

         content = storedAsset.Text ?? string.Empty;
      }

      return new ProjectContentEditViewModel()
      {
         ProjectId = contentData.ProjectId,
         ContentAssetId = contentData.ContentAssetId,
         ContentAssetKey = contentData.ContentAssetKey,
         Content = content,
         AssetLinks = contentData.AssetLinks
            .Where(link => !string.IsNullOrWhiteSpace(link.ReferenceKey))
            .Select(link => new AssetLinkInputViewModel()
            {
               SiteAssetId = link.SiteAssetId,
               ReferenceKey = link.ReferenceKey!,
               Caption = link.Caption,
               AltTextOverride = link.AltTextOverride,
            })
            .ToList(),
      };
   }

   private async Task<ProjectMetadataOptionsViewModel> GetMetadataOptionsAsync(
      CancellationToken cancellationToken)
   {
      return new ProjectMetadataOptionsViewModel()
      {
         Statuses = await GetStatusOptionsAsync(cancellationToken),
         Types = await GetTypeOptionsAsync(cancellationToken),
         Origins = await GetOriginOptionsAsync(cancellationToken),
         Visibilities = await GetVisibilityOptionsAsync(cancellationToken),
         Tags = await GetTagOptionsAsync(cancellationToken),
         StackTags = await GetStackTagOptionsAsync(cancellationToken),
         LinkTypes = await GetLinkTypeOptionsAsync(cancellationToken),
      };
   }

   private async Task PopulateProjectMetadataOptionsAsync(
      ProjectMetadataViewModel model,
      CancellationToken cancellationToken)
   {
      model.SetOptions(await GetMetadataOptionsAsync(cancellationToken));
   }

   private async Task<AssetChooserViewModel> GetAssetChooserViewModelAsync(
      CancellationToken cancellationToken)
   {
      var assets = await _uncategorizedStore.GetSiteAssetsAsync(
         cancellationToken);

      var usedKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
      var items = new List<AssetChooserItemViewModel>();

      foreach (var asset in assets.OrderBy(asset => asset.AssetKey))
      {
         string baseKey = Path.GetFileNameWithoutExtension(asset.AssetKey);
         string referenceKey = _slugService.CreateSlug(baseKey);
         string uniqueReferenceKey = GetUniqueReferenceKey(referenceKey, usedKeys);

         items.Add(new AssetChooserItemViewModel()
         {
            Id = asset.Id,
            AssetKey = asset.AssetKey,
            ContentType = asset.ContentType,
            ReferenceKey = uniqueReferenceKey,
         });
      }

      return new AssetChooserViewModel()
      {
         Assets = items,
      };
   }

   private static string GetUniqueReferenceKey(
      string referenceKey,
      ISet<string> usedKeys)
   {
      if (usedKeys.Add(referenceKey))
      {
         return referenceKey;
      }

      for (int i = 2; ; i++)
      {
         string candidate = $"{referenceKey}-{i}";

         if (usedKeys.Add(candidate))
         {
            return candidate;
         }
      }
   }

   private async Task<IReadOnlyList<ProjectAssetLink>> BuildProjectAssetLinksAsync(
      IReadOnlyList<AssetLinkInputViewModel> inputLinks,
      CancellationToken cancellationToken)
   {
      int? defaultAssetRoleId =
         await _projectStore.GetRequiredProjectAssetRoleIdAsync(
            ProjectAssetRoleSlugs.DetailsContent, cancellationToken);

      if (defaultAssetRoleId is null)
      {
         throw new InvalidOperationException(
            $"Required project asset role '{ProjectAssetRoleSlugs.DetailsContent}' was not found.");
      }

      var links = new List<ProjectAssetLink>();

      foreach (var inputLink in inputLinks
         .GroupBy(link => link.ReferenceKey, StringComparer.OrdinalIgnoreCase)
         .Select(group => group.First()))
      {
         links.Add(new ProjectAssetLink()
         {
            SiteAssetId = inputLink.SiteAssetId,
            ProjectAssetRoleId = defaultAssetRoleId.Value,
            ReferenceKey = inputLink.ReferenceKey,
            Caption = inputLink.Caption,
            AltTextOverride = inputLink.AltTextOverride,
         });
      }

      return links;
   }

   private async Task<IReadOnlyList<LinkedAssetReference>> BuildLinkedAssetReferencesAsync(
      IReadOnlyList<AssetLinkInputViewModel> inputLinks,
      CancellationToken cancellationToken)
   {
      var references = new List<LinkedAssetReference>();

      foreach (var inputLink in inputLinks
         .GroupBy(link => link.ReferenceKey, StringComparer.OrdinalIgnoreCase)
         .Select(group => group.First()))
      {
         var asset = await _uncategorizedStore.GetAssetAsync(
            inputLink.SiteAssetId,
            cancellationToken);

         if (asset is null)
         {
            throw new InvalidOperationException(
               $"The linked asset '{inputLink.ReferenceKey}' no longer exists.");
         }

         references.Add(new LinkedAssetReference()
         {
            ReferenceKey = inputLink.ReferenceKey,
            AssetKey = asset.AssetKey,
            AltText = inputLink.AltTextOverride ?? asset.AltText,
            Caption = inputLink.Caption,
         });
      }

      return references;
   }

   private IActionResult RedirectToProjectEdit(int projectId)
   {
      var url = Url.Action(nameof(ProjectEdit), new
      {
         id = projectId,
      });

      if (string.IsNullOrWhiteSpace(url))
      {
         throw new InvalidOperationException(
            "Could not build the project edit URL.");
      }

      if (Request.Headers.ContainsKey("HX-Request"))
      {
         Response.Headers["HX-Redirect"] = url;
         return Ok();
      }

      return RedirectToAction(nameof(ProjectEdit), new
      {
         id = projectId,
      });
   }

   private async Task<ProjectLookupPanelViewModel> GetStatusPanelAsync(
      CancellationToken cancellationToken)
   {
      var items = await _projectStore.GetProjectStatusesAsync(cancellationToken);
      return new ProjectLookupPanelViewModel()
      {
         Title = "Project Statuses",
         Description = "Create and edit project statuses",
         CreateAction = nameof(StatusCreate),
         EditAction = nameof(StatusEdit),
         RegionId = ProjectAdminIds.ProjectStatusPanelRegion,
         Items = items.Select(i => new ProjectLookupItemViewModel()
         {
            Id = i.Id,
            DisplayName = i.DisplayName,
            Slug = i.Slug,
            Description = i.Description,
            IsActive = i.IsActive,
            SortOrder = i.SortOrder,
         }).ToList()
      };
   }

   private async Task<ProjectLookupPanelViewModel> GetOriginPanelAsync(
      CancellationToken cancellationToken)
   {
      var items = await _projectStore.GetProjectOriginsAsync(cancellationToken);
      return new ProjectLookupPanelViewModel()
      {
         Title = "Project Origins",
         Description = "Create and edit project origins.",
         CreateAction = nameof(OriginCreate),
         EditAction = nameof(OriginEdit),
         RegionId = ProjectAdminIds.ProjectOriginPanelRegion,
         Items = items.Select(i => new ProjectLookupItemViewModel()
         {
            Id = i.Id,
            DisplayName = i.DisplayName,
            Slug = i.Slug,
            Description = i.Description,
            IsActive = i.IsActive,
            SortOrder = i.SortOrder,
         }).ToList()
      };
   }

   private async Task<ProjectLookupPanelViewModel> GetTypePanelAsync(
      CancellationToken cancellationToken)
   {
      var items = await _projectStore.GetProjectTypesAsync(cancellationToken);
      return new ProjectLookupPanelViewModel()
      {
         Title = "Project Types",
         Description = "Create and edit project types.",
         CreateAction = nameof(TypeCreate),
         EditAction = nameof(TypeEdit),
         RegionId = ProjectAdminIds.ProjectTypePanelRegion,
         Items = items.Select(i => new ProjectLookupItemViewModel()
         {
            Id = i.Id,
            DisplayName = i.DisplayName,
            Slug = i.Slug,
            Description = i.Description,
            IsActive = i.IsActive,
            SortOrder = i.SortOrder,
         }).ToList()
      };
   }

   private async Task<ProjectLookupPanelViewModel> GetVisibilityPanelAsync(
      CancellationToken cancellationToken)
   {
      var items = await _projectStore.GetProjectVisibilitiesAsync(
         cancellationToken);
      return new ProjectLookupPanelViewModel()
      {
         Title = "Project Visibilities",
         Description = "Define how projects show up on the main site.",
         CreateAction = nameof(VisibilityCreate),
         EditAction = nameof(VisibilityEdit),
         RegionId = ProjectAdminIds.ProjectVisibilityPanelRegion,
         Items = items.Select(i => new ProjectLookupItemViewModel()
         {
            Id = i.Id,
            DisplayName = i.DisplayName,
            Slug = i.Slug,
            Description = i.Description,
            IsActive = i.IsActive,
            SortOrder = i.SortOrder,
         }).ToList()
      };
   }

   private async Task<ProjectLookupPanelViewModel> GetTagPanelAsync(
      CancellationToken cancellationToken)
   {
      var items = await _projectStore.GetProjectTagsAsync(cancellationToken);
      return new ProjectLookupPanelViewModel()
      {
         Title = "Project Tags",
         Description = "Create and edit project tags.",
         CreateAction = nameof(TagCreate),
         EditAction = nameof(TagEdit),
         RegionId = ProjectAdminIds.ProjectTagPanelRegion,
         Items = items.Select(i => new ProjectLookupItemViewModel()
         {
            Id = i.Id,
            DisplayName = i.DisplayName,
            Slug = i.Slug,
            Description = i.Description,
            IsActive = i.IsActive,
            SortOrder = i.SortOrder,
         }).ToList()
      };
   }

   private async Task<ProjectLookupPanelViewModel> GetStackTagPanelAsync(
      CancellationToken cancellationToken)
   {
      var items = await _projectStore.GetProjectStackTagsAsync(
         cancellationToken);
      return new ProjectLookupPanelViewModel()
      {
         Title = "Technology Stacks",
         Description = "Create and edit technology tags.",
         CreateAction = nameof(StackCreate),
         EditAction = nameof(StackEdit),
         RegionId = ProjectAdminIds.ProjectStackTagPanelRegion,
         Items = items.Select(i => new ProjectLookupItemViewModel()
         {
            Id = i.Id,
            DisplayName = i.DisplayName,
            Slug = i.Slug,
            Description = i.Description,
            IsActive = i.IsActive,
            SortOrder = i.SortOrder,
         }).ToList()
      };
   }

   private async Task<ReorderListViewModel> GetProjectReorderListViewModelAsync(
      CancellationToken cancellationToken)
   {
      var projects = await _projectStore.GetProjectsAsync(cancellationToken);

      return new ReorderListViewModel()
      {
         Title = "Projects",
         Description = null,
         RenderCard = false,
         EmptyMessage = "No projects have been created.",
         ReoderParameters = new ReoderParameters()
         {
            ReorderController = "Projects",
            ReorderAction = nameof(ProjectReorder),
         },
         Items = projects
            .OrderBy(project => project.SortOrder)
            .ThenBy(project => project.Name)
            .Select(project => new ReorderListItemViewModel()
            {
               Id = project.Id,
               DisplayName = project.Name,
               SecondaryText = GetProjectSecondaryText(project),
               SortOrder = project.SortOrder,
               IsActive = null,
               EditController = "Projects",
               EditAction = nameof(ProjectEdit),
               DeleteController = "Projects",
               DeleteAction = nameof(ProjectDelete),
            })
            .ToList(),
      };
   }

   private static string GetProjectSecondaryText(Project project)
   {
      var parts = new[]
      {
      project.Slug,
      project.ProjectStatus?.DisplayName,
      project.ProjectOrigin?.DisplayName,
      project.ProjectType?.DisplayName,
      project.ProjectVisibility?.DisplayName,
   };

      return string.Join(" · ", parts.Where(
         part => !string.IsNullOrWhiteSpace(part)));
   }

   private async Task<IReadOnlyList<LookupOptionViewModel>> GetStatusOptionsAsync(
      CancellationToken cancellationToken)
   {
      var projectStatuses = await _projectStore.GetProjectStatusesAsync(
         cancellationToken);
      return projectStatuses.Select(s => new LookupOptionViewModel()
      {
         Id = s.Id,
         DisplayName = s.DisplayName,
         IsActive = s.IsActive,
      }).ToList();
   }

   private async Task<IReadOnlyList<LookupOptionViewModel>> GetTypeOptionsAsync(
      CancellationToken cancellationToken)
   {
      var projectTypes = await _projectStore.GetProjectTypesAsync(
         cancellationToken);
      return projectTypes.Select(t => new LookupOptionViewModel()
      {
         Id = t.Id,
         DisplayName = t.DisplayName,
         IsActive = t.IsActive,
      }).ToList();
   }

   private async Task<IReadOnlyList<LookupOptionViewModel>> GetOriginOptionsAsync(
      CancellationToken cancellationToken)
   {
      var origins = await _projectStore.GetProjectOriginsAsync(cancellationToken);
      return origins.Select(origin => new LookupOptionViewModel()
      {
         DisplayName = origin.DisplayName,
         IsActive = origin.IsActive,
         Id = origin.Id,
      }).ToList();
   }

   private async Task<IReadOnlyList<LookupOptionViewModel>> GetVisibilityOptionsAsync(
      CancellationToken cancellationToken)
   {
      var visibilities = await _projectStore.GetProjectVisibilitiesAsync(
         cancellationToken);
      return visibilities.Select(v => new LookupOptionViewModel()
      {
         Id = v.Id,
         DisplayName = v.DisplayName,
         IsActive = v.IsActive,
      }).ToList();
   }

   private async Task<IReadOnlyList<LookupOptionViewModel>> GetStackTagOptionsAsync(
      CancellationToken cancellationToken)
   {
      var stacks = await _projectStore.GetProjectStackTagsAsync(cancellationToken);
      return stacks.Select(stack => new LookupOptionViewModel()
      {
         Id = stack.Id,
         DisplayName = stack.DisplayName,
         IsActive = stack.IsActive,
      }).ToList();
   }

   private async Task<IReadOnlyList<LookupOptionViewModel>> GetTagOptionsAsync(
      CancellationToken cancellationToken)
   {
      var tags = await _projectStore.GetProjectTagsAsync(cancellationToken);
      return tags.Select(tag => new LookupOptionViewModel()
      {
         DisplayName = tag.DisplayName,
         IsActive = tag.IsActive,
         Id = tag.Id,
      }).ToList();
   }

   private async Task<IReadOnlyList<LookupOptionViewModel>> GetLinkTypeOptionsAsync(
      CancellationToken cancellationToken)
   {
      var linkTypes = await _projectStore.GetProjectLinkTypesAsync(
         cancellationToken);

      return linkTypes
         .Where(type => type.IsActive)
         .Select(type => new LookupOptionViewModel()
         {
            Id = type.Id,
            DisplayName = type.DisplayName,
            IsActive = type.IsActive,
         })
         .ToList();
   }

   private static string CreateProjectDetailsAssetKey(string projectSlug)
   {
      ArgumentException.ThrowIfNullOrWhiteSpace(projectSlug);
      return $"projects/{projectSlug}/details.md";
   }

   private readonly ISlugService _slugService;
   private readonly IProjectStore _projectStore;
   private readonly IContentStore _contentStore;
   private readonly IUncategorizedStore _uncategorizedStore;
   private readonly IMarkdownDocumentRenderer _markdownRenderer;

   private const string _detailsContentType = "text/markdown";
}