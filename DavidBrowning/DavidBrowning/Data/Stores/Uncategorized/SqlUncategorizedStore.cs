// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DavidBrowning.Data.Stores.Uncategorized;

internal sealed class SqlUncategorizedStore : IUncategorizedStore
{
   public SqlUncategorizedStore(
      ILogger<SqlUncategorizedStore> logger,
      SiteDbContext context)
   {
      _logger = logger;
      _context = context;
   }

   public async Task<IReadOnlyList<Interest>> GetInterestsAsync(
      CancellationToken cancellationToken = default)
   {
      return await _context.Interests
         .AsNoTracking()
         .OrderBy(interest =>  interest.SortOrder)
         .ThenBy(interest => interest.DisplayName)
         .ToListAsync(cancellationToken);
   }

   private readonly ILogger<SqlUncategorizedStore> _logger;
   private readonly SiteDbContext _context;
}
