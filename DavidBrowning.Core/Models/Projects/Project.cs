// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DavidBrowning.Models.Projects;

/// <summary>
/// Maps to db_Projects.
/// Represents a portfolio project, professional case study, tool, experiment, or system.
/// </summary>
[PrimaryKey(nameof(Id))]
[Index(nameof(Slug), IsUnique = true)]
public sealed class Project : IDateCreatedTrackedEntity, IDateUpdatedTrackedEntity
{
   [Required, Key]
   public int Id { get; set; }

   /// <summary>
   /// URL-friendly identifier for the project.
   /// Example: "personal-website".
   /// </summary>
   [Required]
   [StringLength(DataConstants.MaxSlugLength)]
   public required string Slug { get; set; }

   /// <summary>
   /// User-facing project name.
   /// Example: "Personal Website".
   /// </summary>
   [Required]
   [StringLength(DataConstants.MaxLabelLength)]
   public required string Name { get; set; }

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
   public int ProjectStatusId { get; set; }

   [ForeignKey(nameof(ProjectStatusId))]
   public ProjectStatus? ProjectStatus { get; set; }

   /// <summary>
   /// Foreign key to db_ProjectTypes.
   /// User-facing project type/category.
   /// </summary>
   public int ProjectTypeId { get; set; }

   [ForeignKey(nameof(ProjectTypeId))]
   public ProjectType? ProjectType { get; set; }

   /// <summary>
   /// Foreign key to db_ProjectOrigins.
   /// User-facing project origin/context.
   /// </summary>
   public int ProjectOriginId { get; set; }

   [ForeignKey(nameof(ProjectOriginId))]
   public ProjectOrigin? ProjectOrigin { get; set; }

   /// <summary>
   /// Foreign key to db_ProjectVisibilities.
   /// Controls whether/how the project is surfaced publicly.
   /// </summary>
   public int ProjectVisibilityId { get; set; }

   [ForeignKey(nameof(ProjectVisibilityId))]
   public ProjectVisibility? ProjectVisibility { get; set; }

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
   public bool IsFeatured { get; set; }

   /// <summary>
   /// Manual ordering for project lists.
   /// Lower numbers appear earlier.
   /// </summary>
   public int SortOrder { get; set; }

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
   public required DateTime CreatedAtUtc { get; set; }

   /// <summary>
   /// When the project record was last updated.
   /// Stored in UTC.
   /// </summary>
   public required DateTime UpdatedAtUtc { get; set; }

   /// <summary>
   /// Topic/category tags attached to this project.
   /// </summary>
   public ICollection<ProjectTagLink> TagLinks { get; } = new List<ProjectTagLink>();

   /// <summary>
   /// Technology/stack tags attached to this project.
   /// </summary>
   public ICollection<ProjectStackTagLink> StackTagLinks { get; } = new List<ProjectStackTagLink>();

   /// <summary>
   /// External links for this project.
   /// Examples: GitHub, live demo, documentation, download.
   /// </summary>
   public ICollection<ProjectLink> Links { get; } = new List<ProjectLink>();

   /// <summary>
   /// Links to reusable site assets used by this project.
   /// </summary>
   public ICollection<ProjectAssetLink> AssetLinks { get; } = new List<ProjectAssetLink>();

   /// <summary>
   /// Writing posts related to this project.
   /// </summary>
   public ICollection<ProjectPost> RelatedPosts { get; } = new List<ProjectPost>();
}