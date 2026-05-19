// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DavidBrowning.Models;
using DavidBrowning.Models.Error;
using DavidBrowning.Models.Writing;
using Microsoft.EntityFrameworkCore;

namespace DavidBrowning.Data
{
   public sealed class SiteDbContext : DbContext
   {
      public SiteDbContext(DbContextOptions<SiteDbContext> options)
         : base(options)
      {
      }

      public DbSet<Post> Posts => Set<Post>();

      public DbSet<PostRevision> PostRevisions => Set<PostRevision>();

      public DbSet<WritingTag> WritingTags => Set<WritingTag>();

      public DbSet<PostTag> PostTags => Set<PostTag>();

      public DbSet<PostAsset> PostAssets => Set<PostAsset>();

      public DbSet<PostAssetLink> PostAssetLinks => Set<PostAssetLink>();
      
      public DbSet<WebsiteError> ErrorLogEntries => Set<WebsiteError>();

      protected override void OnModelCreating(ModelBuilder modelBuilder)
      {
         base.OnModelCreating(modelBuilder);

         ConfigureErrors(modelBuilder);
         ConfigurePosts(modelBuilder);
         ConfigurePostRevisions(modelBuilder);
         ConfigureWritingTags(modelBuilder);
         ConfigurePostTags(modelBuilder);
         ConfigurePostAssets(modelBuilder);
         ConfigurePostAssetLinks(modelBuilder);
      }

      private static void ConfigureErrors(ModelBuilder modelBuilder)
      {
         modelBuilder.Entity<WebsiteError>(entity =>
         {
            entity.ToTable("db_ErrorLogEntries");
            entity.HasKey(error => error.Id);

            entity.HasIndex(error => error.OccurredAtUtc);

            entity.HasIndex(error => error.TraceIdentifier);

            entity.Property(error => error.OccurredAtUtc)
               .HasColumnType("datetime2(0)")
               .HasDefaultValueSql("sysutcdatetime()")
               .IsRequired();
         });
      }

      private static void ConfigurePosts(ModelBuilder modelBuilder)
      {
         modelBuilder.Entity<Post>(entity =>
         {
            entity.ToTable("db_Posts");

            entity.HasKey(post => post.Id);

            entity.HasIndex(post => post.Slug)
               .IsUnique();

            entity.Property(post => post.Slug)
               .HasMaxLength(DataConstants.MaxSlugLength)
               .IsRequired();

            entity.Property(post => post.Title)
               .HasMaxLength(DataConstants.MaxLabelLength)
               .IsRequired();

            entity.Property(post => post.Subtitle)
               .HasMaxLength(DataConstants.MaxLabelLength);

            entity.Property(post => post.Summary)
               .HasMaxLength(DataConstants.MaxMetadataLength);

            entity.Property(post => post.MetaDescription)
               .HasMaxLength(DataConstants.MaxMetadataLength);

            entity.Property(post => post.Status)
               .HasConversion<byte>()
               .IsRequired();

            entity.Property(post => post.IsFeatured)
               .HasDefaultValue(false)
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

            entity.Property(revision => revision.CreatedBy)
               .HasMaxLength(DataConstants.MaxNameLength)
               .IsRequired();

            entity.Property(revision => revision.CreatedAtUtc)
               .HasColumnType("datetime2(0)")
               .HasDefaultValueSql("sysutcdatetime()")
               .IsRequired();

            entity.Property(revision => revision.Content)
               .HasColumnType("nvarchar(max)");

            entity.Property(revision => revision.CachedHtml)
               .HasColumnName("CachedHtml")
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

            entity.Property(tag => tag.Name)
               .HasMaxLength(DataConstants.MaxLabelLength)
               .IsRequired();

            entity.Property(tag => tag.Slug)
               .HasMaxLength(DataConstants.MaxSlugLength)
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

      private static void ConfigurePostAssets(ModelBuilder modelBuilder)
      {
         modelBuilder.Entity<PostAsset>(entity =>
         {
            entity.ToTable("db_PostAssets");

            entity.HasKey(asset => asset.Id);

            entity.HasIndex(asset => new
            {
               asset.BlobContainer,
               asset.BlobName
            })
            .IsUnique();

            entity.Property(asset => asset.AssetType)
               .HasConversion<byte>()
               .IsRequired();

            entity.Property(asset => asset.OriginalFileName)
               .HasMaxLength(DataConstants.MaxNameLength);

            entity.Property(asset => asset.BlobContainer)
               .HasMaxLength(DataConstants.MaxAzureAssetLength)
               .IsRequired();

            entity.Property(asset => asset.BlobName)
               .HasMaxLength(DataConstants.MaxAzureAssetLength)
               .IsRequired();

            entity.Property(asset => asset.SizeBytes)
               .IsRequired();

            entity.Property(asset => asset.CreatedAtUtc)
               .HasColumnType("datetime2(0)")
               .HasDefaultValueSql("sysutcdatetime()")
               .IsRequired();
         });
      }

      private static void ConfigurePostAssetLinks(ModelBuilder modelBuilder)
      {
         modelBuilder.Entity<PostAssetLink>(entity =>
         {
            entity.ToTable("db_PostAssetLinks");

            entity.HasKey(link => new
            {
               link.PostId,
               link.PostAssetId,
               link.Role
            });

            entity.Property(link => link.Role)
               .HasConversion<byte>()
               .IsRequired();

            entity.Property(link => link.SortOrder)
               .HasDefaultValue(0)
               .IsRequired();

            entity.HasOne(link => link.Post)
               .WithMany(post => post.AssetLinks)
               .HasForeignKey(link => link.PostId)
               .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(link => link.PostAsset)
               .WithMany(asset => asset.PostLinks)
               .HasForeignKey(link => link.PostAssetId)
               .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(link => link.PostAssetId);

            entity.HasIndex(link => new
            {
               link.PostId,
               link.Role,
               link.SortOrder
            });
         });
      }
   }
}