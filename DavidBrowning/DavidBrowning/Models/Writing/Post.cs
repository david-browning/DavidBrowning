// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DavidBrowning.Models.Writing;

/// <summary>
/// Maps to db_Posts
/// </summary>
[PrimaryKey("Id")]
[Index(nameof(Slug), IsUnique = true)]
public sealed class Post
{
   [Required, Key]
   public int Id { get; set; }

   /// <summary>
   /// The URL-friendly text.
   /// </summary>
   [StringLength(DataConstants.MaxSlugLength)]
   public required string Slug { get; set; }

   /// <summary>
   /// Title of the post
   /// </summary>
   [StringLength(DataConstants.MaxLabelLength)]
   public required string Title { get; set; }

   /// <summary>
   /// Optional subtitle.
   /// </summary>
   [StringLength(DataConstants.MaxLabelLength)]
   public string? Subtitle { get; set; }

   [StringLength(DataConstants.MaxMetadataLength)]
   public string? Summary { get; set; }

   [StringLength(DataConstants.MaxMetadataLength)]
   public string? MetaDescription { get; set; }

   /// <summary>
   /// The status of the post.
   /// Instead of having it be a foreign key, we'll just map it to an enum
   /// in code.
   /// </summary>
   public PostStatus Status { get; set; } = PostStatus.Draft;

   public bool IsFeatured { get; set; } = false;

   /// <summary>
   /// When the post was first created.
   /// </summary>
   public required DateTime CreatedDateUtc { get; set; }

   /// <summary>
   /// When the post was last updated. This is for the post as a whole. The
   /// post revisions also contain a date.
   /// </summary>
   public required DateTime LastUpdatedDateUtc { get; set; }

   /// <summary>
   /// Optional: When the post was published.
   /// </summary>
   public DateTime? PublishedDateUtc { get; set; }

   /// <summary>
   /// Maps to db_PostRevisions to reference the latest revision of this 
   /// post.
   /// </summary>
   public int? CurrentRevisionId { get; set; }

   /// <summary>
   /// The latest revision of the post content.
   /// </summary>
   [ForeignKey(nameof(CurrentRevisionId))]
   public PostRevision? CurrentRevision { get; set; }

   /// <summary>
   /// All revisions. The list can be empty but not null.
   /// </summary>
   public ICollection<PostRevision> Revisions { get; } =
      new List<PostRevision>();

   /// <summary>
   /// Tags for the post. The list can be empty but not null.
   /// </summary>
   public ICollection<PostTag> Tags { get; } =
      new List<PostTag>();

   /// <summary>
   /// Links to the assets attached to the post.
   /// </summary>
   public ICollection<SiteAssetLink> AssetLinks { get; } =
      new List<SiteAssetLink>();
}
