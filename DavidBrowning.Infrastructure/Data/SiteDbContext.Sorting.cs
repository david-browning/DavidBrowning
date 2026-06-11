// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Linq.Expressions;
using DavidBrowning.Models;
using Microsoft.EntityFrameworkCore;

namespace DavidBrowning.Infrastructure.Data;

public partial class SiteDbContext
{
   public Task<int> ApplySortOrderAsync<TEntity>(
      IReadOnlyList<int> idsInDisplayOrder,
      CancellationToken cancellationToken = default)
      where TEntity : class, ISortableRecord
   {
      return ApplySortOrderAsync<TEntity>(
         idsInDisplayOrder,
         scope: null,
         cancellationToken);
   }

   public async Task<int> ApplySortOrderAsync<TEntity>(
      IReadOnlyList<int> idsInDisplayOrder,
      Expression<Func<TEntity, bool>>? scope,
      CancellationToken cancellationToken = default)
      where TEntity : class, ISortableRecord
   {
      ArgumentNullException.ThrowIfNull(idsInDisplayOrder);

      if (idsInDisplayOrder.Count == 0)
      {
         return 0;
      }

      var submittedIds = idsInDisplayOrder.ToHashSet();
      if (submittedIds.Count != idsInDisplayOrder.Count)
      {
         throw new InvalidOperationException(
            "The submitted sort order contains duplicate IDs.");
      }

      IQueryable<TEntity> query = Set<TEntity>();
      if (scope is not null)
      {
         query = query.Where(scope);
      }

      var records = await query.ToListAsync(cancellationToken);
      if (records.Count != idsInDisplayOrder.Count)
      {
         throw new InvalidOperationException(
            "The submitted sort order must contain every record " +
            "in the sortable list exactly once.");
      }

      var recordsById = records.ToDictionary(record => record.Id);
      int changedRecordCount = 0;
      for (int index = 0; index < idsInDisplayOrder.Count; index++)
      {
         int recordId = idsInDisplayOrder[index];

         if (!recordsById.TryGetValue(recordId, out TEntity? record))
         {
            throw new InvalidOperationException(
               "The submitted sort order contains an unknown " +
               "or out-of-scope ID.");
         }

         if (record.SortOrder == index)
         {
            continue;
         }

         record.SortOrder = index;
         changedRecordCount++;
      }

      return changedRecordCount;
   }
}