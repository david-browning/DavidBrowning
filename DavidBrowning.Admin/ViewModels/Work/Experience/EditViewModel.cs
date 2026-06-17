// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using DavidBrowning.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

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

   [ValidateNever]
   public required RoleEditListViewModel Roles { get; set; }

   public EditViewModel()
   {

   }

   [SetsRequiredMembers]
   public EditViewModel(
      Models.Work.Experience experience,
      ReorderListViewModel roleModel)
   {
      EditMode = EditModes.Edit;
      Id = experience.Id;
      CompanyName = experience.CompanyName;
      LocationDisplayText = experience.LocationDisplayText;
      IsActive = experience.IsActive;
      Roles = new RoleEditListViewModel()
      {
         Create = new RoleEditViewModel()
         {
            ExperienceId = experience.Id,
         },

         Roles = roleModel,
      };
   }

   public Models.Work.Experience ToExperience()
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
