// Copyright © 2026 David Browning. All rights reserved.
//
// Source-available for viewing only. No license granted.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using DavidBrowning.Models;
using DavidBrowning.Models.Writing;

namespace DavidBrowning.Admin.ViewModels.Writing;

public sealed class PostEditViewModel
{
   public EditModes EditMode { get; set; } = EditModes.Create;

   public int? Id { get; set; }

   [Required]
   [StringLength(DataConstants.MaxSlugLength)]
   [RegularExpression(
      DataConstants.SlugRegex,
      ErrorMessage = DataConstants.SlugRegexError)]
   public string? Slug { get; set; }

   [Required]
   [StringLength(DataConstants.MaxLabelLength)]
   public string? Title { get; set; }

   [StringLength(DataConstants.MaxLabelLength)]
   public string? Subtitle { get; set; }

   [StringLength(DataConstants.MaxMetadataLength)]
   public string? Summary { get; set; }

   [Required]
   [Range(1, int.MaxValue)]
   public int? PostStyleId { get; set; }

   [EnumDataType(typeof(PostStatus))]
   public PostStatus Status { get; set; } = PostStatus.Draft;

   public bool IsFeatured { get; set; }

   public DateTime? PublishedDateUtc { get; set; }

   // Treat CurrentRevisionId as a controlled selection. The application
   // service must verify that the selected revision belongs to this post.
   public int? CurrentRevisionId { get; set; }

   // Simple many-to-many selections. Persist by diffing the submitted IDs
   // against Post.Tags.
   public ISet<int> WritingTagIds { get; set; } = new HashSet<int>();

   // Display-only metadata. Do not trust posted values when saving.
   public DateTime? CreatedDateUtc { get; init; }

   // Display-only metadata. Do not trust posted values when saving.
   public DateTime? LastUpdatedDateUtc { get; init; }

   public PostEditViewModel()
   {
   }

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
      PublishedDateUtc = post.PublishedDateUtc;
      CurrentRevisionId = post.CurrentRevisionId;
      WritingTagIds = post.Tags
         .Select(tag => tag.WritingTagId)
         .ToHashSet();
      CreatedDateUtc = post.CreatedDateUtc;
      LastUpdatedDateUtc = post.LastUpdatedDateUtc;
   }
}
