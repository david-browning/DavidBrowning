// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DavidBrowning.Models;

namespace DavidBrowning.Admin.ViewModels.About;

public sealed class InterestListViewModel
{
   public required ReorderListViewModel ReorderList { get; set; }

   [SetsRequiredMembers]
   public InterestListViewModel(IEnumerable<Interest> interests)
   {
      ReorderList = new ReorderListViewModel
      {
         Title = "Interests",
         Description =
            "Edit About-page interests or change their display order.",
         EmptyMessage =
            "No interests have been created yet.",
         ReorderController = "About",
         ReorderAction = "InterestReorder",
         Items = interests
            .OrderBy(i => i.SortOrder)
            .ThenBy(i => i.DisplayName)
            .Select(i => new ReorderListItemViewModel
            {
               Id = i.Id,
               DisplayName = i.DisplayName,
               SecondaryText = i.Slug,
               IconCssClass = i.IconCssClass,
               IsActive = i.IsActive,
               SortOrder = i.SortOrder,
               EditController = "About",
               EditAction = "InterestEdit",
               DeleteController = "About",
               DeleteAction = "InterestDelete",
            })
            .ToList(),
      };
   }
}