// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;

namespace DavidBrowning.Services.Cache
{
   /// <summary>
   /// Options for caching data.
   /// </summary>
   public sealed class LookupCacheOptions
   {
      public TimeSpan CacheDuration { get; set; } = TimeSpan.FromMinutes(10);
   }
}
