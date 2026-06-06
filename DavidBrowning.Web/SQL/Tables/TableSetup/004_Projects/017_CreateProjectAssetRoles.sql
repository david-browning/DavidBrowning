-- Copyright © 2026 David Browning. All rights reserved.
-- Source-available for viewing only. No license granted.
CREATE TABLE dbo.db_ProjectAssetRoles
(
    Id int IDENTITY(1,1) NOT NULL,
    Slug nvarchar(64) NOT NULL,
    DisplayName nvarchar(128) NOT NULL,
    Description nvarchar(512) NULL,
    SortOrder int NOT NULL
        CONSTRAINT DF_db_ProjectAssetRoles_SortOrder DEFAULT (0),
    IsActive bit NOT NULL
        CONSTRAINT DF_db_ProjectAssetRoles_IsActive DEFAULT (1),

    CONSTRAINT PK_db_ProjectAssetRoles PRIMARY KEY (Id),
    CONSTRAINT UQ_db_ProjectAssetRoles_Slug UNIQUE (Slug)
);