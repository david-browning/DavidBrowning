CREATE TABLE dbo.db_Posts
(
    Id int IDENTITY(1,1) NOT NULL,
    Slug nvarchar(64) NOT NULL,
    Title nvarchar(128) NOT NULL,
    Subtitle nvarchar(128) NULL,
    Summary nvarchar(512) NULL,
    MetaDescription nvarchar(512) NULL,

    -- PostStatus enum:
    -- Draft = 0, Published = 1, Archived = 2, Unlisted = 3
    Status tinyint NOT NULL
        CONSTRAINT DF_db_Posts_Status DEFAULT (0),

    IsFeatured bit NOT NULL
        CONSTRAINT DF_db_Posts_IsFeatured DEFAULT (0),

    CreatedDateUtc datetime2(0) NOT NULL
        CONSTRAINT DF_db_Posts_CreatedDateUtc DEFAULT (sysutcdatetime()),

    LastUpdatedDateUtc datetime2(0) NOT NULL
        CONSTRAINT DF_db_Posts_LastUpdatedDateUtc DEFAULT (sysutcdatetime()),

    PublishedDateUtc datetime2(0) NULL,

    CurrentRevisionId int NULL,

    CONSTRAINT PK_db_Posts PRIMARY KEY (Id),
    CONSTRAINT UQ_db_Posts_Slug UNIQUE (Slug)
);