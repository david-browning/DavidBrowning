using System.Collections.Generic;

namespace DavidBrowning.Admin.ViewModels;

public interface IListViewModel<T>
{
   ListModes ListMode { get; set; }

   List<T>? Items { get; set; }

   int Count => Items?.Count ?? 0;
}
