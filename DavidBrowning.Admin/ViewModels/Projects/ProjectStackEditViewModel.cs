// Copyright © 2026 David Browning. All rights reserved.
//
// Source-available for viewing only. No license granted.

using System.ComponentModel.DataAnnotations;

using DavidBrowning.Models;
using DavidBrowning.Models.Projects;

namespace DavidBrowning.Admin.ViewModels.Projects;

public sealed class ProjectStackEditViewModel
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

   public ProjectStackEditViewModel()
   {
   }

   public ProjectStackEditViewModel(ProjectStackTag stack)
   {
      EditMode = EditModes.Edit;
      Id = stack.Id;
      Slug = stack.Slug;
      DisplayName = stack.DisplayName;
      Description = stack.Description;
      SortOrder = stack.SortOrder;
      IsActive = stack.IsActive;
   }
}
