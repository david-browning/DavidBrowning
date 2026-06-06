-- Copyright © 2026 David Browning. All rights reserved.
-- Source-available for viewing only. No license granted.

SET XACT_ABORT ON;
GO

BEGIN TRANSACTION;

SET IDENTITY_INSERT dbo.db_PostStyles ON;

WITH SourceData AS
(
   SELECT
      1 AS Id,
      N'blog' AS Slug,
      N'Blog' AS DisplayName,
      N'A full-length writing post with a traditional article structure.'
         AS Description,
      10 AS SortOrder,
      CAST(1 AS bit) AS IsActive

   UNION ALL

   SELECT
      2 AS Id,
      N'quip' AS Slug,
      N'Quip' AS DisplayName,
      N'A short observation, aside, or compact idea.' AS Description,
      20 AS SortOrder,
      CAST(1 AS bit) AS IsActive

   UNION ALL

   SELECT
      3 AS Id,
      N'blurb' AS Slug,
      N'Blurb' AS DisplayName,
      N'A brief writing post that is more developed than a quip.'
         AS Description,
      30 AS SortOrder,
      CAST(1 AS bit) AS IsActive
)
MERGE dbo.db_PostStyles AS Target
USING SourceData AS Source
   ON Target.Id = Source.Id
WHEN MATCHED THEN
   UPDATE SET
      Target.Slug = Source.Slug,
      Target.DisplayName = Source.DisplayName,
      Target.Description = Source.Description,
      Target.SortOrder = Source.SortOrder,
      Target.IsActive = Source.IsActive
WHEN NOT MATCHED BY TARGET THEN
   INSERT
   (
      Id,
      Slug,
      DisplayName,
      Description,
      SortOrder,
      IsActive
   )
   VALUES
   (
      Source.Id,
      Source.Slug,
      Source.DisplayName,
      Source.Description,
      Source.SortOrder,
      Source.IsActive
   );

SET IDENTITY_INSERT dbo.db_PostStyles OFF;

COMMIT TRANSACTION;
GO