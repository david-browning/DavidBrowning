// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using DavidBrowning.Infrastructure.Cache;
using DavidBrowning.Models;
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

   public async Task<int> InsertProjectAsync(
      Project project,
      IList<int> projectTags,
      IList<int> stackIds,
      CancellationToken cancellationToken = default)
   {
      ArgumentNullException.ThrowIfNull(project);
      ArgumentException.ThrowIfNullOrWhiteSpace(project.Slug);

      if (await _dbContext.Posts.AnyAsync(
         p => p.Id != project.Id && p.Slug == project.Slug,
         cancellationToken))
      {
         throw new DuplicateSlugException(project.Slug);
      }

      int? maxSortOrder = await _dbContext.Projects
         .Select(project => (int?)project.SortOrder)
         .MaxAsync(cancellationToken);

      project.SortOrder = (maxSortOrder ?? 0) + 1;

      foreach (int tagId in projectTags.Distinct())
      {
         project.TagLinks.Add(new ProjectTagLink()
         {
            Project = project,
            ProjectTagId = tagId,
         });
      }

      foreach (int stackId in stackIds.Distinct())
      {
         project.StackTagLinks.Add(new ProjectStackTagLink()
         {
            Project = project,
            ProjectStackTagId = stackId,
         });
      }

      _dbContext.Projects.Add(project);
      await _dbContext.SaveChangesAsync(cancellationToken);
      return project.Id;
   }

   public async Task<bool> UpdateProjectAsync(
      Project project,
      IList<int> projectTags,
      IList<int> stackIds,
      CancellationToken cancellationToken = default)
   {
      ArgumentNullException.ThrowIfNull(project);
      ArgumentException.ThrowIfNullOrWhiteSpace(project.Slug);

      if (await _dbContext.Posts.AnyAsync(
         p => p.Id != project.Id && p.Slug == project.Slug,
         cancellationToken))
      {
         throw new DuplicateSlugException(project.Slug);
      }

      var storedProject = await _dbContext.Projects
         .Include(project => project.TagLinks)
         .Include(project => project.StackTagLinks)
         .SingleOrDefaultAsync(
            existingProject => existingProject.Id == project.Id,
            cancellationToken);

      if (storedProject is null)
      {
         return false;
      }

      storedProject.Slug = project.Slug;
      storedProject.Name = project.Name;
      storedProject.Description = project.Description;
      storedProject.ProjectStatusId = project.ProjectStatusId;
      storedProject.ProjectTypeId = project.ProjectTypeId;
      storedProject.ProjectOriginId = project.ProjectOriginId;
      storedProject.ProjectVisibilityId = project.ProjectVisibilityId;
      storedProject.Role = project.Role;
      storedProject.ContributionSummary = project.ContributionSummary;
      storedProject.IsFeatured = project.IsFeatured;
      storedProject.StartDate = project.StartDate;
      storedProject.EndDate = project.EndDate;
      storedProject.DateDisplayText = project.DateDisplayText;

      ReplaceProjectTagLinks(storedProject, projectTags);
      ReplaceProjectStackTagLinks(storedProject, stackIds);

      await _dbContext.SaveChangesAsync(cancellationToken);
      return true;
   }

   public async Task<ProjectContentData?> GetProjectContentAsync(
      int projectId,
      CancellationToken cancellationToken = default)
   {
      var project = await _dbContext.Projects
         .AsNoTracking()
         .AsSplitQuery()
         .Include(project => project.AssetLinks)
            .ThenInclude(link => link.SiteAsset)
         .Include(project => project.AssetLinks)
            .ThenInclude(link => link.ProjectAssetRole)
         .SingleOrDefaultAsync(
            project => project.Id == projectId, cancellationToken);

      if (project is null)
      {
         return null;
      }

      var contentLink = project.AssetLinks.SingleOrDefault(link =>
         link.ProjectAssetRole?.Slug == _detailsContentRoleSlug);

      return new ProjectContentData()
      {
         ProjectId = project.Id,
         ProjectSlug = project.Slug,
         ContentAssetId = contentLink?.SiteAssetId,
         ContentAssetKey = contentLink?.SiteAsset?.AssetKey,
         AssetLinks = project.AssetLinks
            .Where(link => !string.IsNullOrWhiteSpace(link.ReferenceKey))
            .OrderBy(link => link.SortOrder)
            .ToList(),
      };
   }

   public async Task<bool> UpdateProjectContentAsync(
      int projectId,
      string contentAssetKey,
      long contentLengthBytes,
      IReadOnlyList<ProjectAssetLink> assetLinks,
      CancellationToken cancellationToken = default)
   {
      ArgumentException.ThrowIfNullOrWhiteSpace(contentAssetKey);

      var project = await _dbContext.Projects
         .Include(project => project.AssetLinks)
            .ThenInclude(link => link.SiteAsset)
         .Include(project => project.AssetLinks)
            .ThenInclude(link => link.ProjectAssetRole)
         .SingleOrDefaultAsync(
            project => project.Id == projectId,
            cancellationToken);

      if (project is null)
      {
         return false;
      }

      int detailsContentRoleId = await GetRequiredRoleIdAsync(
         _detailsContentRoleSlug, cancellationToken);

      var contentLink = project.AssetLinks.SingleOrDefault(link =>
         link.ProjectAssetRoleId == detailsContentRoleId);

      SiteAsset? contentAsset = contentLink?.SiteAsset;

      if (contentAsset is null)
      {
         contentAsset = await _dbContext.SiteAssets.SingleOrDefaultAsync(
            asset => asset.AssetKey == contentAssetKey, cancellationToken);

         if (contentAsset is null)
         {
            contentAsset = new SiteAsset()
            {
               AssetKey = contentAssetKey,
               ContentType = _detailsContentType,
               OriginalFileName = Path.GetFileName(contentAssetKey),
               SizeBytes = contentLengthBytes,
            };

            _dbContext.SiteAssets.Add(contentAsset);
         }

         project.AssetLinks.Add(new ProjectAssetLink()
         {
            ProjectId = project.Id,
            SiteAsset = contentAsset,
            ProjectAssetRoleId = detailsContentRoleId,
            SortOrder = 0,
         });
      }
      else
      {
         contentAsset.AssetKey = contentAssetKey;
         contentAsset.ContentType = _detailsContentType;
         contentAsset.OriginalFileName ??= Path.GetFileName(contentAssetKey);
         contentAsset.SizeBytes = contentLengthBytes;
      }

      ReplaceProjectInlineAssetLinks(project, assetLinks);
      await _dbContext.SaveChangesAsync(cancellationToken);
      return true;
   }

   public async Task ReorderProjectsAsync(
      IReadOnlyList<int> idsInDisplayOrder,
      CancellationToken cancellationToken = default)
   {
      int changedRecordCount = await _dbContext.ApplySortOrderAsync<Project>(
       idsInDisplayOrder, cancellationToken);
      if (changedRecordCount == 0)
      {
         return;
      }

      await _dbContext.SaveChangesAsync(cancellationToken);
   }

   public async Task<bool> DeleteProjectAsync(
      int id,
      CancellationToken cancellationToken = default)
   {
      bool exists = await _dbContext.Projects
         .AnyAsync(project => project.Id == id, cancellationToken);
      if (!exists)
      {
         return false;
      }

      await _dbContext.ProjectTagLinks
         .Where(link => link.ProjectId == id)
         .ExecuteDeleteAsync(cancellationToken);
      await _dbContext.ProjectStackTagLinks
         .Where(link => link.ProjectId == id)
         .ExecuteDeleteAsync(cancellationToken);
      await _dbContext.ProjectLinks
         .Where(link => link.ProjectId == id)
         .ExecuteDeleteAsync(cancellationToken);
      await _dbContext.ProjectAssetLinks
         .Where(link => link.ProjectId == id)
         .ExecuteDeleteAsync(cancellationToken);
      await _dbContext.ProjectPosts
         .Where(link => link.ProjectId == id)
         .ExecuteDeleteAsync(cancellationToken);
      await _dbContext.Projects
         .Where(project => project.Id == id)
         .ExecuteDeleteAsync(cancellationToken);
      return true;
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

   public async Task InsertProjectStatusAsync(
      ProjectStatus status,
      CancellationToken cancellationToken = default)
   {
      ArgumentNullException.ThrowIfNull(status);
      if (await _dbContext.SlugExistsAsync<ProjectStatus>(
         status.Slug, cancellationToken: cancellationToken))
      {
         throw new DuplicateSlugException(status.Slug);
      }

      _dbContext.ProjectStatuses.Add(status);
      await _dbContext.SaveChangesAsync(cancellationToken);
   }

   public async Task InsertProjectVisibilityAsync(
      ProjectVisibility visibility,
      CancellationToken cancellationToken = default)
   {
      ArgumentNullException.ThrowIfNull(visibility);
      if (await _dbContext.SlugExistsAsync<ProjectVisibility>(
         visibility.Slug, cancellationToken: cancellationToken))
      {
         throw new DuplicateSlugException(visibility.Slug);
      }

      _dbContext.ProjectVisibilities.Add(visibility);
      await _dbContext.SaveChangesAsync(cancellationToken);
   }

   public async Task InsertProjectOriginAsync(
      ProjectOrigin origin,
      CancellationToken cancellationToken = default)
   {
      ArgumentNullException.ThrowIfNull(origin);
      if (await _dbContext.SlugExistsAsync<ProjectOrigin>(
         origin.Slug, cancellationToken: cancellationToken))
      {
         throw new DuplicateSlugException(origin.Slug);
      }

      _dbContext.ProjectOrigins.Add(origin);
      await _dbContext.SaveChangesAsync(cancellationToken);
   }

   public async Task InsertProjectTypeAsync(
      ProjectType type,
      CancellationToken cancellationToken = default)
   {
      ArgumentNullException.ThrowIfNull(type);
      if (await _dbContext.SlugExistsAsync<ProjectType>(
         type.Slug, cancellationToken: cancellationToken))
      {
         throw new DuplicateSlugException(type.Slug);
      }

      _dbContext.ProjectTypes.Add(type);
      await _dbContext.SaveChangesAsync(cancellationToken);
   }

   public async Task InsertProjectTagAsync(
      ProjectTag tag,
      CancellationToken cancellationToken = default)
   {
      ArgumentNullException.ThrowIfNull(tag);
      if (await _dbContext.SlugExistsAsync<ProjectTag>(
         tag.Slug, cancellationToken: cancellationToken))
      {
         throw new DuplicateSlugException(tag.Slug);
      }

      _dbContext.ProjectTags.Add(tag);
      await _dbContext.SaveChangesAsync(cancellationToken);
   }

   public async Task InsertProjectStackTagAsync(
      ProjectStackTag tag,
      CancellationToken cancellationToken = default)
   {
      ArgumentNullException.ThrowIfNull(tag);
      if (await _dbContext.SlugExistsAsync<ProjectStackTag>(
         tag.Slug, cancellationToken: cancellationToken))
      {
         throw new DuplicateSlugException(tag.Slug);
      }

      _dbContext.ProjectStackTags.Add(tag);
      await _dbContext.SaveChangesAsync(cancellationToken);
   }

   public async Task<bool> UpdateProjectStatusAsync(
      ProjectStatus status,
      CancellationToken cancellationToken = default)
   {
      ArgumentNullException.ThrowIfNull(status);
      if (await _dbContext.SlugExistsAsync<ProjectStatus>(
         status.Slug, excludedId: status.Id, cancellationToken))
      {
         throw new DuplicateSlugException(status.Slug);
      }

      var existing = await _dbContext.ProjectStatuses
         .SingleOrDefaultAsync(e => e.Id == status.Id, cancellationToken);
      if (existing is null)
      {
         return false;
      }

      existing.Slug = status.Slug;
      existing.SortOrder = status.SortOrder;
      existing.DisplayName = status.DisplayName;
      existing.Description = status.Description;
      existing.IsActive = status.IsActive;
      await _dbContext.SaveChangesAsync(cancellationToken);
      return true;
   }

   public async Task<bool> UpdateProjectOriginAsync(
      ProjectOrigin origin,
      CancellationToken cancellationToken = default)
   {

      ArgumentNullException.ThrowIfNull(origin);
      if (await _dbContext.SlugExistsAsync<ProjectOrigin>(
         origin.Slug, excludedId: origin.Id, cancellationToken))
      {
         throw new DuplicateSlugException(origin.Slug);
      }

      var existing = await _dbContext.ProjectOrigins
         .SingleOrDefaultAsync(e => e.Id == origin.Id, cancellationToken);
      if (existing is null)
      {
         return false;
      }

      existing.Slug = origin.Slug;
      existing.SortOrder = origin.SortOrder;
      existing.DisplayName = origin.DisplayName;
      existing.Description = origin.Description;
      existing.IsActive = origin.IsActive;
      await _dbContext.SaveChangesAsync(cancellationToken);
      return true;
   }

   public async Task<bool> UpdateProjectTypeAsync(
      ProjectType type,
      CancellationToken cancellationToken = default)
   {
      ArgumentNullException.ThrowIfNull(type);
      if (await _dbContext.SlugExistsAsync<ProjectType>(
         type.Slug, excludedId: type.Id, cancellationToken))
      {
         throw new DuplicateSlugException(type.Slug);
      }

      var existing = await _dbContext.ProjectTypes
         .SingleOrDefaultAsync(e => e.Id == type.Id, cancellationToken);
      if (existing is null)
      {
         return false;
      }

      existing.Slug = type.Slug;
      existing.SortOrder = type.SortOrder;
      existing.DisplayName = type.DisplayName;
      existing.Description = type.Description;
      existing.IsActive = type.IsActive;
      await _dbContext.SaveChangesAsync(cancellationToken);
      return true;
   }

   public async Task<bool> UpdateProjectVisibilityAsync(
      ProjectVisibility visibility,
      CancellationToken cancellationToken = default)
   {
      ArgumentNullException.ThrowIfNull(visibility);
      if (await _dbContext.SlugExistsAsync<ProjectVisibility>(
         visibility.Slug, excludedId: visibility.Id, cancellationToken))
      {
         throw new DuplicateSlugException(visibility.Slug);
      }

      var existing = await _dbContext.ProjectVisibilities
         .SingleOrDefaultAsync(e => e.Id == visibility.Id, cancellationToken);
      if (existing is null)
      {
         return false;
      }

      existing.Slug = visibility.Slug;
      existing.SortOrder = visibility.SortOrder;
      existing.DisplayName = visibility.DisplayName;
      existing.Description = visibility.Description;
      existing.IsActive = visibility.IsActive;
      await _dbContext.SaveChangesAsync(cancellationToken);
      return true;
   }

   public async Task<bool> UpdateProjectTagAsync(
      ProjectTag tag,
      CancellationToken cancellationToken = default)
   {
      ArgumentNullException.ThrowIfNull(tag);
      if (await _dbContext.SlugExistsAsync<ProjectTag>(
         tag.Slug, excludedId: tag.Id, cancellationToken))
      {
         throw new DuplicateSlugException(tag.Slug);
      }

      var existing = await _dbContext.ProjectTags
         .SingleOrDefaultAsync(e => e.Id == tag.Id, cancellationToken);
      if (existing is null)
      {
         return false;
      }

      existing.Slug = tag.Slug;
      existing.SortOrder = tag.SortOrder;
      existing.DisplayName = tag.DisplayName;
      existing.Description = tag.Description;
      existing.IsActive = tag.IsActive;
      await _dbContext.SaveChangesAsync(cancellationToken);
      return true;
   }

   public async Task<bool> UpdateProjectStackTagAsync(
      ProjectStackTag tag,
      CancellationToken cancellationToken = default)
   {
      ArgumentNullException.ThrowIfNull(tag);
      if (await _dbContext.SlugExistsAsync<ProjectStackTag>(
         tag.Slug, excludedId: tag.Id, cancellationToken))
      {
         throw new DuplicateSlugException(tag.Slug);
      }

      var existing = await _dbContext.ProjectStackTags
         .SingleOrDefaultAsync(e => e.Id == tag.Id, cancellationToken);
      if (existing is null)
      {
         return false;
      }

      existing.Slug = tag.Slug;
      existing.SortOrder = tag.SortOrder;
      existing.DisplayName = tag.DisplayName;
      existing.Description = tag.Description;
      existing.IsActive = tag.IsActive;
      await _dbContext.SaveChangesAsync(cancellationToken);
      return true;
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

   private static void ReplaceProjectTagLinks(
      Project project,
      IEnumerable<int> tagIds)
   {
      project.TagLinks.Clear();
      foreach (int tagId in tagIds.Distinct())
      {
         project.TagLinks.Add(new ProjectTagLink()
         {
            ProjectId = project.Id,
            ProjectTagId = tagId,
         });
      }
   }

   private static void ReplaceProjectStackTagLinks(
      Project project,
      IEnumerable<int> stackIds)
   {
      project.StackTagLinks.Clear();
      foreach (int stackId in stackIds.Distinct())
      {
         project.StackTagLinks.Add(new ProjectStackTagLink()
         {
            ProjectId = project.Id,
            ProjectStackTagId = stackId,
         });
      }
   }

   private async Task<int> GetRequiredRoleIdAsync(
      string roleSlug,
      CancellationToken cancellationToken)
   {
      int? roleId = await GetRequiredProjectAssetRoleIdAsync(
         roleSlug, cancellationToken);

      if (roleId is null)
      {
         throw new InvalidOperationException(
            $"Required project asset role '{roleSlug}' was not found.");
      }

      return roleId.Value;
   }

   private static void ReplaceProjectInlineAssetLinks(
      Project project,
      IReadOnlyList<ProjectAssetLink> assetLinks)
   {
      var existingInlineLinks = project.AssetLinks
         .Where(link => !string.IsNullOrWhiteSpace(link.ReferenceKey))
         .ToList();

      foreach (var existingLink in existingInlineLinks)
      {
         project.AssetLinks.Remove(existingLink);
      }

      int sortOrder = 0;

      foreach (var assetLink in assetLinks
         .Where(link => !string.IsNullOrWhiteSpace(link.ReferenceKey))
         .GroupBy(link => new
         {
            link.SiteAssetId,
            link.ProjectAssetRoleId,
         })
         .Select(group => group.First()))
      {
         project.AssetLinks.Add(new ProjectAssetLink()
         {
            ProjectId = project.Id,
            SiteAssetId = assetLink.SiteAssetId,
            ProjectAssetRoleId = assetLink.ProjectAssetRoleId,
            ReferenceKey = assetLink.ReferenceKey,
            Caption = assetLink.Caption,
            AltTextOverride = assetLink.AltTextOverride,
            SortOrder = sortOrder++,
         });
      }
   }

   private const string _detailsContentRoleSlug = "details-content";
   private const string _detailsContentType = "text/markdown";

   private readonly ILogger<SqlProjectStore> _logger;
   private readonly SiteDbContext _dbContext;
   private readonly ISlugLookupService<ProjectOrigin> _originLookup;
   private readonly ISlugLookupService<ProjectStatus> _statusLookup;
   private readonly ISlugLookupService<ProjectVisibility> _visibilityLookup;
}
