-- Copyright © 2026 David Browning. All rights reserved.
-- Source-available for viewing only. No license granted.

CREATE TABLE dbo.db_ExperienceRoleBullets
(
   Id int IDENTITY(1,1) NOT NULL,

   ExperienceRoleId int NOT NULL,

   Text nvarchar(512) NOT NULL,

   SortOrder int NOT NULL
      CONSTRAINT DF_db_ExperienceRoleBullets_SortOrder DEFAULT (0),

   IsActive bit NOT NULL
      CONSTRAINT DF_db_ExperienceRoleBullets_IsActive DEFAULT (1),

   CONSTRAINT PK_db_ExperienceRoleBullets
      PRIMARY KEY (Id),

   CONSTRAINT FK_db_ExperienceRoleBullets_db_ExperienceRoles_ExperienceRoleId
      FOREIGN KEY (ExperienceRoleId)
      REFERENCES dbo.db_ExperienceRoles(Id)
      ON DELETE CASCADE
);

CREATE INDEX IX_db_ExperienceRoleBullets_ExperienceRoleId_SortOrder
ON dbo.db_ExperienceRoleBullets(ExperienceRoleId, SortOrder);