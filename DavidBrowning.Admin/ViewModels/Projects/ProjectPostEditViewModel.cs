// Copyright © 2026 David Browning. All rights reserved.
//
// Source-available for viewing only. No license granted.

using System.ComponentModel.DataAnnotations;

using DavidBrowning.Models;
using DavidBrowning.Models.Projects;

namespace DavidBrowning.Admin.ViewModels.Projects;

// ProjectPost uses a composite key: (ProjectId, PostId).
// Treat those key fields as immutable while editing.
public sealed class ProjectPostEditViewModel
{
   public EditModes EditMode { get; set; } = EditModes.Create;

   [Range(1, int.MaxValue)]
   public int ProjectId { get; set; }

   [Range(1, int.MaxValue)]
   public int PostId { get; set; }

   [StringLength(DataConstants.MaxLabelLength)]
   public string? RelationshipLabel { get; set; }

   [Range(0, int.MaxValue)]
   public int SortOrder { get; set; }

   public ProjectPostEditViewModel()
   {
   }

   public ProjectPostEditViewModel(ProjectPost link)
   {
      EditMode = EditModes.Edit;
      ProjectId = link.ProjectId;
      PostId = link.PostId;
      RelationshipLabel = link.RelationshipLabel;
      SortOrder = link.SortOrder;
   }
}
