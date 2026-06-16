// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using DavidBrowning.Models.Projects;

namespace DavidBrowning.Infrastructure.Data.Stores;

public interface IProjectStore
{
   Task<IReadOnlyList<Project>> GetPublishedProjectsAsync(
      CancellationToken cancellationToken = default);

   Task<PagedResult<Project>> GetPagedPublishedProjectsAsync(
      int page,
      int pageSize,
      CancellationToken cancellationToken = default);

   /// <summary>
   /// Get public projects that are marked with IsFeatued.
   /// </summary>
   /// <param name="cancellationToken"></param>
   /// <returns></returns>
   Task<IReadOnlyList<Project>> GetFeaturedProjectsAsync(
      CancellationToken cancellationToken = default);

   Task<IReadOnlyList<Project>> GetFeaturedWorkProjectsAsync(
      CancellationToken cancellationToken = default);

   /// <summary>
   /// Assume the slug is normalized.
   /// </summary>
   /// <param name="normalizedSlug"></param>
   /// <param name="cancellationToken"></param>
   /// <returns></returns>
   Task<Project?> GetPublishedProjectBySlugAsync(
      string normalizedSlug,
      CancellationToken cancellationToken = default);

   /// <summary>
   /// Assume the slug is normalized.
   /// </summary>
   /// <param name="normalizedStatusSlug"></param>
   /// <param name="cancellationToken"></param>
   /// <returns></returns>
   Task<IReadOnlyList<Project>> GetPublishedProjectsByStatusSlugAsync(
      string normalizedStatusSlug,
      CancellationToken cancellationToken = default);

   /// <summary>
   /// Assume the slug is normalized.
   /// </summary>
   /// <param name="statusSlug"></param>
   /// <param name="cancellationToken"></param>
   /// <returns></returns>
   Task<IReadOnlyList<Project>> GetPublishedProjectsByOriginSlugAsync(
      string normalizedOriginSlug,
      CancellationToken cancellationToken = default);

   Task<IReadOnlyList<Project>> GetPublishedProjectsByTypeSlugAsync(
      string normalizedTypeSlug,
      CancellationToken cancellationToken = default);

   /// <summary>
   /// Assume the slug is normalized.
   /// </summary>
   /// <param name="normalizedTagSlug"></param>
   /// <param name="cancellationToken"></param>
   /// <returns></returns>
   Task<IReadOnlyList<Project>> GetPublishedProjectsByTagSlugAsync(
      string normalizedTagSlug,
      CancellationToken cancellationToken = default);

   /// <summary>
   /// Assume the slug is normalized.
   /// </summary>
   /// <param name="normalizedStackTagSlug"></param>
   /// <param name="cancellationToken"></param>
   /// <returns></returns>
   Task<IReadOnlyList<Project>> GetPublishedProjectsByStackTagSlugAsync(
      string normalizedStackTagSlug,
      CancellationToken cancellationToken = default);

   /// <summary>
   /// Gets all project tags.
   /// </summary>
   /// <param name="cancellationToken"></param>
   /// <returns></returns>
   Task<IReadOnlyList<ProjectTag>> GetProjectTagsAsync(
      CancellationToken cancellationToken = default);

   /// <summary>
   /// Gets all project stack tags.
   /// </summary>
   /// <param name="cancellationToken"></param>
   /// <returns></returns>
   Task<IReadOnlyList<ProjectStackTag>> GetProjectStackTagsAsync(
      CancellationToken cancellationToken = default);

   /// <summary>
   /// Gets all project statuses.
   /// </summary>
   /// <param name="cancellationToken"></param>
   /// <returns></returns>
   Task<IReadOnlyList<ProjectStatus>> GetProjectStatusesAsync(
      CancellationToken cancellationToken = default);

   /// <summary>
   /// Gets all project types.
   /// </summary>
   /// <param name="cancellationToken"></param>
   /// <returns></returns>
   Task<IReadOnlyList<ProjectType>> GetProjectTypesAsync(
      CancellationToken cancellationToken = default);

