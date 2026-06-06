// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Services.Cache.Estimators;
using DavidBrowning.Services.Cache.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace DavidBrowning.Services.Cache;

public class AsyncMemoryCache<T> : IDisposable
   where T : class
{
   internal int LockCount => _cacheLocks.Count;

   public AsyncMemoryCache(
      ILogger<AsyncMemoryCache<T>> logger,
      ICacheOptions cacheOptions,
      ICacheSizeEstimator<T> sizeEstimator)
   {
      _logger = logger;
      _cacheOptions = cacheOptions;
      _sizeEstimator = sizeEstimator;

      _memoryCache = new MemoryCache(
         new MemoryCacheOptions()
         {
            SizeLimit = _cacheOptions.ObjectCacheSize,
            TrackStatistics = _cacheOptions.TrackCacheStatistics,
         });
   }

   /// <summary>
   /// Gets a cached value or creates one.
   /// A null factory result violates the method contract.
   /// </summary>
   public async Task<T> GetOrCreateAsync(
      string cacheKey,
      Func<CancellationToken, Task<T>> factory,
      CancellationToken cancellationToken = default)
   {
      var value = await GetOrCreateCoreAsync(
         cacheKey,
         token => InvokeFactoryAsync(factory, token),
         cancellationToken);

      return value ??
         throw new InvalidOperationException(
            $"Cache factory returned null for '{cacheKey}'.");
   }

   /// <summary>
   /// Gets a cached value or attempts to create one.
   /// A null factory result is returned but is not cached.
   /// </summary>
   public Task<T?> TryGetOrCreateAsync(
      string cacheKey,
      Func<CancellationToken, Task<T?>> factory,
      CancellationToken cancellationToken = default)
   {
      return GetOrCreateCoreAsync(
         cacheKey,
         factory,
         cancellationToken);
   }

   private async Task<T?> GetOrCreateCoreAsync(
      string cacheKey,
      Func<CancellationToken, Task<T?>> factory,
      CancellationToken cancellationToken)
   {
      if (_memoryCache.TryGetValue(cacheKey, out T? cachedValue) &&
          cachedValue is not null)
      {
         return cachedValue;
      }

      var cacheLock = AcquireCacheLock(cacheKey);
      var enteredSemaphore = false;

      try
      {
         await cacheLock.Semaphore.WaitAsync(cancellationToken);
         enteredSemaphore = true;

         if (_memoryCache.TryGetValue(cacheKey, out cachedValue) &&
             cachedValue is not null)
         {
            return cachedValue;
         }

         _logger.LogInformation($"{cacheKey} is not cached. Caching...");
         var fetchedValue = await factory(cancellationToken);

         if (fetchedValue is null)
         {
            return null;
         }

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
         if (enteredSemaphore)
         {
            cacheLock.Semaphore.Release();
         }

         if (cacheLock.ReleaseReference())
         {
            _cacheLocks.TryRemove(
               new KeyValuePair<string, CacheLock>(cacheKey, cacheLock));
            cacheLock.Dispose();
         }
      }
   }

   private static async Task<T?> InvokeFactoryAsync(
      Func<CancellationToken, Task<T>> factory,
      CancellationToken cancellationToken)
   {
      return await factory(cancellationToken);
   }

   private CacheLock AcquireCacheLock(string cacheKey)
   {
      while (true)
      {
         var cacheLock = _cacheLocks.GetOrAdd(
            cacheKey,
            _ => new CacheLock());
         if (cacheLock.TryAddReference())
         {
            return cacheLock;
         }

         _cacheLocks.TryRemove(
            new KeyValuePair<string, CacheLock>(cacheKey, cacheLock));
      }
   }

   public void Dispose()
   {
      foreach (var cacheLock in _cacheLocks.Values)
      {
         cacheLock.Dispose();
      }

      _cacheLocks.Clear();
      _memoryCache.Dispose();
   }

   private readonly ILogger<AsyncMemoryCache<T>> _logger;
   private readonly ICacheSizeEstimator<T> _sizeEstimator;
   private readonly ICacheOptions _cacheOptions;
   private readonly IMemoryCache _memoryCache;

   // Map each cache key to a semaphore.
   // When that content is requested and the cache does not contain the asset,
   // lock the semaphore and cache the asset. Release the semaphore after 
   // caching and return the asset.
   private readonly ConcurrentDictionary<string, CacheLock> _cacheLocks =
      new();
}