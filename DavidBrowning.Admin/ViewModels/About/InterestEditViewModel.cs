using System.Diagnostics.CodeAnalysis;
using DavidBrowning.Models;

namespace DavidBrowning.Admin.ViewModels.About;

public class InterestEditViewModel
{
   public required EditModes EditMode { get; init; }

   public InterestEditViewModel()
   {

   }

   [SetsRequiredMembers]
   public InterestEditViewModel(Interest interest)
   {
      EditMode = EditModes.Edit;
   }
}
