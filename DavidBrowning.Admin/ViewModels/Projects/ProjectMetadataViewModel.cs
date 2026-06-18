// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DavidBrowning.Infrastructure.Validators;
using DavidBrowning.Models;
using DavidBrowning.Models.Projects;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace DavidBrowning.Admin.ViewModels.Projects;

public sealed class ProjectMetadataViewModel
{
   public EditModes EditMode { get; set; } = EditModes.Create;

   public int? Id { get; set; }

   [Required]
   [RegularExpression(DataConstants.SlugRegex,
      ErrorMessage = DataConstants.SlugRegexError)]
   [StringLength(DataConstants.MaxSlugLength)]
   public string? Slug { get; set; }

   [DisplayName("Project Name")]
   [Required]
   [StringLength(DataConstants.MaxLabelLength)]
   public string? Name { get; set; }

   [StringLength(DataConstants.MaxMetadataLength)]
   public string? Description { get; set; }

   [DisplayName("Project Status")]
   [Required(ErrorMessage = "Select a project status.")]
   public int? ProjectStatusId { get; set; }

   [DisplayName("Project Type")]
   [Required(ErrorMessage = "Select a project type.")]
   public int? ProjectTypeId { get; set; }

   [DisplayName("Project Origin")]
   [Required(ErrorMessage = "Choose where the project came from.")]
   public int? ProjectOriginId { get; set; }

   [DisplayName("Project Visibility")]
   [Required(ErrorMessage = "Select a project visibility.")]
   public int? ProjectVisibilityId { get; set; }

   [DisplayName("Project Contribution Role")]
   [StringLength(DataConstants.MaxNameLength)]
   public string? Role { get; set; }

   [DisplayName("Project Contribution Summary")]
   [StringLength(DataConstants.MaxMetadataLength)]
   public string? ContributionSummary { get; set; }

   public bool IsFeatured { get; set; } = false;

   [DisplayName("Project Start Date")]
   public DateOnly? StartDate { get; set; }

   [DisplayName("Project End Date")]
   public DateOnly? EndDate { get; set; }

   [DisplayName("Project Display Date")]
   [StringLength(DataConstants.MaxLabelLength)]
   public string? DateDisplayText { get; set; }

   [DisplayName("Project Tags")]
   [CollectionCount(0, MaximumCount = 20,
      ErrorMessage = "Select no more than 20 project tags.")]
   public List<int> ProjectTagIds { get; set; } = new();

   [DisplayName("Technology Stacks")]
   [CollectionCount(0, MaximumCount = 20,
      ErrorMessage = "Select no more than 20 stack tags.")]
   public List<int> ProjectStackTagIds { get; set; } = new();

   [ValidateNever]
   [BindNever]
   public IReadOnlyList<LookupOptionViewModel> StatusOptions { get; set; } =
      Array.Empty<LookupOptionViewModel>();

   [ValidateNever]
   [BindNever]
   public IReadOnlyList<LookupOptionViewModel> TypeOptions { get; set; } =
      Array.Empty<LookupOptionViewModel>();

   [ValidateNever]
   [BindNever]
   public IReadOnlyList<LookupOptionViewModel> OriginOptions { get; set; } =
      Array.Empty<LookupOptionViewModel>();

   [ValidateNever]
   [BindNever]
   public IReadOnlyList<LookupOptionViewModel> VisibilityOptions { get; set; } =
      Array.Empty<LookupOptionViewModel>();

   [ValidateNever]
   [BindNever]
   public IReadOnlyList<LookupOptionViewModel> TagOptions { get; set; } =
      Array.Empty<LookupOptionViewModel>();

   [ValidateNever]
   [BindNever]
   public IReadOnlyList<LookupOptionViewModel> StackTagOptions { get; set; } =
      Array.Empty<LookupOptionViewModel>();

   [DisplayName("Project Links")]
   public List<ProjectLinkInputViewModel> Links { get; set; } = new();

   [ValidateNever]
   [BindNever]
   public IReadOnlyList<LookupOptionViewModel> LinkTypeOptions { get; set; } =
      Array.Empty<LookupOptionViewModel>();

   public ProjectMetadataViewModel()
   {
   }

   [SetsRequiredMembers]
   public ProjectMetadataViewModel(
      Project project,
      ProjectMetadataOptionsViewModel options)
   {
      EditMode = EditModes.Edit;
      Id = project.Id;
      Slug = project.Slug;
      Name = project.Name;
      Description = project.Description;
      ProjectStatusId = project.ProjectStatusId;
      ProjectTypeId = project.ProjectTypeId;
      ProjectOriginId = project.ProjectOriginId;
      ProjectVisibilityId = project.ProjectVisibilityId;
      Role = project.Role;
      ContributionSummary = project.ContributionSummary;
      IsFeatured = project.IsFeatured;
      StartDate = project.StartDate;
      EndDate = project.EndDate;
      DateDisplayText = project.DateDisplayText;

      ProjectTagIds = project.TagLinks
         .Select(link => link.ProjectTagId)
         .Order()
         .ToList();

      ProjectStackTagIds = project.StackTagLinks
         .Select(link => link.ProjectStackTagId)
         .Order()
         .ToList();

      Links = project.Links
         .OrderBy(link => link.SortOrder)
         .ThenBy(link => link.Label)
         .Select(link => new ProjectLinkInputViewModel(link))
         .ToList();

      SetOptions(options);
   }

   public void SetOptions(ProjectMetadataOptionsViewModel options)
   {
      StatusOptions = options.Statuses;
      TypeOptions = options.Types;
      OriginOptions = options.Origins;
      VisibilityOptions = options.Visibilities;
      TagOptions = options.Tags;
      StackTagOptions = options.StackTags;
      LinkTypeOptions = options.LinkTypes;
   }

   public Project ToProject()
   {
      ArgumentException.ThrowIfNullOrWhiteSpace(Slug);
      ArgumentException.ThrowIfNullOrWhiteSpace(Name);
      ArgumentNullException.ThrowIfNull(ProjectStatusId);
      ArgumentNullException.ThrowIfNull(ProjectTypeId);
      ArgumentNullException.ThrowIfNull(ProjectOriginId);
      ArgumentNullException.ThrowIfNull(ProjectVisibilityId);

      var project = new Project()
      {
         Id = Id ?? 0,
         Slug = Slug,
         Name = Name,
         Description = Description,
         ProjectStatusId = ProjectStatusId.Value,
         ProjectTypeId = ProjectTypeId.Value,
         ProjectOriginId = ProjectOriginId.Value,
         ProjectVisibilityId = ProjectVisibilityId.Value,
         Role = Role,
         ContributionSummary = ContributionSummary,
         IsFeatured = IsFeatured,
         StartDate = StartDate,
         EndDate = EndDate,
         DateDisplayText = DateDisplayText,

         // Store assigns authoritative timestamps.
         CreatedAtUtc = default,
         UpdatedAtUtc = default,
      };

      int sortOrder = 0;
      foreach (var link in Links
         .Where(link => !link.IsEmpty())
         .OrderBy(link => link.SortOrder))
      {
         project.Links.Add(link.ToProjectLink(sortOrder++));
      }

      return project;
   }
}