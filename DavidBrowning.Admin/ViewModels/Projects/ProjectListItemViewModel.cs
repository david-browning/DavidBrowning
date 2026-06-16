// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using DavidBrowning.Models.Projects;

namespace DavidBrowning.Admin.ViewModels.Projects;

public sealed class ProjectListItemViewModel
{
   public ProjectListItemViewModel(Project project)
   {
      Id = project.Id;
      Slug = project.Slug;
      Name = project.Name;
      Status = project.ProjectStatus?.DisplayName;
      Origin = project.ProjectOrigin?.DisplayName;
      Type = project.ProjectType?.DisplayName;
      Visibility = project.ProjectVisibility?.DisplayName;
      IsFeatured = project.IsFeatured;
      SortOrder = project.SortOrder;
   }

   public int Id { get; }

   public string Slug { get; }

   public string Name { get; }

   public string? Status { get; }

   public string? Origin { get; }

   public string? Type { get; }

   public string? Visibility { get; }

   public bool IsFeatured { get; }

   public int SortOrder { get; }
}