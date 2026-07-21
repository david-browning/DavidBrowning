// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

namespace DavidBrowning.Models.Published;

public sealed record PublishedSiteManifest
{
   public int SchemaVersion { get; init; } = 1;

   public required string Version { get; init; }

   public required string SnapshotKey { get; init; }

   public required DateTimeOffset PublishedAtUtc { get; init; }
}