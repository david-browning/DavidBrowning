// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Collections.Generic;
using DavidBrowning.Models;
using DavidBrowning.Models.Projects;
using Microsoft.EntityFrameworkCore;

namespace DavidBrowning.Data;

internal sealed partial class SiteDbContext
{
   private static void ConfigureProjects(ModelBuilder modelBuilder)
   {
      ConfigureProjectStatuses(modelBuilder);
      ConfigureProjectTypes(modelBuilder);
      ConfigureProjectOrigins(modelBuilder);
      ConfigureProjectVisibilities(modelBuilder);

      ConfigureProjectEntities(modelBuilder);
      ConfigureProjectTags(modelBuilder);
      ConfigureProjectTagLinks(modelBuilder);
      ConfigureProjectStackTags(modelBuilder);
      ConfigureProjectStackTagLinks(modelBuilder);
      ConfigureProjectLinkTypes(modelBuilder);
      ConfigureProjectLinks(modelBuilder);
      ConfigureProjectAssetRoles(modelBuilder);
      ConfigureProjectAssetLinks(modelBuilder);
      ConfigureProjectPosts(modelBuilder);
   }

   private static void ConfigureProjectStatuses(ModelBuilder modelBuilder)
   {
      ConfigureProjectLookup<ProjectStatus>(
         modelBuilder,
         "db_ProjectStatuses",
         status => status.Projects);
   }

   private static void ConfigureProjectTypes(ModelBuilder modelBuilder)
   {
      ConfigureProjectLookup<ProjectType>(
         modelBuilder,
         "db_ProjectTypes",
         type => type.Projects);
   }

   private static void ConfigureProjectOrigins(ModelBuilder modelBuilder)
   {
      ConfigureProjectLookup<ProjectOrigin>(
         modelBuilder,
         "db_ProjectOrigins",
         origin => origin.Projects);
   }

   private static void ConfigureProjectVisibilities(ModelBuilder modelBuilder)
   {
      ConfigureProjectLookup<ProjectVisibility>(
         modelBuilder,
         "db_ProjectVisibilities",
         visibility => visibility.Projects);
   }

   private static void ConfigureProjectLinkTypes(ModelBuilder modelBuilder)
   {
      modelBuilder.Entity<ProjectLinkType>(entity =>
      {
         entity.ToTable("db_ProjectLinkTypes");

         entity.HasKey(type => type.Id);

         entity.HasIndex(type => type.Slug)
            .IsUnique();

         entity.Property(type => type.SortOrder)
            .HasDefaultValue(0)
            .IsRequired();

         entity.Property(type => type.IsActive)
            .HasDefaultValue(true)
            .IsRequired();
      });
   }

   private static void ConfigureProjectEntities(ModelBuilder modelBuilder)
   {
      modelBuilder.Entity<Project>(entity =>
      {
         entity.ToTable("db_Projects");

         entity.HasKey(project => project.Id);

         entity.HasIndex(project => project.Slug)
            .IsUnique();

         entity.Property(project => project.Problem)
            .HasColumnType("nvarchar(max)");

         entity.Property(project => project.Solution)
            .HasColumnType("nvarchar(max)");

         entity.Property(project => project.Result)
            .HasColumnType("nvarchar(max)");

         entity.Property(project => project.Tradeoffs)
            .HasColumnType("nvarchar(max)");

         entity.Property(project => project.IsFeatured)
            .HasDefaultValue(false)
            .IsRequired();

         entity.Property(project => project.SortOrder)
            .HasDefaultValue(0)
            .IsRequired();

         entity.Property(project => project.CreatedAtUtc)
            .HasColumnType("datetime2(0)")
            .HasDefaultValueSql("sysutcdatetime()")
            .IsRequired();

         entity.Property(project => project.UpdatedAtUtc)
            .HasColumnType("datetime2(0)")
            .HasDefaultValueSql("sysutcdatetime()")
            .IsRequired();

         entity.HasOne(project => project.ProjectStatus)
            .WithMany(status => status.Projects)
            .HasForeignKey(project => project.ProjectStatusId)
            .OnDelete(DeleteBehavior.Restrict);

         entity.HasOne(project => project.ProjectType)
            .WithMany(type => type.Projects)
            .HasForeignKey(project => project.ProjectTypeId)
            .OnDelete(DeleteBehavior.Restrict);

         entity.HasOne(project => project.ProjectOrigin)
            .WithMany(origin => origin.Projects)
            .HasForeignKey(project => project.ProjectOriginId)
            .OnDelete(DeleteBehavior.Restrict);

         entity.HasOne(project => project.ProjectVisibility)
            .WithMany(visibility => visibility.Projects)
            .HasForeignKey(project => project.ProjectVisibilityId)
            .OnDelete(DeleteBehavior.Restrict);

         entity.HasIndex(project => new
         {
            project.ProjectVisibilityId,
            project.SortOrder
         });

         entity.HasIndex(project => new
         {
            project.IsFeatured,
            project.SortOrder
         });
      });
   }

