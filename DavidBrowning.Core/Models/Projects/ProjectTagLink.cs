// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.ComponentModel.DataAnnotations.Schema;

namespace DavidBrowning.Models.Projects;

/// <summary>
/// Maps to db_ProjectTagLinks.
/// Join table connecting projects to project topic/category tags.
/// </summary>
public sealed class ProjectTagLink
{
   /// <summary>
   /// Foreign key to db_Projects.
   /// </summary>
   public int ProjectId { get; set; }

   [ForeignKey(nameof(ProjectId))]
   public Project? Project { get; set; }

   /// <summary>
   /// Foreign key to db_ProjectTags.
   /// </summary>
   public int ProjectTagId { get; set; }

   [ForeignKey(nameof(ProjectTagId))]
   public ProjectTag? ProjectTag { get; set; }
}