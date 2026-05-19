// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Models.Writing;

namespace DavidBrowning.Data.Stores.Writing
{
   public sealed class DummyWritingStore : IWritingStore
   {
      private readonly IReadOnlyList<Post> _posts;
      private readonly IReadOnlyList<WritingTag> _tags;

      public DummyWritingStore()
      {
         WritingTag engineeringTag = new()
         {
            Id = 1,
            Name = "Engineering",
            Slug = "engineering"
         };

         WritingTag systemsTag = new()
         {
            Id = 2,
            Name = "Systems",
            Slug = "systems"
         };

         WritingTag designTag = new()
         {
            Id = 3,
            Name = "Design",
            Slug = "design"
         };

         _tags = new[]
         {
            engineeringTag,
            systemsTag,
            designTag
         };

         Post firstPost = CreatePost(
            id: 1,
            slug: "configuration-is-an-api",
            title: "Configuration Is an API",
            summary: "A short note about treating configuration as a real contract.",
            isFeatured: true,
            tags: new[] { engineeringTag, systemsTag });

         Post secondPost = CreatePost(
            id: 2,
            slug: "software-should-explain-itself",
            title: "Software Should Explain Itself",
            summary: "A musing about visible state, user trust, and good internal tools.",
            isFeatured: true,
            tags: new[] { engineeringTag, designTag });

         Post thirdPost = CreatePost(
            id: 3,
            slug: "fast-flexible-familiar-friendly",
            title: "Fast, Flexible, Familiar, Friendly",
            summary: "A compact design heuristic for APIs, tools, and systems.",
            isFeatured: false,
            tags: new[] { engineeringTag, designTag });

         _posts = new[]
         {
            firstPost,
            secondPost,
            thirdPost
         };
      }

      public Task<IReadOnlyList<Post>> GetPublishedPostsAsync(
         CancellationToken cancellationToken = default)
      {
         IReadOnlyList<Post> posts = _posts
            .Where(post => post.Status == PostStatus.Published)
            .OrderByDescending(post => post.PublishedDateUtc)
            .ToList();

         return Task.FromResult(posts);
      }

      public Task<IReadOnlyList<Post>> GetFeaturedPostsAsync(
         CancellationToken cancellationToken = default)
      {
         IReadOnlyList<Post> posts = _posts
            .Where(post => post.Status == PostStatus.Published && post.IsFeatured)
            .OrderByDescending(post => post.PublishedDateUtc)
            .ToList();

         return Task.FromResult(posts);
      }

      public Task<Post?> GetPublishedPostBySlugAsync(
         string slug,
         CancellationToken cancellationToken = default)
      {
         Post? post = _posts.SingleOrDefault(candidate =>
            candidate.Status == PostStatus.Published &&
            string.Equals(candidate.Slug, slug, StringComparison.OrdinalIgnoreCase));

         return Task.FromResult(post);
      }

      public Task<IReadOnlyList<WritingTag>> GetTagsAsync(
         CancellationToken cancellationToken = default)
      {
         return Task.FromResult(_tags);
      }

      public Task<IReadOnlyList<Post>> GetPublishedPostsByTagSlugAsync(
         string tagSlug,
         CancellationToken cancellationToken = default)
      {
         IReadOnlyList<Post> posts = _posts
            .Where(post =>
               post.Status == PostStatus.Published &&
               post.Tags.Any(postTag =>
                  string.Equals(
                     postTag.WritingTag?.Slug,
                     tagSlug,
                     StringComparison.OrdinalIgnoreCase)))
            .OrderByDescending(post => post.PublishedDateUtc)
            .ToList();

         return Task.FromResult(posts);
      }

      private static Post CreatePost(
         int id,
         string slug,
         string title,
         string summary,
         bool isFeatured,
         IReadOnlyList<WritingTag> tags)
      {
         DateTime nowUtc = DateTime.UtcNow;

         Post post = new()
         {
            Id = id,
            Slug = slug,
            Title = title,
            Subtitle = null,
            Summary = summary,
            MetaDescription = summary,
            Status = PostStatus.Published,
            IsFeatured = isFeatured,
            CreatedDateUtc = nowUtc.AddDays(-id * 10),
            LastUpdatedDateUtc = nowUtc.AddDays(-id),
            PublishedDateUtc = nowUtc.AddDays(-id * 5)
         };

         PostRevision revision = new()
         {
            Id = id,
            PostId = id,
            Post = post,
            RevisionNumber = 1,
            ContentFormat = PostContentFormat.Markdown,
            RenderMode = PostRenderMode.Article,
            CreatedBy = "Dummy",
            CreatedAtUtc = nowUtc.AddDays(-id * 5),
            Content = $"# {title}\n\nThis is dummy content for the test writing store.",
            CachedHtml = $"<h1>{title}</h1><p>This is dummy content for the test writing store.</p>"
         };

         post.Revisions.Add(revision);
         post.CurrentRevisionId = revision.Id;
         post.CurrentRevision = revision;

         foreach (WritingTag tag in tags)
         {
            post.Tags.Add(new PostTag
            {
               PostId = post.Id,
               Post = post,
               WritingTagId = tag.Id,
               WritingTag = tag
            });
         }

         return post;
      }
   }
}