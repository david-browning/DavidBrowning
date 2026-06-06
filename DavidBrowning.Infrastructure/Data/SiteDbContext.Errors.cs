// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using DavidBrowning.Models.Error;
using Microsoft.EntityFrameworkCore;

namespace DavidBrowning.Data;

public sealed partial class SiteDbContext
{
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
}