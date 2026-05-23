// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Models.Error;
using Microsoft.Extensions.Logging;

namespace DavidBrowning.Data.Stores.Error
{
   internal sealed class SqlErrorStore : IErrorStore
   {
      public SqlErrorStore(ILogger<SqlErrorStore> logger)
      {
      }

      public Task<PagedResult<WebsiteError>> GetErrorsAsync(int page, int pageSize, CancellationToken cancellationToken = default)
      {
         throw new System.NotImplementedException();
      }

      public Task<WebsiteError> GetErrorAsync(int id, CancellationToken cancellationToken = default)
      {
         throw new System.NotImplementedException();
      }

      public Task InsertErrorAsync(WebsiteError error, CancellationToken cancellationToken = default)
      {
         throw new System.NotImplementedException();
      }
   }
}
