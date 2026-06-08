// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using DavidBrowning.Models;
using DavidBrowning.Models.Projects;

namespace DavidBrowning.Admin.ViewModels.Projects;

public class ProjectLinkEditViewModel
{
   public required EditModes EditMode { get; init; }

   [Required, Key]
   public int? Id { get; set; }

   /// <summary>
   /// Foreign key to db_ProjectLinkTypes.
   /// </summary>
   [Required]
   public int? ProjectLinkTypeId { get; set; }

   /// <summary>
   /// User-facing label for this link.
   /// Example: "GitHub", "Live Demo", "Download".
   /// </summary>
   [Required]
   [StringLength(DataConstants.MaxLabelLength)]
   public string? Label { get; set; }

   /// <summary>
   /// Destination URL.
   /// </summary>
   [Required]
   [StringLength(DataConstants.MaxUrlLength)]
   public string? Url { get; set; }

   /// <summary>
   /// Manual display ordering for links within a project.
   /// </summary>
   public int SortOrder { get; set; } = 0;

   public ProjectLinkEditViewModel()
   {

   }

   [SetsRequiredMembers]
   public ProjectLinkEditViewModel(ProjectLink link)
   {
      EditMode = EditModes.Edit;
      Id = link.Id;
      Label = link.Label;
      Url = link.Url;
      SortOrder = link.SortOrder;
      ProjectLinkTypeId = link.ProjectLinkTypeId;
   }
}
