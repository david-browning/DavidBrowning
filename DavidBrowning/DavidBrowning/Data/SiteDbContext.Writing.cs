// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using DavidBrowning.Models;
using DavidBrowning.Models.Writing;
using Microsoft.EntityFrameworkCore;

namespace DavidBrowning.Data;

public sealed partial class SiteDbContext
{
   private static void ConfigureWriting(ModelBuilder modelBuilder)
   {
      ConfigurePosts(modelBuilder);
      ConfigurePostRevisions(modelBuilder);
      ConfigureWritingTags(modelBuilder);
      ConfigurePostTags(modelBuilder);
   }

   private static void ConfigurePosts(ModelBuilder modelBuilder)
   {
      modelBuilder.Entity<Post>(entity =>
      {
         entity.ToTable("db_Posts");

         entity.HasKey(post => post.Id);

         entity.HasIndex(post => post.Slug)
            .IsUnique();

         entity.Property(post => post.Status)
            .HasConversion<byte>()
            .IsRequired();

         entity.Property(post => post.CreatedDateUtc)
            .HasColumnType("datetime2(0)")
            .HasDefaultValueSql("sysutcdatetime()")
            .IsRequired();

         entity.Property(post => post.LastUpdatedDateUtc)
            .HasColumnType("datetime2(0)")
            .HasDefaultValueSql("sysutcdatetime()")
            .IsRequired();

         entity.Property(post => post.PublishedDateUtc)
            .HasColumnType("datetime2(0)");

         entity.HasMany(post => post.Revisions)
            .WithOne(revision => revision.Post)
            .HasForeignKey(revision => revision.PostId)
            .OnDelete(DeleteBehavior.Cascade);

         entity.HasOne(post => post.CurrentRevision)
            .WithMany()
            .HasForeignKey(post => post.CurrentRevisionId)
            .OnDelete(DeleteBehavior.Restrict);
      });
   }

   private static void ConfigurePostRevisions(ModelBuilder modelBuilder)
   {
      modelBuilder.Entity<PostRevision>(entity =>
      {
         entity.ToTable("db_PostRevisions");

         entity.HasKey(revision => revision.Id);

         entity.HasIndex(revision => new
         {
            revision.PostId,
            revision.RevisionNumber
         })
         .IsUnique();

         entity.Property(revision => revision.ContentFormat)
            .HasConversion<byte>()
            .IsRequired();

         entity.Property(revision => revision.RenderMode)
            .HasConversion<byte>()
            .IsRequired();

         entity.Property(revision => revision.CreatedAtUtc)
            .HasColumnType("datetime2(0)")
            .HasDefaultValueSql("sysutcdatetime()")
            .IsRequired();

         entity.Property(revision => revision.Content)
            .HasColumnType("nvarchar(max)");

         entity.Property(revision => revision.CachedHtml)
            .HasColumnType("nvarchar(max)");
      });
   }

   private static void ConfigureWritingTags(ModelBuilder modelBuilder)
   {
      modelBuilder.Entity<WritingTag>(entity =>
      {
         entity.ToTable("db_WritingTags");

         entity.HasKey(tag => tag.Id);

         entity.HasIndex(tag => tag.Slug)
            .IsUnique();

         entity.Property(tag => tag.DisplayName)
         .HasMaxLength(DataConstants.MaxLabelLength)
         .IsRequired();
      });
   }

   private static void ConfigurePostTags(ModelBuilder modelBuilder)
   {
      modelBuilder.Entity<PostTag>(entity =>
      {
         entity.ToTable("db_PostTags");

         entity.HasKey(postTag => new
         {
            postTag.PostId,
            postTag.WritingTagId
         });

         entity.HasOne(postTag => postTag.Post)
            .WithMany(post => post.Tags)
            .HasForeignKey(postTag => postTag.PostId)
            .OnDelete(DeleteBehavior.Cascade);

         entity.HasOne(postTag => postTag.WritingTag)
            .WithMany(tag => tag.PostTags)
            .HasForeignKey(postTag => postTag.WritingTagId)
            .OnDelete(DeleteBehavior.Cascade);

         entity.HasIndex(postTag => postTag.WritingTagId);
      });
   }
}