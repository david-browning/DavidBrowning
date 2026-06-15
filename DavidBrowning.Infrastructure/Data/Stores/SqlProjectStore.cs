// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using DavidBrowning.Infrastructure.Cache;
using DavidBrowning.Models.Projects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DavidBrowning.Infrastructure.Data.Stores;

public sealed class SqlProjectStore : IProjectStore
{
   public SqlProjectStore(
      ILogger<SqlProjectStore> logger,
      SiteDbContext context,
      ISlugLookupService<ProjectOrigin> originLookup,
      ISlugLookupService<ProjectStatus> statusLookup,
      ISlugLookupService<ProjectVisibility> visibleLookup)
   {
      _logger = logger;
      _dbContext = context;
      _originLookup = originLookup;
      _statusLookup = statusLookup;
      _visibilityLookup = visibleLookup;
   }

   public async Task<IReadOnlyList<Project>> GetPublishedProjectsAsync(
      CancellationToken cancellationToken = default)
   {
      var query = await BuildPublishedProjectQueryAsync(cancellationToken);
      return await query
         .OrderBy(project => project.SortOrder)
         .ThenBy(project => project.Name)
         .ToListAsync(cancellationToken);
   }

   public async Task<IReadOnlyList<Project>> GetFeaturedWorkProjectsAsync(
      CancellationToken cancellationToken = default)
   {
      var workOrigin = await _originLookup.GetIdBySlugAsync(
         "professional", cancellationToken);
      if (workOrigin == null)
      {
         throw new InvalidOperationException(
            "Required project origin 'professional' was not found while " +
            "loading featured work projects.");
      }

      var query = await BuildPublishedProjectQueryAsync(cancellationToken);
      return await query
         .Where(project => project.ProjectOriginId == workOrigin)
         .OrderBy(project => project.SortOrder)
         .ThenBy(project => project.Name)
         .ToListAsync(cancellationToken);
   }

   public async Task<PagedResult<Project>> GetPagedPublishedProjectsAsync(
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

      var query = await BuildPublishedProjectQueryAsync(cancellationToken);
      var totalCount = await query.CountAsync(cancellationToken);

      var projects = await query
         .OrderBy(project => project.SortOrder)
         .ThenBy(project => project.Name)
         .Skip((page - 1) * pageSize)
         .Take(pageSize)
         .ToListAsync(cancellationToken);

      return new PagedResult<Project>
      {
         Items = projects,
         TotalCount = totalCount,
         Page = page,
         PageSize = pageSize
      };
   }

   public async Task<Project?> GetPublishedProjectBySlugAsync(
      string slug,
      CancellationToken cancellationToken = default)
   {
      ArgumentException.ThrowIfNullOrWhiteSpace(slug);
      var publicId = await _visibilityLookup.GetIdBySlugAsync(
        "public", cancellationToken);

      return await _dbContext.Projects
         .AsNoTracking()
         .AsSplitQuery()
         .Where(project => project.Slug == slug)
         .Where(project => project.ProjectVisibilityId == publicId)
         .Include(project => project.ProjectStatus)
         .Include(project => project.ProjectVisibility)
         .Include(project => project.ProjectOrigin)
         .Include(project => project.ProjectType)
         .Include(project => project.AssetLinks.OrderBy(link => link.SortOrder))
            .ThenInclude(link => link.SiteAsset)
         .Include(project => project.AssetLinks)
            .ThenInclude(link => link.ProjectAssetRole)
         .Include(project => project.TagLinks)
            .ThenInclude(link => link.ProjectTag)
         .Include(project => project.StackTagLinks)
            .ThenInclude(link => link.ProjectStackTag)
         .Include(project => project.Links.OrderBy(link => link.SortOrder))
            .ThenInclude(link => link.ProjectLinkType)
         .Include(project => project.RelatedPosts.OrderBy(post => post.SortOrder))
            .ThenInclude(project => project.Post)
               .ThenInclude(post => post!.Tags)
                  .ThenInclude(tag => tag.WritingTag)
         .Include(project => project.RelatedPosts)
            .ThenInclude(project => project.Post)
               .ThenInclude(post => post!.PostStyle)
         .SingleOrDefaultAsync(cancellationToken);
   }

