// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using DavidBrowning.Models;
using DavidBrowning.Models.Writing;

namespace DavidBrowning.Admin.ViewModels.Writing.Posts;

public class PostRevisionContentViewModel
{
   public required int PostId { get; set; }

   public int? Id { get; set; }

   public int? RevisionNumber { get; set; }

   public  ContentFormat? ContentFormat { get; set; }

   [StringLength(DataConstants.MaxNameLength)]
   public string? CreatedBy { get; set; }

   public string? Content { get; set; }

   public EditModes EditMode { get; set; } = EditModes.Create;

   public DateTime? CreatedAtUtc { get; set; }

   public bool IsCurrentRevision { get; set; }

   public PostRevisionContentViewModel()
   {

   }

   [SetsRequiredMembers]
   public PostRevisionContentViewModel(
      PostRevision revision,
      int? currentRevisionId)
   {
      EditMode = EditModes.Edit;
      PostId = revision.PostId;
      Id = revision.Id;
      RevisionNumber = revision.RevisionNumber;
      CreatedBy = revision.CreatedBy;
      CreatedAtUtc = revision.CreatedAtUtc;
      ContentFormat = revision.ContentFormat;
      Content = revision.Content;
      IsCurrentRevision = revision.Id == currentRevisionId;
   }

   public PostRevision ToRevision()
   {
      ArgumentNullException.ThrowIfNull(ContentFormat);
      ArgumentNullException.ThrowIfNullOrEmpty(CreatedBy);

      return new PostRevision()
      {
         Id = Id ?? 0,
         PostId = PostId,
         ContentFormat = ContentFormat.Value,
         CreatedBy = CreatedBy,
         Content = Content,
      };
   }
}
