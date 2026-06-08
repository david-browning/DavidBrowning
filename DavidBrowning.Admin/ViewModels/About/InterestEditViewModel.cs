// Copyright © 2026 David Browning. All rights reserved.
//
// Source-available for viewing only. No license granted.

using System;
using System.ComponentModel.DataAnnotations;

using DavidBrowning.Models;

namespace DavidBrowning.Admin.ViewModels.About;

public sealed class InterestEditViewModel
{
   public EditModes EditMode { get; set; } = EditModes.Create;

   public int? Id { get; set; }

   [Required]
   [StringLength(DataConstants.MaxSlugLength)]
   [RegularExpression(
      DataConstants.SlugRegex,
      ErrorMessage = DataConstants.SlugRegexError)]
   public string? Slug { get; set; }

   [Required]
   [StringLength(DataConstants.MaxLabelLength)]
   public string? DisplayName { get; set; }

   [Required]
   [StringLength(DataConstants.MaxInterestSummaryLength)]
   public string? Summary { get; set; }

   public bool IsActive { get; set; } = true;

   [Range(0, int.MaxValue)]
   public int SortOrder { get; set; }

   [StringLength(DataConstants.MaxIconCssClassLength)]
   public string? IconCssClass { get; set; }

   // Display-only metadata. Do not trust posted values when saving.
   public DateTime? CreatedAtUtc { get; init; }

   // Display-only metadata. Do not trust posted values when saving.
   public DateTime? UpdatedAtUtc { get; init; }

   public InterestEditViewModel()
   {
   }

   public InterestEditViewModel(Interest interest)
   {
      EditMode = EditModes.Edit;
      Id = interest.Id;
      Slug = interest.Slug;
      DisplayName = interest.DisplayName;
      Summary = interest.Summary;
      IsActive = interest.IsActive;
      SortOrder = interest.SortOrder;
      IconCssClass = interest.IconCssClass;
      CreatedAtUtc = interest.CreatedAtUtc;
      UpdatedAtUtc = interest.UpdatedAtUtc;
   }
}
