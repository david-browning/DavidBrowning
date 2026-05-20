-- Copyright © 2026 David Browning. All rights reserved.
-- Source-available for viewing only. No license granted.
CREATE TABLE dbo.db_ProjectTagLinks
(
    ProjectId int NOT NULL,
    ProjectTagId int NOT NULL,

    CONSTRAINT PK_db_ProjectTagLinks PRIMARY KEY (ProjectId, ProjectTagId),

    CONSTRAINT FK_db_ProjectTagLinks_db_Projects_ProjectId
        FOREIGN KEY (ProjectId)
        REFERENCES dbo.db_Projects(Id)
        ON DELETE CASCADE,

    CONSTRAINT FK_db_ProjectTagLinks_db_ProjectTags_ProjectTagId
        FOREIGN KEY (ProjectTagId)
        REFERENCES dbo.db_ProjectTags(Id)
        ON DELETE CASCADE
);

CREATE INDEX IX_db_ProjectTagLinks_ProjectTagId
ON dbo.db_ProjectTagLinks(ProjectTagId);