// Copyright © 2026 David Browning. All rights reserved.
//
// Source-available for viewing only. No license granted.

using System.ComponentModel.DataAnnotations;

using DavidBrowning.Models;
using DavidBrowning.Models.Projects;

namespace DavidBrowning.Admin.ViewModels.Projects;

public sealed class ProjectLinkEditViewModel
{
   public EditModes EditMode { get; set; } = EditModes.Create;

   public int? Id { get; set; }

   [Required]
   [Range(1, int.MaxValue)]
   public int? ProjectId { get; set; }

   [Required]
   [Range(1, int.MaxValue)]
   public int? ProjectLinkTypeId { get; set; }

   [Required]
   [StringLength(DataConstants.MaxLabelLength)]
   public string? Label { get; set; }

   [Required]
   [StringLength(DataConstants.MaxUrlLength)]
   [Url]
   public string? Url { get; set; }

   [Range(0, int.MaxValue)]
   public int SortOrder { get; set; }

   public ProjectLinkEditViewModel()
   {
   }

   public ProjectLinkEditViewModel(ProjectLink link)
   {
      EditMode = EditModes.Edit;
      Id = link.Id;
      ProjectId = link.ProjectId;
      ProjectLinkTypeId = link.ProjectLinkTypeId;
      Label = link.Label;
      Url = link.Url;
      SortOrder = link.SortOrder;
   }
}
