CREATE TABLE dbo.db_PostAssetLinks
(
    PostId int NOT NULL,
    PostAssetId int NOT NULL,

    -- PostAssetRole enum:
    -- Attachment = 0, HeroImage = 1, InlineImage = 2,
    -- SocialImage = 3, GeneratedPdf = 4, Download = 5
    Role tinyint NOT NULL,

    SortOrder int NOT NULL
        CONSTRAINT DF_db_PostAssetLinks_SortOrder DEFAULT (0),

    CONSTRAINT PK_db_PostAssetLinks PRIMARY KEY (PostId, PostAssetId, Role),

    CONSTRAINT FK_db_PostAssetLinks_db_Posts_PostId
        FOREIGN KEY (PostId)
        REFERENCES dbo.db_Posts(Id)
        ON DELETE CASCADE,

    CONSTRAINT FK_db_PostAssetLinks_db_PostAssets_PostAssetId
        FOREIGN KEY (PostAssetId)
        REFERENCES dbo.db_PostAssets(Id)
        ON DELETE CASCADE
);

CREATE INDEX IX_db_PostAssetLinks_PostAssetId
ON dbo.db_PostAssetLinks(PostAssetId);

CREATE INDEX IX_db_PostAssetLinks_PostId_Role_SortOrder
ON dbo.db_PostAssetLinks(PostId, Role, SortOrder);