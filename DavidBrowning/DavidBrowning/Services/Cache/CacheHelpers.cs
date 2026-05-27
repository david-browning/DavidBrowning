using System;
using System.Collections;

namespace DavidBrowning.Services.Cache;

internal static class CacheHelpers
{
   public static long EstimateSize<T>(T model, long defaultSize = 1)
   {
      switch (model)
      {
         case string value:
         {
            return EstimateStringSize(value);
         }
         case byte[] value:
         {
            return EstimateByteArraySize(value);
         }
         case ICollection value:
         {
            return EstimateCollectionSize(value);
         }
         default:
         {
            return defaultSize;
         }
      }
   }

   public static long EstimateStringSize(string model)
   {
      return sizeof(char) * model.Length;
   }

   public static long EstimateByteArraySize(byte[] model)
   {
      return sizeof(byte) * model.LongLength;
   }

   private static long EstimateCollectionSize(ICollection value)
   {
      return Math.Max(1, value.Count);
   }
}
