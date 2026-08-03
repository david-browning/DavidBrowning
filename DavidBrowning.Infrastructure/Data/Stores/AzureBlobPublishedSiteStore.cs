// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using DavidBrowning.Helpers;
using DavidBrowning.Infrastructure.Cache;
using DavidBrowning.Models.Projects;
using DavidBrowning.Models.Publishing;

namespace DavidBrowning.Infrastructure.Data.Stores;

public class AzureBlobPublishedSiteStore : IPublishedSiteStore
{
   public AzureBlobPublishedSiteStore(
      JsonCache jsonCache)
   {
      _jsonCache = jsonCache;
   }

   public async Task<IReadOnlyList<PublishedInterest>> GetInterestsAsync(
      CancellationToken cancellationToken = default)
   {
      var data = await GetSnapshotAsync(cancellationToken);
      return data.Interests;
   }

   public async Task<IReadOnlyList<PublishedExperience>> GetExperienceAsync(
      CancellationToken cancellationToken = default)
   {
      var data = await GetSnapshotAsync(cancellationToken);
      return data.Experience;
   }

   public async Task<IReadOnlyList<PublishedCredential>> GetCredentialsAsync(
      CancellationToken cancellationToken = default)
   {
      var data = await GetSnapshotAsync(cancellationToken);
      return data.Credentials;
   }

   public async Task<IReadOnlyList<PublishedProject>> GetProjectsAsync(
      CancellationToken cancellationToken = default)
   {
      var data = await GetSnapshotAsync(cancellationToken);
      return data.Projects;
   }

   public async Task<IReadOnlyList<PublishedProject>> GetFeaturedProjectsAsync(
      CancellationToken cancellationToken = default)
   {
      var data = await GetSnapshotAsync(cancellationToken);
      return data.Projects.Where(p => p.IsFeatured).ToList();
   }

   public async Task<IReadOnlyList<PublishedProject>> GetFeaturedWorkProjectsAsync(
      CancellationToken cancellationToken = default)
   {
      var data = await GetSnapshotAsync(cancellationToken);
      return data.Projects.Where(
         p => p.ProjectOrigin.Slug.EqualsOrdinalIgnoreCase("professional"))
         .ToList();
   }

   public async Task<PublishedProject?> GetProjectBySlugAsync(
      string slug,
      CancellationToken cancellationToken = default)
   {
      var data = await GetSnapshotAsync(cancellationToken);
      return data.Projects.FirstOrDefault(
         p => p.Slug.EqualsOrdinalIgnoreCase(slug));
   }

   public async Task<IReadOnlyList<PublishedLookup>> GetAllProjectTagsAsync(
      CancellationToken cancellationToken = default)
   {
      var data = await GetSnapshotAsync(cancellationToken);
      return data.AllProjectProjectTags;
   }

   public async Task<IReadOnlyList<PublishedLookup>> GetAllProjectStackTagsAsync(
      CancellationToken cancellationToken = default)
   {
      var data = await GetSnapshotAsync(cancellationToken);
      return data.AllProjectStacks;
   }

   public async Task<IReadOnlyList<PublishedProject>> GetPublishedProjectsByTagSlugAsync(
      string slug,
      CancellationToken cancellationToken = default)
   {
      var data = await GetSnapshotAsync(cancellationToken);
      return data.Projects.Where(
         p => p.TagLinks.Any(t => t.ProjectTag.Slug.EqualsOrdinalIgnoreCase(slug)))
         .ToList();
   }

   public async Task<IReadOnlyList<PublishedProject>> GetPublishedProjectsByStackTagSlugAsync(
      string slug,
      CancellationToken cancellationToken = default)
   {
      var data = await GetSnapshotAsync(cancellationToken);
      return data.Projects.Where(
         p => p.StackTagLinks.Any(t => t.ProjectStackTag.Slug.EqualsOrdinalIgnoreCase(slug)))
         .ToList();
   }

