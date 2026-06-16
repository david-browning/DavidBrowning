// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
namespace DavidBrowning.Web.ViewModels;

public sealed class FeaturedPostLinkViewModel
{
   public FeaturedPostLinkViewModel(
      string title,
      string slug,
      string? summary)
   {
      Title = title;
      Slug = slug;
      Summary = summary;
   }

   public string Title { get; }

   public string Slug { get; }

   public string? Summary { get; }
}