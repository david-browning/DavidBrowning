// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DavidBrowning.Models.Projects;

/// <summary>
/// Maps to db_ProjectLinks.
/// External URL associated with a project.
/// Examples: repository, live demo, release, documentation, download.
/// </summary>
[PrimaryKey(nameof(Id))]
public sealed class ProjectLink
{
   [Required, Key]
   public int Id { get; set; }

   /// <summary>
   /// Foreign key to db_Projects.
   /// </summary>
   public int ProjectId { get; set; }

   [ForeignKey(nameof(ProjectId))]
   public Project? Project { get; set; }

   /// <summary>
   /// Foreign key to db_ProjectLinkTypes.
   /// </summary>
   public int ProjectLinkTypeId { get; set; }

   [ForeignKey(nameof(ProjectLinkTypeId))]
   public ProjectLinkType? ProjectLinkType { get; set; }

   /// <summary>
   /// User-facing label for this link.
   /// Example: "GitHub", "Live Demo", "Download".
   /// </summary>
   [Required]
   [StringLength(DataConstants.MaxLabelLength)]
   public required string Label { get; set; }

   /// <summary>
   /// Destination URL.
   /// </summary>
   [Required]
   [StringLength(DataConstants.MaxUrlLength)]
   public required string Url { get; set; }

   /// <summary>
   /// Whether this link should be treated as the primary call-to-action.
   /// </summary>
   public bool IsPrimary { get; set; }

   /// <summary>
   /// Manual display ordering for links within a project.
   /// </summary>
   public int SortOrder { get; set; }
}