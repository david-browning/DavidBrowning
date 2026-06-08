// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;


namespace DavidBrowning;
public interface IDateUpdatedTrackedEntity
{
   DateTime UpdatedAtUtc { get; set; }
}
