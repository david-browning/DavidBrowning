// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace DavidBrowning.Models.Projects;

/// <summary>
/// Maps to db_ProjectVisibilities.
/// Publishing/listing state for a project.
/// Examples: Draft, Published, Unlisted, Archived.
/// </summary>
[PrimaryKey(nameof(Id))]
[Index(nameof(Slug), IsUnique = true)]
public sealed class ProjectVisibility : ISlugLookup
{
   [Required, Key]
   public int Id { get; set; }

   [Required]
   [StringLength(DataConstants.MaxSlugLength)]
   public required string Slug { get; set; }

   [Required]
   [StringLength(DataConstants.MaxLabelLength)]
   public required string DisplayName { get; set; }

   [StringLength(DataConstants.MaxMetadataLength)]
   public string? Description { get; set; }

   public int SortOrder { get; set; }

   public bool IsActive { get; set; } = true;

   public ICollection<Project> Projects { get; } = new List<Project>();
}