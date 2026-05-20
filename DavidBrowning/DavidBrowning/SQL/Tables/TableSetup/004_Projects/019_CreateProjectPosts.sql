-- Copyright © 2026 David Browning. All rights reserved.
-- Source-available for viewing only. No license granted.
CREATE TABLE dbo.db_ProjectPosts
(
    ProjectId int NOT NULL,
    PostId int NOT NULL,

    RelationshipLabel nvarchar(128) NULL,

    SortOrder int NOT NULL
        CONSTRAINT DF_db_ProjectPosts_SortOrder DEFAULT (0),

    CONSTRAINT PK_db_ProjectPosts PRIMARY KEY (ProjectId, PostId),

    CONSTRAINT FK_db_ProjectPosts_db_Projects_ProjectId
        FOREIGN KEY (ProjectId)
        REFERENCES dbo.db_Projects(Id)
        ON DELETE CASCADE,

    CONSTRAINT FK_db_ProjectPosts_db_Posts_PostId
        FOREIGN KEY (PostId)
        REFERENCES dbo.db_Posts(Id)
        ON DELETE CASCADE
);

CREATE INDEX IX_db_ProjectPosts_PostId
ON dbo.db_ProjectPosts(PostId);

CREATE INDEX IX_db_ProjectPosts_ProjectId_SortOrder
ON dbo.db_ProjectPosts(ProjectId, SortOrder);