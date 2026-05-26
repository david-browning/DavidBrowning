// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DavidBrowning.Models.Writing;

/// <summary>
/// Maps to db_PostRevisions
/// </summary>
[PrimaryKey("Id")]
[Index(nameof(PostId), nameof(RevisionNumber), IsUnique = true)]
public sealed class PostRevision
{
   [Required, Key]
   public int Id { get; set; }

   /// <summary>
   /// Id of the post this revision is attached to.
   /// </summary>
   public required int PostId { get; set; }

   /// <summary>
   /// The post this revision is attached to.
   /// </summary>
   [ForeignKey(nameof(PostId))]
   public Post? Post { get; set; }

   /// <summary>
   /// The logical revision number. Not the same as the database Id.
   /// Ex: 1, 2, 3rd revision.
   /// </summary>
   public int RevisionNumber { get; set; }

   /// <summary>
   /// What kind of content is stored in "Content"
   /// Instead of having it be a foreign key, we'll just map it to an enum
   /// in code.
   /// </summary>
   public required ContentFormat ContentFormat { get; set; }

   /// <summary>
   /// How to render the content.
   /// Instead of having it be a foreign key, we'll just map it to an enum
   /// in code.
   /// </summary>
   public required RenderMode RenderMode { get; set; }

   [Required]
   [StringLength(DataConstants.MaxNameLength)]
   public required string CreatedBy { get; set; }

   public required DateTime CreatedAtUtc { get; set; }

   /// <summary>
   /// A revision may or may not have text.
   /// </summary>
   public string? Content { get; set; }

   /// <summary>
   /// Cached HTML so that the code doesn't need to be reconstructed 
   /// every time someone renders the page.
   /// </summary>
   public string? CachedHtml { get; set; }
}
