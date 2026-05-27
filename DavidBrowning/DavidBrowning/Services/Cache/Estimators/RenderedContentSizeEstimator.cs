// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using DavidBrowning.Models.ViewModels;

namespace DavidBrowning.Services.Cache.Estimators;

internal sealed class RenderedContentSizeEstimator : 
   ICacheSizeEstimator<RenderedContent?>
{
   public long EstimateSize(RenderedContent? value)
   {
      // If the value is null or contains no text, then it is just 1
      if(value == null || string.IsNullOrEmpty(value.Html))
      {
         return 1;
      }
      else
      {
         // Calculate how much space the HTML code uses
         return sizeof(char) * value.Html.Length;
      }
   }
}
