-- Copyright © 2026 David Browning. All rights reserved.
-- Source-available for viewing only. No license granted.

SET XACT_ABORT ON;
GO

BEGIN TRANSACTION;

SET IDENTITY_INSERT dbo.db_ProjectStatuses ON;

WITH SourceData AS
(
    SELECT
        1 AS Id,
        N'active' AS Slug,
        N'Active' AS DisplayName,
        N'Currently maintained or in active development.' AS Description,
        10 AS SortOrder,
        CAST(1 AS bit) AS IsActive
    UNION ALL
    SELECT
        2 AS Id,
        N'complete' AS Slug,
        N'Complete' AS DisplayName,
        N'Complete enough to present as finished work.' AS Description,
        20 AS SortOrder,
        CAST(1 AS bit) AS IsActive
    UNION ALL
    SELECT
        3 AS Id,
        N'archived' AS Slug,
        N'Archived' AS DisplayName,
        N'Historical work preserved for reference.' AS Description,
        30 AS SortOrder,
        CAST(1 AS bit) AS IsActive
)
MERGE dbo.db_ProjectStatuses AS Target
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

SET IDENTITY_INSERT dbo.db_ProjectStatuses OFF;

COMMIT TRANSACTION;
GO
