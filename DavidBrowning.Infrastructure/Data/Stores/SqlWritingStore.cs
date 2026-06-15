// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using DavidBrowning.Models;
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
      return await _dbContext.Posts
         .AsNoTracking()
         .AsSplitQuery()
         .Include(post => post.PostStyle)
         .Include(post => post.Revisions)
         .Include(post => post.Tags)
         .OrderByDescending(post => post.PublishedDateUtc)
         .ThenByDescending(post => post.CreatedDateUtc)
         .ThenByDescending(post => post.Id)
         .ToListAsync(cancellationToken);
   }

   public async Task<IReadOnlyList<Post>> GetFeaturedPostsAsync(
      CancellationToken cancellationToken = default)
   {
      return await CreatePublishedPostSummaryQuery()
         .Where(post => post.IsFeatured)
         .ToListAsync(cancellationToken);
   }

   public async Task<Post?> GetPublishedPostBySlugAsync(
      string slug,
      CancellationToken cancellationToken = default)
   {
      ArgumentException.ThrowIfNullOrWhiteSpace(slug);
      return await CreatePublishedPostDetailQuery()
         .SingleOrDefaultAsync(post => post.Slug == slug, cancellationToken);
   }

   public async Task<Post?> GetPostAsync(
      int id,
      CancellationToken cancellationToken = default)
   {
      return await _dbContext.Posts
         .AsNoTracking()
         .AsSplitQuery()
         .Include(post => post.PostStyle)
         .Include(post => post.Revisions.OrderByDescending(
            revision => revision.RevisionNumber))
         .Include(post => post.Tags)
            .ThenInclude(tag => tag.WritingTag)
         .SingleOrDefaultAsync(post => post.Id == id, cancellationToken);
   }

   public async Task<int> InsertPostAsync(
      Post post,
      IReadOnlyList<int> writingTagIds,
      CancellationToken cancellationToken = default)
   {
      ArgumentNullException.ThrowIfNull(post);
      ArgumentNullException.ThrowIfNull(writingTagIds);

      if (await _dbContext.Posts.AnyAsync(
         p => p.Id != post.Id && p.Slug == post.Slug,
         cancellationToken))
      {
         throw new DuplicateSlugException(post.Slug);
      }

      bool styleExists = await _dbContext.PostStyles
         .AnyAsync(style => style.Id == post.PostStyleId, cancellationToken);
      if (!styleExists)
      {
         throw new InvalidOperationException(
            "The selected post style does not exist.");
      }

      var distinctTagIds = writingTagIds.Distinct().ToHashSet();
      int validTagCount = await _dbContext.WritingTags
         .CountAsync(tag => distinctTagIds.Contains(tag.Id), cancellationToken);
      if (validTagCount != distinctTagIds.Count)
      {
         throw new InvalidOperationException(
            "One or more selected writing tags do not exist.");
      }

      var utcNow = DateTime.UtcNow;

      post.Id = 0;
      post.CreatedDateUtc = utcNow;
      post.LastUpdatedDateUtc = utcNow;
      post.CurrentRevisionId = null;
      post.CurrentRevision = null;
      post.PostStyle = null;

      foreach (int tagId in distinctTagIds)
      {
         post.Tags.Add(new PostTag()
         {
            WritingTagId = tagId,
         });
      }

      _dbContext.Posts.Add(post);
      await _dbContext.SaveChangesAsync(cancellationToken);
      return post.Id;
   }

   public async Task<bool> UpdatePostAsync(
      Post post,
      IReadOnlyList<int> writingTagIds,
      CancellationToken cancellationToken = default)
   {
      ArgumentNullException.ThrowIfNull(post);
      ArgumentNullException.ThrowIfNull(writingTagIds);
      var existing = await _dbContext.Posts
         .Include(p => p.Tags)
         .SingleOrDefaultAsync(p => p.Id == post.Id, cancellationToken);
      if (existing is null)
      {
         return false;
      }

      if (await _dbContext.Posts.AnyAsync(
         p => p.Id != post.Id && p.Slug == post.Slug,
         cancellationToken))
      {
         throw new DuplicateSlugException(post.Slug);
      }

      var distinctTagIds = writingTagIds.Distinct().ToHashSet();

      int validTagCount = await _dbContext.WritingTags
         .CountAsync(tag => distinctTagIds.Contains(tag.Id), cancellationToken);
      if (validTagCount != distinctTagIds.Count)
      {
         throw new InvalidOperationException(
            "One or more selected writing tags do not exist.");
      }

      existing.IsFeatured = post.IsFeatured;
      existing.PostStyleId = post.PostStyleId;
      existing.Slug = post.Slug;
      existing.Status = post.Status;
      existing.Subtitle = post.Subtitle;
      existing.Summary = post.Summary;
      existing.Title = post.Title;
      existing.PublishedDateUtc = post.PublishedDateUtc;
      existing.LastUpdatedDateUtc = DateTime.UtcNow;

      UpdatePostTags(existing, distinctTagIds);
      await _dbContext.SaveChangesAsync(cancellationToken);
      return true;
   }

   public async Task<PostRevision?> GetPostRevisionAsync(
      int postId,
      int revisionId,
      CancellationToken cancellationToken = default)
   {
      return await _dbContext.PostRevisions
         .AsNoTracking()
         .SingleOrDefaultAsync(rev =>
            rev.PostId == postId && rev.Id == revisionId, cancellationToken);
   }

   public async Task<PostRevision?> InsertPostRevisionAsync(
      int postId,
      ContentFormat contentFormat,
      string? content,
      string createdBy,
      CancellationToken cancellationToken = default)
   {
      var post = await _dbContext.Posts
         .Include(post => post.Revisions)
         .SingleOrDefaultAsync(post => post.Id == postId, cancellationToken);
      if (post is null)
      {
         return null;
      }

      int nextRevisionNumber = post.Revisions
         .Select(revision => revision.RevisionNumber)
         .DefaultIfEmpty(0)
         .Max() + 1;

      var revision = new PostRevision()
      {
         PostId = post.Id,
         RevisionNumber = nextRevisionNumber,
         ContentFormat = contentFormat,
         Content = content,
         CreatedBy = createdBy,
      };

      _dbContext.PostRevisions.Add(revision);
      await _dbContext.SaveChangesAsync(cancellationToken);
      if (post.CurrentRevisionId is null)
      {
         post.CurrentRevisionId = revision.Id;
         await _dbContext.SaveChangesAsync(cancellationToken);
      }

      return revision;
   }

   public async Task<bool> SetCurrentRevisionAsync(
      int postId,
      int revisionId,
      CancellationToken cancellationToken = default)
   {
      var post = await _dbContext.Posts
         .SingleOrDefaultAsync(post => post.Id == postId);
      if (post is null)
      {
         return false;
      }

      var revisionBelongsToPost = await _dbContext.PostRevisions
         .AnyAsync(revision => 
            revision.Id == revisionId && revision.PostId == postId,
            cancellationToken);
      if (!revisionBelongsToPost)
      {
         return false;
      }

      post.CurrentRevisionId = revisionId;
      await _dbContext.SaveChangesAsync(cancellationToken);
      return true;
   }

   public async Task<IReadOnlyList<WritingTag>> GetTagsAsync(
      CancellationToken cancellationToken = default)
   {
      return await _dbContext.WritingTags
         .AsNoTracking()
         .OrderBy(tag => tag.DisplayName)
         .ToListAsync(cancellationToken);
   }

   public Task<WritingTag?> GetTagAsync(
      int id,
      CancellationToken cancellationToken = default)
   {
      return _dbContext.WritingTags
         .AsNoTracking()
         .SingleOrDefaultAsync(tag => tag.Id == id, cancellationToken);
   }

   public async Task InsertTagAsync(
      WritingTag tag,
      CancellationToken cancellationToken = default)
   {
      ArgumentNullException.ThrowIfNull(tag);
      if (await _dbContext.SlugExistsAsync<WritingTag>(
         tag.Slug, cancellationToken: cancellationToken))
      {
         throw new DuplicateSlugException(tag.Slug);
      }

      _dbContext.WritingTags.Add(tag);
      await _dbContext.SaveChangesAsync(cancellationToken);
   }

   public async Task<bool> UpdateTagAsync(
      WritingTag tag,
      CancellationToken cancellationToken = default)
   {
      ArgumentNullException.ThrowIfNull(tag);
      if (await _dbContext.SlugExistsAsync<WritingTag>(
         tag.Slug, excludedId: tag.Id, cancellationToken))
      {
         throw new DuplicateSlugException(tag.Slug);
      }

      var stored = await _dbContext.WritingTags
         .SingleOrDefaultAsync(t => t.Id == tag.Id, cancellationToken);
      if (stored is null)
      {
         return false;
      }

      stored.Slug = tag.Slug;
      stored.DisplayName = tag.DisplayName;
      await _dbContext.SaveChangesAsync(cancellationToken);
      return true;
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

   public Task<PostStyle?> GetPostStyleAsync(
      int id,
      CancellationToken cancellationToken = default)
   {
      return _dbContext.PostStyles
         .AsNoTracking()
         .SingleOrDefaultAsync(style => style.Id == id, cancellationToken);
   }

   public async Task InsertPostStyleAsync(
      PostStyle style,
      CancellationToken cancellationToken = default)
   {
      ArgumentNullException.ThrowIfNull(style);
      if (await _dbContext.SlugExistsAsync<PostStyle>(
         style.Slug, cancellationToken: cancellationToken))
      {
         throw new DuplicateSlugException(style.Slug);
      }

      _dbContext.PostStyles.Add(style);
      await _dbContext.SaveChangesAsync(cancellationToken);
   }

   public async Task<bool> UpdatePostStyleAsync(
      PostStyle style,
      CancellationToken cancellationToken = default)
   {
      ArgumentNullException.ThrowIfNull(style);
      if (await _dbContext.SlugExistsAsync<PostStyle>(
         style.Slug, excludedId: style.Id, cancellationToken))
      {
         throw new DuplicateSlugException(style.Slug);
      }

      var stored = await _dbContext.PostStyles
         .SingleOrDefaultAsync(s => s.Id == style.Id, cancellationToken);
      if (stored is null)
      {
         return false;
      }

      stored.IsActive = style.IsActive;
      stored.Slug = style.Slug;
      stored.Description = style.Description;
      stored.DisplayName = style.DisplayName;
      await _dbContext.SaveChangesAsync(cancellationToken);
      return true;
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

   private static void UpdatePostTags(
      Post existing,
      IReadOnlySet<int> selectedTagIds)
   {
      var existingTagIds = existing.Tags
         .Select(tag => tag.WritingTagId)
         .ToHashSet();

      var tagsToRemove = existing.Tags
         .Where(tag => !selectedTagIds.Contains(tag.WritingTagId))
         .ToList();

      foreach (var tag in tagsToRemove)
      {
         existing.Tags.Remove(tag);
      }

      var tagIdsToAdd = selectedTagIds
         .Where(tagId => !existingTagIds.Contains(tagId));

      foreach (int tagId in tagIdsToAdd)
      {
         existing.Tags.Add(new PostTag()
         {
            PostId = existing.Id,
            WritingTagId = tagId,
         });
      }
   }

   private readonly ILogger<SqlWritingStore> _logger;
   private readonly SiteDbContext _dbContext;
}
