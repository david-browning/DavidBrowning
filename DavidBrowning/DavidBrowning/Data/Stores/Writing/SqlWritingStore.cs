// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Models.Writing;

namespace DavidBrowning.Data.Stores.Writing
{
   internal class SqlWritingStore : IWritingStore
   {
      public Task<IReadOnlyList<Post>> GetPublishedPostsAsync(
         CancellationToken cancellationToken = default)
      {
         throw new NotImplementedException();
      }

      public Task<IReadOnlyList<Post>> GetFeaturedPostsAsync(
         CancellationToken cancellationToken = default)
      {
         throw new NotImplementedException();
      }

      public Task<Post?> GetPublishedPostBySlugAsync(
         string slug,
         CancellationToken cancellationToken = default)
      {
         throw new NotImplementedException();
      }

      public Task<IReadOnlyList<WritingTag>> GetTagsAsync(
         CancellationToken cancellationToken = default)
      {
         throw new NotImplementedException();
      }

      public Task<IReadOnlyList<Post>> GetPublishedPostsByTagSlugAsync(
         string tagSlug,
         CancellationToken cancellationToken = default)
      {
         throw new NotImplementedException();
      }
   }
}
