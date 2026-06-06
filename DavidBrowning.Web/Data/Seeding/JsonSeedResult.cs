// Copyright © 2026 David Browning. All rights reserved.
// Source-available for viewing only. No license granted.
using System;
using System.Collections.Generic;
using System.Linq;

namespace DavidBrowning.Web.Data.Seeding;

public sealed class JsonSeedResult
{
   public List<JsonSeedFileResult> Files { get; } = [];

   public int InsertedFileCount => Files.Count(
      file => file.Status == JsonSeedFileStatus.Inserted);

   public int SkippedFileCount => Files.Count(
      file => file.Status == JsonSeedFileStatus.Skipped);

   public int InsertedEntityCount => Files.Sum(file => file.EntityCount);
}

public sealed record JsonSeedFileResult(
    string FilePath,
    string EntityTypeName,
    JsonSeedFileStatus Status,
    int EntityCount,
    int SavedRowCount,
    string? Message)
{
   public static JsonSeedFileResult Inserted(
       string filePath,
       Type entityType,
       int entityCount,
       int savedRowCount)
   {
      return new JsonSeedFileResult(
          filePath,
          entityType.Name,
          JsonSeedFileStatus.Inserted,
          entityCount,
          savedRowCount,
          Message: null);
   }

   public static JsonSeedFileResult Skipped(
       string filePath,
       Type entityType,
       string message)
   {
      return new JsonSeedFileResult(
          filePath,
          entityType.Name,
          JsonSeedFileStatus.Skipped,
          EntityCount: 0,
          SavedRowCount: 0,
          message);
   }
}

public enum JsonSeedFileStatus
{
   Inserted,
   Skipped
}