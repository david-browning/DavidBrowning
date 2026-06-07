// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Text.Json.Serialization;

namespace DavidBrowning.Infrastructure.Seo;
public sealed class SitemapEntry
{
   [JsonPropertyName("Path")]
   public required string RelativePath { get; init; }

   [JsonPropertyName("Modified")]
   public DateTimeOffset? LastModifiedUtc { get; init; } = null;
}
