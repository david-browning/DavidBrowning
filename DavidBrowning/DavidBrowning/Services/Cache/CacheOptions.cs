// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;

namespace DavidBrowning.Services.Cache;

/// <summary>
/// Options for caching data.
/// </summary>
public sealed class CacheOptions
{
   //public bool EnableContentCache { get; set; } = false;
   public TimeSpan LookupCacheDuration { get; set; } = TimeSpan.FromMinutes(10);
   public TimeSpan ContentCacheDuration { get; set; } = TimeSpan.FromMinutes(10);
   public TimeSpan ContentCacheTimeout { get; set; } = TimeSpan.FromMinutes(20);
   
   // Store up to 32MB of objects.
   public long ObjectCacheSize { get; set; } = 1024 * 1024 * 32;

   public bool TrackCacheStatistics { get; set; } = false;
}
