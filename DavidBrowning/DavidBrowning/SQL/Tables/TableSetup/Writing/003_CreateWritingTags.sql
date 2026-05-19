CREATE TABLE dbo.db_WritingTags
(
    Id int IDENTITY(1,1) NOT NULL,
    Name nvarchar(128) NOT NULL,
    Slug nvarchar(64) NOT NULL,

    CONSTRAINT PK_db_WritingTags PRIMARY KEY (Id),
    CONSTRAINT UQ_db_WritingTags_Slug UNIQUE (Slug)
);

CREATE TABLE dbo.db_PostTags
(
    PostId int NOT NULL,
    WritingTagId int NOT NULL,

    CONSTRAINT PK_db_PostTags PRIMARY KEY (PostId, WritingTagId),

    CONSTRAINT FK_db_PostTags_db_Posts_PostId
        FOREIGN KEY (PostId)
        REFERENCES dbo.db_Posts(Id)
        ON DELETE CASCADE,

    CONSTRAINT FK_db_PostTags_db_WritingTags_WritingTagId
        FOREIGN KEY (WritingTagId)
        REFERENCES dbo.db_WritingTags(Id)
        ON DELETE CASCADE
);

CREATE INDEX IX_db_PostTags_WritingTagId
ON dbo.db_PostTags(WritingTagId);