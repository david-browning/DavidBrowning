// Copyright © 2026 David Browning. All rights reserved.
//
// Source-available for viewing only. No license granted.

using System.ComponentModel.DataAnnotations;

namespace DavidBrowning.Admin.ViewModels.Work;

public sealed class CredentialDeleteViewModel
{
   [Range(1, int.MaxValue)]
   public int Id { get; set; }

   [Required]
   public string? IssuingOrganization { get; set; }

   [Required]
   public string? Name { get; set; }
}
