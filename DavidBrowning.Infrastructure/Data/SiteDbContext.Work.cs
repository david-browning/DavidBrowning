// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using DavidBrowning.Models.Work;
using Microsoft.EntityFrameworkCore;
namespace DavidBrowning.Data;

public sealed partial class SiteDbContext
{
   private static void ConfigureWork(ModelBuilder modelBuilder)
   {
      ConfigureExperiences(modelBuilder);
      ConfigureExperienceRoles(modelBuilder);
      ConfigureExperienceRoleBullets(modelBuilder);
      ConfigureCredentials(modelBuilder);
   }

   private static void ConfigureExperiences(ModelBuilder modelBuilder)
   {
      modelBuilder.Entity<Experience>(entity =>
      {
         entity.ToTable("db_Experiences");

         entity.HasKey(experience => experience.Id);

         entity.Property(experience => experience.SortOrder)
            .HasDefaultValue(0)
            .IsRequired();

         entity.Property(experience => experience.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

         entity.Property(experience => experience.CreatedAtUtc)
            .HasColumnType("datetime2(0)")
            .HasDefaultValueSql("sysutcdatetime()")
            .IsRequired();

         entity.Property(experience => experience.UpdatedAtUtc)
            .HasColumnType("datetime2(0)")
            .HasDefaultValueSql("sysutcdatetime()")
            .IsRequired();

         entity.HasIndex(experience => experience.SortOrder);
      });
   }

   private static void ConfigureExperienceRoles(ModelBuilder modelBuilder)
   {
      modelBuilder.Entity<ExperienceRole>(entity =>
      {
         entity.ToTable("db_ExperienceRoles");

         entity.HasKey(role => role.Id);

         entity.Property(role => role.SortOrder)
            .HasDefaultValue(0)
            .IsRequired();

         entity.Property(role => role.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

         entity.Property(role => role.CreatedAtUtc)
            .HasColumnType("datetime2(0)")
            .HasDefaultValueSql("sysutcdatetime()")
            .IsRequired();

         entity.Property(role => role.UpdatedAtUtc)
            .HasColumnType("datetime2(0)")
            .HasDefaultValueSql("sysutcdatetime()")
            .IsRequired();

         entity.HasOne(role => role.Experience)
            .WithMany(experience => experience.Roles)
            .HasForeignKey(role => role.ExperienceId)
            .OnDelete(DeleteBehavior.Cascade);

         entity.HasIndex(role => new
         {
            role.ExperienceId,
            role.SortOrder,
         });
      });
   }

   private static void ConfigureExperienceRoleBullets(
      ModelBuilder modelBuilder)
   {
      modelBuilder.Entity<ExperienceRoleBullet>(entity =>
      {
         entity.ToTable("db_ExperienceRoleBullets");

         entity.HasKey(bullet => bullet.Id);

         entity.Property(bullet => bullet.SortOrder)
            .HasDefaultValue(0)
            .IsRequired();

         entity.Property(bullet => bullet.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

         entity.HasOne(bullet => bullet.ExperienceRole)
            .WithMany(role => role.Bullets)
            .HasForeignKey(bullet => bullet.ExperienceRoleId)
            .OnDelete(DeleteBehavior.Cascade);

         entity.HasIndex(bullet => new
         {
            bullet.ExperienceRoleId,
            bullet.SortOrder,
         });
      });
   }

   private static void ConfigureCredentials(ModelBuilder modelBuilder)
   {
      modelBuilder.Entity<Credential>(entity =>
      {
         entity.ToTable("db_Credentials");

         entity.HasKey(credential => credential.Id);

         entity.Property(credential => credential.SortOrder)
            .HasDefaultValue(0)
            .IsRequired();

         entity.Property(credential => credential.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

         entity.Property(credential => credential.CreatedAtUtc)
            .HasColumnType("datetime2(0)")
            .HasDefaultValueSql("sysutcdatetime()")
            .IsRequired();

         entity.Property(credential => credential.UpdatedAtUtc)
            .HasColumnType("datetime2(0)")
            .HasDefaultValueSql("sysutcdatetime()")
            .IsRequired();

         entity.HasIndex(credential => credential.SortOrder);
      });
   }
}