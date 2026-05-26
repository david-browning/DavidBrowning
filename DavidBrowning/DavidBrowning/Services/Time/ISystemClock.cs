// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;

namespace DavidBrowning.Services.Time;

/// <summary>
/// Used to get the current time. This is an interface so we can write "fake"
/// clocks for use in testing.
/// </summary>
public interface ISystemClock
{
   DateTime UtcNow { get; }
}
