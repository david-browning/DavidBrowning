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
      CancellationToken cancellation = default);

   Task<bool> DeleteExperienceAsync(
      int id, 
      CancellationToken cancellationToken = default);

   Task ReorderExperienceAsync(
      IReadOnlyList<int> idsInDisplayOrder,
      CancellationToken cancellationToken = default);
   
   Task ReorderExperienceRolesAsync(
      IReadOnlyList<int> idsInDisplayOrder,
      CancellationToken cancellationToken = default);

   Task ReorderRoleBullets(
      IReadOnlyList<int> idsInDisplayOrder,
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
      CancellationToken cancellationToken= default);

   Task<bool> DeleteCredentialAsync(
      int id,
      CancellationToken cancellationToken = default);

   Task ReorderCredentialsAsync(
      IReadOnlyList<int> idsInDisplayOrder,
      CancellationToken cancellationToken = default);
}