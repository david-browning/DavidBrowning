// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Collections.Generic;
using DavidBrowning.Models.Publishing;

namespace DavidBrowning.Infrastructure.Data.Stores;

public interface IPublishedSiteStore
{
   Task<IReadOnlyList<PublishedInterest>> GetInterestsAsync(
      CancellationToken cancellationToken = default);

   Task<IReadOnlyList<PublishedExperience>> GetExperienceAsync(
      CancellationToken cancellationToken = default);

   Task<IReadOnlyList<PublishedCredential>> GetCredentialsAsync(
      CancellationToken cancellationToken = default);

   Task<IReadOnlyList<PublishedProject>> GetProjectsAsync(
      CancellationToken cancellationToken = default);

   Task<IReadOnlyList<PublishedProject>> GetFeaturedProjectsAsync(
      CancellationToken cancellationToken = default);

   Task<IReadOnlyList<PublishedProject>> GetFeaturedWorkProjectsAsync(
      CancellationToken cancellationToken = default);

   Task<PublishedProject?> GetProjectBySlugAsync(
      string slug,
      CancellationToken cancellationToken = default);

   Task<IReadOnlyList<PublishedLookup>> GetAllProjectTagsAsync(
      CancellationToken cancellationToken = default);

   Task<IReadOnlyList<PublishedLookup>> GetAllProjectStackTagsAsync(
      CancellationToken cancellationToken = default);
   
   Task<IReadOnlyList<PublishedProject>> GetPublishedProjectsByTagSlugAsync(
      string slug,
      CancellationToken cancellationToken = default);

   Task<IReadOnlyList<PublishedProject>> GetPublishedProjectsByStackTagSlugAsync(
      string slug,
      CancellationToken cancellationToken = default);

   Task<IReadOnlyList<PublishedProject>> GetPublishedProjectsByStatusSlugAsync(
      string slug,
      CancellationToken cancellationToken = default);

   Task<IReadOnlyList<PublishedProject>> GetPublishedProjectsByOriginSlugAsync(
      string slug,
      CancellationToken cancellationToken = default);

   Task<IReadOnlyList<PublishedProject>> GetPublishedProjectsByTypeSlugAsync(
      string slug,
      CancellationToken cancellationToken = default);

   Task<PagedResult<PublishedWriting>> GetWritingPageAsync(
      int page,
      int pageSize,
      CancellationToken cancellationToken = default);

   Task<IReadOnlyList<PublishedWriting>> GetFeaturedWritingsAsync(
      CancellationToken cancellationToken = default);

   Task<IReadOnlyList<PublishedWriting>> GetPublishedWritingsAsync(
      CancellationToken cancellationToken = default);

   Task<IReadOnlyList<PublishedWriting>> GetPublishedWritingsByTagSlugAsync(
      string slug,
      CancellationToken cancellationToken = default);

   Task<IReadOnlyList<PublishedLookup>> GetAllWritingTagsAsync(
      CancellationToken cancellationToken = default);

   Task<PublishedWriting?> GetWritingBySlugAsync(
      string slug,
      CancellationToken cancellationToken = default);

   Task<PublishedLookup?> GetProjectTagAsync(
      string slug,
      CancellationToken cancellationToken = default);

   Task<PublishedLookup?> GetProjectStackTagAsync(
      string slug,
      CancellationToken cancellationToken = default);

   Task<PublishedLookup?> GetProjectStatusAsync(
      string slug,
      CancellationToken cancellationToken = default);

   Task<PublishedLookup?> GetProjectOriginAsync(
      string slug,
      CancellationToken cancellationToken = default);

   Task<PublishedLookup?> GetProjectTypeAsync(
      string slug,
      CancellationToken cancellationToken = default);

   Task<PublishedLookup?> GetWritingTagAsync(
      string slug,
      CancellationToken cancellationToken = default);

   Task WarmupAsync(CancellationToken cancellationToken = default);
}
