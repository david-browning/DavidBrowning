// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;

namespace DavidBrowning.Services.Time;

public sealed class SystemClock : ISystemClock
{
   public DateTime UtcNow
   {
      get
      {
         return DateTime.UtcNow;
      }
   }
}