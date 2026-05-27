// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace DavidBrowning.Services.Cache;

public class AsyncMemoryCache : IAsyncCache
{
   public AsyncMemoryCache(IMemoryCache memoryCache)
   {
      _memoryCache = memoryCache;
   }

   public async Task<T> GetOrCreateAsync<T>(
      string cacheKey,
      Func<CancellationToken, Task<T>> factory,
      MemoryCacheEntryOptions options,
      CancellationToken cancellationToken = default)
   {
      if (_memoryCache.TryGetValue(cacheKey, out T? cachedValue) &&
          cachedValue is not null)
      {
         return cachedValue;
      }

      var cacheLock = _cacheLocks.GetOrAdd(
         cacheKey, _ => new SemaphoreSlim(1, 1));
      await cacheLock.WaitAsync(cancellationToken);

      try
      {
         if (_memoryCache.TryGetValue(cacheKey, out cachedValue) &&
             cachedValue is not null)
         {
            return cachedValue;
         }

         var fetchedValue = await factory(cancellationToken);
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

   private readonly IMemoryCache _memoryCache;

   // Map each cache key to a semaphore.
   // When that content is requested and the cache does not contain the asset,
   // lock the semaphore and cache the asset. Release the semaphore after 
   // caching and return the asset.
   private readonly ConcurrentDictionary<string, SemaphoreSlim> _cacheLocks =
      new();
}
