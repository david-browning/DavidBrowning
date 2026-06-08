// Copyright © 2026 David Browning. All rights reserved.
//
// Source-available for viewing only. No license granted.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using DavidBrowning.Models;
using DavidBrowning.Models.Projects;

namespace DavidBrowning.Admin.ViewModels.Projects;

public sealed class ProjectEditViewModel : IValidatableObject
{
   public EditModes EditMode { get; set; } = EditModes.Create;

   public int? Id { get; set; }

   [Required]
   [StringLength(DataConstants.MaxSlugLength)]
   [RegularExpression(
      DataConstants.SlugRegex,
      ErrorMessage = DataConstants.SlugRegexError)]
   public string? Slug { get; set; }

   [Required]
   [StringLength(DataConstants.MaxLabelLength)]
   public string? Name { get; set; }

   [StringLength(DataConstants.MaxMetadataLength)]
   public string? Description { get; set; }

   [Required]
   [Range(1, int.MaxValue)]
   public int? ProjectStatusId { get; set; }

   [Required]
   [Range(1, int.MaxValue)]
   public int? ProjectTypeId { get; set; }

   [Required]
   [Range(1, int.MaxValue)]
   public int? ProjectOriginId { get; set; }

   [Required]
   [Range(1, int.MaxValue)]
   public int? ProjectVisibilityId { get; set; }

   [StringLength(DataConstants.MaxNameLength)]
   public string? Role { get; set; }

   [StringLength(DataConstants.MaxMetadataLength)]
   public string? ContributionSummary { get; set; }

   public bool IsFeatured { get; set; }

   [Range(0, int.MaxValue)]
   public int SortOrder { get; set; }

   public DateOnly? StartDate { get; set; }

   public DateOnly? EndDate { get; set; }

   [StringLength(DataConstants.MaxLabelLength)]
   public string? DateDisplayText { get; set; }

   // Simple many-to-many selections. Persist these by diffing the submitted
   // IDs against Project.TagLinks and Project.StackTagLinks.
   public ISet<int> ProjectTagIds { get; set; } = new HashSet<int>();

   public ISet<int> ProjectStackTagIds { get; set; } = new HashSet<int>();

   public ProjectEditViewModel()
   {
   }

   public ProjectEditViewModel(Project project)
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
      SortOrder = project.SortOrder;
      StartDate = project.StartDate;
      EndDate = project.EndDate;
      DateDisplayText = project.DateDisplayText;
      ProjectTagIds = project.TagLinks
         .Select(link => link.ProjectTagId)
         .ToHashSet();
      ProjectStackTagIds = project.StackTagLinks
         .Select(link => link.ProjectStackTagId)
         .ToHashSet();
   }

   public IEnumerable<ValidationResult> Validate(
      ValidationContext validationContext)
   {
      if (StartDate is not null &&
          EndDate is not null &&
          EndDate < StartDate)
      {
         yield return new ValidationResult(
            "The end date cannot precede the start date.",
            [nameof(EndDate)]);
      }
   }
}
