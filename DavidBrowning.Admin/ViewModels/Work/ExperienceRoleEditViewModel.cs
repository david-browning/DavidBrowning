using System.Diagnostics.CodeAnalysis;
using DavidBrowning.Models.Work;

namespace DavidBrowning.Admin.ViewModels.Work;

public class ExperienceRoleEditViewModel
{
   public required EditModes EditMode { get; init; }

   public ExperienceRoleEditViewModel()
   {

   }

   [SetsRequiredMembers]
   public ExperienceRoleEditViewModel(ExperienceRole role)
   {
      EditMode = EditModes.Edit;
   }
}
