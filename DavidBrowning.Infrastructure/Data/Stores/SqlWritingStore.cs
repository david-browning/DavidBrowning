// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using DavidBrowning.Models.Writing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DavidBrowning.Infrastructure.Data.Stores;

public class SqlWritingStore : IWritingStore
{
   public SqlWritingStore(
      ILogger<SqlWritingStore> logger,
      SiteDbContext dbContext)
   {
      _logger = logger;
      _dbContext = dbContext;
   }

   public async Task<IReadOnlyList<Post>> GetPublishedPostsAsync(
      CancellationToken cancellationToken = default)
   {
      var posts = await CreatePublishedPostSummaryQuery()
         .ToListAsync(cancellationToken);
      return posts;
   }

   public async Task<PagedResult<Post>> GetPagedPublishedPostsAsync(
      int page,
      int pageSize,
      CancellationToken cancellationToken = default)
   {
      if (page < 1)
      {
         throw new ArgumentOutOfRangeException(
            nameof(page), "Page must be greater than or equal to 1.");
      }

      if (pageSize < 1)
      {
         throw new ArgumentOutOfRangeException(
            nameof(pageSize), "Page size must be greater than or equal to 1.");
      }

      var query = CreatePublishedPostSummaryQuery();
      var totalCount = await query.CountAsync(cancellationToken);
      var posts = await query
         .Skip((page - 1) * pageSize)
         .Take(pageSize)
         .ToListAsync(cancellationToken);

      return new PagedResult<Post>
      {
         Items = posts,
         Page = page,
         PageSize = pageSize,
         TotalCount = totalCount
      };
   }

   public async Task<IReadOnlyList<Post>> GetAllPostsAsync(
      CancellationToken cancellationToken = default)
   {
      var posts = _dbContext.Posts
         .AsNoTracking()
         .Include(post => post.PostStyle)
         .Include(post => post.CurrentRevision)
         .Include(post => post.Tags)
            .ThenInclude(postTag => postTag.WritingTag)
         .OrderByDescending(post => post.PublishedDateUtc)
         .ThenByDescending(post => post.CreatedDateUtc)
         .ThenByDescending(post => post.Id);
      return await posts.ToListAsync(cancellationToken);
   }

   public async Task<IReadOnlyList<Post>> GetFeaturedPostsAsync(
      CancellationToken cancellationToken = default)
   {
      var posts = await CreatePublishedPostSummaryQuery()
         .Where(post => post.IsFeatured)
         .ToListAsync(cancellationToken);
      return posts;
   }

   public async Task<Post?> GetPublishedPostBySlugAsync(
      string slug,
      CancellationToken cancellationToken = default)
   {
      ArgumentException.ThrowIfNullOrWhiteSpace(slug);

      var post = await CreatePublishedPostDetailQuery()
         .SingleOrDefaultAsync(post => post.Slug == slug, cancellationToken);
      return post;
   }

   public async Task<IReadOnlyList<WritingTag>> GetTagsAsync(
      CancellationToken cancellationToken = default)
   {
      return await _dbContext.WritingTags
         .OrderBy(tag => tag.DisplayName)
         .ToListAsync(cancellationToken);
   }

   public async Task<IReadOnlyList<Post>> GetPublishedPostsByTagSlugAsync(
      string tagSlug,
      CancellationToken cancellationToken = default)
   {
      ArgumentException.ThrowIfNullOrWhiteSpace(tagSlug);

      var posts = await CreatePublishedPostSummaryQuery()
         .Where(post => post.Tags.Any(postTag => postTag.WritingTag!.Slug == tagSlug))
         .ToListAsync(cancellationToken);
      return posts;
   }

   public async Task<IReadOnlyList<PostStyle>> GetPostStylesAsync(
      CancellationToken cancellationToken = default)
   {
      return await _dbContext.PostStyles
         .AsNoTracking()
         .OrderBy(style => style.SortOrder)
         .ToListAsync(cancellationToken);
   }

   private IQueryable<Post> CreatePublishedPostSummaryQuery()
   {
      return _dbContext.Posts
         .AsNoTracking()
         .Include(post => post.PostStyle)
         .Include(post => post.CurrentRevision)
         .Include(post => post.Tags)
            .ThenInclude(postTag => postTag.WritingTag)
         .Where(post => post.Status == PostStatus.Published)
         .OrderByDescending(post => post.PublishedDateUtc)
         .ThenByDescending(post => post.CreatedDateUtc)
         .ThenByDescending(post => post.Id);
   }

   private IQueryable<Post> CreatePublishedPostDetailQuery()
   {
      return _dbContext.Posts
         .AsNoTracking()
         .Include(post => post.PostStyle)
         .Include(post => post.CurrentRevision)
         .Include(post => post.Tags)
            .ThenInclude(postTag => postTag.WritingTag)
         .Include(post => post.CurrentRevision)
            .ThenInclude(revision => revision!.AssetLinks)
               .ThenInclude(link => link.SiteAsset)
         .Where(post => post.Status == PostStatus.Published)
         .OrderByDescending(post => post.PublishedDateUtc)
         .ThenByDescending(post => post.CreatedDateUtc)
         .ThenByDescending(post => post.Id);
   }

   private readonly ILogger<SqlWritingStore> _logger;
   private readonly SiteDbContext _dbContext;
}
