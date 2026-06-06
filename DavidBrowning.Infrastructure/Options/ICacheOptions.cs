// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
namespace DavidBrowning.Infrastructure.Options;

/// <summary>
/// Options for caching data.
/// </summary>
public interface ICacheOptions
{
   TimeSpan CacheDuration { get; set; }

   TimeSpan CacheTimeout { get; set; }

   long ObjectCacheSize { get; set; }

   bool TrackCacheStatistics { get; set; }
}