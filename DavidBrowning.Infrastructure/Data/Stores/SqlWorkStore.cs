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
         .SingleOrDefaultAsync(
            experience => experience.Id == id, cancellationToken);
   }

   public async Task InsertExperienceAsync(
      Experience experience,
      CancellationToken cancellationToken = default)
   {
      ArgumentNullException.ThrowIfNull(experience);
      _dbContext.Experiences.Add(experience);
      await _dbContext.SaveChangesAsync(cancellationToken);
   }

   public async Task<bool> UpdateExperienceAsync(
      Experience experience,
      CancellationToken cancellationToken = default)
   {
      ArgumentNullException.ThrowIfNull(experience);
      var stored = await _dbContext.Experiences
         .SingleOrDefaultAsync(storedExperience =>
            storedExperience.Id == experience.Id, cancellationToken);
      if (stored is null)
      {
         return false;
      }

      stored.CompanyName = experience.CompanyName;
      stored.LocationDisplayText = experience.LocationDisplayText;
      stored.IsActive = experience.IsActive;
      await _dbContext.SaveChangesAsync(cancellationToken);
      return true;
   }

   public async Task<bool> DeleteExperienceAsync(
      int id,
      CancellationToken cancellationToken = default)
   {
      var experience = await _dbContext.Experiences
         .SingleOrDefaultAsync(experience => experience.Id == id,
         cancellationToken);
      if (experience is null)
      {
         return false;
      }

      _dbContext.Experiences.Remove(experience);
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
      int experienceId,
      IReadOnlyList<int> idsInDisplayOrder,
      CancellationToken cancellationToken = default)
   {
      int changeCount = await _dbContext.ApplySortOrderAsync<ExperienceRole>(
         idsInDisplayOrder,
         role => role.ExperienceId == experienceId,
         cancellationToken);
      if (changeCount == 0)
      {
         return;
      }

      await _dbContext.SaveChangesAsync(cancellationToken);
   }

   public Task<ExperienceRoleBullet?> GetRoleBulletAsync(
      int id,
      CancellationToken cancellationToken = default)
   {
      return _dbContext.ExperienceRoleBullets
         .AsNoTracking()
         .SingleOrDefaultAsync(bullet => bullet.Id == id, cancellationToken);
   }

   public async Task InsertRoleBulletAsync(
      ExperienceRoleBullet bullet,
      CancellationToken cancellationToken = default)
   {
      ArgumentNullException.ThrowIfNull(bullet);

      int? lastSortOrder = await _dbContext.ExperienceRoleBullets
         .Where(existingBullet =>
            existingBullet.ExperienceRoleId == bullet.ExperienceRoleId)
         .Select(existingBullet => (int?)existingBullet.SortOrder)
         .MaxAsync(cancellationToken);

      bullet.SortOrder = (lastSortOrder ?? -1) + 1;

      _dbContext.ExperienceRoleBullets.Add(bullet);
      await _dbContext.SaveChangesAsync(cancellationToken);
   }

   public async Task<bool> UpdateRoleBulletAsync(
      ExperienceRoleBullet bullet,
      CancellationToken cancellationToken = default)
   {
      ArgumentNullException.ThrowIfNull(bullet);
      var stored = await _dbContext.ExperienceRoleBullets
         .SingleOrDefaultAsync(storedBullet => storedBullet.Id == bullet.Id,
            cancellationToken);
      if (stored is null)
      {
         return false;
      }

      stored.Text = bullet.Text;
      stored.IsActive = bullet.IsActive;
      await _dbContext.SaveChangesAsync(cancellationToken);
      return true;
   }

   public async Task<bool> DeleteRoleBulletAsync(
      int id,
      CancellationToken cancellationToken = default)
   {
      var bullet = await _dbContext.ExperienceRoleBullets
         .SingleOrDefaultAsync(bullet => bullet.Id == id, cancellationToken);
      if (bullet is null)
      {
         return false;
      }

      _dbContext.ExperienceRoleBullets.Remove(bullet);
      await _dbContext.SaveChangesAsync(cancellationToken);
      return true;
   }

   public async Task ReorderRoleBullets(
      int roleId,
      IReadOnlyList<int> idsInDisplayOrder,
      CancellationToken cancellationToken = default)
   {
      int changeCount =
         await _dbContext
            .ApplySortOrderAsync<ExperienceRoleBullet>(
               idsInDisplayOrder, bullet => bullet.ExperienceRoleId == roleId,
               cancellationToken);

      if (changeCount == 0)
      {
         return;
      }

      await _dbContext.SaveChangesAsync(cancellationToken);
   }

   public async Task<IReadOnlyList<ExperienceRole>> GetExperienceRolesAsync(
      int experienceId,
      CancellationToken cancellationToken = default)
   {
      return await _dbContext.ExperienceRoles
         .AsNoTracking()
         .Where(role => role.ExperienceId == experienceId)
         .Include(role => role.Bullets
            .OrderBy(bullet => bullet.SortOrder))
         .OrderBy(role => role.SortOrder)
         .ThenBy(role => role.Title)
         .ToListAsync(cancellationToken);
   }

   public async Task<ExperienceRole?> GetRoleAsync(
      int roleId,
      CancellationToken cancellationToken = default)
   {
      return await _dbContext.ExperienceRoles
         .AsNoTracking()
         .Include(role => role.Bullets
            .OrderBy(bullet => bullet.SortOrder))
         .SingleOrDefaultAsync(role => role.Id == roleId, cancellationToken);
   }

   public async Task InsertRoleAsync(
      ExperienceRole role,
      CancellationToken cancellationToken = default)
   {
      ArgumentNullException.ThrowIfNull(role);
      int? lastSortOrder = await _dbContext.ExperienceRoles
         .Where(existingRole => existingRole.ExperienceId == role.ExperienceId)
         .Select(existingRole => (int?)existingRole.SortOrder)
         .MaxAsync(cancellationToken);
      role.SortOrder = (lastSortOrder ?? -1) + 1;
      _dbContext.ExperienceRoles.Add(role);
      await _dbContext.SaveChangesAsync(cancellationToken);
   }

   public async Task<bool> UpdateRoleAsync(
      ExperienceRole role,
      CancellationToken cancellationToken = default)
   {
      ArgumentNullException.ThrowIfNull(role);

      var stored = await _dbContext.ExperienceRoles.SingleOrDefaultAsync(
         storedRole => storedRole.Id == role.Id, cancellationToken);
      if (stored is null)
      {
         return false;
      }

      stored.Title = role.Title;
      stored.DateDisplayText = role.DateDisplayText;
      stored.Description = role.Description;
      stored.IsActive = role.IsActive;
      await _dbContext.SaveChangesAsync(cancellationToken);
      return true;
   }

   public async Task<bool> DeleteRoleAsync(
      int id,
      CancellationToken cancellationToken = default)
   {
      var role = await _dbContext.ExperienceRoles.SingleOrDefaultAsync(
         role => role.Id == id, cancellationToken);

      if (role is null)
      {
         return false;
      }

      _dbContext.ExperienceRoles.Remove(role);
      await _dbContext.SaveChangesAsync(cancellationToken);
      return true;
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
      var stored = await _dbContext.Credentials.SingleOrDefaultAsync(
         sc => sc.Id == credential.Id, cancellationToken);
      if (stored is null)
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
      var credential = await _dbContext.Credentials.SingleOrDefaultAsync(
         credential => credential.Id == id, cancellationToken);
      if (credential is null)
      {
         return false;
      }

      _dbContext.Credentials.Remove(credential);
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
