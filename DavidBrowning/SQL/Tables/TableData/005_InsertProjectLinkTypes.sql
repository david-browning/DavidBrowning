-- Copyright © 2026 David Browning. All rights reserved.
-- Source-available for viewing only. No license granted.

SET XACT_ABORT ON;
GO

BEGIN TRANSACTION;

SET IDENTITY_INSERT dbo.db_ProjectLinkTypes ON;

WITH SourceData AS
(
    SELECT
        1 AS Id,
        N'repository' AS Slug,
        N'Repository' AS DisplayName,
        N'Source repository or code browser.' AS Description,
        NULL AS IconCssClass,
        10 AS SortOrder,
        CAST(1 AS bit) AS IsActive
    UNION ALL
    SELECT
        2 AS Id,
        N'live-site' AS Slug,
        N'Live Site' AS DisplayName,
        N'Publicly hosted application or website.' AS Description,
        NULL AS IconCssClass,
        20 AS SortOrder,
        CAST(1 AS bit) AS IsActive
    UNION ALL
    SELECT
        3 AS Id,
        N'documentation' AS Slug,
        N'Documentation' AS DisplayName,
        N'Documentation, notes, or reference material.' AS Description,
        NULL AS IconCssClass,
        30 AS SortOrder,
        CAST(1 AS bit) AS IsActive
)
MERGE dbo.db_ProjectLinkTypes AS Target
USING SourceData AS Source
    ON Target.Id = Source.Id
WHEN MATCHED THEN
    UPDATE SET
    Target.Slug = Source.Slug,
    Target.DisplayName = Source.DisplayName,
    Target.Description = Source.Description,
    Target.IconCssClass = Source.IconCssClass,
    Target.SortOrder = Source.SortOrder,
    Target.IsActive = Source.IsActive
WHEN NOT MATCHED BY TARGET THEN
    INSERT (Id, Slug, DisplayName, Description, IconCssClass, SortOrder, IsActive)
    VALUES (Source.Id, Source.Slug, Source.DisplayName, Source.Description, Source.IconCssClass, Source.SortOrder, Source.IsActive);

SET IDENTITY_INSERT dbo.db_ProjectLinkTypes OFF;

COMMIT TRANSACTION;
GO
