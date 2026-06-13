// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using DavidBrowning.Models;
using DavidBrowning.Models.Writing;

namespace DavidBrowning.Infrastructure.Data.Stores;

public interface IWritingStore
{
   Task<IReadOnlyList<Post>> GetPublishedPostsAsync(
      CancellationToken cancellationToken = default);

   Task<PagedResult<Post>> GetPagedPublishedPostsAsync(
      int page,
      int pageSize,
      CancellationToken cancellationToken = default);

   Task<IReadOnlyList<Post>> GetAllPostsAsync(
      CancellationToken cancellationToken = default);

   Task<IReadOnlyList<Post>> GetFeaturedPostsAsync(
      CancellationToken cancellationToken = default);

   Task<Post?> GetPublishedPostBySlugAsync(
      string slug,
      CancellationToken cancellationToken = default);

   Task<Post?> GetPostAsync(
      int id,
      CancellationToken cancellationToken= default);

   Task<int> InsertPostAsync(
      Post post,
      IReadOnlyList<int> writingTagIds,
      CancellationToken cancellationToken = default);

   Task<bool> UpdatePostAsync(
      Post post,
      IReadOnlyList<int> writingTagIds,
      CancellationToken cancellationToken = default);

   Task<PostRevision?> GetPostRevisionAsync(
      int postId,
      int revisionId,
      CancellationToken cancellationToken = default);

   Task<PostRevision> InsertPostRevisionAsync(
      int postId,
      ContentFormat contentFormat,
      string? content,
      string createdBy,
      CancellationToken cancellationToken = default);

   Task<bool> SetCurrentRevisionAsync(
      int postId,
      int revisionId,
      CancellationToken cancellationToken = default);

   /// <summary>
   /// Gets all post tags.
   /// </summary>
   /// <param name="cancellationToken"></param>
   /// <returns></returns>
   Task<IReadOnlyList<WritingTag>> GetTagsAsync(
      CancellationToken cancellationToken = default);

   Task<WritingTag?> GetTagAsync(
      int id,
      CancellationToken cancellationToken = default);

   Task InsertTagAsync(
      WritingTag tag,
      CancellationToken cancellationToken = default);

   Task<bool> UpdateTagAsync(
      WritingTag tag,
      CancellationToken cancellationToken= default);

   Task<IReadOnlyList<Post>> GetPublishedPostsByTagSlugAsync(
      string tagSlug,
      CancellationToken cancellationToken = default);

   Task<IReadOnlyList<PostStyle>> GetPostStylesAsync(
      CancellationToken cancellationToken = default);

   Task<PostStyle?> GetPostStyleAsync(
      int id,
      CancellationToken cancellationToken = default);

   Task InsertPostStyleAsync(
      PostStyle style,
      CancellationToken cancellationToken = default);

   Task<bool> UpdatePostStyleAsync(
      PostStyle style,
      CancellationToken cancellationToken = default);
}