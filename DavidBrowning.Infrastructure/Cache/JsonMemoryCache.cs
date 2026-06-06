// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using DavidBrowning.Infrastructure.Cache.Estimators;
using DavidBrowning.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DavidBrowning.Infrastructure.Cache;

public sealed class JsonMemoryCache : AsyncMemoryCache<object>
{
   public JsonMemoryCache(
      ILogger<AsyncMemoryCache<object>> logger,
      IOptions<JsonCacheOptions> options,
      ICacheSizeEstimator<object> sizeEstimator)
      : base(logger, options.Value, sizeEstimator)
   {
   }
}
