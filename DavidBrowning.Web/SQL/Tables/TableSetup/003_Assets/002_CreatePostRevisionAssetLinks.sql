-- Copyright © 2026 David Browning. All rights reserved.
-- Source-available for viewing only. No license granted.
CREATE TABLE dbo.db_PostRevisionAssetLinks
(
   PostRevisionId int NOT NULL,
   SiteAssetId int NOT NULL,
   ReferenceKey nvarchar(64) NOT NULL,
   Caption nvarchar(512) NULL,
   AltTextOverride nvarchar(512) NULL,

   CONSTRAINT PK_db_PostRevisionAssetLinks
      PRIMARY KEY (PostRevisionId, SiteAssetId),

   CONSTRAINT FK_db_PostRevisionAssetLinks_db_PostRevisions
      FOREIGN KEY (PostRevisionId)
      REFERENCES dbo.db_PostRevisions (Id)
      ON DELETE CASCADE,

   CONSTRAINT FK_db_PostRevisionAssetLinks_db_SiteAssets
      FOREIGN KEY (SiteAssetId)
      REFERENCES dbo.db_SiteAssets (Id)
      ON DELETE CASCADE
);

CREATE UNIQUE INDEX UX_db_PostRevisionAssetLinks_RevisionId_ReferenceKey
   ON dbo.db_PostRevisionAssetLinks (PostRevisionId, ReferenceKey);

CREATE INDEX IX_db_PostRevisionAssetLinks_SiteAssetId
   ON dbo.db_PostRevisionAssetLinks (SiteAssetId);