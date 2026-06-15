// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using System.ComponentModel.DataAnnotations;
using DavidBrowning.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace DavidBrowning.Admin.ViewModels.Projects;

public sealed class ProjectLookupEditViewModel
{
   public int? Id { get; set; }

   [Required]
   [RegularExpression(DataConstants.SlugRegex,
      ErrorMessage = DataConstants.SlugRegexError)]
   [StringLength(DataConstants.MaxSlugLength)]
   public string? Slug { get; set; }

   [Required]
   [StringLength(DataConstants.MaxLabelLength)]
   public string? DisplayName { get; set; }

   [StringLength(DataConstants.MaxMetadataLength)]
   public string? Description { get; set; }

   public int SortOrder { get; set; }

   public bool IsActive { get; set; } = true;

   [BindNever]
   [ValidateNever]
   public string Title { get; set; } = string.Empty;

   [BindNever]
   [ValidateNever]
   public string FormAction { get; set; } = string.Empty;

   [BindNever]
   [ValidateNever]
   public string RefreshRegionId { get; set; } = string.Empty;
}