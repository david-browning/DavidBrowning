# Copyright © 2026 David Browning. All rights reserved.
# Source-available for viewing only. No license granted.

<#
.SYNOPSIS
Drops tables represented by SQL setup scripts.

.DESCRIPTION
Accepts SQL setup files from the pipeline, parses each file for a CREATE TABLE
statement, and drops that table if it exists.

The input files are processed in reverse order by default. This is important
because create scripts usually create principal tables before dependent tables,
but drop operations should remove dependent tables before principal tables.

.EXAMPLE
.\Get-TableSetupScripts.ps1 |
   .\Invoke-SqlDropTableScript.ps1 -ConnectionString $connectionString

.EXAMPLE
Get-Item .\Tables\TableSetup\003_Projects\001_CreateProjects.sql |
   .\Invoke-SqlDropTableScript.ps1 -ConnectionString $connectionString
#>

[CmdletBinding()]
param
(
    [Parameter(Mandatory = $true, ValueFromPipeline = $true)]
    [ValidateNotNull()]
    [System.IO.FileInfo]$File,

    [Parameter(Mandatory = $true)]
    [ValidateNotNullOrEmpty()]
    [string]$ConnectionString,

    [Parameter()]
    [ValidateNotNullOrEmpty()]
    [string]$DefaultSchemaName = "dbo",

    [Parameter()]
    [int]$CommandTimeoutSeconds = 30,

    [Parameter()]
    [switch]$DoNotReverseInputOrder
)

begin {
    Set-StrictMode -Version Latest

    $files = New-Object System.Collections.Generic.List[System.IO.FileInfo]

    function ConvertTo-SqlIdentifierText {
        [CmdletBinding()]
        param
        (
            [Parameter(Mandatory = $true)]
            [string]$Identifier
        )

        $trimmedIdentifier = $Identifier.Trim()

        if ($trimmedIdentifier.StartsWith("[") -and
            $trimmedIdentifier.EndsWith("]")) {
            $innerIdentifier = $trimmedIdentifier.Substring(
                1,
                $trimmedIdentifier.Length - 2)

            return $innerIdentifier.Replace("]]", "]")
        }

        return $trimmedIdentifier
    }

    function ConvertTo-DelimitedSqlIdentifier {
        [CmdletBinding()]
        param
        (
            [Parameter(Mandatory = $true)]
            [string]$Identifier
        )

        $escapedIdentifier = $Identifier.Replace("]", "]]")

        return "[$escapedIdentifier]"
    }

    function Get-CreateTableNameFromScript {
        [CmdletBinding()]
        param
        (
            [Parameter(Mandatory = $true)]
            [string]$ScriptText,

            [Parameter(Mandatory = $true)]
            [string]$DefaultSchemaName
        )

        $pattern = "(?is)\bCREATE\s+TABLE\s+" +
        "(?:(?<Schema>\[[^\]]+(?:\]\])*\]|[A-Za-z_][\w]*)\s*\.\s*)?" +
        "(?<Table>\[[^\]]+(?:\]\])*\]|[A-Za-z_][\w]*)"

        $match = [regex]::Match($ScriptText, $pattern)
        if (-not $match.Success) {
            return $null
        }

        $schemaName = $DefaultSchemaName

        if ($match.Groups["Schema"].Success) {
            $schemaName = ConvertTo-SqlIdentifierText `
                -Identifier $match.Groups["Schema"].Value
        }

        $tableName = ConvertTo-SqlIdentifierText `
            -Identifier $match.Groups["Table"].Value

        [PSCustomObject]@{
            SchemaName = $schemaName
            TableName  = $tableName
        }
    }

    function Invoke-SqlNonQuery {
        [CmdletBinding()]
        param
        (
            [Parameter(Mandatory = $true)]
            [System.Data.SqlClient.SqlConnection]$Connection,

            [Parameter(Mandatory = $true)]
            [string]$CommandText,

            [Parameter(Mandatory = $true)]
            [int]$TimeoutSeconds
        )

        $command = $Connection.CreateCommand()
        $command.CommandText = $CommandText
        $command.CommandTimeout = $TimeoutSeconds

        try {
            [void]$command.ExecuteNonQuery()
        }
        finally {
            $command.Dispose()
        }
    }

    $connection = [System.Data.SqlClient.SqlConnection]::new(
        $ConnectionString)

    $connection.Open()
}

process {
    if (-not $File.Exists) {
        throw "SQL setup script does not exist: $($File.FullName)"
    }

    $files.Add($File)
}

end {
    try {
        $filesToProcess = $files.ToArray()

        if (-not $DoNotReverseInputOrder) {
            [array]::Reverse($filesToProcess)
        }

        foreach ($scriptFile in $filesToProcess) {
            Write-Verbose "Reading SQL setup script: $($scriptFile.FullName)"

            $scriptText = Get-Content `
                -Path $scriptFile.FullName `
                -Raw `
                -ErrorAction Stop

            $tableNameParts = Get-CreateTableNameFromScript `
                -ScriptText $scriptText `
                -DefaultSchemaName $DefaultSchemaName

            if ($null -eq $tableNameParts) {
                Write-Warning (
                    "No CREATE TABLE statement was found in '{0}'. Skipping." -f
                    $scriptFile.FullName)

                continue
            }

            $delimitedSchemaName = ConvertTo-DelimitedSqlIdentifier `
                -Identifier $tableNameParts.SchemaName

            $delimitedTableName = ConvertTo-DelimitedSqlIdentifier `
                -Identifier $tableNameParts.TableName

            $fullTableName = "$delimitedSchemaName.$delimitedTableName"

            $dropSql = "DROP TABLE IF EXISTS $fullTableName;"

            try {
                Write-Verbose "Dropping table if it exists: $fullTableName"

                Invoke-SqlNonQuery `
                    -Connection $connection `
                    -CommandText $dropSql `
                    -TimeoutSeconds $CommandTimeoutSeconds

                [PSCustomObject]@{
                    File       = $scriptFile.FullName
                    SchemaName = $tableNameParts.SchemaName
                    TableName  = $tableNameParts.TableName
                    Status     = "DroppedIfExists"
                }
            }
            catch {
                Write-Error (
                    "Failed to drop table parsed from '{0}' as '{1}': {2}" -f
                    $scriptFile.FullName,
                    $fullTableName,
                    $_.Exception.Message)

                throw
            }
        }
    }
    finally {
        if ($null -ne $connection) {
            $connection.Dispose()
        }
    }
}