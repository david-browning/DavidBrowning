// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
namespace DavidBrowning.Web.Data.Seeding;

public sealed class JsonSeedDatabaseSeederOptions
{
   /// <summary>
   /// Physical SQL table prefix to ignore when matching JSON file names.
   /// Example: db_Projects maps to Projects.json when TablePrefix is "db_".
   /// </summary>
   public string TablePrefix { get; init; } = "db_";

   /// <summary>
   /// If true, a seed file is skipped when its target table already 
   /// contains at least one row.
   /// This keeps startup seeding idempotent enough for development.
   /// </summary>
   public bool SkipFileWhenTargetTableHasRows { get; init; } = true;

   /// <summary>
   /// If true, unmatched JSON files cause startup failure. This is 
   /// intentionally strict by default because seed data is part of the 
   /// application contract, not user input.
   /// </summary>
   public bool ThrowOnUnmatchedJsonFiles { get; init; } = true;

   /// <summary>
   /// If true, SQL Server identity insert is enabled temporarily for tables 
   /// whose seed rows contain explicit integer identity keys.
   /// </summary>
   public bool UseSqlServerIdentityInsertWhenNeeded { get; init; } = true;
}