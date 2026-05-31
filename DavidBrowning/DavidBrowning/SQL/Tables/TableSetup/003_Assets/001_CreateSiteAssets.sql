-- Copyright © 2026 David Browning. All rights reserved.
-- Source-available for viewing only. No license granted.

CREATE TABLE dbo.db_SiteAssets
(
   Id int IDENTITY(1,1) NOT NULL,
   AssetKey nvarchar(256) NOT NULL,
   ContentType nvarchar(128) NOT NULL,
   OriginalFileName nvarchar(256) NULL,
   AltText nvarchar(512) NULL,
   SizeBytes bigint NOT NULL,
   WidthPixels int NULL,
   HeightPixels int NULL,
   CreatedAtUtc datetime2(0) NOT NULL
      CONSTRAINT DF_db_SiteAssets_CreatedAtUtc
      DEFAULT (sysutcdatetime()),

   CONSTRAINT PK_db_SiteAssets
      PRIMARY KEY (Id),

   CONSTRAINT UQ_db_SiteAssets_AssetKey
      UNIQUE (AssetKey),

   CONSTRAINT CK_db_SiteAssets_SizeBytes
      CHECK (SizeBytes >= 0),

   CONSTRAINT CK_db_SiteAssets_WidthPixels
      CHECK (WidthPixels IS NULL OR WidthPixels > 0),

   CONSTRAINT CK_db_SiteAssets_HeightPixels
      CHECK (HeightPixels IS NULL OR HeightPixels > 0)
);