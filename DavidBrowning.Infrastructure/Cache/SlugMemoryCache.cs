// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using DavidBrowning.Infrastructure.Cache.Estimators;
using DavidBrowning.Infrastructure.Options;
using DavidBrowning.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DavidBrowning.Infrastructure.Cache;

public sealed class SlugMemoryCache<TLookup> : AsyncMemoryCache<TLookup>
   where TLookup : class, IQueryableSlug
{
   public SlugMemoryCache(
      ILogger<AsyncMemoryCache<TLookup>> logger,
      IOptions<SlugCacheOptions> options,
      ICacheSizeEstimator<TLookup> sizeEstimator)
      : base(logger, options.Value, sizeEstimator)
   {
   }
}
