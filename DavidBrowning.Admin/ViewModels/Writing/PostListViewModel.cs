// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Collections.Generic;
using System.Linq;
using DavidBrowning.Models.Writing;

namespace DavidBrowning.Admin.ViewModels.Writing;

public class PostListViewModel
{
   public List<PostEditViewModel>? Items { get; set; }

   public PostListViewModel(IEnumerable<Post> posts)
   {
      Items = posts.Select(p => new PostEditViewModel(p)).ToList();
   }
}
