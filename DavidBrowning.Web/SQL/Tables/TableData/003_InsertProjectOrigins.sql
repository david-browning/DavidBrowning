-- Copyright © 2026 David Browning. All rights reserved.
-- Source-available for viewing only. No license granted.

SET XACT_ABORT ON;
GO

BEGIN TRANSACTION;

SET IDENTITY_INSERT dbo.db_ProjectOrigins ON;

WITH SourceData AS
(
    SELECT
        1 AS Id,
        N'personal' AS Slug,
        N'Personal' AS DisplayName,
        N'Personal project or independent study.' AS Description,
        10 AS SortOrder,
        CAST(1 AS bit) AS IsActive
    UNION ALL
    SELECT
        2 AS Id,
        N'professional' AS Slug,
        N'Professional' AS DisplayName,
        N'Professional work or case study.' AS Description,
        20 AS SortOrder,
        CAST(1 AS bit) AS IsActive
    UNION ALL
    SELECT
        3 AS Id,
        N'academic' AS Slug,
        N'Academic' AS DisplayName,
        N'Coursework or academic project.' AS Description,
        30 AS SortOrder,
        CAST(1 AS bit) AS IsActive
)
MERGE dbo.db_ProjectOrigins AS Target
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
    INSERT (Id, Slug, DisplayName, Description, SortOrder, IsActive)
    VALUES (Source.Id, Source.Slug, Source.DisplayName, Source.Description, Source.SortOrder, Source.IsActive);

SET IDENTITY_INSERT dbo.db_ProjectOrigins OFF;

COMMIT TRANSACTION;
GO
