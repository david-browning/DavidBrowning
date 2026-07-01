// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;

namespace DavidBrowning.Diagnostics;
public class WarmupOptions
{

   public bool Enabled { get; set; } = false;

   public string? Token { get; set; }

   public TimeSpan MinimumInterval { get; set; } = TimeSpan.FromMinutes(30);
}
