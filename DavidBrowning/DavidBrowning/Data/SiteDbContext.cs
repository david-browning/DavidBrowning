// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using DavidBrowning.Models;
using DavidBrowning.Models.Error;
using DavidBrowning.Models.Projects;
using DavidBrowning.Models.Writing;
using Microsoft.EntityFrameworkCore;

namespace DavidBrowning.Data;

public sealed partial class SiteDbContext : DbContext
{
   public SiteDbContext(DbContextOptions<SiteDbContext> options)
      : base(options)
   {
   }

   public DbSet<Post> Posts => Set<Post>();
   
   public DbSet<PostStyle> PostStyles => Set<PostStyle>();

   public DbSet<PostRevision> PostRevisions => Set<PostRevision>();

   public DbSet<WritingTag> WritingTags => Set<WritingTag>();

   public DbSet<PostTag> PostTags => Set<PostTag>();

   public DbSet<SiteAsset> SiteAssets => Set<SiteAsset>();

   public DbSet<SiteAssetLink> SiteAssetLinks => Set<SiteAssetLink>();

   public DbSet<WebsiteError> WebsiteErrors => Set<WebsiteError>();

   public DbSet<Project> Projects => Set<Project>();

   public DbSet<ProjectStatus> ProjectStatuses => Set<ProjectStatus>();

   public DbSet<ProjectType> ProjectTypes => Set<ProjectType>();

   public DbSet<ProjectOrigin> ProjectOrigins => Set<ProjectOrigin>();

   public DbSet<ProjectVisibility> ProjectVisibilities => Set<ProjectVisibility>();

   public DbSet<ProjectTag> ProjectTags => Set<ProjectTag>();

   public DbSet<ProjectTagLink> ProjectTagLinks => Set<ProjectTagLink>();

   public DbSet<ProjectStackTag> ProjectStackTags => Set<ProjectStackTag>();

   public DbSet<ProjectStackTagLink> ProjectStackTagLinks => Set<ProjectStackTagLink>();

   public DbSet<ProjectLinkType> ProjectLinkTypes => Set<ProjectLinkType>();

   public DbSet<ProjectLink> ProjectLinks => Set<ProjectLink>();

   public DbSet<ProjectAssetRole> ProjectAssetRoles => Set<ProjectAssetRole>();

   public DbSet<ProjectAssetLink> ProjectAssetLinks => Set<ProjectAssetLink>();

   public DbSet<ProjectPost> ProjectPosts => Set<ProjectPost>();

   public DbSet<Interest> Interests => Set<Interest>();

   protected override void OnModelCreating(ModelBuilder modelBuilder)
   {
      base.OnModelCreating(modelBuilder);

      ConfigureErrors(modelBuilder);
      ConfigureSiteAssets(modelBuilder);
      ConfigureWriting(modelBuilder);
      ConfigureProjects(modelBuilder);
   }
}