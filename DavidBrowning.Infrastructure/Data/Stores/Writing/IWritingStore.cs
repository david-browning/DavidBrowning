// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Models.Writing;

namespace DavidBrowning.Data.Stores.Writing;

public interface IWritingStore
{
   Task<IReadOnlyList<Post>> GetPublishedPostsAsync(
      CancellationToken cancellationToken = default);

   Task<PagedResult<Post>> GetPagedPublishedPostsAsync(
      int page,
      int pageSize,
      CancellationToken cancellationToken = default);

   Task<IReadOnlyList<Post>> GetFeaturedPostsAsync(
      CancellationToken cancellationToken = default);

   Task<Post?> GetPublishedPostBySlugAsync(
      string slug,
      CancellationToken cancellationToken = default);

   /// <summary>
   /// Gets all post tags.
   /// </summary>
   /// <param name="cancellationToken"></param>
   /// <returns></returns>
   Task<IReadOnlyList<WritingTag>> GetTagsAsync(
      CancellationToken cancellationToken = default);

   Task<IReadOnlyList<Post>> GetPublishedPostsByTagSlugAsync(
      string tagSlug,
      CancellationToken cancellationToken = default);
}