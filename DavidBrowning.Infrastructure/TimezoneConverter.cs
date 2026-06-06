// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using DavidBrowning.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace DavidBrowning.Infrastructure;

public sealed class TimezoneConverter
{
   public TimezoneConverter(IOptions<DateTimeDisplayOptions> opts)
   {
      _timeZone = TimeZoneInfo.FindSystemTimeZoneById(opts.Value.TimeZoneId);
   }

   public DateTimeOffset ConvertFromUtc(DateTimeOffset utc)
   {
      return TimeZoneInfo.ConvertTime(utc, _timeZone);
   }

   public string FormatDate(DateTimeOffset utc)
   {
      return ConvertFromUtc(utc).ToString("MMMM d, yyyy");
   }

   private readonly TimeZoneInfo _timeZone;
}
