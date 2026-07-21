// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using System.Diagnostics.CodeAnalysis;
using DavidBrowning.Models;
using DavidBrowning.Models.Writing;

namespace DavidBrowning.Models.Published;

public sealed class PublishedInterest
{
   public required string Slug { get; set; }

   public required string DisplayName { get; set; }

   public required string Summary { get; set; }

   public string? IconCssClass { get; set; }

   public int SortOrder { get; set; }

   public PublishedFeaturedPostLink? FeaturedPost { get; set; }

   public PublishedInterest()
   {

   }

   [SetsRequiredMembers]
   public PublishedInterest(Interest interest)
   {
      ArgumentNullException.ThrowIfNull(interest);

      Slug = interest.Slug;
      DisplayName = interest.DisplayName;
      Summary = interest.Summary;
      IconCssClass = interest.IconCssClass;
      SortOrder = interest.SortOrder;

      if (interest.FeaturedPost?.Status == PostStatus.Published)
      {
         FeaturedPost = new PublishedFeaturedPostLink(interest.FeaturedPost);
      }
   }
}

public sealed class PublishedFeaturedPostLink
{
   public required string Slug { get; set; }

   public required string Title { get; set; }

   public string? Summary { get; set; }

   public PublishedFeaturedPostLink()
   {

   }

   [SetsRequiredMembers]
   public PublishedFeaturedPostLink(Post post)
   {
      ArgumentNullException.ThrowIfNull(post);

      Slug = post.Slug;
      Title = post.Title;
      Summary = post.Summary;
   }
}
