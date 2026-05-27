// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace DavidBrowning.Services.Cache;

public interface IAsyncCache
{
   Task<T> GetOrCreateAsync<T>(
      string cacheKey,
      Func<CancellationToken, Task<T>> factory,
      MemoryCacheEntryOptions options,
      CancellationToken cancellationToken = default);
}
