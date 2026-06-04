// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
namespace DavidBrowning.Services.Cache.Estimators;

public interface ICacheSizeEstimator<T>
      where T : notnull
{
   long EstimateSize(T value);
}
