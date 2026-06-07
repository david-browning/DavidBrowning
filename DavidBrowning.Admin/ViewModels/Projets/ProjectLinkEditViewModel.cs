using System.Diagnostics.CodeAnalysis;
using DavidBrowning.Models.Projects;

namespace DavidBrowning.Admin.ViewModels.Projets;

public class ProjectLinkEditViewModel
{
   public required EditModes EditMode { get; init; }

   public ProjectLinkEditViewModel()
   {

   }

   [SetsRequiredMembers]
   public ProjectLinkEditViewModel(ProjectLink link)
   {
      EditMode = EditModes.Edit;
   }
}
