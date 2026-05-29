// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using DavidBrowning.Models;
using DavidBrowning.Services.Cache.Estimators;
using DavidBrowning.Services.Cache.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DavidBrowning.Services.Cache;

public sealed class SlugMemoryCache<TLookup> : AsyncMemoryCache<TLookup>
   where TLookup : class, IQueryableSlug
{
   public SlugMemoryCache(
      ILogger<AsyncMemoryCache<TLookup>> logger,
      IOptions<SlugCacheOptions> options,
      ICacheSizeEstimator<TLookup?> sizeEstimator)
      : base(logger, options.Value, sizeEstimator)
   {
   }
}
