// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace DavidBrowning.Models.Projects;

/// <summary>
/// Maps to db_ProjectStatuses.
/// User-facing status label for a project.
/// Examples: Active, Prototype, Archived.
/// </summary>
[PrimaryKey(nameof(Id))]
[Index(nameof(Slug), IsUnique = true)]
public sealed class ProjectStatus : IQueryableSlug
{
   [Required, Key]
   public int Id { get; set; }

   /// <summary>
   /// URL/config-friendly identifier for the status.
   /// Example: "active".
   /// </summary>
   [Required]
   [StringLength(DataConstants.MaxSlugLength)]
   public required string Slug { get; set; }

   /// <summary>
   /// User-facing display text.
   /// Example: "Active".
   /// </summary>
   [Required]
   [StringLength(DataConstants.MaxLabelLength)]
   public required string DisplayName { get; set; }

   /// <summary>
   /// Optional explanation of what this status means.
   /// </summary>
   [StringLength(DataConstants.MaxMetadataLength)]
   public string? Description { get; set; }

   /// <summary>
   /// Manual display ordering for status lists.
   /// </summary>
   public int SortOrder { get; set; }

   /// <summary>
   /// Whether this status is available for new/current projects.
   /// </summary>
   public bool IsActive { get; set; } = true;

   public ICollection<Project> Projects { get; } = new List<Project>();
}