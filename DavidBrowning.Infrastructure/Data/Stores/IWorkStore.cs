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

   /// <summary>
   /// Gets active degrees, certifications, and other credentials.
   /// </summary>
   /// <param name="cancellationToken"></param>
   /// <returns></returns>
   Task<IReadOnlyList<Credential>> GetCredentialsAsync(
      CancellationToken cancellationToken = default);
}