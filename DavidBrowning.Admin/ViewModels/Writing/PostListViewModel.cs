// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using DavidBrowning.Models.Writing;

namespace DavidBrowning.Admin.ViewModels.Writing;

public sealed class PostListViewModel
{
   public IReadOnlyList<PostListItemViewModel> Items { get; set; } =
      Array.Empty<PostListItemViewModel>();
}

public sealed class PostListItemViewModel
{
   public int Id { get; set; }

   public required string Slug { get; set; }

   public required string Title { get; set; }

   public string? Subtitle { get; set; }

   public required string StyleDisplayName { get; set; }

   public PostStatus Status { get; set; }

   public bool IsFeatured { get; set; }

   public DateTime? PublishedDateUtc { get; set; }

   public DateTime LastUpdatedDateUtc { get; set; }

   public int RevisionCount { get; set; }

   public int TagCount { get; set; }

   public PostListItemViewModel()
   {

   }

   [SetsRequiredMembers]
   public PostListItemViewModel(Post post)
   {
      Id = post.Id;
      IsFeatured = post.IsFeatured;
      Slug = post.Slug;
      Subtitle = post.Subtitle;
      Title = post.Title;
      Status = post.Status;
      LastUpdatedDateUtc = post.LastUpdatedDateUtc;
      PublishedDateUtc = post.PublishedDateUtc;
      RevisionCount = post.Revisions.Count;
      TagCount = post.Tags.Count;
      StyleDisplayName = post.PostStyle!.DisplayName;
   }
}
