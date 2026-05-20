// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Data;
using DavidBrowning.Models;
using Microsoft.EntityFrameworkCore;

namespace DavidBrowning.Services.Cache
{
   /// <summary>
   /// Generic EF Core implementation of ISlugLookupService.
   /// 
   /// This one class can handle any lookup model that:
   /// - is mapped in SiteDbContext
   /// - implements ISlugLookup
   /// 
   /// Example:
   /// SlugLookupService<ProjectVisibility> queries db_ProjectVisibilities.
   /// SlugLookupService<ProjectStatus> queries db_ProjectStatuses.
   /// </summary>
   internal sealed class SlugLookupService<TLookup> : ISlugLookupService<TLookup>
      where TLookup : class, ISlugLookup
   {
      public SlugLookupService(
        SiteDbContext dbContext,
        ILookupCache lookupCache)
      {
         _dbContext = dbContext;
         _lookupCache = lookupCache;
      }

      public Task<TLookup?> GetByIdAsync(
         int id,
         CancellationToken cancellationToken = default)
      {
         string cacheKey = GetCacheKey("by-id", id.ToString());
         return _lookupCache.GetOrCreateAsync(
            cacheKey,
            async token =>
               await _dbContext.Set<TLookup>()
                  .AsNoTracking()
                  .SingleOrDefaultAsync(row => row.Id == id, token),
            cancellationToken);
      }

      /// <summary>
      /// Assume the slug has been normalized correctly.
      /// </summary>
      /// <param name="slug"></param>
      /// <param name="cancellationToken"></param>
      /// <returns></returns>
      public Task<TLookup?> GetBySlugAsync(
         string slug,
         CancellationToken cancellationToken = default)
      {
         string cacheKey = GetCacheKey("by-slug", slug);
         return _lookupCache.GetOrCreateAsync(
            cacheKey,
            async token =>
               await _dbContext.Set<TLookup>()
                  .AsNoTracking()
                  .SingleOrDefaultAsync(row => row.Slug == slug, token),
            cancellationToken);
      }

      public async Task<int?> GetIdBySlugAsync(
         string slug,
         CancellationToken cancellationToken = default)
      {
         TLookup? row = await GetBySlugAsync(slug, cancellationToken);
         return row?.Id;
      }

      public async Task<string?> GetDisplayNameByIdAsync(
         int id,
         CancellationToken cancellationToken = default)
      {
         TLookup? row = await GetByIdAsync(id, cancellationToken);
         return row?.DisplayName;
      }

      private string GetCacheKey(string type, string value)
      {
         return $"lookup:{typeof(TLookup).FullName}:{type}:{value}";
      }

      private readonly SiteDbContext _dbContext;
      private readonly ILookupCache _lookupCache;
   }
}
