// Copyright © 2026 David Browning. All rights reserved.
//
// Source-available for viewing only. No license granted.

using System.ComponentModel.DataAnnotations;

using DavidBrowning.Models;
using DavidBrowning.Models.Projects;

namespace DavidBrowning.Admin.ViewModels.Projects;

public sealed class ProjectOriginEditViewModel
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

   public ProjectOriginEditViewModel()
   {
   }

   public ProjectOriginEditViewModel(ProjectOrigin origin)
   {
      EditMode = EditModes.Edit;
      Id = origin.Id;
      Slug = origin.Slug;
      DisplayName = origin.DisplayName;
      Description = origin.Description;
      SortOrder = origin.SortOrder;
      IsActive = origin.IsActive;
   }
}
