// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using System.Diagnostics.CodeAnalysis;
using DavidBrowning.Models.Writing;

namespace DavidBrowning.Models.Published;

public sealed class PublishedWriting
{
   public required string Slug { get; set; }

   public required string Title { get; set; }

   public string? Subtitle { get; set; }

   public string? Summary { get; set; }

   public required PublishedLookup PostStyle { get; set; }

   public bool IsFeatured { get; set; }

   public DateTime? PublishedDateUtc { get; set; }

   public DateTime LastUpdatedDateUtc { get; set; }

   public IReadOnlyList<PublishedPostTag> Tags { get; set; } =
      Array.Empty<PublishedPostTag>();

   public required PublishedTextContent CurrentRevision { get; set; }

   public PublishedWriting()
   {

   }

   [SetsRequiredMembers]
   public PublishedWriting(Post post)
   {
      ArgumentNullException.ThrowIfNull(post);

      Slug = post.Slug;
      Title = post.Title;
      Subtitle = post.Subtitle;
      Summary = post.Summary;
      PostStyle = new PublishedLookup(
         post.PostStyle ?? throw new InvalidOperationException(
            $"Writing '{post.Slug}' is missing its post style."));
      IsFeatured = post.IsFeatured;
      PublishedDateUtc = post.PublishedDateUtc;
      LastUpdatedDateUtc = post.LastUpdatedDateUtc;
      Tags = post.Tags
         .Where(tag => tag.WritingTag is not null)
         .OrderBy(tag => tag.WritingTag!.DisplayName)
         .Select(tag => new PublishedPostTag(tag))
         .ToArray();
      CurrentRevision = new PublishedTextContent(
         post.CurrentRevision ?? throw new InvalidOperationException(
            $"Writing '{post.Slug}' does not have a current revision."));
   }
}
