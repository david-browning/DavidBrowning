// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DavidBrowning.Models.Work;

/// <summary>
/// Maps to db_ExperienceRoles.
/// Represents one role held during a professional experience.
/// </summary>
[PrimaryKey(nameof(Id))]
[Index(nameof(ExperienceId), nameof(SortOrder))]
public sealed class ExperienceRole :
   ISortableRecord,
   IDateCreatedTrackedEntity, IDateUpdatedTrackedEntity
{
   [Required, Key]
   public int Id { get; set; }

   /// <summary>
   /// Foreign key to db_Experiences.
   /// </summary>
   public int ExperienceId { get; set; }

   [ForeignKey(nameof(ExperienceId))]
   public Experience? Experience { get; set; }

   /// <summary>
   /// User-facing role title.
   /// </summary>
   [Required]
   [StringLength(DataConstants.MaxNameLength)]
   public required string Title { get; set; }

   /// <summary>
   /// Optional user-facing date range.
   /// Example: "January 2020–March 2024".
   /// </summary>
   [StringLength(DataConstants.MaxLabelLength)]
   public string? DateDisplayText { get; set; }

   /// <summary>
   /// Optional summary of the role.
   /// </summary>
   [StringLength(DataConstants.MaxMetadataLength)]
   public string? Description { get; set; }

   /// <summary>
   /// Manual ordering within an employer.
   /// Lower numbers appear earlier.
   /// </summary>
   public int SortOrder { get; set; }

   /// <summary>
   /// Whether this role should be shown publicly.
   /// </summary>
   public bool IsActive { get; set; } = true;

   /// <summary>
   /// When the role record was created.
   /// Stored in UTC.
   /// </summary>
   public DateTime CreatedAtUtc { get; set; }

   /// <summary>
   /// When the role record was last updated.
   /// Stored in UTC.
   /// </summary>
   public DateTime UpdatedAtUtc { get; set; }

   public ICollection<ExperienceRoleBullet> Bullets { get; } =
      new List<ExperienceRoleBullet>();
}