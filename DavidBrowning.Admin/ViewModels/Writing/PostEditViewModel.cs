// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using DavidBrowning.Models;
using DavidBrowning.Models.Writing;

namespace DavidBrowning.Admin.ViewModels.Writing;

public class PostEditViewModel
{
   public required EditModes EditMode { get; init; }

   [Required, Key]
   public int? Id { get; set; }

   /// <summary>
   /// The URL-friendly text.
   /// </summary>
   [Required]
   [StringLength(DataConstants.MaxSlugLength)]
   [RegularExpression(DataConstants.SlugRegex,
      ErrorMessage = DataConstants.SlugRegexError)]
   public string? Slug { get; set; }

   /// <summary>
   /// Title of the post
   /// </summary>
   [Required]
   [StringLength(DataConstants.MaxLabelLength)]
   public string? Title { get; set; }

   /// <summary>
   /// Optional subtitle.
   /// </summary>
   [StringLength(DataConstants.MaxLabelLength)]
   public string? Subtitle { get; set; }

   [StringLength(DataConstants.MaxMetadataLength)]
   public string? Summary { get; set; }

   /// <summary>
   /// Foreign key to db_PostStyles.
   /// Controls how the post should be presented.
   /// </summary>
   [Required]
   public int? PostStyleId { get; set; }

   /// <summary>
   /// The status of the post.
   /// Instead of having it be a foreign key, we'll just map it to an enum
   /// in code.
   /// </summary>
   [Required]
   public PostStatus Status { get; set; } = PostStatus.Draft;

   public bool IsFeatured { get; set; } = false;

   /// <summary>
   /// When the post was first created.
   /// </summary>
   [Required]
   public DateTime CreatedDateUtc { get; set; } = DateTime.UtcNow;

   /// <summary>
   /// When the post was last updated. This is for the post as a whole. The
   /// post revisions also contain a date.
   /// </summary>
   [Required]
   public DateTime LastUpdatedDateUtc { get; set; } = DateTime.UtcNow;

   /// <summary>
   /// Optional: When the post was published.
   /// </summary>
   public DateTime? PublishedDateUtc { get; set; }

   /// <summary>
   /// Maps to db_PostRevisions to reference the latest revision of this 
   /// post.
   /// </summary>
   public int? CurrentRevisionId { get; set; }



   public PostEditViewModel()
   {

   }

   [SetsRequiredMembers]
   public PostEditViewModel(Post post)
   {
      EditMode = EditModes.Edit;
      Id = post.Id;
      Slug = post.Slug;
      Title = post.Title;
      Subtitle = post.Subtitle;
      Summary = post.Summary;
      PostStyleId = post.PostStyleId;
      Status = post.Status;
      IsFeatured = post.IsFeatured;
      CreatedDateUtc = post.CreatedDateUtc;
      LastUpdatedDateUtc   = post.LastUpdatedDateUtc;
      PublishedDateUtc = post.PublishedDateUtc;
      CurrentRevisionId = post.CurrentRevisionId;
   }
}
