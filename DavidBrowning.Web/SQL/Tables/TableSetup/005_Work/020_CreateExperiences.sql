-- Copyright © 2026 David Browning. All rights reserved.
-- Source-available for viewing only. No license granted.

CREATE TABLE dbo.db_Experiences
(
   Id int IDENTITY(1,1) NOT NULL,

   CompanyName nvarchar(256) NOT NULL,

   LocationDisplayText nvarchar(128) NULL,

   SortOrder int NOT NULL
      CONSTRAINT DF_db_Experiences_SortOrder DEFAULT (0),

   IsActive bit NOT NULL
      CONSTRAINT DF_db_Experiences_IsActive DEFAULT (1),

   CreatedAtUtc datetime2(0) NOT NULL
      CONSTRAINT DF_db_Experiences_CreatedAtUtc
      DEFAULT (sysutcdatetime()),

   UpdatedAtUtc datetime2(0) NOT NULL
      CONSTRAINT DF_db_Experiences_UpdatedAtUtc
      DEFAULT (sysutcdatetime()),

   CONSTRAINT PK_db_Experiences
      PRIMARY KEY (Id)
);

CREATE INDEX IX_db_Experiences_SortOrder
ON dbo.db_Experiences(SortOrder);