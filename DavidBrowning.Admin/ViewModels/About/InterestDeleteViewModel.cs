// Copyright © 2026 David Browning. All rights reserved.
//
// Source-available for viewing only. No license granted.

using System.ComponentModel.DataAnnotations;

namespace DavidBrowning.Admin.ViewModels.About;

public sealed class InterestDeleteViewModel
{
   [Range(1, int.MaxValue)]
   public int Id { get; set; }

   [Required]
   public string? DisplayName { get; set; }

   [Required]
   public string? Slug { get; set; }
}
