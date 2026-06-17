// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using DavidBrowning.Models;

namespace DavidBrowning.Admin.ViewModels.Projects;

public sealed class ProjectEditPageViewModel
{
   public required ProjectMetadataViewModel Metadata { get; set; }

   public required ProjectContentEditViewModel Content { get; set; }

   public required AssetChooserViewModel AssetChooser { get; set; }

   public RenderedContent? ContentPreview { get; set; }
}