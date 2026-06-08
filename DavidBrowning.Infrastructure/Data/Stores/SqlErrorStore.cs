// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using DavidBrowning.Models.Error;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DavidBrowning.Infrastructure.Data.Stores;

public sealed class SqlErrorStore : IErrorStore
{
   public SqlErrorStore(
      ILogger<SqlErrorStore> logger,
      SiteDbContext context)
   {
      _logger = logger;
      _context = context;
   }

   public async Task<PagedResult<WebsiteError>> GetErrorsAsync(
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

      var query = _context.WebsiteErrors
         .AsNoTracking()
         .OrderByDescending(error => error.OccurredAtUtc);

      var totalCount =
         await query.CountAsync(cancellationToken);

      var errors = await query
         .Skip((page - 1) * pageSize)
         .Take(pageSize)
         .ToListAsync(cancellationToken);

      return new PagedResult<WebsiteError>
      {
         Items = errors,
         TotalCount = totalCount,
         Page = page,
         PageSize = pageSize
      };
   }

   public Task<WebsiteError?> GetErrorAsync(
      int id,
      CancellationToken cancellationToken = default)
   {
      return _context.WebsiteErrors
         .AsNoTracking()
         .SingleOrDefaultAsync(e => e.Id == id, cancellationToken);
   }

   public async Task<WebsiteError> InsertErrorAsync(
      WebsiteError error,
      CancellationToken cancellationToken = default)
   {
      ArgumentNullException.ThrowIfNull(error);
      _context.WebsiteErrors.Add(error);
      await _context.SaveChangesAsync(cancellationToken);
      return error;
   }

   private readonly ILogger<SqlErrorStore> _logger;
   private readonly SiteDbContext _context;

   public async Task<PagedResult<WebsiteError>> GetPagedErrorsAsync(
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

      var query = _context.WebsiteErrors.AsNoTracking();
      var totalCount = await query.CountAsync(cancellationToken);
      var errors = await query
         .Skip((page - 1) * pageSize)
         .Take(pageSize)
         .ToListAsync(cancellationToken);

      return new PagedResult<WebsiteError>()
      {
         Items = errors,
         Page = page,
         TotalCount = totalCount,
         PageSize = pageSize,
      };
   }
}
