// Copyright © 2026 David Browning. All rights reserved.
//
// Source-available for viewing only. No license granted.

using System;
using System.ComponentModel.DataAnnotations;

using DavidBrowning.Models;
using DavidBrowning.Models.Work;

namespace DavidBrowning.Admin.ViewModels.Work;

public sealed class ExperienceRoleEditViewModel
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

   [Range(0, int.MaxValue)]
   public int SortOrder { get; set; }

   public bool IsActive { get; set; } = true;

   public DateTime? CreatedAtUtc { get; init; }

   public DateTime? UpdatedAtUtc { get; init; }

   public ExperienceRoleEditViewModel()
   {
   }

   public ExperienceRoleEditViewModel(ExperienceRole role)
   {
      EditMode = EditModes.Edit;
      Id = role.Id;
      ExperienceId = role.ExperienceId;
      Title = role.Title;
      DateDisplayText = role.DateDisplayText;
      Description = role.Description;
      SortOrder = role.SortOrder;
      IsActive = role.IsActive;
      CreatedAtUtc = role.CreatedAtUtc;
      UpdatedAtUtc = role.UpdatedAtUtc;
   }
}
