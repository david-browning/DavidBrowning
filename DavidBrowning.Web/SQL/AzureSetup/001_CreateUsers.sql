/*
    Run this script while connected directly to the final free-offer website database,
    not [master].

    Replace the placeholder passwords before running.
    Avoid semicolons in the password while you are debugging connection strings.
*/

IF USER_ID(N'DavidBrowningApp') IS NOT NULL
BEGIN
    DROP USER [DavidBrowningApp];
END;
GO

IF USER_ID(N'DavidBrowningAdmin') IS NOT NULL
BEGIN
    DROP USER [DavidBrowningAdmin];
END;
GO

CREATE USER [DavidBrowningApp]
WITH PASSWORD = N'<NEW-STRONG-PASSWORD-FOR-DavidBrowningApp>';
GO

CREATE USER [DavidBrowningAdmin]
WITH PASSWORD = N'<NEW-STRONG-PASSWORD-FOR-DavidBrowningAdmin>';
GO

ALTER ROLE [db_datareader]
ADD MEMBER [DavidBrowningApp];
GO

ALTER ROLE [db_datawriter]
ADD MEMBER [DavidBrowningApp];
GO

ALTER ROLE [db_owner]
ADD MEMBER [DavidBrowningAdmin];
GO