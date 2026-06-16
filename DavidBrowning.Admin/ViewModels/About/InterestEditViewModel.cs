// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using DavidBrowning.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DavidBrowning.Admin.ViewModels.About;

public sealed class InterestEditViewModel
{
   public EditModes EditMode { get; set; } = EditModes.Create;

   public int? Id { get; set; }

   [Required]
   [StringLength(DataConstants.MaxSlugLength)]
   [RegularExpression(DataConstants.SlugRegex,
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

   public int? FeaturedPostId { get; set; }

   [BindNever]
   [ValidateNever]
   public IReadOnlyList<SelectListItem> FeaturedPostOptions { get; set; } =
      Array.Empty<SelectListItem>();

   public InterestEditViewModel()
   {
   }

   [SetsRequiredMembers]
   public InterestEditViewModel(
      Interest interest,
      FontAwesomeIconPickerViewModel picker,
      IReadOnlyList<SelectListItem> featuredPostOptions)
   {
      EditMode = EditModes.Edit;
      Id = interest.Id;
      Slug = interest.Slug;
      DisplayName = interest.DisplayName;
      Summary = interest.Summary;
      IsActive = interest.IsActive;
      SelectedIconCssClass = interest.IconCssClass;
      FeaturedPostId = interest.FeaturedPostId;
      IconPicker = picker;
      FeaturedPostOptions = featuredPostOptions;
   }

   public Interest ToInterest()
   {
      ArgumentException.ThrowIfNullOrEmpty(Slug, nameof(Slug));
      ArgumentException.ThrowIfNullOrEmpty(DisplayName, nameof(DisplayName));
      ArgumentException.ThrowIfNullOrEmpty(Summary, nameof(Summary));

      return new()
      {
         Id = Id ?? 0,
         Slug = Slug,
         DisplayName = DisplayName,
         Summary = Summary,
         IsActive = IsActive,
         IconCssClass = SelectedIconCssClass,
         FeaturedPostId = FeaturedPostId
      };
   }
}
