// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace DavidBrowning.Models.Projects
{
   /// <summary>
   /// Maps to db_ProjectLinkTypes.
   /// User-facing classification for project links.
   /// Examples: Repository, Live Demo, Documentation.
   /// </summary>
   [PrimaryKey(nameof(Id))]
   [Index(nameof(Slug), IsUnique = true)]
   public sealed class ProjectLinkType : ISlugLookup
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

      /// <summary>
      /// Optional Font Awesome CSS class.
      /// Example: "fa-brands fa-github".
      /// </summary>
      [StringLength(DataConstants.MaxIconCssClassLength)]
      public string? IconCssClass { get; set; }

      public int SortOrder { get; set; }

      public bool IsActive { get; set; } = true;

      public ICollection<ProjectLink> Links { get; } = new List<ProjectLink>();
   }
}