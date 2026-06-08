// Copyright © 2026 David Browning. All rights reserved.
//
// Source-available for viewing only. No license granted.

using System.ComponentModel.DataAnnotations;

using DavidBrowning.Models;
using DavidBrowning.Models.Projects;

namespace DavidBrowning.Admin.ViewModels.Projects;

public sealed class ProjectTagEditViewModel
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

   [StringLength(DataConstants.MaxMetadataLength)]
   public string? Description { get; set; }

   [Range(0, int.MaxValue)]
   public int SortOrder { get; set; }

   public bool IsActive { get; set; } = true;

   public ProjectTagEditViewModel()
   {
   }

   public ProjectTagEditViewModel(ProjectTag tag)
   {
      EditMode = EditModes.Edit;
      Id = tag.Id;
      Slug = tag.Slug;
      DisplayName = tag.DisplayName;
      Description = tag.Description;
      SortOrder = tag.SortOrder;
      IsActive = tag.IsActive;
   }
}
