// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using DavidBrowning.Models;
using Microsoft.Extensions.Options;

namespace DavidBrowning.Services.Time;

public sealed class BasicDateTimeDisplayService : IDateTimeDisplayService
{
   public BasicDateTimeDisplayService(IOptions<DateTimeDisplayOptions> opts)
   {
      _timeZone = TimeZoneInfo.FindSystemTimeZoneById(
         opts.Value.TimeZoneId);
   }

   public DateTimeOffset ConvertFromUtc(DateTimeOffset utc)
   {
      return TimeZoneInfo.ConvertTime(utc, _timeZone);
   }

   public string FormatDate(DateTimeOffset utc)
   {
      var local = ConvertFromUtc(utc);

      return local.ToString("MMMM d, yyyy");
   }

   private readonly TimeZoneInfo _timeZone;
}
