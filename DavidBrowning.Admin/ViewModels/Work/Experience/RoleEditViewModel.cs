// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DavidBrowning.Models;
using DavidBrowning.Models.Work;

namespace DavidBrowning.Admin.ViewModels.Work.Experience;

public sealed class RoleEditViewModel
{
   public EditModes EditMode { get; set; } = EditModes.Create;

   public int? Id { get; set; }

   [Required]
   [Range(1, int.MaxValue)]
   public int? ExperienceId { get; set; }

   [Required]
   [StringLength(DataConstants.MaxNameLength)]
   public string? Title { get; set; }

   [StringLength(DataConstants.MaxLabelLength)]
   public string? DateDisplayText { get; set; }

   [StringLength(DataConstants.MaxMetadataLength)]
   public string? Description { get; set; }

   public bool IsActive { get; set; } = true;

   public IList<RoleBulletEditViewModel>? Bullets { get; set; }

   public RoleEditViewModel()
   {
   }

   public RoleEditViewModel(ExperienceRole role)
   {
      EditMode = EditModes.Edit;
      Id = role.Id;
      ExperienceId = role.ExperienceId;
      Title = role.Title;
      DateDisplayText = role.DateDisplayText;
      Description = role.Description;
      IsActive = role.IsActive;
      Bullets = role.Bullets.Select(b => new RoleBulletEditViewModel(b)).ToList();
   }

   public ExperienceRole ToRole()
   {
      ArgumentNullException.ThrowIfNull(Id);
      ArgumentNullException.ThrowIfNull(ExperienceId);
      ArgumentNullException.ThrowIfNull(Title);

      return new()
      {
         Id = Id.Value,
         ExperienceId = ExperienceId.Value,
         Title = Title,
         DateDisplayText = DateDisplayText,
         Description = Description,
         IsActive = IsActive,
      };
   }
}