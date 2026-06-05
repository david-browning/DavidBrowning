// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using System;
using System.ComponentModel.DataAnnotations;

using Microsoft.EntityFrameworkCore;

namespace DavidBrowning.Models.Work;

/// <summary>
/// Maps to db_Credentials.
/// Represents a degree, certification, or other professional credential.
/// </summary>
[PrimaryKey(nameof(Id))]
[Index(nameof(SortOrder))]
public sealed class Credential
{
   [Required, Key]
   public int Id { get; set; }

   /// <summary>
   /// Organization that issued the credential.
   /// Example: "Eastern Washington University".
   /// </summary>
   [Required]
   [StringLength(DataConstants.MaxNameLength)]
   public required string IssuingOrganization { get; set; }

   /// <summary>
   /// User-facing credential name.
   /// Example: "Bachelor of Science in Computer Science".
   /// </summary>
   [Required]
   [StringLength(DataConstants.MaxNameLength)]
   public required string Name { get; set; }

   /// <summary>
   /// Optional credential category.
   /// Examples: "Degree", "Certification", or "Certificate".
   /// </summary>
   [StringLength(DataConstants.MaxLabelLength)]
   public string? Type { get; set; }

   /// <summary>
   /// Optional month when the credential was awarded.
   /// </summary>
   public int? AwardedMonth { get; set; }

   /// <summary>
   /// Optional year when the credential was awarded.
   /// </summary>
   public int? AwardedYear { get; set; }

   /// <summary>
   /// Optional user-facing date override.
   /// Example: "Expected 2027".
   /// </summary>
   [StringLength(DataConstants.MaxLabelLength)]
   public string? DateDisplayText { get; set; }

   /// <summary>
   /// Optional additional credential details.
   /// </summary>
   [StringLength(DataConstants.MaxMetadataLength)]
   public string? Description { get; set; }

   /// <summary>
   /// Optional URL for verifying or describing the credential.
   /// </summary>
   [StringLength(DataConstants.MaxUrlLength)]
   public string? CredentialUrl { get; set; }

   /// <summary>
   /// Manual ordering for credentials.
   /// Lower numbers appear earlier.
   /// </summary>
   public int SortOrder { get; set; }

   /// <summary>
   /// Whether this credential should be shown publicly.
   /// </summary>
   public bool IsActive { get; set; } = true;

   /// <summary>
   /// When the credential record was created.
   /// Stored in UTC.
   /// </summary>
   public required DateTime CreatedAtUtc { get; set; }

   /// <summary>
   /// When the credential record was last updated.
   /// Stored in UTC.
   /// </summary>
   public required DateTime UpdatedAtUtc { get; set; }
}