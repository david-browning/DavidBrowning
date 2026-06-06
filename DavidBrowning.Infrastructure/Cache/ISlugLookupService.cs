// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using DavidBrowning.Models;

namespace DavidBrowning.Infrastructure.Cache;

/// <summary>
/// Typed lookup service for lookup tables that have Id, Slug, and 
/// DisplayName.
/// 
/// TLookup specifies the model/table being queried.
/// Example:
/// ISlugLookupService<ProjectVisibility>
/// ISlugLookupService<ProjectStatus>
/// ISlugLookupService<ProjectTag>
/// </summary>
public interface ISlugLookupService<TLookup>
     where TLookup : class, IQueryableSlug
{
   /// <summary>
   /// Gets the full lookup row by database Id.
   /// </summary>
   Task<TLookup?> GetByIdAsync(
      int id,
      CancellationToken cancellationToken = default);

   /// <summary>
   /// Gets the full lookup row by stable slug which should be 
   /// unique.
   /// </summary>
   Task<TLookup?> GetBySlugAsync(
      string slug,
      CancellationToken cancellationToken = default);

   /// <summary>
   /// Gets only the database Id for a given slug.
   /// Useful when assigning foreign keys from stable slugs.
   /// </summary>
   Task<int?> GetIdBySlugAsync(
      string slug,
      CancellationToken cancellationToken = default);

   /// <summary>
   /// Gets only the display name for a given database Id.
   /// Useful when showing lookup values without loading a full 
   /// object graph.
   /// </summary>
   Task<string?> GetDisplayNameByIdAsync(
      int id,
      CancellationToken cancellationToken = default);
}
