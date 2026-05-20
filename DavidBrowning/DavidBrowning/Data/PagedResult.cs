// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Collections.Generic;

namespace DavidBrowning.Data
{
   public sealed class PagedResult<T>
   {
      public required IReadOnlyList<T> Items { get; init; }

      public required int Page { get; init; }

      public required int PageSize { get; init; }

      public required int TotalCount { get; init; }

      public int TotalPages
      {
         get
         {
            if (PageSize <= 0)
            {
               return 0;
            }

            return (int)Math.Ceiling((double)TotalCount / PageSize);
         }
      }
   }
}
