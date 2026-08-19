// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

namespace DavidBrowning.Infrastructure.Publishing;

public interface IPublicSitePublisher
{
   Task<PublishResult> PublishAsync(
      CancellationToken cancellationToken = default);
}