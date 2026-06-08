// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using DavidBrowning.Models;
using DavidBrowning.Models.Writing;

namespace DavidBrowning.Admin.ViewModels.Writing;

public class PostRevisionEditViewModel
{
   public required EditModes EditMode { get; init; }

   [Required, Key]
   public int? Id { get; set; }

   /// <summary>
   /// Id of the post this revision is attached to.
   /// </summary>
   [Required]
   public int? PostId { get; set; }

   /// <summary>
   /// The logical revision number. Not the same as the database Id.
   /// Ex: 1, 2, 3rd revision.
   /// </summary>
   [Required]
   public required int RevisionNumber { get; set; }

   /// <summary>
   /// What kind of content is stored in "Content"
   /// Instead of having it be a foreign key, we'll just map it to an enum
   /// in code.
   /// </summary>
   [Required]
   public ContentFormat? ContentFormat { get; set; }

   [Required]
   [StringLength(DataConstants.MaxNameLength)]
   public string? CreatedBy { get; set; }

   [Required]
   public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

   /// <summary>
   /// A revision may or may not have text.
   /// </summary>
   public string? Content { get; set; }

   public PostRevisionEditViewModel()
   {

   }

   [SetsRequiredMembers]
   public PostRevisionEditViewModel(PostRevision revision)
   {
      EditMode = EditModes.Edit;
      Id = revision.Id;
      PostId = revision.PostId;
      RevisionNumber = revision.RevisionNumber;
      ContentFormat = revision.ContentFormat;
      CreatedAtUtc = revision.CreatedAtUtc;
      CreatedBy = revision.CreatedBy;
      Content = revision.Content;
   }
}
