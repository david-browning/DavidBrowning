// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.ComponentModel.DataAnnotations.Schema;

namespace DavidBrowning.Models.Projects;

/// <summary>
/// Maps to db_ProjectStackTagLinks.
/// Join table connecting projects to technology/stack tags.
/// </summary>
public sealed class ProjectStackTagLink
{
   /// <summary>
   /// Foreign key to db_Projects.
   /// </summary>
   public int ProjectId { get; set; }

   [ForeignKey(nameof(ProjectId))]
   public Project? Project { get; set; }

   /// <summary>
   /// Foreign key to db_ProjectStackTags.
   /// </summary>
   public int ProjectStackTagId { get; set; }

   [ForeignKey(nameof(ProjectStackTagId))]
   public ProjectStackTag? ProjectStackTag { get; set; }
}