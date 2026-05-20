// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DavidBrowning.Models.Writing;

namespace DavidBrowning.Models.Projects
{
   /// <summary>
   /// Maps to db_ProjectPosts.
   /// Join table connecting projects to related writing posts.
   /// </summary>
   public sealed class ProjectPost
   {
      /// <summary>
      /// Foreign key to db_Projects.
      /// </summary>
      public int ProjectId { get; set; }

      [ForeignKey(nameof(ProjectId))]
      public Project? Project { get; set; }

      /// <summary>
      /// Foreign key to db_Posts.
      /// </summary>
      public int PostId { get; set; }

      [ForeignKey(nameof(PostId))]
      public Post? Post { get; set; }

      /// <summary>
      /// Optional user-facing explanation of the relationship.
      /// Example: "Design notes", "Implementation writeup", "Related essay".
      /// </summary>
      [StringLength(DataConstants.MaxLabelLength)]
      public string? RelationshipLabel { get; set; }

      /// <summary>
      /// Manual display ordering for related writings.
      /// </summary>
      public int SortOrder { get; set; }
   }
}