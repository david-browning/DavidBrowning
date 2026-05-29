// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using DavidBrowning.Models.Projects;

namespace DavidBrowning.Models.ViewModels.Projects;

public class DetailsViewModel
{
   [SetsRequiredMembers]
   public DetailsViewModel(Project project)
   {
      ProjectName = project.Name;
      Description = project.Description;
      DateDisplayText = project.DateDisplayText;
      ProjectStatus = project.ProjectStatus!.DisplayName;
      ProjectType = project.ProjectType!.DisplayName;
      ProjectOrigin = project.ProjectOrigin!.DisplayName;

      Role = project.Role;
      ContributionSummary = project.ContributionSummary;

      CaseStudyProblem = project.Problem;
      CaseStudySolution = project.Solution;
      CaseStudyResult = project.Result;
      CaseStudyTradeoffs = project.Tradeoffs;

      TagLinks = project.TagLinks;
      StackTagLinks = project.StackTagLinks;
      Links = project.Links;
      AssetLinks = project.AssetLinks;
      RelatedPosts = project.RelatedPosts;
   }

   public required string ProjectName { get; init; }
   public string? Description { get; init; }
   public string? DateDisplayText { get; init; }
   public required string ProjectStatus { get; init; }
   public required string ProjectType { get; init; }
   public required string ProjectOrigin { get; init; }


   public string? Role { get; init; }
   public string? ContributionSummary { get; init; }

   public string? CaseStudyProblem { get; init; }
   public string? CaseStudySolution { get; init; }
   public string? CaseStudyResult { get; init; }
   public string? CaseStudyTradeoffs { get; init; }

   public required ICollection<ProjectTagLink> TagLinks { get; init; }
   public required ICollection<ProjectStackTagLink> StackTagLinks { get; init; }
   public required ICollection<ProjectLink> Links { get; init; }
   public required ICollection<ProjectAssetLink> AssetLinks { get; init; }
   public required ICollection<ProjectPost> RelatedPosts { get; init; }
}
