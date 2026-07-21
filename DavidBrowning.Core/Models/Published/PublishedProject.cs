// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using System.Diagnostics.CodeAnalysis;
using DavidBrowning.Models.Projects;
using DavidBrowning.Models.Writing;

namespace DavidBrowning.Models.Published;

public sealed class PublishedProject
{
   public required string Slug { get; set; }

   public required string Name { get; set; }

   public string? Description { get; set; }

   public required PublishedLookup ProjectStatus { get; set; }

   public required PublishedLookup ProjectType { get; set; }

   public required PublishedLookup ProjectOrigin { get; set; }

   public string? Role { get; set; }

   public string? ContributionSummary { get; set; }

   public bool IsFeatured { get; set; }

   public int SortOrder { get; set; }

   public DateOnly? StartDate { get; set; }

   public DateOnly? EndDate { get; set; }

   public string? DateDisplayText { get; set; }

   public DateTime UpdatedAtUtc { get; set; }

   public IReadOnlyList<PublishedProjectTagLink> TagLinks { get; set; } =
      Array.Empty<PublishedProjectTagLink>();

   public IReadOnlyList<PublishedProjectStackTagLink> StackTagLinks { get; set; } =
      Array.Empty<PublishedProjectStackTagLink>();

   public IReadOnlyList<PublishedProjectLink> Links { get; set; } =
      Array.Empty<PublishedProjectLink>();

   public PublishedTextContent? Content { get; set; }

   public IReadOnlyList<PublishedAssetBlock> AssetBlocks { get; set; } =
      Array.Empty<PublishedAssetBlock>();

   public IReadOnlyList<PublishedProjectPost> RelatedPosts { get; set; } =
      Array.Empty<PublishedProjectPost>();

   public PublishedProject()
   {

   }

   [SetsRequiredMembers]
   public PublishedProject(Project project)
   {
      ArgumentNullException.ThrowIfNull(project);

      Slug = project.Slug;
      Name = project.Name;
      Description = project.Description;
      ProjectStatus = new PublishedLookup(
         project.ProjectStatus ?? throw MissingNavigation(
            project, nameof(project.ProjectStatus)));
      ProjectType = new PublishedLookup(
         project.ProjectType ?? throw MissingNavigation(
            project, nameof(project.ProjectType)));
      ProjectOrigin = new PublishedLookup(
         project.ProjectOrigin ?? throw MissingNavigation(
            project, nameof(project.ProjectOrigin)));
      Role = project.Role;
      ContributionSummary = project.ContributionSummary;
      IsFeatured = project.IsFeatured;
      SortOrder = project.SortOrder;
      StartDate = project.StartDate;
      EndDate = project.EndDate;
      DateDisplayText = project.DateDisplayText;
      UpdatedAtUtc = project.UpdatedAtUtc;

      TagLinks = project.TagLinks
         .Where(link => link.ProjectTag?.IsActive == true)
         .OrderBy(link => link.ProjectTag!.SortOrder)
         .ThenBy(link => link.ProjectTag!.DisplayName)
         .Select(link => new PublishedProjectTagLink(link))
         .ToArray();

      StackTagLinks = project.StackTagLinks
         .Where(link => link.ProjectStackTag?.IsActive == true)
         .OrderBy(link => link.ProjectStackTag!.SortOrder)
         .ThenBy(link => link.ProjectStackTag!.DisplayName)
         .Select(link => new PublishedProjectStackTagLink(link))
         .ToArray();

      Links = project.Links
         .Where(link => link.ProjectLinkType?.IsActive == true)
         .OrderBy(link => link.SortOrder)
         .ThenBy(link => link.Label)
         .Select(link => new PublishedProjectLink(link))
         .ToArray();

      AssetBlocks = project.AssetLinks
         .Where(link =>
            link.ProjectAssetRole is not null &&
            !link.ProjectAssetRole.Slug.Equals(
               _detailsContentRoleSlug,
               StringComparison.OrdinalIgnoreCase))
         .OrderBy(link => link.ProjectAssetRole!.SortOrder)
         .ThenBy(link => link.SortOrder)
         .Select(link => new PublishedAssetBlock(link))
         .ToArray();

      RelatedPosts = project.RelatedPosts
         .Where(link => link.Post?.Status == PostStatus.Published)
         .OrderBy(link => link.SortOrder)
         .Select(link => new PublishedProjectPost(link))
         .ToArray();
   }

   private static InvalidOperationException MissingNavigation(
      Project project,
      string navigationName)
   {
      return new InvalidOperationException(
         $"Project '{project.Slug}' is missing required navigation " +
         $"property '{navigationName}'.");
   }

   private const string _detailsContentRoleSlug = "details-content";
}
