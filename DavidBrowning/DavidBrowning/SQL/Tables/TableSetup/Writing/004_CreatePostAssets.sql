CREATE TABLE dbo.db_PostAssets
(
    Id int IDENTITY(1,1) NOT NULL,

    -- PostAssetType enum:
    -- Binary = 0, Image = 1, Pdf = 2, Code = 3
    AssetType tinyint NOT NULL,

    OriginalFileName nvarchar(256) NULL,

    BlobContainer nvarchar(256) NOT NULL,
    BlobName nvarchar(256) NOT NULL,

    AltText nvarchar(max) NULL,

    SizeBytes bigint NOT NULL,

    CreatedAtUtc datetime2(0) NOT NULL
        CONSTRAINT DF_db_PostAssets_CreatedAtUtc DEFAULT (sysutcdatetime()),

    CONSTRAINT PK_db_PostAssets PRIMARY KEY (Id),
    CONSTRAINT UQ_db_PostAssets_BlobContainer_BlobName UNIQUE (BlobContainer, BlobName)
);