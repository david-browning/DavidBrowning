// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Text.Json.Nodes;
using DavidBrowning.Infrastructure.Options;
using DavidBrowning.Models.Projects;
using DavidBrowning.Models.Writing;
using Microsoft.Extensions.Options;

namespace DavidBrowning.Infrastructure;

/// <summary>
/// Used to build JsonObjects that get emitted in HTML for Seo.
/// </summary>
public class JsonDataBuilder
{
   public JsonDataBuilder(
      IOptions<SiteMetadataOptions> metadataOptions,
      UrlBuilder urlBuilder)
   {
      _metadataOptions = metadataOptions.Value;
      _urlBuilder = urlBuilder;
   }

   public JsonObject CreateWritingPostMetadata(Post post)
   {
      var result = new JsonObject
      {
         ["@context"] = "https://schema.org",
         ["@type"] = "BlogPosting",
         ["headline"] = post.Title,
         ["description"] = post.Summary,
         ["url"] = _urlBuilder.GetAbsoluteUrl($"/writing/{post.Slug}"),
         ["datePublished"] = post.PublishedDateUtc?.ToString("O") ??
            post.CreatedDateUtc.ToString("O"),
         ["dateModified"] = post.LastUpdatedDateUtc.ToString("O"),
         ["author"] = CreateAuthor(),
      };

      //if (!string.IsNullOrWhiteSpace(imageUrl))
      //{
      //   result["image"] = imageUrl;
      //}

      return result;
   }

   public JsonObject CreateProjectMetadata(Project project)
   {
      return null;
   }

   private JsonObject CreateAuthor()
   {
      return new JsonObject
      {
         ["@type"] = "Person",
         ["name"] = _metadataOptions.AuthorName,
         ["url"] = _urlBuilder.GetAbsoluteUrl("/about")
      };
   }

   private readonly UrlBuilder _urlBuilder;
   private readonly SiteMetadataOptions _metadataOptions;
}
