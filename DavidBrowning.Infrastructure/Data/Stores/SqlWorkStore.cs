
// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using DavidBrowning.Models.Work;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DavidBrowning.Infrastructure.Data.Stores;

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

   public Task<Experience?> GetExperienceAsync(
     int id,
     CancellationToken cancellationToken = default)
   {
      return _dbContext.Experiences
         .AsNoTracking()
         .Include(experience => experience.Roles
            .Where(role => role.IsActive)
            .OrderBy(role => role.SortOrder))
         .ThenInclude(role => role.Bullets
            .Where(bullet => bullet.IsActive)
            .OrderBy(bullet => bullet.SortOrder))
         .OrderBy(experience => experience.SortOrder)
         .ThenBy(experience => experience.CompanyName)
         .SingleOrDefaultAsync(
            experience => experience.Id == id, cancellationToken);
   }

   public async Task InsertExperience(
      Experience experience,
      CancellationToken cancellationToken = default)
   {
      ArgumentNullException.ThrowIfNull(experience);
      _dbContext.Experiences.Add(experience);
      await _dbContext.SaveChangesAsync(cancellationToken);
   }

   public async Task<bool> DeleteExperienceAsync(
      int id,
      CancellationToken cancellationToken = default)
   {
      var exp = await _dbContext.Experiences
         .SingleOrDefaultAsync(e => e.Id == id, cancellationToken);
      if (exp is null)
      {
         return false;
      }

      _dbContext.Experiences.Remove(exp);
      await _dbContext.SaveChangesAsync(cancellationToken);
      return true;
   }

   public async Task ReorderExperienceAsync(
      IReadOnlyList<int> idsInDisplayOrder,
      CancellationToken cancellationToken = default)
   {
      int changeCount = await _dbContext.ApplySortOrderAsync<Experience>(
         idsInDisplayOrder, cancellationToken);
      if (changeCount == 0)
      {
         return;
      }

      await _dbContext.SaveChangesAsync(cancellationToken);
   }

   public async Task ReorderExperienceRolesAsync(
      IReadOnlyList<int> idsInDisplayOrder,
      CancellationToken cancellationToken = default)
   {
      int changeCount = await _dbContext.ApplySortOrderAsync<ExperienceRole>(
         idsInDisplayOrder, cancellationToken);
      if (changeCount == 0)
      {
         return;
      }

      await _dbContext.SaveChangesAsync(cancellationToken);
   }

   public async Task ReorderRoleBullets(
      IReadOnlyList<int> idsInDisplayOrder,
      CancellationToken cancellationToken = default)
   {
      int changeCount = await _dbContext.ApplySortOrderAsync<ExperienceRoleBullet>(
         idsInDisplayOrder, cancellationToken);
      if (changeCount == 0)
      {
         return;
      }

      await _dbContext.SaveChangesAsync(cancellationToken);
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

   public async Task<Credential?> GetCredentialAsync(
      int id,
      CancellationToken cancellationToken = default)
   {
      return await _dbContext.Credentials
         .AsNoTracking()
         .SingleOrDefaultAsync(c => c.Id == id, cancellationToken);
   }

   public async Task InsertCredentialAsync(
      Credential credential,
      CancellationToken cancellationToken = default)
   {
      ArgumentNullException.ThrowIfNull(credential);
      await _dbContext.Credentials.AddAsync(credential, cancellationToken);
      await _dbContext.SaveChangesAsync(cancellationToken);
   }

   public async Task<bool> UpdateCredentialAsync(
      Credential credential,
      CancellationToken cancellationToken = default)
   {
      ArgumentNullException.ThrowIfNull(credential);
      var stored = await _dbContext.Credentials
         .SingleOrDefaultAsync(
            cred => cred.Id == credential.Id, cancellationToken);
      if(stored is null)
      {
         return false;
      }

      stored.AwardedMonth = credential.AwardedMonth;
      stored.AwardedYear = credential.AwardedYear;
      stored.CredentialUrl = credential.CredentialUrl;
      stored.DateDisplayText = credential.DateDisplayText;
      stored.Description = credential.Description;
      stored.IsActive = credential.IsActive;
      stored.IssuingOrganization = credential.IssuingOrganization;
      stored.Name = credential.Name;

      await _dbContext.SaveChangesAsync(cancellationToken);
      return true;
   }

   public async Task<bool> DeleteCredentialAsync(
      int id,
      CancellationToken cancellationToken = default)
   {
      var cred = await _dbContext.Credentials
         .SingleOrDefaultAsync(c => c.Id == id, cancellationToken);
      if (cred is null)
      {
         return false;
      }

      _dbContext.Credentials.Remove(cred);
      await _dbContext.SaveChangesAsync(cancellationToken);
      return true;
   }

   public async Task ReorderCredentialsAsync(
      IReadOnlyList<int> idsInDisplayOrder,
      CancellationToken cancellationToken = default)
   {
      int changeCount = await _dbContext.ApplySortOrderAsync<Credential>(
         idsInDisplayOrder, cancellationToken);
      if (changeCount == 0)
      {
         return;
      }
      await _dbContext.SaveChangesAsync(cancellationToken);
   }

   private readonly ILogger<SqlWorkStore> _logger;
   private readonly SiteDbContext _dbContext;
}