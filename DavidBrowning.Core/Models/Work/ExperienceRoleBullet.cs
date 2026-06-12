// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DavidBrowning.Models.Work;

/// <summary>
/// Maps to db_ExperienceRoleBullets.
/// Represents one résumé-style bullet beneath an experience role.
/// </summary>
[PrimaryKey(nameof(Id))]
[Index(nameof(ExperienceRoleId), nameof(SortOrder))]
public sealed class ExperienceRoleBullet : ISortableRecord
{
   [Required, Key]
   public int Id { get; set; }

   /// <summary>
   /// Foreign key to db_ExperienceRoles.
   /// </summary>
   public int ExperienceRoleId { get; set; }

   [ForeignKey(nameof(ExperienceRoleId))]
   public ExperienceRole? ExperienceRole { get; set; }

   /// <summary>
   /// User-facing bullet text.
   /// </summary>
   [Required]
   [StringLength(DataConstants.MaxMetadataLength)]
   public required string Text { get; set; }

   /// <summary>
   /// Manual ordering within a role.
   /// Lower numbers appear earlier.
   /// </summary>
   public int SortOrder { get; set; }

   /// <summary>
   /// Whether this bullet should be shown publicly.
   /// </summary>
   public bool IsActive { get; set; } = true;
}