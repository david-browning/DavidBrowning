using System.Collections.Generic;

namespace DavidBrowning.Admin.ViewModels;

public class AssetChooserViewModel
{
   public required IReadOnlyList<AssetChooserItemViewModel> Assets { get; set; }

   public string? SelectedAssetKey { get; set; }
}
