// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using Microsoft.IdentityModel.Tokens;

namespace DavidBrowning.Services.Cache.Estimators;

internal sealed class StringSizeEstimator : ICacheSizeEstimator<string>
{
   public long EstimateSize(string value)
   {
      return value.IsNullOrEmpty() ? 1 : sizeof(char) * value.Length;
   }
}
