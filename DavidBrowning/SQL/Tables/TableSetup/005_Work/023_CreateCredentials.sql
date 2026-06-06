-- Copyright © 2026 David Browning. All rights reserved.
-- Source-available for viewing only. No license granted.

CREATE TABLE dbo.db_Credentials
(
   Id int IDENTITY(1,1) NOT NULL,

   IssuingOrganization nvarchar(256) NOT NULL,

   Name nvarchar(256) NOT NULL,

   Type nvarchar(128) NULL,

   AwardedMonth int NULL,

   AwardedYear int NULL,

   DateDisplayText nvarchar(128) NULL,

   Description nvarchar(512) NULL,

   CredentialUrl nvarchar(2048) NULL,

   SortOrder int NOT NULL
      CONSTRAINT DF_db_Credentials_SortOrder DEFAULT (0),

   IsActive bit NOT NULL
      CONSTRAINT DF_db_Credentials_IsActive DEFAULT (1),

   CreatedAtUtc datetime2(0) NOT NULL
      CONSTRAINT DF_db_Credentials_CreatedAtUtc
      DEFAULT (sysutcdatetime()),

   UpdatedAtUtc datetime2(0) NOT NULL
      CONSTRAINT DF_db_Credentials_UpdatedAtUtc
      DEFAULT (sysutcdatetime()),

   CONSTRAINT PK_db_Credentials
      PRIMARY KEY (Id),

   CONSTRAINT CK_db_Credentials_AwardedMonth
      CHECK (AwardedMonth IS NULL OR AwardedMonth BETWEEN 1 AND 12)
);

CREATE INDEX IX_db_Credentials_SortOrder
ON dbo.db_Credentials(SortOrder);