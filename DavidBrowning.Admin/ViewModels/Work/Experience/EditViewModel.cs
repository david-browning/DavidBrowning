// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DavidBrowning.Models;

namespace DavidBrowning.Admin.ViewModels.Work.Experience;

public sealed class EditViewModel
{
   public EditModes EditMode { get; set; } = EditModes.Create;

   public int? Id { get; set; }

   [Required]
   [Display(Name = "Company Name")]
   [StringLength(DataConstants.MaxNameLength)]
   public string? CompanyName { get; set; }

   [Display(Name = "Company Location")]
   [StringLength(DataConstants.MaxLabelLength)]
   public string? LocationDisplayText { get; set; }

   public bool IsActive { get; set; } = true;

   public IList<RoleEditViewModel>? Roles { get; set; }

   public EditViewModel()
   {
   }

   public EditViewModel(DavidBrowning.Models.Work.Experience experience)
   {
      EditMode = EditModes.Edit;
      Id = experience.Id;
      CompanyName = experience.CompanyName;
      LocationDisplayText = experience.LocationDisplayText;
      IsActive = experience.IsActive;
      Roles = experience.Roles.Select(r => new RoleEditViewModel(r)).ToList();
   }

   public DavidBrowning.Models.Work.Experience ToExperience()
   {
      ArgumentNullException.ThrowIfNullOrEmpty(CompanyName);

      return new()
      {
         Id = Id ?? 0,
         CompanyName = CompanyName,
         LocationDisplayText = LocationDisplayText,
         IsActive = IsActive,
      };
   }
}
