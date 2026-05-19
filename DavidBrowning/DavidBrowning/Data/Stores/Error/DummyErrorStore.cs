// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Models.Error;
using Microsoft.Extensions.Logging;

namespace DavidBrowning.Data.Stores.Error
{
   public sealed class DummyErrorStore : IErrorStore
   {
      public DummyErrorStore(ILogger<DummyErrorStore> logger)
      {

      }

      public Task<IReadOnlyList<WebsiteError>> GetErrors(
         CancellationToken cancellationToken = default)
      {
         throw new System.NotImplementedException();
      }

      public Task InsertError(
         WebsiteError error, 
         CancellationToken cancellationToken = default)
      {
         throw new System.NotImplementedException();
      }
   }
}
