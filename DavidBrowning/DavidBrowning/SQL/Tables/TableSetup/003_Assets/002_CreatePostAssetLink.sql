-- Copyright © 2026 David Browning. All rights reserved.
-- Source-available for viewing only. No license granted.
CREATE TABLE dbo.db_PostAssetLinks
(
    PostId int NOT NULL,
    SiteAssetId int NOT NULL,

    -- PostAssetRole enum:
    -- Attachment = 0, HeroImage = 1, InlineImage = 2,
    -- SocialImage = 3, GeneratedPdf = 4, Download = 5
    Role tinyint NOT NULL,

    ReferenceKey nvarchar(64) NULL,
    Caption nvarchar(512) NULL,
    AltTextOverride nvarchar(512) NULL,

    SortOrder int NOT NULL
        CONSTRAINT DF_db_PostAssetLinks_SortOrder DEFAULT (0),

    CONSTRAINT PK_db_PostAssetLinks PRIMARY KEY (PostId, SiteAssetId, Role),

    CONSTRAINT FK_db_PostAssetLinks_db_Posts_PostId
        FOREIGN KEY (PostId)
        REFERENCES dbo.db_Posts(Id)
        ON DELETE CASCADE,

    CONSTRAINT FK_db_PostAssetLinks_db_SiteAssets_SiteAssetId
        FOREIGN KEY (SiteAssetId)
        REFERENCES dbo.db_SiteAssets(Id)
        ON DELETE CASCADE
);

CREATE INDEX IX_db_PostAssetLinks_SiteAssetId
ON dbo.db_PostAssetLinks(SiteAssetId);

CREATE INDEX IX_db_PostAssetLinks_PostId_Role_SortOrder
ON dbo.db_PostAssetLinks(PostId, Role, SortOrder);

CREATE UNIQUE INDEX UX_db_PostAssetLinks_PostId_ReferenceKey
   ON dbo.db_PostAssetLinks (PostId, ReferenceKey)
   WHERE ReferenceKey IS NOT NULL;