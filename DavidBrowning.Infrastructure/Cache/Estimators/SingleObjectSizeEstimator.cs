// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
namespace DavidBrowning.Infrastructure.Cache.Estimators;

public sealed class SingleObjectSizeEstimator<T> : ICacheSizeEstimator<T>
   where T : notnull
{
   public long EstimateSize(T value)
   {
      return 1;
   }
}
