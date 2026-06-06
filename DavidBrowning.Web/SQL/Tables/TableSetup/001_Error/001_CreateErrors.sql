-- Copyright © 2026 David Browning. All rights reserved.
-- Source-available for viewing only. No license granted.
CREATE TABLE dbo.db_ErrorLogEntries
(
    Id int IDENTITY(1,1) NOT NULL,

    OccurredAtUtc datetime2(0) NOT NULL
        CONSTRAINT DF_db_ErrorLogEntries_OccurredAtUtc DEFAULT (sysutcdatetime()),

    TraceIdentifier nvarchar(128) NULL,
    CorrelationId nvarchar(128) NULL,

    EnvironmentName nvarchar(64) NULL,
    ApplicationVersion nvarchar(128) NULL,
    MachineName nvarchar(128) NULL,

    HttpMethod nvarchar(16) NULL,
    Path nvarchar(2048) NULL,
    QueryString nvarchar(2048) NULL,
    EndpointName nvarchar(256) NULL,
    RouteValuesJson nvarchar(max) NULL,

    StatusCode int NULL,

    ExceptionType nvarchar(512) NOT NULL,
    ExceptionMessage nvarchar(max) NOT NULL,
    ExceptionSource nvarchar(512) NULL,
    StackTrace nvarchar(max) NULL,

    InnerExceptionType nvarchar(512) NULL,
    InnerExceptionMessage nvarchar(max) NULL,

    UserName nvarchar(256) NULL,
    UserAgent nvarchar(1024) NULL,
    Referrer nvarchar(2048) NULL,
    RemoteIpAddress nvarchar(128) NULL,

    IsHandled bit NOT NULL
        CONSTRAINT DF_db_ErrorLogEntries_IsHandled DEFAULT (0),

    Notes nvarchar(max) NULL,

    CONSTRAINT PK_db_ErrorLogEntries PRIMARY KEY (Id)
);

CREATE INDEX IX_db_ErrorLogEntries_OccurredAtUtc
ON dbo.db_ErrorLogEntries(OccurredAtUtc DESC);

CREATE INDEX IX_db_ErrorLogEntries_TraceIdentifier
ON dbo.db_ErrorLogEntries(TraceIdentifier);