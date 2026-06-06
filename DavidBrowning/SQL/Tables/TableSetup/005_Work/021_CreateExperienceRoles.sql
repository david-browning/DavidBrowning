-- Copyright © 2026 David Browning. All rights reserved.
-- Source-available for viewing only. No license granted.

CREATE TABLE dbo.db_ExperienceRoles
(
   Id int IDENTITY(1,1) NOT NULL,

   ExperienceId int NOT NULL,

   Title nvarchar(256) NOT NULL,

   DateDisplayText nvarchar(128) NULL,

   Description nvarchar(512) NULL,

   SortOrder int NOT NULL
      CONSTRAINT DF_db_ExperienceRoles_SortOrder DEFAULT (0),

   IsActive bit NOT NULL
      CONSTRAINT DF_db_ExperienceRoles_IsActive DEFAULT (1),

   CreatedAtUtc datetime2(0) NOT NULL
      CONSTRAINT DF_db_ExperienceRoles_CreatedAtUtc
      DEFAULT (sysutcdatetime()),

   UpdatedAtUtc datetime2(0) NOT NULL
      CONSTRAINT DF_db_ExperienceRoles_UpdatedAtUtc
      DEFAULT (sysutcdatetime()),

   CONSTRAINT PK_db_ExperienceRoles
      PRIMARY KEY (Id),

   CONSTRAINT FK_db_ExperienceRoles_db_Experiences_ExperienceId
      FOREIGN KEY (ExperienceId)
      REFERENCES dbo.db_Experiences(Id)
      ON DELETE CASCADE
);

CREATE INDEX IX_db_ExperienceRoles_ExperienceId_SortOrder
ON dbo.db_ExperienceRoles(ExperienceId, SortOrder);