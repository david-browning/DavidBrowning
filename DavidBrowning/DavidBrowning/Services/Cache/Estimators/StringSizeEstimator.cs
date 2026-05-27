// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
namespace DavidBrowning.Services.Cache.Estimators;

internal sealed class StringSizeEstimator : ICacheSizeEstimator<string>
{
   public long EstimateSize(string? value)
   {
      return value == null ? 1 : sizeof(char) * value.Length;
   }
}
