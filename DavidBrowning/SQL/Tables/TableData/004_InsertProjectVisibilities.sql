-- Copyright © 2026 David Browning. All rights reserved.
-- Source-available for viewing only. No license granted.

SET XACT_ABORT ON;
GO

BEGIN TRANSACTION;

SET IDENTITY_INSERT dbo.db_ProjectVisibilities ON;

WITH SourceData AS
(
    SELECT
        1 AS Id,
        N'public' AS Slug,
        N'Public' AS DisplayName,
        N'Visible on the public site.' AS Description,
        10 AS SortOrder,
        CAST(1 AS bit) AS IsActive
    UNION ALL
    SELECT
        2 AS Id,
        N'unlisted' AS Slug,
        N'Unlisted' AS DisplayName,
        N'Available by direct link, but not featured in standard lists.' AS Description,
        20 AS SortOrder,
        CAST(1 AS bit) AS IsActive
    UNION ALL
    SELECT
        3 AS Id,
        N'private' AS Slug,
        N'Private' AS DisplayName,
        N'Hidden from public presentation.' AS Description,
        30 AS SortOrder,
        CAST(1 AS bit) AS IsActive
)
MERGE dbo.db_ProjectVisibilities AS Target
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

SET IDENTITY_INSERT dbo.db_ProjectVisibilities OFF;

COMMIT TRANSACTION;
GO