   /// <summary>
   /// Gets all project origins.
   /// </summary>
   /// <param name="cancellationToken"></param>
   /// <returns></returns>
   Task<IReadOnlyList<ProjectOrigin>> GetProjectOriginsAsync(
      CancellationToken cancellationToken = default);

   /// <summary>
   /// Gets all project visibilities.
   /// </summary>
   /// <param name="cancellationToken"></param>
   /// <returns></returns>
   Task<IReadOnlyList<ProjectVisibility>> GetProjectVisibilitiesAsync(
      CancellationToken cancellationToken = default);

   Task<Project?> GetProjectAsync(
      int id,
      CancellationToken cancellationToken = default);

   Task<int> InsertProjectAsync(
      Project project, 
      IList<int> projectTags,
      IList<int> stackIds,
      CancellationToken cancellationToken = default);

   Task<bool> UpdateProjectAsync(
      Project project,
      IList<int> projectTags,
      IList<int> stackIds,
      CancellationToken cancellationToken = default);

   Task<ProjectContentData?> GetProjectContentAsync(
      int projectId,
      CancellationToken cancellationToken = default);

   Task<bool> UpdateProjectContentAsync(
      int projectId,
      string? content,
      IReadOnlyList<ProjectAssetLink> assetLinks,
      CancellationToken cancellationToken = default);

   Task<IReadOnlyList<Project>> GetProjectsAsync(
      CancellationToken cancellationToken = default);

   Task<int?> GetRequiredProjectAssetRoleIdAsync(
      string slug,
      CancellationToken cancellationToken = default);

   Task<ProjectStatus?> GetProjectStatusAsync(
      int id, 
      CancellationToken token = default);

   Task<ProjectOrigin?> GetProjectOriginAsync(
      int id, 
      CancellationToken token = default);
   
   Task<ProjectType?> GetProjectTypeAsync(
      int id, 
      CancellationToken token = default);
   
   Task<ProjectVisibility?> GetProjectVisibilityAsync(
      int id, 
      CancellationToken token = default);
   
   Task<ProjectTag?> GetProjectTagAsync(
      int id, 
      CancellationToken token = default);
   
   Task<ProjectStackTag?> GetProjectStackTagAsync(
      int id, 
      CancellationToken token = default);

   Task InsertProjectStatusAsync(
      ProjectStatus status,
      CancellationToken cancellationToken = default);

   Task InsertProjectVisibilityAsync(
      ProjectVisibility visibility,
      CancellationToken cancellationToken = default);

   Task InsertProjectOriginAsync(
      ProjectOrigin origin,
      CancellationToken cancellationToken = default);

   Task InsertProjectTypeAsync(
      ProjectType type,
      CancellationToken cancellationToken = default);

   Task InsertProjectTagAsync(
      ProjectTag tag,
      CancellationToken cancellationToken = default);

   Task InsertProjectStackTagAsync(
      ProjectStackTag tag,
      CancellationToken cancellationToken = default);

   Task<bool> UpdateProjectStatusAsync(
      ProjectStatus status,
      CancellationToken cancellationToken = default);

   Task<bool> UpdateProjectOriginAsync(
      ProjectOrigin origin,
      CancellationToken cancellationToken = default);

   Task<bool> UpdateProjectTypeAsync(
      ProjectType type,
      CancellationToken cancellationToken = default);

   Task<bool> UpdateProjectVisibilityAsync(
      ProjectVisibility visibility,
      CancellationToken cancellationToken = default);

   Task<bool> UpdateProjectTagAsync(
      ProjectTag tag,
      CancellationToken cancellationToken = default);

   Task<bool> UpdateProjectStackTagAsync(
      ProjectStackTag tag,
      CancellationToken cancellationToken = default);
}

public sealed class ProjectContentData
{
   public int? ContentAssetId { get; init; }

   public string? ContentAssetKey { get; init; }

   public string? Content { get; init; }

   public IReadOnlyList<ProjectAssetLink> AssetLinks { get; init; } =
      Array.Empty<ProjectAssetLink>();
}
