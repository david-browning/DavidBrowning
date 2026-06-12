// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
namespace DavidBrowning.Admin.ViewModels.About;

public sealed class IndexViewModel
{
   public required InterestEditViewModel Create { get; set; }

   public required InterestListViewModel List { get; set; }

   public required AdminOffcanvasViewModel EditOffcanvas { get; set; }
}