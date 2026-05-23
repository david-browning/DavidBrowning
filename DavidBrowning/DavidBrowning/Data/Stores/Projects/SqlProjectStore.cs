// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Models.Projects;
using DavidBrowning.Services.Cache;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DavidBrowning.Data.Stores.Projects
{
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

      public Task<Project?> GetPublishedProjectBySlugAsync(string slug, CancellationToken cancellationToken = default)
      {
         throw new System.NotImplementedException();
      }

      public Task<IReadOnlyList<Project>> GetPublishedProjectsByTagSlugAsync(string tagSlug, CancellationToken cancellationToken = default)
      {
         throw new System.NotImplementedException();
      }

      public Task<IReadOnlyList<Project>> GetPublishedProjectsByStackTagSlugAsync(string stackTagSlug, CancellationToken cancellationToken = default)
      {
         throw new System.NotImplementedException();
      }

      public Task<IReadOnlyList<ProjectTag>> GetProjectTagsAsync(CancellationToken cancellationToken = default)
      {
         throw new System.NotImplementedException();
      }

      public Task<IReadOnlyList<ProjectStackTag>> GetProjectStackTagsAsync(CancellationToken cancellationToken = default)
      {
         throw new System.NotImplementedException();
      }

      public Task<IReadOnlyList<ProjectStatus>> GetProjectStatusesAsync(CancellationToken cancellationToken = default)
      {
         throw new System.NotImplementedException();
      }

      public Task<IReadOnlyList<ProjectType>> GetProjectTypesAsync(CancellationToken cancellationToken = default)
      {
         throw new System.NotImplementedException();
      }

      public Task<IReadOnlyList<ProjectOrigin>> GetProjectOriginsAsync(CancellationToken cancellationToken = default)
      {
         throw new System.NotImplementedException();
      }

      public Task<IReadOnlyList<ProjectVisibility>> GetProjectVisibilitiesAsync(CancellationToken cancellationToken = default)
      {
         throw new System.NotImplementedException();
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
}
