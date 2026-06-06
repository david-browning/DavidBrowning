-- Copyright © 2026 David Browning. All rights reserved.
-- Source-available for viewing only. No license granted.
CREATE TABLE dbo.db_ProjectStackTagLinks
(
    ProjectId int NOT NULL,
    ProjectStackTagId int NOT NULL,

    CONSTRAINT PK_db_ProjectStackTagLinks PRIMARY KEY (ProjectId, ProjectStackTagId),

    CONSTRAINT FK_db_ProjectStackTagLinks_db_Projects_ProjectId
        FOREIGN KEY (ProjectId)
        REFERENCES dbo.db_Projects(Id)
        ON DELETE CASCADE,

    CONSTRAINT FK_db_ProjectStackTagLinks_db_ProjectStackTags_ProjectStackTagId
        FOREIGN KEY (ProjectStackTagId)
        REFERENCES dbo.db_ProjectStackTags(Id)
        ON DELETE CASCADE
);

CREATE INDEX IX_db_ProjectStackTagLinks_ProjectStackTagId
ON dbo.db_ProjectStackTagLinks(ProjectStackTagId);