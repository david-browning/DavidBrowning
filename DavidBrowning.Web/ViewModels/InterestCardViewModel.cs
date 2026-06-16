// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Diagnostics.CodeAnalysis;
using DavidBrowning.Models;
using DavidBrowning.Models.Writing;

namespace DavidBrowning.Web.ViewModels;

public sealed class InterestCardViewModel
{
   [SetsRequiredMembers]
   public InterestCardViewModel(Interest interest)
   {
      Summary = interest.Summary;
      Title = interest.DisplayName;
      IconCssClass = interest.IconCssClass;
      if (interest.FeaturedPost is not null &&
         interest.FeaturedPost.Status == PostStatus.Published)
      {
         FeaturedPost = new FeaturedPostLinkViewModel(
            title: interest.FeaturedPost.Title,
            slug: interest.FeaturedPost.Slug,
            summary: interest.FeaturedPost.Summary);
      }
   }

   public required string Title { get; init; }

   public required string Summary { get; init; }

   public string? IconCssClass { get; init; }

   public RenderedContent? Image { get; init; }

   public FeaturedPostLinkViewModel? FeaturedPost { get; init; }
}
