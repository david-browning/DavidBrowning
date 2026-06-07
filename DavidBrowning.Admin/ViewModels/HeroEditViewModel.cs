using System.Diagnostics.CodeAnalysis;
using DavidBrowning.Models;

namespace DavidBrowning.Admin.ViewModels;

public class HeroEditViewModel
{
   public required EditModes EditMode { get; init; }

   public HeroEditViewModel()
   {

   }

   [SetsRequiredMembers]
   public HeroEditViewModel(HeroData data)
   {
      EditMode = EditModes.Edit;
   }
}
