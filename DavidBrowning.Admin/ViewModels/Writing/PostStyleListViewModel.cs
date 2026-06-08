// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DavidBrowning.Models.Writing;

namespace DavidBrowning.Admin.ViewModels.Writing;

public class PostStyleListViewModel : IReadonlyListViewModel<PostStyle>
{
   public IReadOnlyList<PostStyle>? Items { get; set; }

   [SetsRequiredMembers]
   public PostStyleListViewModel(IEnumerable<PostStyle> styles)
   {
      Items = styles.ToList();
   }
}
