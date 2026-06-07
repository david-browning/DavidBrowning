using System.Diagnostics.CodeAnalysis;
using DavidBrowning.Models.Writing;

namespace DavidBrowning.Admin.ViewModels.Writing;

public class PostRevisionEditViewModel
{
   public required EditModes EditMode { get; init; }

   public PostRevisionEditViewModel()
   {

   }

   [SetsRequiredMembers]
   public PostRevisionEditViewModel(PostRevision revision)
   {
      EditMode = EditModes.Edit;
   }
}
