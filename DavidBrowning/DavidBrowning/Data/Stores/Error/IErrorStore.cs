// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Models.Error;

namespace DavidBrowning.Data.Stores.Error
{
   public interface IErrorStore
   {
      Task<PagedResult<WebsiteError>> GetErrorsAsync(
         int page,
         int pageSize,
         CancellationToken cancellationToken = default);

      Task<WebsiteError> GetErrorAsync(
         int id,
         CancellationToken cancellationToken = default);

      Task InsertErrorAsync(
         WebsiteError error,
         CancellationToken cancellationToken = default);
   }
}
