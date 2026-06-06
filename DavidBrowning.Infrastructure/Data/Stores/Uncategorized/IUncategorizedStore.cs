// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Models;

namespace DavidBrowning.Data.Stores.Uncategorized;

/// <summary>
/// Used to look up data from a data store that is not related to writing,
/// posts, or errors.
/// </summary>
public interface IUncategorizedStore
{
   Task<IReadOnlyList<Interest>> GetInterestsAsync(
      CancellationToken cancellationToken = default);
}
