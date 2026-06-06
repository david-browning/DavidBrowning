-- Copyright © 2026 David Browning. All rights reserved.
-- Source-available for viewing only. No license granted.

SET XACT_ABORT ON;
GO

BEGIN TRANSACTION;

SET IDENTITY_INSERT dbo.db_ProjectTypes ON;

WITH SourceData AS
(
    SELECT
        1 AS Id,
        N'website' AS Slug,
        N'Website' AS DisplayName,
        N'A web application or website project.' AS Description,
        10 AS SortOrder,
        CAST(1 AS bit) AS IsActive
    UNION ALL
    SELECT
        2 AS Id,
        N'tooling' AS Slug,
        N'Tooling' AS DisplayName,
        N'Developer tooling, automation, or workflow infrastructure.' AS Description,
        20 AS SortOrder,
        CAST(1 AS bit) AS IsActive
    UNION ALL
    SELECT
        3 AS Id,
        N'engine' AS Slug,
        N'Engine' AS DisplayName,
        N'Systems or engine-style software.' AS Description,
        30 AS SortOrder,
        CAST(1 AS bit) AS IsActive
)
MERGE dbo.db_ProjectTypes AS Target
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

SET IDENTITY_INSERT dbo.db_ProjectTypes OFF;

COMMIT TRANSACTION;
GO