   public async Task<IReadOnlyList<Project>> GetFeaturedProjectsAsync(
      CancellationToken cancellationToken = default)
   {
      var query = await BuildPublishedProjectQueryAsync(cancellationToken);
      return await query
         .Where(project => project.IsFeatured)
         .OrderBy(project => project.SortOrder)
         .ThenBy(project => project.Name)
         .ToListAsync(cancellationToken);
   }

   public async Task<IReadOnlyList<Project>> GetPublishedProjectsByOriginSlugAsync(
      string originSlug,
      CancellationToken cancellationToken = default)
   {
      ArgumentException.ThrowIfNullOrWhiteSpace(originSlug);
      var query = await BuildPublishedProjectQueryAsync(cancellationToken);
      return await query
         .Where(project => project.ProjectOrigin!.Slug == originSlug)
         .OrderBy(project => project.SortOrder)
         .ThenBy(project => project.Name)
         .ToListAsync(cancellationToken);
   }

   public async Task<IReadOnlyList<Project>> GetPublishedProjectsByTypeSlugAsync(
      string typeSlug,
      CancellationToken cancellationToken = default)
   {
      ArgumentException.ThrowIfNullOrWhiteSpace(typeSlug);
      var query = await BuildPublishedProjectQueryAsync(cancellationToken);
      return await query
         .Where(project => project.ProjectType!.Slug == typeSlug)
         .OrderBy(project => project.SortOrder)
         .ThenBy(project => project.Name)
         .ToListAsync(cancellationToken);
   }

   public async Task<IReadOnlyList<Project>> GetPublishedProjectsByStatusSlugAsync(
      string statusSlug,
      CancellationToken cancellationToken = default)
   {
      ArgumentException.ThrowIfNullOrWhiteSpace(statusSlug);
      var query = await BuildPublishedProjectQueryAsync(cancellationToken);
      return await query
         .Where(project => project.ProjectStatus!.Slug == statusSlug)
         .OrderBy(project => project.SortOrder)
         .ThenBy(project => project.Name)
         .ToListAsync(cancellationToken);
   }

   public async Task<IReadOnlyList<Project>> GetPublishedProjectsByTagSlugAsync(
      string tagSlug,
      CancellationToken cancellationToken = default)
   {
      ArgumentException.ThrowIfNullOrWhiteSpace(tagSlug);
      var query = await BuildPublishedProjectQueryAsync(cancellationToken);
      return await query
         .Where(project => project.TagLinks.Any(link =>
            link.ProjectTag!.Slug == tagSlug))
         .OrderBy(project => project.SortOrder)
         .ThenBy(project => project.Name)
         .ToListAsync(cancellationToken);
   }

   public async Task<IReadOnlyList<Project>> GetPublishedProjectsByStackTagSlugAsync(
      string stackTagSlug,
      CancellationToken cancellationToken = default)
   {
      ArgumentException.ThrowIfNullOrWhiteSpace(stackTagSlug);
      var query = await BuildPublishedProjectQueryAsync(cancellationToken);
      return await query
         .Where(project => project.StackTagLinks.Any(link => link.ProjectStackTag!.Slug == stackTagSlug))
         .OrderBy(project => project.SortOrder)
         .ThenBy(project => project.Name)
         .ToListAsync(cancellationToken);
   }

   public async Task<IReadOnlyList<ProjectTag>> GetProjectTagsAsync(
      CancellationToken cancellationToken = default)
   {
      return await _dbContext.ProjectTags
         .AsNoTracking()
         .OrderBy(p => p.SortOrder)
         .ThenBy(p => p.DisplayName)
         .ToListAsync(cancellationToken);
   }

