// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DavidBrowning.Models;
using DavidBrowning.Models.Writing;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DavidBrowning.Admin.ViewModels.Writing.Posts;

public class PostMetadataViewModel
{
   public EditModes EditMode { get; set; } = EditModes.Create;

   public int? Id { get; set; }

   [Required]
   [RegularExpression(DataConstants.SlugRegex,
      ErrorMessage = DataConstants.SlugRegexError)]
   [StringLength(DataConstants.MaxSlugLength)]
   public string? Slug { get; set; }

   [Required]
   [StringLength(DataConstants.MaxLabelLength)]
   public string? Title { get; set; }

   /// <summary>
   /// Optional subtitle.
   /// </summary>
   [StringLength(DataConstants.MaxLabelLength)]
   public string? Subtitle { get; set; }

   /// <summary>
   /// Optional summary
   /// </summary>
   [StringLength(DataConstants.MaxMetadataLength)]
   public string? Summary { get; set; }

   /// <summary>
   /// Foreign key to db_PostStyles.
   /// Controls how the post should be presented.
   /// </summary>
   [Required]
   public int? PostStyleId { get; set; }

   [Required]
   public PostStatus? Status { get; set; } = PostStatus.Draft;

   public bool IsFeatured { get; set; } = false;

   public DateTime? PublishedDateUtc { get; set; }

   public List<int> WritingTagIds { get; set; } = new List<int>();

   [BindNever]
   public required IReadOnlyList<PostStyleOptionViewModel> PostStyleOptions { get; set; }

   [BindNever]
   public required IReadOnlyList<WritingTagOptionViewModel> WritingTagOptions { get; set; }

   public PostMetadataViewModel()
   {

   }

   [SetsRequiredMembers]
   public PostMetadataViewModel(Post post,
      IReadOnlyList<PostStyleOptionViewModel> styles,
      IReadOnlyList<WritingTagOptionViewModel> tags)
   {
      EditMode = EditModes.Edit;
      Id = post.Id;
      Slug = post.Slug;
      Title = post.Title;
      Subtitle = post.Subtitle;
      Summary = post.Summary;
      PostStyleId = post.PostStyleId;
      Status = post.Status;
      PublishedDateUtc = post.PublishedDateUtc;
      IsFeatured = post.IsFeatured;
      PostStyleOptions = styles;
      WritingTagOptions = tags;
      WritingTagIds = post.Tags.Select(tag => tag.WritingTagId).ToList();
   }
}
