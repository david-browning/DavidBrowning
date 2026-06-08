// Copyright © 2026 David Browning. All rights reserved.
//
// Source-available for viewing only. No license granted.

using System.ComponentModel.DataAnnotations;

namespace DavidBrowning.Admin.ViewModels.Work;

public sealed class ExperienceRoleDeleteViewModel
{
   [Range(1, int.MaxValue)]
   public int Id { get; set; }

   [Range(1, int.MaxValue)]
   public int ExperienceId { get; set; }

   [Required]
   public string? Title { get; set; }

   public int BulletCount { get; set; }
}
