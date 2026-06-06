// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace DavidBrowning.Models.Writing;

/// <summary>
/// Maps to db_PostStyles.
/// User-facing classification for how a writing post should be presented.
/// Examples: Blog, Quip, Blurb.
/// </summary>
[PrimaryKey(nameof(Id))]
[Index(nameof(Slug), IsUnique = true)]
public sealed class PostStyle : IQueryableSlug
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

   public ICollection<Post> Posts { get; } = new List<Post>();
}