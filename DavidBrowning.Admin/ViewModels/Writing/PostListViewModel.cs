// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Collections.Generic;
using System.Linq;
using DavidBrowning.Models.Writing;

namespace DavidBrowning.Admin.ViewModels.Writing;

public class PostListViewModel : IListViewModel<PostEditViewModel>
{
   public ListModes ListMode { get; set; }

   public List<PostEditViewModel>? Items { get; set; }

   public PostListViewModel(IEnumerable<Post> posts, ListModes mode)
   {
      ListMode = mode;
      Items = posts.Select(p => new PostEditViewModel(p)).ToList();
   }
}
