// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using DavidBrowning.Models;
using DavidBrowning.Services.Cache.Estimators;
using DavidBrowning.Services.Cache.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DavidBrowning.Services.Cache;

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