   public async Task<IReadOnlyList<ProjectStackTag>> GetProjectStackTagsAsync(
      CancellationToken cancellationToken = default)
   {
      return await _dbContext.ProjectStackTags
         .AsNoTracking()
         .OrderBy(p => p.SortOrder)
         .ThenBy(p => p.DisplayName)
         .ToListAsync(cancellationToken);
   }

   public async Task<IReadOnlyList<ProjectStatus>> GetProjectStatusesAsync(
      CancellationToken cancellationToken = default)
   {
      return await _dbContext.ProjectStatuses
         .AsNoTracking()
         .OrderBy(p => p.SortOrder)
         .ThenBy(p => p.DisplayName)
         .ToListAsync(cancellationToken);
   }

   public async Task<IReadOnlyList<ProjectType>> GetProjectTypesAsync(
      CancellationToken cancellationToken = default)
   {
      return await _dbContext.ProjectTypes
         .AsNoTracking()
         .OrderBy(p => p.SortOrder)
         .ThenBy(p => p.DisplayName)
         .ToListAsync(cancellationToken);
   }

   public async Task<IReadOnlyList<ProjectOrigin>> GetProjectOriginsAsync(
      CancellationToken cancellationToken = default)
   {
      return await _dbContext.ProjectOrigins
         .AsNoTracking()
         .OrderBy(p => p.SortOrder)
         .ThenBy(p => p.DisplayName)
         .ToListAsync(cancellationToken);
   }

   public async Task<IReadOnlyList<ProjectVisibility>> GetProjectVisibilitiesAsync(
      CancellationToken cancellationToken = default)
   {
      return await _dbContext.ProjectVisibilities
         .AsNoTracking()
         .OrderBy(p => p.SortOrder)
         .ThenBy(p => p.DisplayName)
         .ToListAsync(cancellationToken);
   }

   public async Task<Project?> GetProjectAsync(
      int id,
      CancellationToken cancellationToken = default)
   {
      return await _dbContext.Projects
         .AsNoTracking()
         .AsSplitQuery()
         .Where(project => project.Id == id)
         .Include(project => project.ProjectStatus)
         .Include(project => project.ProjectVisibility)
         .Include(project => project.ProjectOrigin)
         .Include(project => project.ProjectType)
         .Include(project => project.AssetLinks.OrderBy(link => link.SortOrder))
            .ThenInclude(link => link.SiteAsset)
         .Include(project => project.AssetLinks)
            .ThenInclude(link => link.ProjectAssetRole)
         .Include(project => project.TagLinks)
            .ThenInclude(link => link.ProjectTag)
         .Include(project => project.StackTagLinks)
            .ThenInclude(link => link.ProjectStackTag)
         .Include(project => project.Links.OrderBy(link => link.SortOrder))
            .ThenInclude(link => link.ProjectLinkType)
         .Include(project => project.RelatedPosts.OrderBy(post => post.SortOrder))
            .ThenInclude(project => project.Post)
               .ThenInclude(post => post!.Tags)
                  .ThenInclude(tag => tag.WritingTag)
         .Include(project => project.RelatedPosts)
            .ThenInclude(project => project.Post)
               .ThenInclude(post => post!.PostStyle)
         .SingleOrDefaultAsync(cancellationToken);
   }

   public Task<int> InsertProjectAsync(
      Project project,
      IList<int> projectTags,
      IList<int> stackIds,
      CancellationToken cancellationToken = default)
   {
      throw new NotImplementedException();
   }

   public Task<bool> UpdateProjectAsync(
      Project project,
      IList<int> projectTags,
      IList<int> stackIds,
      CancellationToken cancellationToken = default)
   {
      throw new NotImplementedException();
   }

   public Task<ProjectContentData?> GetProjectContentAsync(
      int projectId,
      CancellationToken cancellationToken = default)
   {
      throw new NotImplementedException();
   }

