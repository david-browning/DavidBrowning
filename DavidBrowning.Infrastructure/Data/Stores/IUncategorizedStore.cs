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
   Task<IReadOnlyList<Interest>> GetInterestsAsync(
      CancellationToken cancellationToken = default);
}
