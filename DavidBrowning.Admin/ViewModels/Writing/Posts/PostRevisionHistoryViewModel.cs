// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using DavidBrowning.Models;
using DavidBrowning.Models.Writing;

namespace DavidBrowning.Admin.ViewModels.Writing.Posts;

public sealed class PostRevisionHistoryViewModel
{
   public required int PostId { get; set; }

   public int? CurrentRevisionId { get; set; }

   public int? SelectedRevisionId { get; set; }

   public IReadOnlyList<PostRevisionListItemViewModel> Items
   {
      get;
      set;
   } = Array.Empty<PostRevisionListItemViewModel>();
}

public sealed class PostRevisionListItemViewModel
{
   public int Id { get; set; }

   public int RevisionNumber { get; set; }

   public ContentFormat ContentFormat { get; set; }

   public required string CreatedBy { get; set; }

   public DateTime CreatedAtUtc { get; set; }

   public bool IsCurrentRevision { get; set; }

   public bool IsSelectedRevision { get; set; }

   public PostRevisionListItemViewModel()
   {

   }

   [SetsRequiredMembers]
   public PostRevisionListItemViewModel(
      PostRevision revision,
      int? currentRevisionId,
      int? selectedRevisionId)
   {
      Id = revision.Id;
      RevisionNumber = revision.RevisionNumber;
      ContentFormat = revision.ContentFormat;
      CreatedBy = revision.CreatedBy;
      CreatedAtUtc = revision.CreatedAtUtc;
      IsCurrentRevision = revision.Id == currentRevisionId;
      IsSelectedRevision = revision.Id == selectedRevisionId;
   }
}