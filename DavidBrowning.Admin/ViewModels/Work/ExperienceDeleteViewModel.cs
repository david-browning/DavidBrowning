// Copyright © 2026 David Browning. All rights reserved.
//
// Source-available for viewing only. No license granted.

using System.ComponentModel.DataAnnotations;

namespace DavidBrowning.Admin.ViewModels.Work;

public sealed class ExperienceDeleteViewModel
{
   [Range(1, int.MaxValue)]
   public int Id { get; set; }

   [Required]
   public string? CompanyName { get; set; }

   public int RoleCount { get; set; }

   public int RoleBulletCount { get; set; }
}
