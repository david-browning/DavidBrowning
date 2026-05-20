// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Models;

namespace DavidBrowning.Services.Cache
{
   public sealed class DummySlugLookupService<TLookup> : ISlugLookupService<TLookup>
      where TLookup : class, ISlugLookup
   {
      public Task<TLookup?> GetByIdAsync(
         int id, 
         CancellationToken cancellationToken = default)
      {
         throw new System.NotImplementedException();
      }

      public Task<TLookup?> GetBySlugAsync(
         string slug,
         CancellationToken cancellationToken = default)
      {
         throw new System.NotImplementedException();
      }

      public Task<int?> GetIdBySlugAsync(
         string slug, 
         CancellationToken cancellationToken = default)
      {
         throw new System.NotImplementedException();
      }

      public Task<string?> GetDisplayNameByIdAsync(
         int id,
         CancellationToken cancellationToken = default)
      {
         throw new System.NotImplementedException();
      }
   }
}
