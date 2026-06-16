// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

namespace DavidBrowning.Admin.ViewModels.Projects;

public sealed class ProjectAdminIndexViewModel
{
   public required ProjectLookupPanelViewModel Statuses { get; set; }

   public required ProjectLookupPanelViewModel Origins { get; set; }

   public required ProjectLookupPanelViewModel Types { get; set; }

   public required ProjectLookupPanelViewModel Visibilities { get; set; }

   public required ProjectLookupPanelViewModel Tags { get; set; }

   public required ProjectLookupPanelViewModel StackTags { get; set; }

   public required ProjectListViewModel Projects { get; set; }

   public required AdminOffcanvasViewModel LookupEditorOffcanvas { get; set; }
}