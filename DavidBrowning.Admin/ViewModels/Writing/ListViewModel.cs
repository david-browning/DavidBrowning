// Copyright Â© 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Collections.Generic;
using System.Linq;
using DavidBrowning.Models.Writing;

namespace DavidBrowning.Admin.ViewModels.Writing;

public class ListViewModel
{
   public List<PostEditViewModel>? Items { get; set; }

   public ListViewModel(IEnumerable<Post> posts)
   {
      Items = posts.Select(p => new PostEditViewModel(p)).ToList();
   }
}
