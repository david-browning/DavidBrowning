// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using DavidBrowning.Models.Publishing;

namespace DavidBrowning.Infrastructure.Publishing;

public interface IPublicSiteSnapshotBuilder
{
   Task<PublishedSiteSnapshot> BuildAsync(
      string version,
      DateTimeOffset publishedAtUtc,
      CancellationToken cancellationToken = default);
}