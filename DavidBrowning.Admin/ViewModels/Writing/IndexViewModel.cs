// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using DavidBrowning.Admin.ViewModels.Writing.Posts;

namespace DavidBrowning.Admin.ViewModels.Writing;

public sealed class IndexViewModel
{
   public required WritingTagPanelViewModel Tags { get; set; }

   public required PostStylePanelViewModel Styles { get; set; }

   public required PostListViewModel Posts { get; set; }

   public required AdminOffcanvasViewModel TagEditOffcanvas { get; set; }

   public required AdminOffcanvasViewModel StyleEditOffcanvas { get; set; }
}
