-- Copyright © 2026 David Browning. All rights reserved.
-- Source-available for viewing only. No license granted.
CREATE TABLE dbo.db_Projects
(
    Id int IDENTITY(1,1) NOT NULL,

    Slug nvarchar(64) NOT NULL,
    Name nvarchar(128) NOT NULL,
    Description nvarchar(512) NULL,

    ProjectStatusId int NOT NULL,
    ProjectTypeId int NOT NULL,
    ProjectOriginId int NOT NULL,
    ProjectVisibilityId int NOT NULL,

    Role nvarchar(256) NULL,
    ContributionSummary nvarchar(512) NULL,

    IsFeatured bit NOT NULL
        CONSTRAINT DF_db_Projects_IsFeatured DEFAULT (0),

    SortOrder int NOT NULL
        CONSTRAINT DF_db_Projects_SortOrder DEFAULT (0),

    StartDate date NULL,
    EndDate date NULL,
    DateDisplayText nvarchar(128) NULL,

    CreatedAtUtc datetime2(0) NOT NULL
        CONSTRAINT DF_db_Projects_CreatedAtUtc DEFAULT (sysutcdatetime()),

    UpdatedAtUtc datetime2(0) NOT NULL
        CONSTRAINT DF_db_Projects_UpdatedAtUtc DEFAULT (sysutcdatetime()),

    CONSTRAINT PK_db_Projects PRIMARY KEY (Id),

    CONSTRAINT UQ_db_Projects_Slug UNIQUE (Slug),

    CONSTRAINT FK_db_Projects_db_ProjectStatuses_ProjectStatusId
        FOREIGN KEY (ProjectStatusId)
        REFERENCES dbo.db_ProjectStatuses(Id),

    CONSTRAINT FK_db_Projects_db_ProjectTypes_ProjectTypeId
        FOREIGN KEY (ProjectTypeId)
        REFERENCES dbo.db_ProjectTypes(Id),

    CONSTRAINT FK_db_Projects_db_ProjectOrigins_ProjectOriginId
        FOREIGN KEY (ProjectOriginId)
        REFERENCES dbo.db_ProjectOrigins(Id),

    CONSTRAINT FK_db_Projects_db_ProjectVisibilities_ProjectVisibilityId
        FOREIGN KEY (ProjectVisibilityId)
        REFERENCES dbo.db_ProjectVisibilities(Id),

    CONSTRAINT CK_db_Projects_DateRange
        CHECK (EndDate IS NULL OR StartDate IS NULL OR EndDate >= StartDate)
);

CREATE INDEX IX_db_Projects_ProjectVisibilityId_SortOrder
ON dbo.db_Projects(ProjectVisibilityId, SortOrder);

CREATE INDEX IX_db_Projects_IsFeatured_SortOrder
ON dbo.db_Projects(IsFeatured, SortOrder);