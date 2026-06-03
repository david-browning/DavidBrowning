-- Copyright © 2026 David Browning. All rights reserved.
-- Source-available for viewing only. No license granted.
CREATE TABLE dbo.db_Interests
(
   Id int IDENTITY(1,1) NOT NULL,

   Slug nvarchar(64) NOT NULL,

   DisplayName nvarchar(128) NOT NULL,

   Summary nvarchar(768) NOT NULL,

   IconCssClass nvarchar(128) NULL,

   SortOrder int NOT NULL
      CONSTRAINT DF_db_Interests_SortOrder DEFAULT (0),

   IsActive bit NOT NULL
      CONSTRAINT DF_db_Interests_IsActive DEFAULT (1),

   CreatedAtUtc datetime2(0) NOT NULL
      CONSTRAINT DF_db_Interests_CreatedAtUtc DEFAULT (sysutcdatetime()),

   UpdatedAtUtc datetime2(0) NOT NULL
      CONSTRAINT DF_db_Interests_UpdatedAtUtc DEFAULT (sysutcdatetime()),

   CONSTRAINT PK_db_Interests PRIMARY KEY (Id),

   CONSTRAINT UQ_db_Interests_Slug UNIQUE (Slug),
);

CREATE INDEX IX_db_Interests_IsActive_SortOrder
ON dbo.db_Interests(IsActive, SortOrder);