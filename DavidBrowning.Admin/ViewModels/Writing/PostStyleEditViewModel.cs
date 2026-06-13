// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using DavidBrowning.Models;
using DavidBrowning.Models.Writing;

namespace DavidBrowning.Admin.ViewModels.Writing;

public sealed class PostStyleEditViewModel
{
   public EditModes EditMode { get; set; } = EditModes.Create;

   public int? Id { get; set; }

   [Required]
   [StringLength(DataConstants.MaxLabelLength)]
   public string? DisplayName { get; set; }

   [Required]
   [StringLength(DataConstants.MaxSlugLength)]
   [RegularExpression(DataConstants.SlugRegex,
      ErrorMessage = DataConstants.SlugRegexError)]
   public string? Slug { get; set; }

   [StringLength(DataConstants.MaxMetadataLength)]
   public string? Description { get; set; }

   [Range(0, int.MaxValue)]
   public int SortOrder { get; set; }

   public bool IsActive { get; set; } = true;

   public PostStyleEditViewModel()
   {
   }

   [SetsRequiredMembers]
   public PostStyleEditViewModel(PostStyle style)
   {
      EditMode = EditModes.Edit;
      Id = style.Id;
      DisplayName = style.DisplayName;
      Slug = style.Slug;
      Description = style.Description;
      SortOrder = style.SortOrder;
      IsActive = style.IsActive;
   }

   public PostStyle ToPostStyle()
   {
      ArgumentNullException.ThrowIfNullOrEmpty(DisplayName);
      ArgumentNullException.ThrowIfNullOrEmpty(Slug);

      return new PostStyle()
      {
         Id = Id ?? 0,
         Description = Description,
         DisplayName = DisplayName,
         Slug = Slug,
         SortOrder = SortOrder,
         IsActive = IsActive,
      };
   }
}
