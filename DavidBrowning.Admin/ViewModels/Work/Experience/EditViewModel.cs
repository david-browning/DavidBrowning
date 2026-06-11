// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DavidBrowning.Admin.Controllers;
using DavidBrowning.Models;
using DavidBrowning.Models.Work;
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

   [SetsRequiredMembers]
   public EditViewModel()
   {
      Roles = new RoleEditListViewModel()
      {
         Roles = new ReorderListViewModel()
         { 
            Items = new List<ReorderListItemViewModel>(),
            Title = "Roles",
            Description = "Add company roles",
            ReoderParameters = new ReoderParameters()
            {
               ReorderAction = nameof(WorkController.ExperienceRoleReorder),
               ReorderController = "Work",
            },
         },
         Create = new RoleEditViewModel()
         {

         }
      };
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
         Roles = roleModel,
         Create = new RoleEditViewModel()
         {
            
         },
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
         Roles = new List<ExperienceRole>(),
      };
   }
}
