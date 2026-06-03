-- Copyright © 2026 David Browning. All rights reserved.
-- Source-available for viewing only. No license granted.
CREATE TABLE dbo.db_ProjectAssetLinks
(
    ProjectId int NOT NULL,
    SiteAssetId int NOT NULL,
    ProjectAssetRoleId int NOT NULL,
    ReferenceKey nvarchar(64) NULL,
    Caption nvarchar(512) NULL,
    AltTextOverride nvarchar(512) NULL,

    SortOrder int NOT NULL
        CONSTRAINT DF_db_ProjectAssetLinks_SortOrder DEFAULT (0),

    CONSTRAINT PK_db_ProjectAssetLinks
        PRIMARY KEY (ProjectId, SiteAssetId, ProjectAssetRoleId),

    CONSTRAINT FK_db_ProjectAssetLinks_db_Projects_ProjectId
        FOREIGN KEY (ProjectId)
        REFERENCES dbo.db_Projects(Id)
        ON DELETE CASCADE,

    CONSTRAINT FK_db_ProjectAssetLinks_db_SiteAssets_SiteAssetId
        FOREIGN KEY (SiteAssetId)
        REFERENCES dbo.db_SiteAssets(Id)
        ON DELETE CASCADE,

    CONSTRAINT FK_db_ProjectAssetLinks_db_ProjectAssetRoles_ProjectAssetRoleId
        FOREIGN KEY (ProjectAssetRoleId)
        REFERENCES dbo.db_ProjectAssetRoles(Id)
);

CREATE INDEX IX_db_ProjectAssetLinks_SiteAssetId
ON dbo.db_ProjectAssetLinks(SiteAssetId);

CREATE INDEX IX_db_ProjectAssetLinks_ProjectId_Role_SortOrder
ON dbo.db_ProjectAssetLinks(ProjectId, ProjectAssetRoleId, SortOrder);

CREATE UNIQUE INDEX UX_db_ProjectAssetLinks_ProjectId_ReferenceKey
   ON dbo.db_ProjectAssetLinks (ProjectId, ReferenceKey)
   WHERE ReferenceKey IS NOT NULL;