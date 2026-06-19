// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using DavidBrowning.Models.Work;

namespace DavidBrowning.Infrastructure.Data.Stores;

public interface IWorkStore
{
   /// <summary>
   /// Gets active professional experiences, including active roles and bullets.
   /// </summary>
   /// <param name="cancellationToken"></param>
   /// <returns></returns>
   Task<IReadOnlyList<Experience>> GetExperienceAsync(
      CancellationToken cancellationToken = default);

   Task<Experience?> GetExperienceAsync(
      int id,
      CancellationToken cancellationToken = default);

   Task InsertExperienceAsync(
      Experience experience,
      CancellationToken cancellationToken = default);

   Task<bool> UpdateExperienceAsync(
      Experience experience,
      CancellationToken cancellationToken = default);

   Task<bool> DeleteExperienceAsync(
      int id,
      CancellationToken cancellationToken = default);

   Task ReorderExperienceAsync(
      IReadOnlyList<int> idsInDisplayOrder,
      CancellationToken cancellationToken = default);

   Task ReorderExperienceRolesAsync(
      int experienceId,
      IReadOnlyList<int> idsInDisplayOrder,
      CancellationToken cancellationToken = default);

   Task<ExperienceRoleBullet?> GetRoleBulletAsync(
      int id,
      CancellationToken cancellationToken = default);

   Task InsertRoleBulletAsync(
      ExperienceRoleBullet bullet,
      CancellationToken cancellationToken = default);

   Task<bool> UpdateRoleBulletAsync(
      ExperienceRoleBullet bullet,
      CancellationToken cancellationToken = default);

   Task<bool> DeleteRoleBulletAsync(
      int id,
      CancellationToken cancellationToken = default);

   Task ReorderRoleBullets(
      int roleId,
      IReadOnlyList<int> idsInDisplayOrder,
      CancellationToken cancellationToken = default);

   Task<IReadOnlyList<ExperienceRole>> GetExperienceRolesAsync(
      int experienceId,
      CancellationToken cancellationToken = default);

   Task<ExperienceRole?> GetRoleAsync(
      int roleId,
      CancellationToken cancellationToken = default);

   Task InsertRoleAsync(
      ExperienceRole role,
      CancellationToken cancellationToken = default);

   Task<bool> UpdateRoleAsync(
      ExperienceRole role,
      CancellationToken cancellationToken = default);

   Task<bool> DeleteRoleAsync(
      int id,
      CancellationToken cancellationToken = default);

   /// <summary>
   /// Gets active degrees, certifications, and other credentials.
   /// </summary>
   /// <param name="cancellationToken"></param>
   /// <returns></returns>
   Task<IReadOnlyList<Credential>> GetCredentialsAsync(
      CancellationToken cancellationToken = default);

   Task<Credential?> GetCredentialAsync(
      int id,
      CancellationToken cancellationToken = default);

   Task InsertCredentialAsync(
      Credential credential,
      CancellationToken cancellationToken = default);

   Task<bool> UpdateCredentialAsync(
      Credential credential,
      CancellationToken cancellationToken = default);

   Task<bool> DeleteCredentialAsync(
      int id,
      CancellationToken cancellationToken = default);

   Task ReorderCredentialsAsync(
      IReadOnlyList<int> idsInDisplayOrder,
      CancellationToken cancellationToken = default);
}
