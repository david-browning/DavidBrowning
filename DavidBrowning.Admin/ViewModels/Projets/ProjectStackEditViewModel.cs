using System.Diagnostics.CodeAnalysis;
using DavidBrowning.Models.Projects;

namespace DavidBrowning.Admin.ViewModels.Projets;

public class ProjectStackEditViewModel
{
   public required EditModes EditMode { get; init; }

   public ProjectStackEditViewModel()
   {

   }

   [SetsRequiredMembers]
   public ProjectStackEditViewModel(ProjectStackTag stack)
   {
      EditMode = EditModes.Edit;
   }
}
