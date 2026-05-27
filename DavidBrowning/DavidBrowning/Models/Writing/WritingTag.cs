// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace DavidBrowning.Models.Writing;

/// <summary>
/// Maps to db_WritingTags
/// </summary>
[PrimaryKey("Id")]
[Index(nameof(Slug), IsUnique = true)]
public sealed class WritingTag : IQueryableSlug
{
   [Required, Key]
   public int Id { get; set; }

   /// <summary>
   /// The content of the tag. Something like ".NET" or "Thoughts".
   /// </summary>
   [Required]
   [StringLength(DataConstants.MaxLabelLength)]
   public required string DisplayName { get; set; }

   /// <summary>
   /// The URL-friendly text so we can query all posts with this tag applied.
   /// </summary>
   [Required]
   [StringLength(DataConstants.MaxSlugLength)]
   public required string Slug { get; set; }

   public ICollection<PostTag> PostTags { get; } = new List<PostTag>();
}
