// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using DavidBrowning.Infrastructure.Assets;

namespace DavidBrowning.Infrastructure.Publishing;

public sealed class PublicSitePublicationOptions
{
   public string ManifestKey { get; set; } =
      "published/current.json";

   public string SnapshotPrefix { get; set; } =
      "published/snapshots";
}

public sealed record PublishResult
{
   public required string Version { get; init; }

   public required string SnapshotKey { get; init; }

   public required DateTimeOffset PublishedAtUtc { get; init; }

   public required ContentWriteResults SnapshotWriteResult { get; init; }

   public required ContentWriteResults ManifestWriteResult { get; init; }
}