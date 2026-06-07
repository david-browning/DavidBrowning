using System.Diagnostics.CodeAnalysis;
using DavidBrowning.Models.Projects;

namespace DavidBrowning.Admin.ViewModels.Projets;

public class ProjectOriginEditViewModel
{
   public required EditModes EditMode { get; init; }

   public ProjectOriginEditViewModel()
   {

   }

   [SetsRequiredMembers]
   public ProjectOriginEditViewModel(ProjectOrigin origin)
   {
      EditMode = EditModes.Edit;
   }
}
