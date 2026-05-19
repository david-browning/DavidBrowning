// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Models.Error;

namespace DavidBrowning.Data.Stores.Error
{
   public interface IErrorStore
   {
      Task<IReadOnlyList<WebsiteError>> GetErrors(
         CancellationToken cancellationToken = default);

      Task InsertError(
         WebsiteError error,
         CancellationToken cancellationToken = default);
   }
}
