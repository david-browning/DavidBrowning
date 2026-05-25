-- Copyright © 2026 David Browning. All rights reserved.
-- Source-available for viewing only. No license granted.

SET XACT_ABORT ON;
GO

BEGIN TRANSACTION;

SET IDENTITY_INSERT dbo.db_ProjectAssetRoles ON;

WITH SourceData AS
(
    SELECT
        1 AS Id,
        N'hero-image' AS Slug,
        N'Hero Image' AS DisplayName,
        N'Primary image used to represent the project.' AS Description,
        10 AS SortOrder,
        CAST(1 AS bit) AS IsActive
    UNION ALL
    SELECT
        2 AS Id,
        N'screenshot' AS Slug,
        N'Screenshot' AS DisplayName,
        N'Screenshot showing the project interface or behavior.' AS Description,
        20 AS SortOrder,
        CAST(1 AS bit) AS IsActive
    UNION ALL
    SELECT
        3 AS Id,
        N'diagram' AS Slug,
        N'Diagram' AS DisplayName,
        N'Diagram explaining architecture, flow, or structure.' AS Description,
        30 AS SortOrder,
        CAST(1 AS bit) AS IsActive
)
MERGE dbo.db_ProjectAssetRoles AS Target
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

SET IDENTITY_INSERT dbo.db_ProjectAssetRoles OFF;

COMMIT TRANSACTION;
GO
