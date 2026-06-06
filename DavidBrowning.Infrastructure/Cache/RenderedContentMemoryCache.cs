// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using DavidBrowning.Infrastructure.Cache.Estimators;
using DavidBrowning.Infrastructure.Options;
using DavidBrowning.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DavidBrowning.Infrastructure.Cache;

public sealed class RenderedContentMemoryCache : AsyncMemoryCache<RenderedContent>
{
   public RenderedContentMemoryCache(
      ILogger<AsyncMemoryCache<RenderedContent>> logger,
      IOptions<RenderedContentCacheOptions> options,
      ICacheSizeEstimator<RenderedContent> sizeEstimator)
      : base(logger, options.Value, sizeEstimator)
   {
   }
}
