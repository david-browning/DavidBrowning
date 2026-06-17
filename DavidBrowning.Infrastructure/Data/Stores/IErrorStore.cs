// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using DavidBrowning.Models.Error;

namespace DavidBrowning.Infrastructure.Data.Stores;

public interface IErrorStore
{
   Task<PagedResult<WebsiteError>> GetPagedErrorsAsync(
      int page,
      int pageSize,
      CancellationToken cancellationToken = default);

   Task<WebsiteError?> GetErrorAsync(
      int id,
      CancellationToken cancellationToken = default);

   Task<WebsiteError> InsertErrorAsync(
      WebsiteError error,
      CancellationToken cancellationToken = default);
}
