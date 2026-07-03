// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;

namespace DavidBrowning.Diagnostics;
public class WarmupOptions
{
   public bool Enabled { get; set; }

   public string? ApiKey { get; set; }

   public TimeSpan MinimumInterval { get; set; } = TimeSpan.FromMinutes(30);

   public TimeSpan MaximumWaitTime { get; set; } = TimeSpan.FromMinutes(2);

   public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(5);

   public int AttemptConnectTimeoutSeconds { get; set; } = 5;

   public int AttemptCommandTimeoutSeconds { get; set; } = 5;
}
