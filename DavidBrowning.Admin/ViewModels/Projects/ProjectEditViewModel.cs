// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using DavidBrowning.Models;
using DavidBrowning.Models.Projects;

namespace DavidBrowning.Admin.ViewModels.Projects;

public class ProjectEditViewModel
{
   public required EditModes EditMode { get; set; }

   [Required]
   public int? Id { get; set; }

   /// <summary>
   /// URL-friendly identifier for the project.
   /// Example: "personal-website".
   /// </summary>
   [Required]
   [StringLength(DataConstants.MaxSlugLength)]
   [RegularExpression(DataConstants.SlugRegex,
      ErrorMessage = DataConstants.SlugRegexError)]
   public string? Slug { get; set; }

   /// <summary>
   /// User-facing project name.
   /// Example: "Personal Website".
   /// </summary>
   [Required]
   [StringLength(DataConstants.MaxLabelLength)]
   public string? Name { get; set; }

   /// <summary>
   /// Short card/list description for the project.
   /// Longer details content can be stored as a linked Markdown asset.
   /// </summary>
   [StringLength(DataConstants.MaxMetadataLength)]
   public string? Description { get; set; }

   /// <summary>
   /// Foreign key to db_ProjectStatuses.
   /// User-facing project status.
   /// </summary>
   [Required]
   public int? ProjectStatusId { get; set; }

   /// <summary>
   /// Foreign key to db_ProjectTypes.
   /// User-facing project type/category.
   /// </summary>
   [Required]
   public int? ProjectTypeId { get; set; }

   /// <summary>
   /// Foreign key to db_ProjectOrigins.
   /// User-facing project origin/context.
   /// </summary>
   [Required]
   public int? ProjectOriginId { get; set; }

   /// <summary>
   /// Foreign key to db_ProjectVisibilities.
   /// Controls whether/how the project is surfaced publicly.
   /// </summary>
   [Required]
   public int? ProjectVisibilityId { get; set; }

   /// <summary>
   /// Your role on the project.
   /// Example: "Creator", "Release Driver", "Tools Engineer".
   /// </summary>
   [StringLength(DataConstants.MaxNameLength)]
   public string? Role { get; set; }

   /// <summary>
   /// Short explanation of your specific contribution.
   /// Useful for professional or collaborative projects.
   /// </summary>
   [StringLength(DataConstants.MaxMetadataLength)]
   public string? ContributionSummary { get; set; }

   /// <summary>
   /// Whether this project should be highlighted on landing pages.
   /// </summary>
   public bool IsFeatured { get; set; } = false;

   /// <summary>
   /// Manual ordering for project lists.
   /// Lower numbers appear earlier.
   /// </summary>
   public int SortOrder { get; set; } = 0;

   /// <summary>
   /// Optional project start date.
   /// Uses date instead of DateTime because projects usually need date-level precision.
   /// </summary>
   public DateOnly? StartDate { get; set; }

   /// <summary>
   /// Optional project end date.
   /// Null usually means ongoing or unspecified.
   /// </summary>
   public DateOnly? EndDate { get; set; }

   /// <summary>
   /// Optional display override for project dates.
   /// Examples: "2024–Present", "Summer 2025", "Internal project, 2021–2023".
   /// </summary>
   [StringLength(DataConstants.MaxLabelLength)]
   public string? DateDisplayText { get; set; }

   /// <summary>
   /// When the project record was created.
   /// Stored in UTC.
   /// </summary>
   [Required]
   public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

   /// <summary>
   /// When the project record was last updated.
   /// Stored in UTC.
   /// </summary>
   [Required]
   public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;


   public ProjectEditViewModel()
   {

   }

   [SetsRequiredMembers]
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
      CreatedAtUtc = project.CreatedAtUtc;
      UpdatedAtUtc = project.UpdatedAtUtc;
   }
}
