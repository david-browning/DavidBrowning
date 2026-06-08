// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using DavidBrowning.Models;

namespace DavidBrowning.Admin.ViewModels.About;

public class InterestEditViewModel
{
   public required EditModes EditMode { get; init; }

   public int? Id { get; set; } = null;

   /// <summary>
   /// URL-friendly identifier for the interest.
   /// Example: "software-systems".
   /// </summary>
   [Required]
   [StringLength(DataConstants.MaxSlugLength)]
   [RegularExpression(
      DataConstants.SlugRegex, 
      ErrorMessage = DataConstants.SlugRegexError)]
   public string? Slug { get; set; }

   /// <summary>
   /// User-facing interest name.
   /// Example: "Software Systems".
   /// </summary>
   [Required]
   [StringLength(DataConstants.MaxLabelLength)]
   public string? DisplayName { get; set; }

   /// <summary>
   /// Short card/list explanation of why this interest matters.
   /// This should explain more than the topic; it should explain the pull.
   /// </summary>
   [Required]
   [StringLength(DataConstants.MaxInterestSummaryLength)]
   public string? Summary { get; set; }

   /// <summary>
   /// Whether this interest should be shown publicly.
   /// </summary>
   public bool IsActive { get; set; } = true;

   /// <summary>
   /// When the interest record was created.
   /// Stored in UTC.
   /// </summary>
   [Required]
   public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

   /// <summary>
   /// When the interest record was last updated.
   /// Stored in UTC.
   /// </summary>
   [Required]
   public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

   /// <summary>
   /// Manual ordering for interest lists.
   /// Lower numbers appear earlier.
   /// </summary>
   [Range(0, int.MaxValue)]
   public int SortOrder { get; set; }

   /// <summary>
   /// Optional Font Awesome or site-specific icon CSS class.
   /// Example: "fa-solid fa-diagram-project".
   /// </summary>
   [StringLength(DataConstants.MaxIconCssClassLength)]
   public string? IconCssClass { get; set; }

   public InterestEditViewModel()
   {
   }

   [SetsRequiredMembers]
   public InterestEditViewModel(Interest interest)
   {
      EditMode = EditModes.Edit;
      Id = interest.Id;
      Slug = interest.Slug;
      DisplayName = interest.DisplayName;
      Summary = interest.Summary;
      IsActive = interest.IsActive;
      SortOrder = interest.SortOrder;
      UpdatedAtUtc = interest.UpdatedAtUtc;
      CreatedAtUtc = interest.CreatedAtUtc;
      IconCssClass = interest.IconCssClass;
   }
}
