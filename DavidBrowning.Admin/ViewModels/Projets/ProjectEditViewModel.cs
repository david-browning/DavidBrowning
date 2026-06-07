using System.Diagnostics.CodeAnalysis;
using DavidBrowning.Models.Projects;

namespace DavidBrowning.Admin.ViewModels.Projets;

public class ProjectEditViewModel
{
   public required EditModes EditMode { get; init; }

   public ProjectEditViewModel()
   {

   }

   [SetsRequiredMembers]
   public ProjectEditViewModel(Project project)
   {
      EditMode = EditModes.Edit;
   }
}
