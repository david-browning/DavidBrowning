// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using DavidBrowning.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

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

   [Required]
   [StringLength(DataConstants.MaxIconCssClassLength)]
   public string? SelectedIconCssClass { get; set; }

   [BindNever]
   [ValidateNever]
   public required FontAwesomeIconPickerViewModel IconPicker { get; set; }

   public InterestEditViewModel()
   {
   }

   [SetsRequiredMembers]
   public InterestEditViewModel(
      Interest interest,
      FontAwesomeIconPickerViewModel picker)
   {
      EditMode = EditModes.Edit;
      Id = interest.Id;
      Slug = interest.Slug;
      DisplayName = interest.DisplayName;
      Summary = interest.Summary;
      IsActive = interest.IsActive;
      SortOrder = interest.SortOrder;
      SelectedIconCssClass = interest.IconCssClass;
      IconPicker = picker;
   }
}
