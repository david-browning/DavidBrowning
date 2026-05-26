// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DavidBrowning.Services.Cache;

public interface ILookupCache
{
   /// <summary>
   /// Gets a cached value of type "TValue" if present. 
   /// Otherwise uses the functor to get and cache it.
   /// </summary>
   /// <typeparam name="TValue">The type of object to cache.</typeparam>
   /// <param name="cacheKey">Unique key name.</param>
   /// <param name="valueFactory">
   /// The function to call to get the item if it is not cached.
   /// </param>
   /// <param name="cancellationToken"></param>
   /// <returns></returns>
   Task<TValue?> GetOrCreateAsync<TValue>(
      string cacheKey,
      Func<CancellationToken, Task<TValue?>> valueFactory,
      CancellationToken cancellationToken = default);

   /// <summary>
   /// Removes the key from the cache.
   /// </summary>
   /// <param name="cacheKey"></param>
   void Remove(string cacheKey);

   /// <summary>
   /// Clears the cache.
   /// </summary>
   void Clear();
}
