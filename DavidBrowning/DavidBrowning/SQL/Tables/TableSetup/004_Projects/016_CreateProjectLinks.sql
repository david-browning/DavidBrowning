-- Copyright © 2026 David Browning. All rights reserved.
-- Source-available for viewing only. No license granted.
CREATE TABLE dbo.db_ProjectLinks
(
    Id int IDENTITY(1,1) NOT NULL,
    ProjectId int NOT NULL,
    ProjectLinkTypeId int NOT NULL,

    Label nvarchar(128) NOT NULL,
    Url nvarchar(2048) NOT NULL,

    SortOrder int NOT NULL
        CONSTRAINT DF_db_ProjectLinks_SortOrder DEFAULT (0),

    CONSTRAINT PK_db_ProjectLinks PRIMARY KEY (Id),

    CONSTRAINT FK_db_ProjectLinks_db_Projects_ProjectId
        FOREIGN KEY (ProjectId)
        REFERENCES dbo.db_Projects(Id)
        ON DELETE CASCADE,

    CONSTRAINT FK_db_ProjectLinks_db_ProjectLinkTypes_ProjectLinkTypeId
        FOREIGN KEY (ProjectLinkTypeId)
        REFERENCES dbo.db_ProjectLinkTypes(Id)
);

CREATE INDEX IX_db_ProjectLinks_ProjectId_SortOrder
ON dbo.db_ProjectLinks(ProjectId, SortOrder);

CREATE INDEX IX_db_ProjectLinks_ProjectLinkTypeId
ON dbo.db_ProjectLinks(ProjectLinkTypeId);