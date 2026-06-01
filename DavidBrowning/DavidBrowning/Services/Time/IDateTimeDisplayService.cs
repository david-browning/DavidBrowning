// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;

namespace DavidBrowning.Services.Time;

public interface IDateTimeDisplayService
{
   DateTimeOffset ConvertFromUtc(DateTimeOffset utc);

   string FormatDate(DateTimeOffset utc);
}
