// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace DavidBrowning.Services.Cache;

/// <summary>
/// Basic implementation of an ILookupCache.
/// </summary>
public class BasicLookupCache : ILookupCache
{
   public BasicLookupCache(
      IMemoryCache memoryCache,
      IOptions<LookupCacheOptions> options)
   {
      _memoryCache = memoryCache;
      _options = options.Value;
   }

   public async Task<TValue?> GetOrCreateAsync<TValue>(
      string cacheKey,
      Func<CancellationToken, Task<TValue?>> valueFactory,
      CancellationToken cancellationToken = default)
   {
      if (_memoryCache.TryGetValue(cacheKey, out TValue? cachedValue))
      {
         return cachedValue;
      }

      TValue? value = await valueFactory(cancellationToken);
      MemoryCacheEntryOptions cacheOptions = new()
      {
         AbsoluteExpirationRelativeToNow = _options.CacheDuration
      };

      _memoryCache.Set(cacheKey, value, cacheOptions);
      return value;
   }

   public void Remove(string cacheKey)
   {
      _memoryCache.Remove(cacheKey);
   }

   public void Clear()
   {
      // If the ASP provided implementation of the memory cache is a 
      // "MemoryCache", then use the Clear() function.
      if (_memoryCache is MemoryCache concreteMemoryCache)
      {
         concreteMemoryCache.Clear();
      }

      // Else the generic interface does not support querying keys or 
      // clearing.
   }

   private readonly IMemoryCache _memoryCache;
   private readonly LookupCacheOptions _options;
}
