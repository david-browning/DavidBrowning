// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.

using DavidBrowning.Models;
using Microsoft.EntityFrameworkCore;

namespace DavidBrowning.Infrastructure.Data;

public sealed partial class SiteDbContext
{
   private static void ConfigureSiteAssets(ModelBuilder modelBuilder)
   {
      modelBuilder.Entity<SiteAsset>(entity =>
      {
         entity.ToTable("db_SiteAssets");

         entity.HasKey(asset => asset.Id);

         entity.HasIndex(asset => asset.AssetKey)
            .IsUnique();

         entity.Property(asset => asset.CreatedAtUtc)
            .HasColumnType("datetime2(0)")
            .HasDefaultValueSql("sysutcdatetime()")
            .IsRequired();
      });

      modelBuilder.Entity<PostRevisionAssetLink>(entity =>
      {
         entity.ToTable("db_PostRevisionAssetLinks");

         entity.HasKey(link => new
         {
            link.PostRevisionId,
            link.SiteAssetId,
         });

         entity.HasOne(link => link.PostRevision)
            .WithMany(revision => revision.AssetLinks)
            .HasForeignKey(link => link.PostRevisionId)
            .OnDelete(DeleteBehavior.Cascade);

         entity.HasOne(link => link.SiteAsset)
            .WithMany(asset => asset.PostRevisionLinks)
            .HasForeignKey(link => link.SiteAssetId)
            .OnDelete(DeleteBehavior.Cascade);

         entity.HasIndex(link => link.SiteAssetId);

         entity.HasIndex(link => new
         {
            link.PostRevisionId,
            link.ReferenceKey,
         })
            .IsUnique();
      });
   }
}