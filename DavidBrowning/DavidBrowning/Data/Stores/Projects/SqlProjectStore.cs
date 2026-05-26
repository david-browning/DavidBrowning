// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Models.Projects;
using DavidBrowning.Services.Cache;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DavidBrowning.Data.Stores.Projects;

internal sealed class SqlProjectStore : IProjectStore
{
   public SqlProjectStore(
      ILogger<SqlProjectStore> logger,
      SiteDbContext context,
      ISlugLookupService<ProjectStatus> statusLookup,
      ISlugLookupService<ProjectVisibility> visibleLookup)
   {
      _logger = logger;
      _dbContext = context;
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

   public async Task<PagedResult<Project>> GetPagedPublishedProjectsAsync(
      int page,
      int pageSize,
      CancellationToken cancellationToken = default)
   {
      if (page < 1)
      {
         throw new ArgumentOutOfRangeException(
            nameof(page),
            "Page must be greater than or equal to 1.");
      }

      if (pageSize < 1)
      {
         throw new ArgumentOutOfRangeException(
            nameof(pageSize),
            "Page size must be greater than or equal to 1.");
      }

      var query = await BuildPublishedProjectQueryAsync(cancellationToken);
      var totalCount =
        await query.CountAsync(cancellationToken);

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
      var publicId = await _visibilityLookup.GetIdBySlugAsync(
        "public",
        cancellationToken);

      return await _dbContext.Projects
         .AsNoTracking()
         .Include(project => project.ProjectStatus)
         .Include(project => project.ProjectVisibility)
         .Include(project => project.ProjectOrigin)
         .Include(project => project.ProjectType)
         .Include(project => project.TagLinks)
            .ThenInclude(link => link.ProjectTag)
         .Include(project => project.StackTagLinks)
            .ThenInclude(link => link.ProjectStackTag)
         .SingleOrDefaultAsync(project => project.Slug == slug);
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
      var query = await BuildPublishedProjectQueryAsync(cancellationToken);
      return await query
         .Where(project => project.StackTagLinks.Any(link =>
            link.ProjectStackTag!.Slug == stackTagSlug))
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

   private async Task<IQueryable<Project>> BuildPublishedProjectQueryAsync(
      CancellationToken cancellationToken)
   {
      var publicId = await _visibilityLookup.GetIdBySlugAsync(
         "public",
         cancellationToken);

      return _dbContext.Projects
         .AsNoTracking()
         .Where(project =>
            project.ProjectVisibilityId == publicId)
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
   private readonly ISlugLookupService<ProjectStatus> _statusLookup;
   private readonly ISlugLookupService<ProjectVisibility> _visibilityLookup;
}