   public async Task<IReadOnlyList<PublishedProject>> GetPublishedProjectsByStatusSlugAsync(
      string slug,
      CancellationToken cancellationToken = default)
   {
      var data = await GetSnapshotAsync(cancellationToken);
      return data.Projects.Where(
         p => p.ProjectStatus.Slug.EqualsOrdinalIgnoreCase(slug)).ToList();
   }

   public async Task<IReadOnlyList<PublishedProject>> GetPublishedProjectsByOriginSlugAsync(
      string slug,
      CancellationToken cancellationToken = default)
   {
      var data = await GetSnapshotAsync(cancellationToken);
      return data.Projects.Where(
         p => p.ProjectOrigin.Slug.EqualsOrdinalIgnoreCase(slug))
         .ToList();
   }

   public async Task<IReadOnlyList<PublishedProject>> GetPublishedProjectsByTypeSlugAsync(
      string slug,
      CancellationToken cancellationToken = default)
   {
      var data = await GetSnapshotAsync(cancellationToken);
      return data.Projects.Where(
         p => p.ProjectType.Slug.EqualsOrdinalIgnoreCase(slug)).ToList();
   }

   public async Task<PagedResult<PublishedWriting>> GetWritingPageAsync(
      int page,
      int pageSize,
      CancellationToken cancellationToken = default)
   {
      if (page < 1)
      {
         throw new ArgumentOutOfRangeException(
            nameof(page), "Page must be greater than or equal to 1.");
      }

      if (pageSize < 1)
      {
         throw new ArgumentOutOfRangeException(
            nameof(pageSize), "Page size must be greater than or equal to 1.");
      }

      var data = await GetSnapshotAsync(cancellationToken);
      var posts = data.Writings.Skip((page - 1) * pageSize).Take(pageSize).ToList();
      return new PagedResult<PublishedWriting>
      {
         Items = posts,
         Page = page,
         PageSize = pageSize,
         TotalCount = data.Writings.Count,
      };
   }

   public async Task<IReadOnlyList<PublishedWriting>> GetFeaturedWritingsAsync(
      CancellationToken cancellationToken = default)
   {
      var data = await GetSnapshotAsync(cancellationToken);
      return data.Writings.Where(w => w.IsFeatured).ToList();
   }

   public async Task<IReadOnlyList<PublishedWriting>> GetPublishedWritingsAsync(
      CancellationToken cancellationToken = default)
   {
      var data = await GetSnapshotAsync(cancellationToken);
      return data.Writings;
   }

   public async Task<IReadOnlyList<PublishedWriting>> GetPublishedWritingsByTagSlugAsync(
      string slug,
      CancellationToken cancellationToken = default)
   {
      var data = await GetSnapshotAsync(cancellationToken);
      return data.Writings
         .Where(writing => writing.Tags.Any(tag =>
            tag.WritingTag.Slug.EqualsOrdinalIgnoreCase(slug)))
         .ToList();
   }

   public async Task<IReadOnlyList<PublishedLookup>> GetAllWritingTagsAsync(
      CancellationToken cancellationToken = default)
   {
      var data = await GetSnapshotAsync(cancellationToken);
      return data.AllWritingTags;
   }

   public async Task<PublishedWriting?> GetWritingBySlugAsync(
      string slug,
      CancellationToken cancellationToken = default)
   {
      var data = await GetSnapshotAsync(cancellationToken);
      return data.Writings.FirstOrDefault(
         w => w.Slug.EqualsOrdinalIgnoreCase(slug));
   }

   public async Task WarmupAsync(CancellationToken cancellationToken = default)
   {
      await GetSnapshotAsync(cancellationToken);
   }

   private async Task<PublishedSiteSnapshot> GetSnapshotAsync(
      CancellationToken cancellationToken = default)
   {
      var manifest = await _jsonCache.GetJsonFileContentAsync<PublishedSiteManifest>(
         FixedAssetKeys.SiteManifestKey, cancellationToken);
      var siteKey = manifest.SnapshotKey;
      return await _jsonCache.GetJsonFileContentAsync<PublishedSiteSnapshot>(
         siteKey, cancellationToken);
   }

   private readonly JsonCache _jsonCache;
}