   private static void ConfigureProjectTags(ModelBuilder modelBuilder)
   {
      modelBuilder.Entity<ProjectTag>(entity =>
      {
         entity.ToTable("db_ProjectTags");

         entity.HasKey(tag => tag.Id);

         entity.HasIndex(tag => tag.Slug)
            .IsUnique();

         entity.Property(tag => tag.SortOrder)
            .HasDefaultValue(0)
            .IsRequired();

         entity.Property(tag => tag.IsActive)
            .HasDefaultValue(true)
            .IsRequired();
      });
   }

   private static void ConfigureProjectTagLinks(ModelBuilder modelBuilder)
   {
      modelBuilder.Entity<ProjectTagLink>(entity =>
      {
         entity.ToTable("db_ProjectTagLinks");

         entity.HasKey(link => new
         {
            link.ProjectId,
            link.ProjectTagId
         });

         entity.HasOne(link => link.Project)
            .WithMany(project => project.TagLinks)
            .HasForeignKey(link => link.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

         entity.HasOne(link => link.ProjectTag)
            .WithMany(tag => tag.ProjectLinks)
            .HasForeignKey(link => link.ProjectTagId)
            .OnDelete(DeleteBehavior.Cascade);

         entity.HasIndex(link => link.ProjectTagId);
      });
   }

   private static void ConfigureProjectStackTags(ModelBuilder modelBuilder)
   {
      modelBuilder.Entity<ProjectStackTag>(entity =>
      {
         entity.ToTable("db_ProjectStackTags");

         entity.HasKey(tag => tag.Id);

         entity.HasIndex(tag => tag.Slug)
            .IsUnique();

         entity.Property(tag => tag.SortOrder)
            .HasDefaultValue(0)
            .IsRequired();

         entity.Property(tag => tag.IsActive)
            .HasDefaultValue(true)
            .IsRequired();
      });
   }

   private static void ConfigureProjectStackTagLinks(ModelBuilder modelBuilder)
   {
      modelBuilder.Entity<ProjectStackTagLink>(entity =>
      {
         entity.ToTable("db_ProjectStackTagLinks");

         entity.HasKey(link => new
         {
            link.ProjectId,
            link.ProjectStackTagId
         });

         entity.HasOne(link => link.Project)
            .WithMany(project => project.StackTagLinks)
            .HasForeignKey(link => link.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

         entity.HasOne(link => link.ProjectStackTag)
            .WithMany(tag => tag.ProjectLinks)
            .HasForeignKey(link => link.ProjectStackTagId)
            .OnDelete(DeleteBehavior.Cascade);

         entity.HasIndex(link => link.ProjectStackTagId);
      });
   }

   private static void ConfigureProjectLinks(ModelBuilder modelBuilder)
   {
      modelBuilder.Entity<ProjectLink>(entity =>
      {
         entity.ToTable("db_ProjectLinks");

         entity.HasKey(link => link.Id);

         entity.Property(link => link.IsPrimary)
            .HasDefaultValue(false)
            .IsRequired();

         entity.Property(link => link.SortOrder)
            .HasDefaultValue(0)
            .IsRequired();

         entity.HasOne(link => link.Project)
            .WithMany(project => project.Links)
            .HasForeignKey(link => link.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

         entity.HasOne(link => link.ProjectLinkType)
            .WithMany(type => type.Links)
            .HasForeignKey(link => link.ProjectLinkTypeId)
            .OnDelete(DeleteBehavior.Restrict);

         entity.HasIndex(link => new
         {
            link.ProjectId,
            link.SortOrder
         });

         entity.HasIndex(link => link.ProjectLinkTypeId);
      });
   }

   private static void ConfigureProjectAssetRoles(ModelBuilder modelBuilder)
   {
      modelBuilder.Entity<ProjectAssetRole>(entity =>
      {
         entity.ToTable("db_ProjectAssetRoles");

         entity.HasKey(role => role.Id);

         entity.HasIndex(role => role.Slug)
            .IsUnique();

         entity.Property(role => role.SortOrder)
            .HasDefaultValue(0)
            .IsRequired();

         entity.Property(role => role.IsActive)
            .HasDefaultValue(true)
            .IsRequired();
      });
   }

   private static void ConfigureProjectAssetLinks(ModelBuilder modelBuilder)
   {
      modelBuilder.Entity<ProjectAssetLink>(entity =>
      {
         entity.ToTable("db_ProjectAssetLinks");

         entity.HasKey(link => new
         {
            link.ProjectId,
            link.SiteAssetId,
            link.ProjectAssetRoleId
         });

         entity.Property(link => link.SortOrder)
            .HasDefaultValue(0)
            .IsRequired();

         entity.HasOne(link => link.Project)
            .WithMany(project => project.AssetLinks)
            .HasForeignKey(link => link.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

         entity.HasOne(link => link.SiteAsset)
            .WithMany(asset => asset.ProjectLinks)
            .HasForeignKey(link => link.SiteAssetId)
            .OnDelete(DeleteBehavior.Cascade);

         entity.HasOne(link => link.ProjectAssetRole)
            .WithMany(role => role.AssetLinks)
            .HasForeignKey(link => link.ProjectAssetRoleId)
            .OnDelete(DeleteBehavior.Restrict);

         entity.HasIndex(link => link.SiteAssetId);

         entity.HasIndex(link => new
         {
            link.ProjectId,
            link.ProjectAssetRoleId,
            link.SortOrder
         });
      });
   }

   private static void ConfigureProjectPosts(ModelBuilder modelBuilder)
   {
      modelBuilder.Entity<ProjectPost>(entity =>
      {
         entity.ToTable("db_ProjectPosts");

         entity.HasKey(link => new
         {
            link.ProjectId,
            link.PostId
         });

         entity.Property(link => link.SortOrder)
            .HasDefaultValue(0)
            .IsRequired();

         entity.HasOne(link => link.Project)
            .WithMany(project => project.RelatedPosts)
            .HasForeignKey(link => link.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

         entity.HasOne(link => link.Post)
            .WithMany()
            .HasForeignKey(link => link.PostId)
            .OnDelete(DeleteBehavior.Cascade);

         entity.HasIndex(link => link.PostId);

         entity.HasIndex(link => new
         {
            link.ProjectId,
            link.SortOrder
         });
      });
   }

   private static void ConfigureProjectLookup<TLookup>(
      ModelBuilder modelBuilder,
      string tableName,
      System.Linq.Expressions.Expression<Func<TLookup, IEnumerable<Project>>> navigationExpression)
      where TLookup : class
   {
      modelBuilder.Entity<TLookup>(entity =>
      {
         entity.ToTable(tableName);

         entity.HasKey("Id");

         entity.HasIndex("Slug")
            .IsUnique();

         entity.Property<string>("Slug")
            .HasMaxLength(DataConstants.MaxSlugLength)
            .IsRequired();

         entity.Property<string>("DisplayName")
            .HasMaxLength(DataConstants.MaxLabelLength)
            .IsRequired();

         entity.Property<string>("Description")
            .HasMaxLength(DataConstants.MaxMetadataLength);

         entity.Property<int>("SortOrder")
            .HasDefaultValue(0)
            .IsRequired();

         entity.Property<bool>("IsActive")
            .HasDefaultValue(true)
            .IsRequired();
      });
   }
}