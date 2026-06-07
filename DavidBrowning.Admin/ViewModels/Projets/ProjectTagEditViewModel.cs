using System.Diagnostics.CodeAnalysis;
using DavidBrowning.Models.Projects;

namespace DavidBrowning.Admin.ViewModels.Projets;

public class ProjectTagEditViewModel
{
   public required EditModes EditMode { get; init; }

   public ProjectTagEditViewModel()
   {

   }

   [SetsRequiredMembers]
   public ProjectTagEditViewModel(ProjectTag tag)
   {
      EditMode = EditModes.Edit;
   }
}
