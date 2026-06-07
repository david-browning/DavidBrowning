using System.Diagnostics.CodeAnalysis;
using DavidBrowning.Models.Writing;

namespace DavidBrowning.Admin.ViewModels.Writing;

public class WritingTagEditViewModel
{
   public required EditModes EditMode { get; init; }

   public WritingTagEditViewModel()
   {

   }

   [SetsRequiredMembers]
   public WritingTagEditViewModel(WritingTag tag)
   {
      EditMode = EditModes.Edit;
   }
}
