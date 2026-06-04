// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DavidBrowning.Extensions;
using DavidBrowning.Models.Projects;

namespace DavidBrowning.Models.ViewModels.Projects;

public class DetailsViewModel
{
   [SetsRequiredMembers]
   public DetailsViewModel(Project project, RenderedContent? body)
   {
      ProjectName = project.Name;
      Description = project.Description;
      DateDisplayText = project.DateDisplayText;
      ProjectStatus = project.ProjectStatus!.DisplayName;
      ProjectType = project.ProjectType!.DisplayName;
      ProjectOrigin = project.ProjectOrigin!.DisplayName;

      Role = project.Role;
      ContributionSummary = project.ContributionSummary;

      TagLinks = project.TagLinks;
      StackTagLinks = project.StackTagLinks;
      Links = project.Links;
      AssetBlocks = project.AssetLinks
         .Where(link =>
            !link.ProjectAssetRole!.Slug.EqualsOrdinalIgnoreCase(
               "details-content"))
         .OrderBy(link => link.ProjectAssetRole!.SortOrder)
         .ThenBy(link => link.SortOrder)
         .Select(link => new AssetBlockViewModel(link))
         .ToList();
      RelatedPosts = project.RelatedPosts;
      this.Body = body;
   }

   public required string ProjectName { get; init; }
   public string? Description { get; init; }
   public string? DateDisplayText { get; init; }
   public required string ProjectStatus { get; init; }
   public required string ProjectType { get; init; }
   public required string ProjectOrigin { get; init; }


   public string? Role { get; init; }
   public string? ContributionSummary { get; init; }

   public RenderedContent? Body { get; init; }

   public required ICollection<ProjectTagLink> TagLinks { get; init; }
   public required ICollection<ProjectStackTagLink> StackTagLinks { get; init; }
   public required ICollection<ProjectLink> Links { get; init; }
   public required IReadOnlyList<AssetBlockViewModel> AssetBlocks { get; init; }
   public required ICollection<ProjectPost> RelatedPosts { get; init; }
}
