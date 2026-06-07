// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Text.Json.Nodes;

namespace DavidBrowning.Web.ViewModels;

public class SeoMetadataViewModel
{
   public required string Title { get; init; }

   public required string Description { get; init; }

   /// <summary>
   /// Absolute public URL for the authoritative version of the page.
   /// </summary>
   public required string CanonicalUrl { get; init; }

   /// <summary>
   /// Optional absolute URL for social previews and structured data.
   /// </summary>
   public string? PreviewImageUrl { get; init; }

   /// <summary>
   /// Use "article" for writing-detail pages and "website" elsewhere.
   /// </summary>
   public string OpenGraphType { get; init; } =
      DavidBrowning.Models.OpenGraphTypes.Website;

   /// <summary>
   /// Use for drafts, preview pages, internal search pages, and other pages
   /// that should remain accessible but should not appear in search results.
   /// </summary>
   public bool NoIndex { get; init; } = false;

   /// <summary>
   /// Optional Json data that gets rendered into the <head> tag.
   /// </summary>
   public JsonObject? StructuredData { get; init; }
}
