-- Copyright © 2026 David Browning. All rights reserved.
-- Source-available for viewing only. No license granted.
CREATE TABLE dbo.db_PostRevisions
(
    Id int IDENTITY(1,1) NOT NULL,
    PostId int NOT NULL,
    RevisionNumber int NOT NULL,

    -- PostContentFormat enum:
    -- PlainText = 0, Markdown = 1, Html = 2, Latex = 3, ExternalBlob = 4
    ContentFormat tinyint NOT NULL,

    CreatedBy nvarchar(256) NOT NULL,

    CreatedAtUtc datetime2(0) NOT NULL
        CONSTRAINT DF_db_PostRevisions_CreatedAtUtc DEFAULT (sysutcdatetime()),

    Content nvarchar(max) NULL,

    CONSTRAINT PK_db_PostRevisions PRIMARY KEY (Id),

    CONSTRAINT FK_db_PostRevisions_db_Posts_PostId
        FOREIGN KEY (PostId)
        REFERENCES dbo.db_Posts(Id)
        ON DELETE CASCADE,

    CONSTRAINT UQ_db_PostRevisions_PostId_RevisionNumber
        UNIQUE (PostId, RevisionNumber)
);

ALTER TABLE dbo.db_Posts
ADD CONSTRAINT FK_db_Posts_db_PostRevisions_CurrentRevisionId
    FOREIGN KEY (CurrentRevisionId)
    REFERENCES dbo.db_PostRevisions(Id)
    ON DELETE NO ACTION;