// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using System;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Admin.Extensions;
using DavidBrowning.Admin.ViewModels.Projects;
using DavidBrowning.Infrastructure.Data;
using DavidBrowning.Models.Projects;
using Microsoft.AspNetCore.Mvc;

namespace DavidBrowning.Admin.Controllers;

public partial class ProjectsController
{
   [HttpGet]
   public IActionResult StatusCreate()
   {
      return CreateLookupView(StatusLookup);
   }

   [HttpGet]
   public Task<IActionResult> StatusEdit(
      int id,
      CancellationToken cancellationToken)
   {
      return EditLookupViewAsync(
         id,
         StatusLookup,
         _projectStore.GetProjectStatusAsync,
         ToLookupData,
         cancellationToken);
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> StatusCreate(
      ProjectLookupEditViewModel model,
      CancellationToken cancellationToken)
   {
      return CreateLookupAsync(
         model,
         StatusLookup,
         ToProjectStatus,
         _projectStore.InsertProjectStatusAsync,
         GetStatusPanelAsync,
         cancellationToken);
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> StatusEdit(
      ProjectLookupEditViewModel model,
      CancellationToken cancellationToken)
   {
      return EditLookupAsync(
         model,
         StatusLookup,
         ToProjectStatus,
         _projectStore.UpdateProjectStatusAsync,
         GetStatusPanelAsync,
         cancellationToken);
   }

   [HttpGet]
   public IActionResult OriginCreate()
   {
      return CreateLookupView(OriginLookup);
   }

   [HttpGet]
   public Task<IActionResult> OriginEdit(
      int id,
      CancellationToken cancellationToken)
   {
      return EditLookupViewAsync(
         id,
         OriginLookup,
         _projectStore.GetProjectOriginAsync,
         ToLookupData,
         cancellationToken);
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> OriginCreate(
      ProjectLookupEditViewModel model,
      CancellationToken cancellationToken)
   {
      return CreateLookupAsync(
         model,
         OriginLookup,
         ToProjectOrigin,
         _projectStore.InsertProjectOriginAsync,
         GetOriginPanelAsync,
         cancellationToken);
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> OriginEdit(
      ProjectLookupEditViewModel model,
      CancellationToken cancellationToken)
   {
      return EditLookupAsync(
         model,
         OriginLookup,
         ToProjectOrigin,
         _projectStore.UpdateProjectOriginAsync,
         GetOriginPanelAsync,
         cancellationToken);
   }

   [HttpGet]
   public IActionResult TypeCreate()
   {
      return CreateLookupView(TypeLookup);
   }

   [HttpGet]
   public Task<IActionResult> TypeEdit(
      int id,
      CancellationToken cancellationToken)
   {
      return EditLookupViewAsync(
         id,
         TypeLookup,
         _projectStore.GetProjectTypeAsync,
         ToLookupData,
         cancellationToken);
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> TypeCreate(
      ProjectLookupEditViewModel model,
      CancellationToken cancellationToken)
   {
      return CreateLookupAsync(
         model,
         TypeLookup,
         ToProjectType,
         _projectStore.InsertProjectTypeAsync,
         GetTypePanelAsync,
         cancellationToken);
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> TypeEdit(
      ProjectLookupEditViewModel model,
      CancellationToken cancellationToken)
   {
      return EditLookupAsync(
         model,
         TypeLookup,
         ToProjectType,
         _projectStore.UpdateProjectTypeAsync,
         GetTypePanelAsync,
         cancellationToken);
   }

   [HttpGet]
   public IActionResult VisibilityCreate()
   {
      return CreateLookupView(VisibilityLookup);
   }

   [HttpGet]
   public Task<IActionResult> VisibilityEdit(
      int id,
      CancellationToken cancellationToken)
   {
      return EditLookupViewAsync(
         id,
         VisibilityLookup,
         _projectStore.GetProjectVisibilityAsync,
         ToLookupData,
         cancellationToken);
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> VisibilityCreate(
      ProjectLookupEditViewModel model,
      CancellationToken cancellationToken)
   {
      return CreateLookupAsync(
         model,
         VisibilityLookup,
         ToProjectVisibility,
         _projectStore.InsertProjectVisibilityAsync,
         GetVisibilityPanelAsync,
         cancellationToken);
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> VisibilityEdit(
      ProjectLookupEditViewModel model,
      CancellationToken cancellationToken)
   {
      return EditLookupAsync(
         model,
         VisibilityLookup,
         ToProjectVisibility,
         _projectStore.UpdateProjectVisibilityAsync,
         GetVisibilityPanelAsync,
         cancellationToken);
   }

   [HttpGet]
   public IActionResult TagCreate()
   {
      return CreateLookupView(TagLookup);
   }

   [HttpGet]
   public Task<IActionResult> TagEdit(
      int id,
      CancellationToken cancellationToken)
   {
      return EditLookupViewAsync(
         id,
         TagLookup,
         _projectStore.GetProjectTagAsync,
         ToLookupData,
         cancellationToken);
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> TagCreate(
      ProjectLookupEditViewModel model,
      CancellationToken cancellationToken)
   {
      return CreateLookupAsync(
         model,
         TagLookup,
         ToProjectTag,
         _projectStore.InsertProjectTagAsync,
         GetTagPanelAsync,
         cancellationToken);
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> TagEdit(
      ProjectLookupEditViewModel model,
      CancellationToken cancellationToken)
   {
      return EditLookupAsync(
         model,
         TagLookup,
         ToProjectTag,
         _projectStore.UpdateProjectTagAsync,
         GetTagPanelAsync,
         cancellationToken);
   }

   [HttpGet]
   public IActionResult StackCreate()
   {
      return CreateLookupView(StackLookup);
   }

   [HttpGet]
   public Task<IActionResult> StackEdit(
      int id,
      CancellationToken cancellationToken)
   {
      return EditLookupViewAsync(
         id,
         StackLookup,
         _projectStore.GetProjectStackTagAsync,
         ToLookupData,
         cancellationToken);
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> StackCreate(
      ProjectLookupEditViewModel model,
      CancellationToken cancellationToken)
   {
      return CreateLookupAsync(
         model,
         StackLookup,
         ToProjectStackTag,
         _projectStore.InsertProjectStackTagAsync,
         GetStackTagPanelAsync,
         cancellationToken);
   }

   [HttpPost]
   [ValidateAntiForgeryToken]
   public Task<IActionResult> StackEdit(
      ProjectLookupEditViewModel model,
      CancellationToken cancellationToken)
   {
      return EditLookupAsync(
         model,
         StackLookup,
         ToProjectStackTag,
         _projectStore.UpdateProjectStackTagAsync,
         GetStackTagPanelAsync,
         cancellationToken);
   }

   private IActionResult CreateLookupView(
      ProjectLookupDefinition definition)
   {
      return PartialView(
         ProjectLookupEditView,
         CreateLookupModel(definition));
   }

   private async Task<IActionResult> EditLookupViewAsync<TLookup>(
      int id,
      ProjectLookupDefinition definition,
      Func<int, CancellationToken, Task<TLookup?>> getLookupAsync,
      Func<TLookup, ProjectLookupData> mapLookup,
      CancellationToken cancellationToken)
   {
      var lookup = await getLookupAsync(id, cancellationToken);

      if (lookup is null)
      {
         return NotFound();
      }

      return PartialView(
         ProjectLookupEditView,
         CreateLookupModel(definition, mapLookup(lookup)));
   }

   private async Task<IActionResult> CreateLookupAsync<TLookup>(
      ProjectLookupEditViewModel model,
      ProjectLookupDefinition definition,
      Func<ProjectLookupEditViewModel, TLookup> createLookup,
      Func<TLookup, CancellationToken, Task> insertAsync,
      Func<CancellationToken, Task<ProjectLookupPanelViewModel>> getPanelAsync,
      CancellationToken cancellationToken)
   {
      ApplyLookupDefinition(model, definition, isEditMode: false);

      if (!ModelState.IsValid)
      {
         return PartialView(ProjectLookupEditView, model);
      }

      try
      {
         await insertAsync(createLookup(model), cancellationToken);
      }
      catch (DuplicateSlugException)
      {
         ModelState.AddModelError(
            nameof(model.Slug),
            "Another lookup item already uses this slug.");

         return PartialView(ProjectLookupEditView, model);
      }

      Response.TriggerAdminOffcanvasClose(
         ProjectAdminIds.ProjectLookupEditorOffcanvas);

      return PartialView(
         ProjectLookupPanelBodyRefreshView,
         await getPanelAsync(cancellationToken));
   }

   private async Task<IActionResult> EditLookupAsync<TLookup>(
      ProjectLookupEditViewModel model,
      ProjectLookupDefinition definition,
      Func<ProjectLookupEditViewModel, TLookup> createLookup,
      Func<TLookup, CancellationToken, Task<bool>> updateAsync,
      Func<CancellationToken, Task<ProjectLookupPanelViewModel>> getPanelAsync,
      CancellationToken cancellationToken)
   {
      ApplyLookupDefinition(model, definition, isEditMode: true);

      if (model.Id is null)
      {
         return BadRequest();
      }

      if (!ModelState.IsValid)
      {
         return PartialView(ProjectLookupEditView, model);
      }

      try
      {
         bool updated = await updateAsync(
            createLookup(model),
            cancellationToken);

         if (!updated)
         {
            return NotFound();
         }
      }
      catch (DuplicateSlugException)
      {
         ModelState.AddModelError(
            nameof(model.Slug),
            "Another lookup item already uses this slug.");

         return PartialView(ProjectLookupEditView, model);
      }

      Response.TriggerAdminOffcanvasClose(
         ProjectAdminIds.ProjectLookupEditorOffcanvas);

      return PartialView(
         ProjectLookupPanelBodyRefreshView,
         await getPanelAsync(cancellationToken));
   }

   private static ProjectLookupEditViewModel CreateLookupModel(
      ProjectLookupDefinition definition)
   {
      return new ProjectLookupEditViewModel()
      {
         Title = definition.CreateTitle,
         FormAction = definition.CreateAction,
         IsActive = true,
      };
   }

   private static ProjectLookupEditViewModel CreateLookupModel(
      ProjectLookupDefinition definition,
      ProjectLookupData lookup)
   {
      return new ProjectLookupEditViewModel()
      {
         Id = lookup.Id,
         Slug = lookup.Slug,
         DisplayName = lookup.DisplayName,
         Description = lookup.Description,
         SortOrder = lookup.SortOrder,
         IsActive = lookup.IsActive,
         Title = definition.EditTitle,
         FormAction = definition.EditAction,
      };
   }

   private static void ApplyLookupDefinition(
      ProjectLookupEditViewModel model,
      ProjectLookupDefinition definition,
      bool isEditMode)
   {
      model.Title = isEditMode
         ? definition.EditTitle
         : definition.CreateTitle;

      model.FormAction = isEditMode
         ? definition.EditAction
         : definition.CreateAction;
   }

   private static ProjectLookupData ToLookupData(ProjectStatus status)
   {
      return new ProjectLookupData(
         status.Id,
         status.Slug,
         status.DisplayName,
         status.Description,
         status.SortOrder,
         status.IsActive);
   }

   private static ProjectLookupData ToLookupData(ProjectOrigin origin)
   {
      return new ProjectLookupData(
         origin.Id,
         origin.Slug,
         origin.DisplayName,
         origin.Description,
         origin.SortOrder,
         origin.IsActive);
   }

   private static ProjectLookupData ToLookupData(ProjectType type)
   {
      return new ProjectLookupData(
         type.Id,
         type.Slug,
         type.DisplayName,
         type.Description,
         type.SortOrder,
         type.IsActive);
   }

   private static ProjectLookupData ToLookupData(ProjectVisibility visibility)
   {
      return new ProjectLookupData(
         visibility.Id,
         visibility.Slug,
         visibility.DisplayName,
         visibility.Description,
         visibility.SortOrder,
         visibility.IsActive);
   }

   private static ProjectLookupData ToLookupData(ProjectTag tag)
   {
      return new ProjectLookupData(
         tag.Id,
         tag.Slug,
         tag.DisplayName,
         tag.Description,
         tag.SortOrder,
         tag.IsActive);
   }

   private static ProjectLookupData ToLookupData(ProjectStackTag tag)
   {
      return new ProjectLookupData(
         tag.Id,
         tag.Slug,
         tag.DisplayName,
         tag.Description,
         tag.SortOrder,
         tag.IsActive);
   }

   private static ProjectStatus ToProjectStatus(
      ProjectLookupEditViewModel model)
   {
      ArgumentException.ThrowIfNullOrWhiteSpace(model.Slug);
      ArgumentException.ThrowIfNullOrWhiteSpace(model.DisplayName);

      return new ProjectStatus()
      {
         Id = model.Id ?? 0,
         Slug = model.Slug,
         DisplayName = model.DisplayName,
         Description = model.Description,
         SortOrder = model.SortOrder,
         IsActive = model.IsActive,
      };
   }

   private static ProjectOrigin ToProjectOrigin(
      ProjectLookupEditViewModel model)
   {
      ArgumentException.ThrowIfNullOrWhiteSpace(model.Slug);
      ArgumentException.ThrowIfNullOrWhiteSpace(model.DisplayName);

      return new ProjectOrigin()
      {
         Id = model.Id ?? 0,
         Slug = model.Slug,
         DisplayName = model.DisplayName,
         Description = model.Description,
         SortOrder = model.SortOrder,
         IsActive = model.IsActive,
      };
   }

   private static ProjectType ToProjectType(
      ProjectLookupEditViewModel model)
   {
      ArgumentException.ThrowIfNullOrWhiteSpace(model.Slug);
      ArgumentException.ThrowIfNullOrWhiteSpace(model.DisplayName);

      return new ProjectType()
      {
         Id = model.Id ?? 0,
         Slug = model.Slug,
         DisplayName = model.DisplayName,
         Description = model.Description,
         SortOrder = model.SortOrder,
         IsActive = model.IsActive,
      };
   }

   private static ProjectVisibility ToProjectVisibility(
      ProjectLookupEditViewModel model)
   {
      ArgumentException.ThrowIfNullOrWhiteSpace(model.Slug);
      ArgumentException.ThrowIfNullOrWhiteSpace(model.DisplayName);

      return new ProjectVisibility()
      {
         Id = model.Id ?? 0,
         Slug = model.Slug,
         DisplayName = model.DisplayName,
         Description = model.Description,
         SortOrder = model.SortOrder,
         IsActive = model.IsActive,
      };
   }

   private static ProjectTag ToProjectTag(
      ProjectLookupEditViewModel model)
   {
      ArgumentException.ThrowIfNullOrWhiteSpace(model.Slug);
      ArgumentException.ThrowIfNullOrWhiteSpace(model.DisplayName);

      return new ProjectTag()
      {
         Id = model.Id ?? 0,
         Slug = model.Slug,
         DisplayName = model.DisplayName,
         Description = model.Description,
         SortOrder = model.SortOrder,
         IsActive = model.IsActive,
      };
   }

   private static ProjectStackTag ToProjectStackTag(
      ProjectLookupEditViewModel model)
   {
      ArgumentException.ThrowIfNullOrWhiteSpace(model.Slug);
      ArgumentException.ThrowIfNullOrWhiteSpace(model.DisplayName);

      return new ProjectStackTag()
      {
         Id = model.Id ?? 0,
         Slug = model.Slug,
         DisplayName = model.DisplayName,
         Description = model.Description,
         SortOrder = model.SortOrder,
         IsActive = model.IsActive,
      };
   }

   private sealed record ProjectLookupDefinition(
      string CreateTitle,
      string EditTitle,
      string CreateAction,
      string EditAction,
      string RefreshRegionId);

   private sealed record ProjectLookupData(
      int Id,
      string Slug,
      string DisplayName,
      string? Description,
      int SortOrder,
      bool IsActive);

   private static readonly ProjectLookupDefinition StatusLookup =
      new(
         "New project status",
         "Edit project status",
         nameof(StatusCreate),
         nameof(StatusEdit),
         ProjectAdminIds.ProjectStatusPanelRegion);

   private static readonly ProjectLookupDefinition OriginLookup =
      new(
         "New project origin",
         "Edit project origin",
         nameof(OriginCreate),
         nameof(OriginEdit),
         ProjectAdminIds.ProjectOriginPanelRegion);

   private static readonly ProjectLookupDefinition TypeLookup =
      new(
         "New project type",
         "Edit project type",
         nameof(TypeCreate),
         nameof(TypeEdit),
         ProjectAdminIds.ProjectTypePanelRegion);

   private static readonly ProjectLookupDefinition VisibilityLookup =
      new(
         "New project visibility",
         "Edit project visibility",
         nameof(VisibilityCreate),
         nameof(VisibilityEdit),
         ProjectAdminIds.ProjectVisibilityPanelRegion);

   private static readonly ProjectLookupDefinition TagLookup =
      new(
         "New project tag",
         "Edit project tag",
         nameof(TagCreate),
         nameof(TagEdit),
         ProjectAdminIds.ProjectTagPanelRegion);

   private static readonly ProjectLookupDefinition StackLookup =
      new(
         "New technology stack",
         "Edit technology stack",
         nameof(StackCreate),
         nameof(StackEdit),
         ProjectAdminIds.ProjectStackTagPanelRegion);

   private const string ProjectLookupEditView = "ProjectLookupEdit";

   private const string ProjectLookupPanelBodyRefreshView =
      "ProjectLookupPanelBodyRefresh";
}