// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using DavidBrowning.Services.Cache.Estimators;
using DavidBrowning.Services.Cache.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DavidBrowning.Services.Cache;

public sealed class JsonMemoryCache : AsyncMemoryCache<object?>
{
   public JsonMemoryCache(
      ILogger<AsyncMemoryCache<object?>> logger,
      IOptions<JsonCacheOptions> options,
      ICacheSizeEstimator<object?> sizeEstimator)
      : base(logger, options.Value, sizeEstimator)
   {
   }
}
