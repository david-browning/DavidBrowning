// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;

namespace DavidBrowning.Services.Cache.Options;

public sealed class SlugCacheOptions : ICacheOptions
{
   public TimeSpan CacheDuration { get; set; }
   public TimeSpan CacheTimeout { get; set; }
   public long ObjectCacheSize { get; set; }
   public bool TrackCacheStatistics { get; set; }
}
