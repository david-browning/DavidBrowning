// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using DavidBrowning.Models;

namespace DavidBrowning.Infrastructure.Data.Stores;

/// <summary>
/// Used to look up data from a data store that is not related to writing,
/// posts, or errors.
/// </summary>
public interface IUncategorizedStore
{
   Task<IReadOnlyList<Interest>> GetAllInterestsAsync(
      CancellationToken cancellationToken = default);

   Task<IReadOnlyList<Interest>> GetInterestsAsync(
      CancellationToken cancellationToken = default);

   Task<Interest?> GetInterestAsync(
      int id,
      CancellationToken cancellationToken = default);

   Task InsertInterestAsync(
      Interest interest,
      CancellationToken cancellationToken = default);

   Task<bool> UpdateInterestAsync(
      Interest interest,
      CancellationToken cancellationToken = default);

   Task<bool> DeleteInterestAsync(
      int id,
      CancellationToken cancellationToken = default);

   Task ReorderInterestsAsync(
      IReadOnlyList<int> idsInDisplayOrder,
      CancellationToken cancellationToken = default);

   Task<IReadOnlyList<SiteAsset>> GetSiteAssetsAsync(
      CancellationToken cancellationToken = default);

   Task<SiteAsset?> GetAssetAsync(
      int id,
      CancellationToken cancellationToken = default);

   Task<bool> DeleteAssetAsync(
      int id,
      CancellationToken cancellationToken = default);

   Task<bool> UpdateAssetAsync(
      SiteAsset asset,
      CancellationToken cancellationToken = default);

   Task InsertAssetAsync(
      SiteAsset asset,
      CancellationToken cancellationToken = default);
}
