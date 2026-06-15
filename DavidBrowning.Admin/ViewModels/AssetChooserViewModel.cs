// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Collections.Generic;

namespace DavidBrowning.Admin.ViewModels;

public class AssetChooserViewModel
{
   public required IReadOnlyList<AssetChooserItemViewModel> Assets { get; set; }
}
