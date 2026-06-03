# Copyright © 2026 David Browning. All rights reserved.
# Source-available for viewing only. No license granted.

<#
.SYNOPSIS
Returns SQL setup files in execution order.

.DESCRIPTION
Scans a TableSetup directory containing numbered child folders, such as:

    001_Assets
    002_Writing
    003_Projects

Each child folder contains numbered SQL files, such as:

    001_CreateSiteAssets.sql
    002_CreatePostAssetLinks.sql

The script sorts folders by their numeric prefix, then sorts SQL files inside
each folder by their numeric prefix. It writes FileInfo objects to the pipeline
so another script can later execute them.

.EXAMPLE
.\Get-TableSetupScripts.ps1

.EXAMPLE
.\Get-TableSetupScripts.ps1 -TableSetupPath .\Tables\TableSetup

.EXAMPLE
.\Get-TableSetupScripts.ps1 | ForEach-Object { $_.FullName }
#>

[CmdletBinding()]
param
(
    [Parameter(Mandatory = $true)]
    [ValidateNotNullOrEmpty()]
    [string]$TableSetupPath = ".\Tables\TableSetup"
)

Set-StrictMode -Version Latest

function Get-NumberedNameParts
{
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory = $true)]
        [System.IO.FileSystemInfo]$Item
    )

    $match = [regex]::Match($Item.Name, '^(?<Number>\d+)_(?<Name>.+)$')

    if (-not $match.Success)
    {
        throw "Item '$($Item.FullName)' does not match the expected naming pattern: {NUMBER}_{Name}"
    }

    [PSCustomObject]@{
        Item = $Item
        Number = [int]$match.Groups["Number"].Value
        Name = $match.Groups["Name"].Value
    }
}

$resolvedTableSetupPath = Resolve-Path -Path $TableSetupPath -ErrorAction Stop

$setupDirectory = Get-Item -Path $resolvedTableSetupPath -ErrorAction Stop

if (-not $setupDirectory.PSIsContainer)
{
    throw "TableSetupPath '$TableSetupPath' is not a directory."
}

$numberedFolders = Get-ChildItem -Path $setupDirectory.FullName -Directory |
    ForEach-Object {
        Get-NumberedNameParts -Item $_
    } |
    Sort-Object -Property Number, Name

foreach ($numberedFolder in $numberedFolders)
{
    $sqlFiles = Get-ChildItem -Path $numberedFolder.Item.FullName -File -Filter "*.sql" |
        ForEach-Object {
            Get-NumberedNameParts -Item $_
        } |
        Sort-Object -Property Number, Name

    foreach ($sqlFile in $sqlFiles)
    {
        # Write the actual FileInfo object to the pipeline.
        $sqlFile.Item
    }
}