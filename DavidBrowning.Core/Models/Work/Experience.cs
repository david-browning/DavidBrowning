// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace DavidBrowning.Models.Work;

/// <summary>
/// Maps to db_Experiences.
/// Represents an employer or organization in the professional work history.
/// </summary>
[PrimaryKey(nameof(Id))]
[Index(nameof(SortOrder))]
public sealed class Experience : 
   ISortableRecord,
   IDateCreatedTrackedEntity, IDateUpdatedTrackedEntity
{
   [Required, Key]
   public int Id { get; set; }

   /// <summary>
   /// User-facing employer or organization name.
   /// </summary>
   [Required]
   [StringLength(DataConstants.MaxNameLength)]
   public required string CompanyName { get; set; }

   /// <summary>
   /// Optional user-facing location text.
   /// Examples: "Redmond, WA", "Remote", or "Spokane, WA · Remote".
   /// </summary>
   [StringLength(DataConstants.MaxLabelLength)]
   public string? LocationDisplayText { get; set; }

   /// <summary>
   /// Manual ordering for professional experience.
   /// Lower numbers appear earlier.
   /// </summary>
   public int SortOrder { get; set; }

   /// <summary>
   /// Whether this experience should be shown publicly.
   /// </summary>
   public bool IsActive { get; set; } = true;

   /// <summary>
   /// When the experience record was created.
   /// Stored in UTC.
   /// </summary>
   public DateTime CreatedAtUtc { get; set; }

   /// <summary>
   /// When the experience record was last updated.
   /// Stored in UTC.
   /// </summary>
   public DateTime UpdatedAtUtc { get; set; }

   public ICollection<ExperienceRole> Roles { get; } =
      new List<ExperienceRole>();
}