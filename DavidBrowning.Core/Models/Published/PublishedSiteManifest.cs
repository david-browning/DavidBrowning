// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using System.Diagnostics.CodeAnalysis;

namespace DavidBrowning.Models.Published;

public sealed class PublishedSiteManifest
{
   public int SchemaVersion { get; set; } = 1;

   public required string Version { get; set; }

   public required string SnapshotKey { get; set; }

   public required DateTimeOffset PublishedAtUtc { get; set; }

   public PublishedSiteManifest()
   {

   }

   [SetsRequiredMembers]
   public PublishedSiteManifest(
      PublishedSiteSnapshot snapshot,
      string snapshotKey)
   {
      ArgumentNullException.ThrowIfNull(snapshot);
      ArgumentException.ThrowIfNullOrWhiteSpace(snapshotKey);

      SchemaVersion = snapshot.SchemaVersion;
      Version = snapshot.Version;
      SnapshotKey = snapshotKey;
      PublishedAtUtc = snapshot.PublishedAtUtc;
   }
}
