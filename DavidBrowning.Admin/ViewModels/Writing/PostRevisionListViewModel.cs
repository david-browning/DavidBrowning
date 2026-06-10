// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Collections.Generic;
using System.Linq;
using DavidBrowning.Models.Writing;

namespace DavidBrowning.Admin.ViewModels.Writing;

public class PostRevisionListViewModel
{
   public List<PostRevisionEditViewModel>? Items { get; set; }

   public PostRevisionListViewModel(IEnumerable<PostRevision> revisions)
   {
      Items = revisions.Select(r => new PostRevisionEditViewModel(r)).ToList();
   }
}
