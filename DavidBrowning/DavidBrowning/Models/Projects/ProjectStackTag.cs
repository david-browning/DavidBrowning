// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace DavidBrowning.Models.Projects;

/// <summary>
/// Maps to db_ProjectStackTags.
/// User-facing technology/stack tag for projects.
/// Examples: ASP.NET Core, SQL, C++, Azure.
/// </summary>
[PrimaryKey(nameof(Id))]
[Index(nameof(Slug), IsUnique = true)]
public sealed class ProjectStackTag : ISlugLookup
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

   public ICollection<ProjectStackTagLink> ProjectLinks { get; } = new List<ProjectStackTagLink>();
}