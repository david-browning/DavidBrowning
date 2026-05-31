-- Copyright © 2026 David Browning. All rights reserved.
-- Source-available for viewing only. No license granted.
CREATE TABLE dbo.db_SiteAssets
(
    Id int IDENTITY(1,1) NOT NULL,

    -- SiteAssetType enum:
    -- Binary = 0, Image = 1, Pdf = 2, Code = 3
    AssetType tinyint NOT NULL,
    AssetKey nvarchar(256) NOT NULL,
    ContentType nvarchar(128) NOT NULL,
    WidthPixels int NULL,
    HeightPixels int NULL,
    OriginalFileName nvarchar(256) NULL,

    BlobContainer nvarchar(256) NOT NULL,
    BlobName nvarchar(256) NOT NULL,

    AltText nvarchar(max) NULL,

    SizeBytes bigint NOT NULL,

    CreatedAtUtc datetime2(0) NOT NULL
        CONSTRAINT DF_db_SiteAssets_CreatedAtUtc DEFAULT (sysutcdatetime()),

    CONSTRAINT PK_db_SiteAssets PRIMARY KEY (Id),
    CONSTRAINT UQ_db_SiteAssets_BlobContainer_BlobName UNIQUE (BlobContainer, BlobName),
    CONSTRAINT UQ_db_SiteAssets_AssetKey UNIQUE (AssetKey),
    CONSTRAINT CK_db_SiteAssets_WidthPixels
      CHECK (WidthPixels IS NULL OR WidthPixels > 0),
    CONSTRAINT CK_db_SiteAssets_HeightPixels
      CHECK (HeightPixels IS NULL OR HeightPixels > 0),
);