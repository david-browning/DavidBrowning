// Copyright © 2026 David Browning. All rights reserved.
//
// Source-available for viewing only. No license granted.

using System.ComponentModel.DataAnnotations;

using DavidBrowning.Models;
using DavidBrowning.Models.Work;

namespace DavidBrowning.Admin.ViewModels.Work;

public sealed class ExperienceRoleBulletEditViewModel
{
   public EditModes EditMode { get; set; } = EditModes.Create;

   public int? Id { get; set; }

   [Required]
   [Range(1, int.MaxValue)]
   public int? ExperienceRoleId { get; set; }

   [Required]
   [StringLength(DataConstants.MaxMetadataLength)]
   public string? Text { get; set; }

   [Range(0, int.MaxValue)]
   public int SortOrder { get; set; }

   public bool IsActive { get; set; } = true;

   public ExperienceRoleBulletEditViewModel()
   {
   }

   public ExperienceRoleBulletEditViewModel(ExperienceRoleBullet bullet)
   {
      EditMode = EditModes.Edit;
      Id = bullet.Id;
      ExperienceRoleId = bullet.ExperienceRoleId;
      Text = bullet.Text;
      SortOrder = bullet.SortOrder;
      IsActive = bullet.IsActive;
   }
}
