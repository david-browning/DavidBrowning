// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DavidBrowning.Models.Writing;

namespace DavidBrowning.Models.ViewModels.Writing;

public class DetailsViewModel
{
   [SetsRequiredMembers]
   public DetailsViewModel(Post post)
   {
      Title = post.Title;
      Subtitle = post.Subtitle;
      Summary = post.Summary;
      MetaDescription = post.MetaDescription;
      PostStyle = post.PostStyle;
      PublishedDateUtc = post.PublishedDateUtc;
      CurrentRevisionId = post.CurrentRevisionId;
      ContentFormat = post.CurrentRevision!.ContentFormat;
      RenderMode = post.CurrentRevision!.RenderMode;
      CreatedBy = post.CurrentRevision!.CreatedBy;
      Content = post.CurrentRevision!.Content;
      TagLinks = post.Tags;
      AssetBlocks = post.AssetLinks
         .OrderBy(link => link.SortOrder)
         .Select(link => new AssetBlockViewModel(link))
         .ToList();
   }

   public required string Title { get; set; }

   public string? Subtitle { get; set; }

   public string? Summary { get; set; }

   public string? MetaDescription { get; set; }

   public required PostStyles PostStyle { get; set; }

   public DateTime? PublishedDateUtc { get; set; }


   public int? CurrentRevisionId { get; set; }
   
   public ContentFormat? ContentFormat { get; set; }
   
   public RenderMode? RenderMode { get; set; }

   public string? CreatedBy { get; set; }

   public string? Content { get; set; }


   public required ICollection<PostTag> TagLinks { get; init; }

   public required IReadOnlyList<AssetBlockViewModel> AssetBlocks { get; init; }
}
