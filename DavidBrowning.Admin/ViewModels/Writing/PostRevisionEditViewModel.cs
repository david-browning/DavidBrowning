// Copyright © 2026 David Browning. All rights reserved.
//
// Source-available for viewing only. No license granted.

using System;
using System.ComponentModel.DataAnnotations;

using DavidBrowning.Models;
using DavidBrowning.Models.Writing;

namespace DavidBrowning.Admin.ViewModels.Writing;

public sealed class PostRevisionEditViewModel
{
   public EditModes EditMode { get; set; } = EditModes.Create;

   public int? Id { get; set; }

   [Required]
   [Range(1, int.MaxValue)]
   public int? PostId { get; set; }

   [Range(1, int.MaxValue)]
   public int RevisionNumber { get; set; }

   [Required]
   [EnumDataType(typeof(ContentFormat))]
   public ContentFormat? ContentFormat { get; set; }

   [Required]
   [StringLength(DataConstants.MaxNameLength)]
   public string? CreatedBy { get; set; }

   public string? Content { get; set; }

   public PostRevisionEditViewModel()
   {
   }

   public PostRevisionEditViewModel(PostRevision revision)
   {
      EditMode = EditModes.Edit;
      Id = revision.Id;
      PostId = revision.PostId;
      RevisionNumber = revision.RevisionNumber;
      ContentFormat = revision.ContentFormat;
      CreatedBy = revision.CreatedBy;
      Content = revision.Content;
   }
}
