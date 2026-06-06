// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

namespace DavidBrowning.Services.Cache.Estimators;

public sealed class StringSizeEstimator : ICacheSizeEstimator<string>
{
   public long EstimateSize(string value)
   {
      return string.IsNullOrEmpty(value) ? 1 : sizeof(char) * value.Length;
   }
}
