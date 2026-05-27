// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Threading;

namespace DavidBrowning.Services.Cache;

internal sealed class CacheLock : IDisposable
{
   public SemaphoreSlim Semaphore { get; } = new(1, 1);

   public bool TryAddReference()
   {
      lock (_syncRoot)
      {
         if (_isRetired)
         {
            return false;
         }

         _referenceCount++;
         return true;
      }
   }

   public bool ReleaseReference()
   {
      lock (_syncRoot)
      {
         _referenceCount--;

         if (_referenceCount < 0)
         {
            throw new InvalidOperationException(
               "Cache lock reference count became negative.");
         }

         if (_referenceCount == 0)
         {
            _isRetired = true;
            return true;
         }

         return false;
      }
   }

   public void Dispose()
   {
      Semaphore.Dispose();
   }

   private readonly object _syncRoot = new();
   private int _referenceCount;
   private bool _isRetired;
}