// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using DavidBrowning.Models;
using DavidBrowning.Models.Projects;

namespace DavidBrowning.Admin.ViewModels.Projects;

public class ProjectTagEditViewModel
{
   public required EditModes EditMode { get; init; }

   public int? Id { get; set; }

   [Required]
   [StringLength(DataConstants.MaxSlugLength)]
   [RegularExpression(DataConstants.SlugRegex,
      ErrorMessage = DataConstants.SlugRegexError)]
   public string? Slug { get; set; }

   [Required]
   [StringLength(DataConstants.MaxLabelLength)]
   public string? DisplayName { get; set; }

   [StringLength(DataConstants.MaxMetadataLength)]
   public string? Description { get; set; }

   public int SortOrder { get; set; } = 0;

   public bool IsActive { get; set; } = true;

   public ProjectTagEditViewModel()
   {

   }

   [SetsRequiredMembers]
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
