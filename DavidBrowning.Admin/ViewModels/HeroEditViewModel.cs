// Copyright © 2026 David Browning. All rights reserved.
//
// Source-available for viewing only. No license granted.

using System.ComponentModel.DataAnnotations;

using DavidBrowning.Models;

namespace DavidBrowning.Admin.ViewModels;

public sealed class HeroEditViewModel
{
   public EditModes EditMode { get; set; } = EditModes.Create;

   [StringLength(DataConstants.MaxLabelLength)]
   public string? Title { get; set; }

   [StringLength(DataConstants.MaxLabelLength)]
   public string? Subtitle { get; set; }

   [StringLength(DataConstants.MaxMetadataLength)]
   public string? Lede { get; set; }

   public HeroEditViewModel()
   {
   }

   public HeroEditViewModel(HeroData data)
   {
      EditMode = EditModes.Edit;
      Title = data.Title;
      Subtitle = data.Subtitle;
      Lede = data.Lede;
   }
}
