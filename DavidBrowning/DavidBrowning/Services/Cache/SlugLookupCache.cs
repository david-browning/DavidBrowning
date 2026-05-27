// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Data;
using DavidBrowning.Models;
using Microsoft.EntityFrameworkCore;

namespace DavidBrowning.Services.Cache;

/// <summary>
/// Generic EF Core implementation of ISlugLookupService.
/// 
/// This one class can handle any lookup model that:
/// - is mapped in SiteDbContext
/// - implements IQueryableSlug
/// 
/// Example:
/// SlugLookupService<ProjectVisibility> queries db_ProjectVisibilities.
/// SlugLookupService<ProjectStatus> queries db_ProjectStatuses.
/// </summary>
public sealed class SlugLookupCache<TLookup> : ISlugLookupService<TLookup>
   where TLookup : class, IQueryableSlug
   //TLookup is a class that implements IQueryableSlug
{
   public SlugLookupCache(
      SiteDbContext dbContext,
      SlugMemoryCache<TLookup?> asyncCache)
   {
      _dbContext = dbContext;
      _asyncCache = asyncCache;
   }

   public async Task<TLookup?> GetByIdAsync(
      int id,
      CancellationToken cancellationToken = default)
   {
      string cacheKey = GetCacheKey("by-id", id.ToString());
      return await _asyncCache.GetOrCreateAsync(
         cacheKey,
         token => _dbContext.Set<TLookup>()
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
   public async Task<TLookup?> GetBySlugAsync(
      string slug,
      CancellationToken cancellationToken = default)
   {
      string cacheKey = GetCacheKey("by-slug", slug);
      return await _asyncCache.GetOrCreateAsync(
         cacheKey,
         token => _dbContext.Set<TLookup>()
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
      return $"slug-lookup:{typeof(TLookup).FullName}:{type}:{value}";
   }

   private readonly SiteDbContext _dbContext;
   private readonly SlugMemoryCache<TLookup?> _asyncCache;
}
