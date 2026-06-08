using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DavidBrowning.Admin.ViewModels;

public sealed class ReorderListRequestViewModel
{
   public required List<ReorderListItemRequestViewModel> Items { get; set; }
}

public sealed class ReorderListItemRequestViewModel
{
   [Range(1, int.MaxValue)]
   public int Id { get; set; }

   [Range(0, int.MaxValue)]
   public int SortOrder { get; set; }
}

public sealed class ReorderListItemViewModel
{
   public int Id { get; set; }

   public required string DisplayName { get; set; }

   public string? SecondaryText { get; set; }

   public string? IconCssClass { get; set; }

   public bool? IsActive { get; set; }

   public int SortOrder { get; set; }

   public required string EditController { get; set; }

   public required string EditAction { get; set; }

   public required string DeleteController { get; set; }

   public required string DeleteAction { get; set; }
}

public sealed class ReorderListViewModel
{
   public required string Title { get; set; }

   public string? Description { get; set; }

   public string EmptyMessage { get; set; } =
      "No items have been created yet.";

   public required string ReorderController { get; set; }

   public required string ReorderAction { get; set; }

   public required IReadOnlyList<ReorderListItemViewModel> Items { get; set; }
}