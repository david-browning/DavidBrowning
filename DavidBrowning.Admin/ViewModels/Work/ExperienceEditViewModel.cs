using System.Diagnostics.CodeAnalysis;
using DavidBrowning.Models.Work;

namespace DavidBrowning.Admin.ViewModels.Work;

public class ExperienceEditViewModel
{
   public required EditModes EditMode { get; init; }

   public ExperienceEditViewModel()
   {

   }

   [SetsRequiredMembers]
   public ExperienceEditViewModel(Experience experience)
   {
      EditMode = EditModes.Edit;
   }
}
