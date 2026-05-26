// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using DavidBrowning.Models;
using Microsoft.EntityFrameworkCore;

namespace DavidBrowning.Data;

internal sealed partial class SiteDbContext
{
   private static void ConfigureSiteAssets(ModelBuilder modelBuilder)
   {
      modelBuilder.Entity<SiteAsset>(entity =>
      {
         entity.ToTable("db_SiteAssets");

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

         entity.Property(asset => asset.CreatedAtUtc)
            .HasColumnType("datetime2(0)")
            .HasDefaultValueSql("sysutcdatetime()")
            .IsRequired();
      });

      modelBuilder.Entity<SiteAssetLink>(entity =>
      {
         entity.ToTable("db_SiteAssetLinks");

         entity.HasKey(link => new
         {
            link.PostId,
            link.SiteAssetId,
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

         entity.HasOne(link => link.SiteAsset)
            .WithMany(asset => asset.PostLinks)
            .HasForeignKey(link => link.SiteAssetId)
            .OnDelete(DeleteBehavior.Cascade);

         entity.HasIndex(link => link.SiteAssetId);

         entity.HasIndex(link => new
         {
            link.PostId,
            link.Role,
            link.SortOrder
         });
      });
   }
}