// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
namespace DavidBrowning.Services.Cache.Estimators;

public class DefaultCacheSizeEstimator<T> : ICacheSizeEstimator<T>
{
   public long EstimateSize(T? value)
   {
      return 1;
   }
}
