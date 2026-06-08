// Copyright Â© 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using DavidBrowning.Models;

namespace DavidBrowning.Admin.ViewModels;

public class HeroEditViewModel
{
   public required EditModes EditMode { get; init; }

   public string? Title { get; set; }

   public string? Subtitle { get; set; }

   public string? Lede { get; set; }

   public HeroEditViewModel()
   {
   }

   [SetsRequiredMembers]
   public HeroEditViewModel(HeroData data)
   {
      EditMode = EditModes.Edit;
      Title = data.Title;
      Subtitle = data.Subtitle;
      Lede = data.Lede;
   }
}
