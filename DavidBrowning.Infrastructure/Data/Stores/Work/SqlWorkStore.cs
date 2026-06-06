
// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using DavidBrowning.Models.Work;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DavidBrowning.Data.Stores.Work;

public sealed class SqlWorkStore : IWorkStore
{
   public SqlWorkStore(
      ILogger<SqlWorkStore> logger,
      SiteDbContext dbContext)
   {
      _logger = logger;
      _dbContext = dbContext;
   }

   public async Task<IReadOnlyList<Experience>> GetExperienceAsync(
      CancellationToken cancellationToken = default)
   {
      return await _dbContext.Experiences
         .AsNoTracking()
         .AsSplitQuery()
         .Where(experience => experience.IsActive)
         .Include(experience => experience.Roles
            .Where(role => role.IsActive)
            .OrderBy(role => role.SortOrder))
         .ThenInclude(role => role.Bullets
            .Where(bullet => bullet.IsActive)
            .OrderBy(bullet => bullet.SortOrder))
         .OrderBy(experience => experience.SortOrder)
         .ThenBy(experience => experience.CompanyName)
         .ToListAsync(cancellationToken);
   }

   public async Task<IReadOnlyList<Credential>> GetCredentialsAsync(
      CancellationToken cancellationToken = default)
   {
      return await _dbContext.Credentials
         .AsNoTracking()
         .Where(credential => credential.IsActive)
         .OrderBy(credential => credential.SortOrder)
         .ThenBy(credential => credential.Name)
         .ToListAsync(cancellationToken);
   }

   private readonly ILogger<SqlWorkStore> _logger;
   private readonly SiteDbContext _dbContext;
}