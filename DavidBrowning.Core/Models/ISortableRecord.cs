// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;

namespace DavidBrowning.Models;

public interface ISortableRecord
{
   int Id { get; }

   int SortOrder { get; set; }
}