# Copyright © 2026 David Browning. All rights reserved.
# Source-available for viewing only. No license granted.

<#
.SYNOPSIS
Executes one or more SQL setup scripts.

.DESCRIPTION
Accepts SQL script files from the pipeline and executes each file against the
database specified by the connection string.

The script splits SQL files into batches on lines containing only GO, then
executes each batch in order.

By default, common "already exists" SQL Server errors are treated as warnings
so setup scripts can be rerun during development without failing just because
a table, constraint, or index already exists.

.EXAMPLE
.\Get-TableSetupScripts.ps1 |
   .\Invoke-SqlSetupScript.ps1 -ConnectionString $connectionString

.EXAMPLE
Get-Item .\Tables\TableSetup\003_Projects\001_CreateProjects.sql |
   .\Invoke-SqlSetupScript.ps1 -ConnectionString $connectionString

.EXAMPLE
.\Get-TableSetupScripts.ps1 |
   .\Invoke-SqlSetupScript.ps1 `
      -ConnectionString $connectionString `
      -ErrorAction Stop `
      -Verbose
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
    [int]$CommandTimeoutSeconds = 30,

    [Parameter()]
    [switch]$DoNotIgnoreAlreadyExistsErrors
)

begin {
    Set-StrictMode -Version Latest

    function Split-SqlScriptIntoBatches {
        [CmdletBinding()]
        param
        (
            [Parameter(Mandatory = $true)]
            [string]$ScriptText
        )

        $batches = New-Object System.Collections.Generic.List[string]
        $currentBatch = New-Object System.Text.StringBuilder

        $lines = $ScriptText -split "`r?`n"
        foreach ($line in $lines) {
            if ($line -match '^\s*GO\s*(?:--.*)?$') {
                $batchText = $currentBatch.ToString().Trim()

                if (-not [string]::IsNullOrWhiteSpace($batchText)) {
                    $batches.Add($batchText)
                }

                [void]$currentBatch.Clear()
                continue
            }

            [void]$currentBatch.AppendLine($line)
        }

        $finalBatchText = $currentBatch.ToString().Trim()

        if (-not [string]::IsNullOrWhiteSpace($finalBatchText)) {
            $batches.Add($finalBatchText)
        }

        $batches
    }

    function Test-IsIgnorableSqlError {
        [CmdletBinding()]
        param
        (
            [Parameter(Mandatory = $true)]
            [System.Exception]$Exception
        )

        if ($DoNotIgnoreAlreadyExistsErrors) {
            return $false
        }

        $sqlException = $Exception

        while ($null -ne $sqlException -and
            $sqlException.GetType().Name -ne "SqlException") {
            $sqlException = $sqlException.InnerException
        }

        if ($null -eq $sqlException) {
            return $false
        }

        foreach ($errorRecord in $sqlException.Errors) {
            switch ($errorRecord.Number) {
                2714 {
                    # Object already exists.
                    return $true
                }

                1913 {
                    # Index or statistics already exists.
                    return $true
                }

                15023 {
                    # User, group, or role already exists in the database.
                    return $true
                }

                1801 {
                    # Database already exists.
                    return $true
                }
            }
        }

        return $false
    }

    function Invoke-SqlBatch {
        [CmdletBinding()]
        param
        (
            [Parameter(Mandatory = $true)]
            [System.Data.SqlClient.SqlConnection]$Connection,

            [Parameter(Mandatory = $true)]
            [string]$BatchText,

            [Parameter(Mandatory = $true)]
            [int]$TimeoutSeconds
        )

        $command = $Connection.CreateCommand()
        $command.CommandText = $BatchText
        $command.CommandTimeout = $TimeoutSeconds

        try {
            [void]$command.ExecuteNonQuery()
        }
        finally {
            $command.Dispose()
        }
    }

    $connection = [System.Data.SqlClient.SqlConnection]::new($ConnectionString)

    $connection.Open()
}

process {
    if (-not $File.Exists) {
        throw "SQL setup script does not exist: $($File.FullName)"
    }

    Write-Verbose "Executing SQL setup script: $($File.FullName)"

    $scriptText = Get-Content `
        -Path $File.FullName `
        -Raw `
        -ErrorAction Stop

    $batches = Split-SqlScriptIntoBatches -ScriptText $scriptText
    $batchNumber = 0

    foreach ($batch in $batches) {
        $batchNumber++

        try {
            Invoke-SqlBatch `
                -Connection $connection `
                -BatchText $batch `
                -TimeoutSeconds $CommandTimeoutSeconds
        }
        catch {
            if (Test-IsIgnorableSqlError -Exception $_.Exception) {
                Write-Warning (
                    "Ignored SQL setup error in file '{0}', batch {1}: {2}" -f
                    $File.Name,
                    $batchNumber,
                    $_.Exception.Message)

                continue
            }

            throw
        }
    }

    [PSCustomObject]@{
        File       = $File.FullName
        BatchCount = @($batches).Count
        Status     = "Executed"
    }
}

end {
    if ($null -ne $connection) {
        $connection.Dispose()
    }
}