using System.Collections.Generic;

namespace DavidBrowning.Admin.ViewModels;

public interface IReadonlyListViewModel<T>
{
   IReadOnlyList<T>? Items { get; set; }

   int Count => Items?.Count ?? 0;
}
