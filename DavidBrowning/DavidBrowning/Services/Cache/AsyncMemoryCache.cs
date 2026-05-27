// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Services.Cache.Estimators;
using DavidBrowning.Services.Cache.Options;
using Microsoft.Extensions.Caching.Memory;

namespace DavidBrowning.Services.Cache;

public class AsyncMemoryCache<T> : IDisposable
{
   public AsyncMemoryCache(
      ICacheOptions cacheOptions,
      ICacheSizeEstimator<T?> sizeEstimator)
   {
      _cacheOptions = cacheOptions;
      _sizeEstimator = sizeEstimator;
      _memoryCache = new MemoryCache(new MemoryCacheOptions()
      {
         SizeLimit = _cacheOptions.ObjectCacheSize,
         TrackStatistics = _cacheOptions.TrackCacheStatistics,
      });
   }

   public async Task<T?> GetOrCreateAsync(
      string cacheKey,
      Func<CancellationToken, Task<T>> factory,
      CancellationToken cancellationToken = default)
   {
      if (_memoryCache.TryGetValue(cacheKey, out T? cachedValue))
      {
         return cachedValue;
      }

      var cacheLock = _cacheLocks.GetOrAdd(
         cacheKey, _ => new SemaphoreSlim(1, 1));
      await cacheLock.WaitAsync(cancellationToken);

      try
      {
         if (_memoryCache.TryGetValue(cacheKey, out T? cachedValue2))
         {
            return cachedValue2;
         }

         var fetchedValue = await factory(cancellationToken);
         var size = _sizeEstimator.EstimateSize(fetchedValue);
         MemoryCacheEntryOptions options = new()
         {
            Size = size,
            SlidingExpiration = _cacheOptions.CacheTimeout,
            AbsoluteExpirationRelativeToNow =
               _cacheOptions.CacheDuration,
         };

         _memoryCache.Set(cacheKey, fetchedValue, options);
         return fetchedValue;
      }
      finally
      {
         cacheLock.Release();
         // The dictionary could grow unbounded. We just keep adding semaphores
         // whenever a new piece of content is requested. So one option is to 
         // remove the semaphore from the dictionary once all but one request is
         // using it.
         // Only problem is that this operation is not atomic.
         //if (cacheLock.CurrentCount == 1)
         //{
         //   _cacheLocks.TryRemove(
         //      new KeyValuePair<string, SemaphoreSlim>(cacheKey, cacheLock));
         //}

         // TODO: Find a way to make removing items from the dictionary AND 
         // checking if its the last one atomic.
      }
   }

   public void Dispose()
   {
      _memoryCache.Dispose();
   }

   private readonly ICacheSizeEstimator<T?> _sizeEstimator;
   private readonly ICacheOptions _cacheOptions;
   private readonly IMemoryCache _memoryCache;

   // Map each cache key to a semaphore.
   // When that content is requested and the cache does not contain the asset,
   // lock the semaphore and cache the asset. Release the semaphore after 
   // caching and return the asset.
   private readonly ConcurrentDictionary<string, SemaphoreSlim> _cacheLocks =
      new();
}
