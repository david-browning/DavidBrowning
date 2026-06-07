using System.Diagnostics.CodeAnalysis;
using DavidBrowning.Models;

namespace DavidBrowning.Admin.ViewModels.Asset;

public class EditViewModel
{
   public required EditModes EditMode { get; init; }

   public EditViewModel()
   {

   }

   [SetsRequiredMembers]
   public EditViewModel(SiteAsset asset)
   {
      EditMode = EditModes.Edit;
   }
}
