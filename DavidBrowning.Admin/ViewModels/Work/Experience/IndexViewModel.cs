// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
namespace DavidBrowning.Admin.ViewModels.Work.Experience;

public sealed class IndexViewModel
{
   public required EditViewModel Create { get; set; }

   public required ListViewModel List { get; set; }

   public required AdminOffcanvasViewModel EditOffcanvas { get; set; }
}
