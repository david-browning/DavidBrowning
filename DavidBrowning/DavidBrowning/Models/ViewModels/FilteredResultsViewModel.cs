// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Collections.Generic;

namespace DavidBrowning.Models.ViewModels;

public class FilteredResultsViewModel
{
   public required string PageTitle { get; init; }

   public required string FilterName { get; init; }

   public required string FilterSlug { get; init; }

   public string? Description { get; init; }

   public required string ResultPartialName { get; init; }

   public required IReadOnlyCollection<object> Results { get; init; }
}
