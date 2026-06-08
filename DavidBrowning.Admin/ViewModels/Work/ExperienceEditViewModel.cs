// Copyright © 2026 David Browning. All rights reserved.
//
// Source-available for viewing only. No license granted.

using System;
using System.ComponentModel.DataAnnotations;

using DavidBrowning.Models;
using DavidBrowning.Models.Work;

namespace DavidBrowning.Admin.ViewModels.Work;

public sealed class ExperienceEditViewModel
{
   public EditModes EditMode { get; set; } = EditModes.Create;

   public int? Id { get; set; }

   [Required]
   [StringLength(DataConstants.MaxNameLength)]
   public string? CompanyName { get; set; }

   [StringLength(DataConstants.MaxLabelLength)]
   public string? LocationDisplayText { get; set; }

   [Range(0, int.MaxValue)]
   public int SortOrder { get; set; }

   public bool IsActive { get; set; } = true;

   public ExperienceEditViewModel()
   {
   }

   public ExperienceEditViewModel(Experience experience)
   {
      EditMode = EditModes.Edit;
      Id = experience.Id;
      CompanyName = experience.CompanyName;
      LocationDisplayText = experience.LocationDisplayText;
      SortOrder = experience.SortOrder;
      IsActive = experience.IsActive;
   }
}
