// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
namespace DavidBrowning.Services.Cache.Estimators;

internal sealed class ByteArraySizeEstimator : ICacheSizeEstimator<byte[]>
{
   public long EstimateSize(byte[] value)
   {
      return value == null ? 1 : sizeof(byte) * value.Length;
   }
}
