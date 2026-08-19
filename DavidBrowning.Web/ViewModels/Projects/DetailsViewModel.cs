// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DavidBrowning.Models;
using DavidBrowning.Models.Publishing;

namespace DavidBrowning.Web.ViewModels.Projects;

public class DetailsViewModel
{
   [SetsRequiredMembers]
   public DetailsViewModel(
      PublishedProject project,
      RenderedContent? body,
      SeoMetadataViewModel seo)
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
      AssetBlocks = project.AssetBlocks
         .Select(link => new AssetBlockViewModel(link))
         .ToList();
      RelatedPosts = project.RelatedPosts;
      Body = body;
      Seo = seo;
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

   public required IReadOnlyCollection<PublishedProjectTagLink> TagLinks { get; init; }
   public required IReadOnlyCollection<PublishedProjectStackTagLink> StackTagLinks { get; init; }
   public required IReadOnlyCollection<PublishedProjectLink> Links { get; init; }
   public required IReadOnlyCollection<AssetBlockViewModel> AssetBlocks { get; init; }
   public required IReadOnlyCollection<PublishedProjectPost> RelatedPosts { get; init; }

   public required SeoMetadataViewModel Seo { get; init; }
}
