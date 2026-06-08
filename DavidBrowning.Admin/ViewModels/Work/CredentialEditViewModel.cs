// Copyright © 2026 David Browning. All rights reserved.
//
// Source-available for viewing only. No license granted.

using System;
using System.ComponentModel.DataAnnotations;

using DavidBrowning.Models;
using DavidBrowning.Models.Work;

namespace DavidBrowning.Admin.ViewModels.Work;

public sealed class CredentialEditViewModel
{
   public EditModes EditMode { get; set; } = EditModes.Create;

   public int? Id { get; set; }

   [Required]
   [StringLength(DataConstants.MaxNameLength)]
   public string? IssuingOrganization { get; set; }

   [Required]
   [StringLength(DataConstants.MaxNameLength)]
   public string? Name { get; set; }

   [StringLength(DataConstants.MaxLabelLength)]
   public string? Type { get; set; }

   [Range(1, 12)]
   public int? AwardedMonth { get; set; }

   [Range(1, 9999)]
   public int? AwardedYear { get; set; }

   [StringLength(DataConstants.MaxLabelLength)]
   public string? DateDisplayText { get; set; }

   [StringLength(DataConstants.MaxMetadataLength)]
   public string? Description { get; set; }

   [StringLength(DataConstants.MaxUrlLength)]
   [Url]
   public string? CredentialUrl { get; set; }

   [Range(0, int.MaxValue)]
   public int SortOrder { get; set; }

   public bool IsActive { get; set; } = true;

   public CredentialEditViewModel()
   {
   }

   public CredentialEditViewModel(Credential credential)
   {
      EditMode = EditModes.Edit;
      Id = credential.Id;
      IssuingOrganization = credential.IssuingOrganization;
      Name = credential.Name;
      Type = credential.Type;
      AwardedMonth = credential.AwardedMonth;
      AwardedYear = credential.AwardedYear;
      DateDisplayText = credential.DateDisplayText;
      Description = credential.Description;
      CredentialUrl = credential.CredentialUrl;
      SortOrder = credential.SortOrder;
      IsActive = credential.IsActive;
   }
}
