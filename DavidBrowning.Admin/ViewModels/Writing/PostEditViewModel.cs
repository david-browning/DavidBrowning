using System.Diagnostics.CodeAnalysis;
using DavidBrowning.Models.Writing;

namespace DavidBrowning.Admin.ViewModels.Writing;

public class PostEditViewModel
{
   public required EditModes EditMode { get; init; }

   public PostEditViewModel()
   {

   }

   [SetsRequiredMembers]
   public PostEditViewModel(Post post)
   {
      EditMode = EditModes.Edit;
   }
}
