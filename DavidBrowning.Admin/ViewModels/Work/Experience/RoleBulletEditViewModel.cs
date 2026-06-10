// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using DavidBrowning.Models;
using DavidBrowning.Models.Work;

namespace DavidBrowning.Admin.ViewModels.Work.Experience;

public sealed class RoleBulletEditViewModel
{
   public EditModes EditMode { get; set; } = EditModes.Create;

   public int? Id { get; set; }

   [Required]
   [Range(1, int.MaxValue)]
   public int? ExperienceRoleId { get; set; }

   [Required]
   [StringLength(DataConstants.MaxMetadataLength)]
   public string? Text { get; set; }

   public bool IsActive { get; set; } = true;

   public RoleBulletEditViewModel()
   {
   }

   [SetsRequiredMembers]
   public RoleBulletEditViewModel(ExperienceRoleBullet bullet)
   {
      EditMode = EditModes.Edit;
      Id = bullet.Id;
      ExperienceRoleId = bullet.ExperienceRoleId;
      Text = bullet.Text;
      IsActive = bullet.IsActive;
   }

   public ExperienceRoleBullet ToBullet()
   {
      ArgumentNullException.ThrowIfNull(Id);
      ArgumentNullException.ThrowIfNull(ExperienceRoleId);
      ArgumentNullException.ThrowIfNullOrEmpty(Text);

      return new()
      {
         Id = Id.Value,
         ExperienceRoleId = ExperienceRoleId.Value,
         Text = Text,
         IsActive = IsActive,
      };
   }
}
