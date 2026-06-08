// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Collections.Generic;
using System.Linq;
using DavidBrowning.Models.Writing;

namespace DavidBrowning.Admin.ViewModels.Writing;

public class PostRevisionListViewModel : IListViewModel<PostRevisionEditViewModel>
{
   public ListModes ListMode { get; set; }
   
   public List<PostRevisionEditViewModel>? Items { get; set; }

   public PostRevisionListViewModel(
      IEnumerable<PostRevision> revisions, 
      ListModes mode)
   {
      ListMode = mode;
      Items = revisions.Select(r => new PostRevisionEditViewModel(r)).ToList();
   }
}
