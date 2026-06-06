// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace DavidBrowning.Models;

/// <summary>
/// Maps to db_Interests.
/// Represents a personal or professional interest shown on the About page.
/// </summary>
[PrimaryKey(nameof(Id))]
[Index(nameof(Slug), IsUnique = true)]
public sealed class Interest : IQueryableSlug
{
   [Required, Key]
   public int Id { get; set; }

   /// <summary>
   /// URL-friendly identifier for the interest.
   /// Example: "software-systems".
   /// </summary>
   [Required]
   [StringLength(DataConstants.MaxSlugLength)]
   public required string Slug { get; set; }

   /// <summary>
   /// User-facing interest name.
   /// Example: "Software Systems".
   /// </summary>
   [Required]
   [StringLength(DataConstants.MaxLabelLength)]
   public required string DisplayName { get; set; }

   /// <summary>
   /// Short card/list explanation of why this interest matters.
   /// This should explain more than the topic; it should explain the pull.
   /// </summary>
   [StringLength(DataConstants.MaxInterestSummaryLength)]
   public required string Summary { get; set; }

   /// <summary>
   /// Optional Font Awesome or site-specific icon CSS class.
   /// Example: "fa-solid fa-diagram-project".
   /// </summary>
   [StringLength(DataConstants.MaxIconCssClassLength)]
   public string? IconCssClass { get; set; }

   /// <summary>
   /// Manual ordering for interest lists.
   /// Lower numbers appear earlier.
   /// </summary>
   public int SortOrder { get; set; }

   /// <summary>
   /// Whether this interest should be shown publicly.
   /// </summary>
   public bool IsActive { get; set; } = true;

   /// <summary>
   /// When the interest record was created.
   /// Stored in UTC.
   /// </summary>
   public required DateTime CreatedAtUtc { get; set; }

   /// <summary>
   /// When the interest record was last updated.
   /// Stored in UTC.
   /// </summary>
   public required DateTime UpdatedAtUtc { get; set; }
}