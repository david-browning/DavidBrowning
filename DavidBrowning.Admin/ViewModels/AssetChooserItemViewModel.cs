namespace DavidBrowning.Admin.ViewModels;

public class AssetChooserItemViewModel
{
   public required int Id { get; set; }

   public required string AssetKey { get; set; }

   public required string ContentType { get; set; }

   public required string ReferenceKey { get; set; }

   public string InsertText => $"{{{{asset:{ReferenceKey}}}}}";
}
