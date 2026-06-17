// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using DavidBrowning.Models;
using DavidBrowning.Models.Error;
using DavidBrowning.Models.Projects;
using DavidBrowning.Models.Work;
using DavidBrowning.Models.Writing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DavidBrowning.Infrastructure.Data;

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

   public DbSet<PostRevisionAssetLink> PostRevisionAssetLinks => Set<PostRevisionAssetLink>();

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

   public DbSet<Experience> Experiences => Set<Experience>();

   public DbSet<ExperienceRole> ExperienceRoles => Set<ExperienceRole>();

   public DbSet<ExperienceRoleBullet> ExperienceRoleBullets =>
      Set<ExperienceRoleBullet>();

   public DbSet<Credential> Credentials => Set<Credential>();

   public override int SaveChanges()
   {
      SetTimestamps();
      return base.SaveChanges();
   }

   public override int SaveChanges(bool acceptAllChangesOnSuccess)
   {
      SetTimestamps();
      return base.SaveChanges(acceptAllChangesOnSuccess);
   }

   public override Task<int> SaveChangesAsync(
      CancellationToken cancellationToken = default)
   {
      SetTimestamps();
      return base.SaveChangesAsync(cancellationToken);
   }

   public override Task<int> SaveChangesAsync(
      bool acceptAllChangesOnSuccess,
      CancellationToken cancellationToken = default)
   {
      SetTimestamps();
      return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
   }

   protected override void OnModelCreating(ModelBuilder modelBuilder)
   {
      base.OnModelCreating(modelBuilder);
      ConfigureErrors(modelBuilder);
      ConfigureSiteAssets(modelBuilder);
      ConfigureWriting(modelBuilder);
      ConfigureProjects(modelBuilder);
      ConfigureWork(modelBuilder);
   }

   private void SetTimestamps()
   {
      var utcNow = DateTime.UtcNow;
      foreach (var entry in ChangeTracker.Entries<IDateCreatedTrackedEntity>())
      {
         SetCreatedTimestamp(entry, utcNow);
      }

      foreach (var entry in ChangeTracker.Entries<IDateUpdatedTrackedEntity>())
      {
         SetUpdatedTimestamp(entry, utcNow);
      }
   }

   private void SetCreatedTimestamp(
      EntityEntry<IDateCreatedTrackedEntity> entity,
      DateTime utcNow)
   {
      switch (entity.State)
      {
         case EntityState.Added:
         {
            entity.Entity.CreatedAtUtc = utcNow;
            break;
         }
         case EntityState.Modified:
         {
            // A posted or mapped entity must not rewrite its creation time.
            entity.Property(entity => entity.CreatedAtUtc).IsModified = false;
            break;
         }
      }
   }

   private void SetUpdatedTimestamp(
      EntityEntry<IDateUpdatedTrackedEntity> entity,
      DateTime utcNow)
   {
      switch (entity.State)
      {
         case EntityState.Added:
         {
            entity.Entity.UpdatedAtUtc = utcNow;
            break;
         }
         case EntityState.Modified:
         {
            entity.Entity.UpdatedAtUtc = utcNow;
            break;
         }
      }
   }
}