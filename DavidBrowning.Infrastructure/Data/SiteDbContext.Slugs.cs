// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using DavidBrowning.Models;
using Microsoft.EntityFrameworkCore;

namespace DavidBrowning.Infrastructure.Data;
public sealed partial class SiteDbContext
{
   public Task<bool> SlugExistsAsync<TEntity>(
      string slug,
      int? excludedId = null,
      CancellationToken cancellationToken = default)
      where TEntity : class, IQueryableSlug
   {
      ArgumentException.ThrowIfNullOrWhiteSpace(slug);

      return Set<TEntity>().AnyAsync(item => item.Slug == slug &&
         (!excludedId.HasValue || item.Id != excludedId.Value),
         cancellationToken);
   }
}