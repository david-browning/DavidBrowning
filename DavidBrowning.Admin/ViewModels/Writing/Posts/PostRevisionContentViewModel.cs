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

public class PostRevisionContentViewModel
{
   [Range(1, int.MaxValue)]
   public required int PostId { get; set; }

   [Required]
   public ContentFormat? ContentFormat { get; set; }

   public string? Content { get; set; }

   public EditModes EditMode { get; set; } = EditModes.Create;

   [BindNever]
   public int? Id { get; set; }

   [BindNever]
   public int? RevisionNumber { get; set; }

   [BindNever]
   public string? CreatedBy { get; set; }

   [BindNever]
   public DateTime? CreatedAtUtc { get; set; }

   [BindNever]
   public bool IsCurrentRevision { get; set; }

   public List<AssetLinkInputViewModel> AssetLinks { get; set; } = new();

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

      AssetLinks = revision.AssetLinks
         .Select(link => new AssetLinkInputViewModel()
         {
            SiteAssetId = link.SiteAssetId,
            ReferenceKey = link.ReferenceKey,
            Caption = link.Caption,
            AltTextOverride = link.AltTextOverride,
         })
         .ToList();
   }
}
