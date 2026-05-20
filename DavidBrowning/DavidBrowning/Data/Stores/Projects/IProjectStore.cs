// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Models.Projects;

namespace DavidBrowning.Data.Stores.Projects
{
   public interface IProjectStore
   {
      Task<IReadOnlyList<Project>> GetPublishedProjectsAsync(
         CancellationToken cancellationToken = default);

      Task<IReadOnlyList<Project>> GetFeaturedProjectsAsync(
         CancellationToken cancellationToken = default);

      Task<Project?> GetPublishedProjectBySlugAsync(
         string slug,
         CancellationToken cancellationToken = default);

      Task<IReadOnlyList<Project>> GetPublishedProjectsByTagSlugAsync(
         string tagSlug,
         CancellationToken cancellationToken = default);

      Task<IReadOnlyList<Project>> GetPublishedProjectsByStackTagSlugAsync(
         string stackTagSlug,
         CancellationToken cancellationToken = default);

      Task<IReadOnlyList<ProjectTag>> GetProjectTagsAsync(
         CancellationToken cancellationToken = default);

      Task<IReadOnlyList<ProjectStackTag>> GetProjectStackTagsAsync(
         CancellationToken cancellationToken = default);

      Task<IReadOnlyList<ProjectStatus>> GetProjectStatusesAsync(
         CancellationToken cancellationToken = default);

      Task<IReadOnlyList<ProjectType>> GetProjectTypesAsync(
         CancellationToken cancellationToken = default);

      Task<IReadOnlyList<ProjectOrigin>> GetProjectOriginsAsync(
         CancellationToken cancellationToken = default);

      Task<IReadOnlyList<ProjectVisibility>> GetProjectVisibilitiesAsync(
         CancellationToken cancellationToken = default);
   }
}
