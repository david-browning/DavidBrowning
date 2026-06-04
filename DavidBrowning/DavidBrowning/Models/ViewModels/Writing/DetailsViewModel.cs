// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using DavidBrowning.Models.Writing;

namespace DavidBrowning.Models.ViewModels.Writing;

public class DetailsViewModel
{
   [SetsRequiredMembers]
   public DetailsViewModel(Post post, RenderedContent body)
   {
      Title = post.Title;
      Subtitle = post.Subtitle;
      Summary = post.Summary;
      MetaDescription = post.MetaDescription;
      PostStyleDisplayName = post.PostStyle?.DisplayName ??
         throw new InvalidOperationException(
            "Post is missing its post style.");

      PublishedDateUtc = post.PublishedDateUtc;

      var revision = post.CurrentRevision ??
         throw new InvalidOperationException(
            "Post is missing its current revision.");

      CurrentRevisionId = revision.Id;
      ContentFormat = revision.ContentFormat;
      CreatedBy = revision.CreatedBy;
      Body = body;

      TagLinks = post.Tags;
   }

   public required string Title { get; set; }

   public string? Subtitle { get; set; }

   public string? Summary { get; set; }

   public string? MetaDescription { get; set; }

   public required string PostStyleDisplayName { get; set; }

   public DateTimeOffset? PublishedDateUtc { get; set; }


   public int? CurrentRevisionId { get; set; }

   public ContentFormat? ContentFormat { get; set; }

   public string? CreatedBy { get; set; }

   public required RenderedContent Body { get; set; }

   public required ICollection<PostTag> TagLinks { get; init; }
}
