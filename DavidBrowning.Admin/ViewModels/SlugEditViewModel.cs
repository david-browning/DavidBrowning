using System.Diagnostics.CodeAnalysis;
using DavidBrowning.Models;

namespace DavidBrowning.Admin.ViewModels;

public class SlugEditViewModel
{
   public required EditModes EditMode { get; init; }

   public SlugEditViewModel()
   {

   }

   [SetsRequiredMembers]
   public SlugEditViewModel(IQueryableSlug slug)
   {
      EditMode = EditModes.Edit;
   }
}
