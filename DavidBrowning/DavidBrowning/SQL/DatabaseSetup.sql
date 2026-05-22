USE master;
GO

-- Create the database if it does not exist.
IF DB_ID(N'DavidBrowning') IS NULL
BEGIN
   CREATE DATABASE DavidBrowning;
END;
GO

-- Use the SIMPLE recovery model.
--
-- This keeps transaction-log management simple because SQL Server can
-- automatically reuse log space after transactions are committed and
-- checkpointed. This is appropriate for a small development/personal
-- website database where we are not doing point-in-time restore from
-- transaction-log backups.
--
-- Tradeoff: with SIMPLE recovery, we can restore only to the latest full
-- or differential backup, not to an arbitrary point in time.
ALTER DATABASE DavidBrowning
SET RECOVERY SIMPLE;
GO

-- Enable Read Committed Snapshot Isolation (RCSI).
--
-- This changes the normal READ COMMITTED behavior so readers use row
-- versions instead of taking shared locks that can block writers. In
-- practical terms, normal SELECT queries are less likely to block INSERT,
-- UPDATE, or DELETE operations, and vice versa.
--
-- WITH ROLLBACK IMMEDIATE disconnects/rolls back existing sessions in
-- this database so SQL Server can apply the option immediately.
ALTER DATABASE DavidBrowning
SET READ_COMMITTED_SNAPSHOT ON
WITH ROLLBACK IMMEDIATE;
GO

-- Keep the database open after the last connection closes.
--
-- AUTO_CLOSE ON is mainly useful for lightweight desktop-style scenarios.
-- For a web application database, closing and reopening the database adds
-- avoidable overhead and can make first requests slower after idle periods.
ALTER DATABASE DavidBrowning
SET AUTO_CLOSE OFF;
GO

-- Do not let SQL Server automatically shrink database files.
--
-- AUTO_SHRINK can cause unnecessary file churn, fragmentation, and
-- unpredictable performance. If the database ever needs to be shrunk,
-- that should be an intentional maintenance action, not an automatic
-- background behavior.
ALTER DATABASE DavidBrowning
SET AUTO_SHRINK OFF;
GO

-- Create a dedicated SQL login for the website. A LOGIN account can simply log 
-- into the SQL server.
--
-- CHECK_POLICY = ON keeps password complexity enforcement enabled.
-- CHECK_EXPIRATION = OFF prevents the application password from expiring
-- automatically and breaking the website unexpectedly. Password rotation
-- should be handled intentionally through deployment/secret-management
-- process, not by surprise login expiration.
IF NOT EXISTS (
   SELECT 1
   FROM sys.sql_logins
   WHERE name = N'DavidBrowningApp')
BEGIN
   CREATE LOGIN DavidBrowningApp
   WITH PASSWORD = N'',
        CHECK_POLICY = ON,
        CHECK_EXPIRATION = OFF;
END;
GO

IF NOT EXISTS (
   SELECT 1
   FROM sys.sql_logins
   WHERE name = N'DavidBrowningAdmin')
BEGIN
   CREATE LOGIN DavidBrowningAdmin
   WITH PASSWORD = N'',
        CHECK_POLICY = ON,
        CHECK_EXPIRATION = OFF;
END;
GO

USE DavidBrowning;
GO

-- Create a user that can use the DavidBrowning database.
IF USER_ID(N'DavidBrowningApp') IS NULL
BEGIN
   CREATE USER DavidBrowningApp
   FOR LOGIN DavidBrowningApp;
END;
GO

ALTER ROLE db_datareader
ADD MEMBER DavidBrowningApp;
GO

ALTER ROLE db_datawriter
ADD MEMBER DavidBrowningApp;
GO

-- Create the admin account.
IF USER_ID(N'DavidBrowningAdmin') IS NULL
BEGIN
   CREATE USER DavidBrowningAdmin
   FOR LOGIN DavidBrowningAdmin;
END;
GO

ALTER ROLE db_owner
ADD MEMBER DavidBrowningAdmin;
GO