   public Task<bool> UpdateProjectContentAsync(
      int projectId,
      string? content,
      IReadOnlyList<ProjectAssetLink> assetLinks,
      CancellationToken cancellationToken = default)
   {
      throw new NotImplementedException();
   }

   public async Task<IReadOnlyList<Project>> GetProjectsAsync(
      CancellationToken cancellationToken = default)
   {
      return await _dbContext.Projects
         .AsNoTracking()
         .Include(project => project.Links.OrderBy(link => link.SortOrder))
         .Include(project => project.AssetLinks.OrderBy(asset => asset.SortOrder))
         .Include(project => project.ProjectStatus)
         .Include(project => project.ProjectVisibility)
         .Include(project => project.ProjectOrigin)
         .Include(project => project.ProjectType)
         .Include(project => project.TagLinks)
            .ThenInclude(link => link.ProjectTag)
         .Include(project => project.StackTagLinks)
            .ThenInclude(link => link.ProjectStackTag)
         .OrderBy(project => project.SortOrder)
         .ToListAsync(cancellationToken);
   }

   public async Task<int?> GetRequiredProjectAssetRoleIdAsync(
      string slug,
      CancellationToken cancellationToken = default)
   {
      return (await _dbContext.ProjectAssetRoles
         .SingleOrDefaultAsync(r => r.Slug == slug, cancellationToken))?.Id;
   }

   public Task<ProjectStatus?> GetProjectStatusAsync(
      int id,
      CancellationToken token = default)
   {
      return _dbContext.ProjectStatuses
         .SingleOrDefaultAsync(x => x.Id == id, token);
   }

   public Task<ProjectOrigin?> GetProjectOriginAsync(
      int id,
      CancellationToken token = default)
   {
      return _dbContext.ProjectOrigins
         .SingleOrDefaultAsync(x => x.Id == id, token);
   }

   public Task<ProjectType?> GetProjectTypeAsync(
      int id,
      CancellationToken token = default)
   {
      return _dbContext.ProjectTypes
         .SingleOrDefaultAsync(x => x.Id == id, token);
   }

   public Task<ProjectVisibility?> GetProjectVisibilityAsync(
      int id,
      CancellationToken token = default)
   {
      return _dbContext.ProjectVisibilities
         .SingleOrDefaultAsync(x => x.Id == id, token);
   }

   public Task<ProjectTag?> GetProjectTagAsync(
      int id,
      CancellationToken token = default)
   {
      return _dbContext.ProjectTags
         .SingleOrDefaultAsync(x => x.Id == id, token);
   }

   public Task<ProjectStackTag?> GetProjectStackTagAsync(
      int id,
      CancellationToken token = default)
   {
      return _dbContext.ProjectStackTags
         .SingleOrDefaultAsync(x => x.Id == id, token);
   }

   private async Task<IQueryable<Project>> BuildPublishedProjectQueryAsync(
      CancellationToken cancellationToken)
   {
      var publicId = await _visibilityLookup.GetIdBySlugAsync(
         "public", cancellationToken);

      if (publicId == null)
      {
         throw new InvalidOperationException(
            "Required project visibility 'public' was not found.");
      }

      return _dbContext.Projects
         .AsNoTracking()
         .Where(project => project.ProjectVisibilityId == publicId)
         .Include(project => project.ProjectStatus)
         .Include(project => project.ProjectVisibility)
         .Include(project => project.ProjectOrigin)
         .Include(project => project.ProjectType)
         .Include(project => project.TagLinks)
            .ThenInclude(link => link.ProjectTag)
         .Include(project => project.StackTagLinks)
            .ThenInclude(link => link.ProjectStackTag);
   }

   private readonly ILogger<SqlProjectStore> _logger;
   private readonly SiteDbContext _dbContext;
   private readonly ISlugLookupService<ProjectOrigin> _originLookup;
   private readonly ISlugLookupService<ProjectStatus> _statusLookup;
   private readonly ISlugLookupService<ProjectVisibility> _visibilityLookup;
}
