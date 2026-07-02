# Azure App Service Outbound IP Allowlisting

This document captures the process for allowing Azure App Service outbound IP addresses through the website project's Azure Key Vault and Azure SQL firewall rules.

The website currently uses public Azure PaaS endpoints with firewall allowlists. App Service calls Key Vault during startup to load secrets, then connects to Azure SQL and Azure Blob Storage using configuration loaded from Key Vault. If Key Vault or SQL firewall rules do not allow the App Service outbound IPs, the app can fail during startup or fail when opening database connections.

## Resources

Update these values before running the scripts.

```powershell
$subscriptionName = "<SUBSCRIPTION NAME>"
$resourceGroupName = "<RESOURCE GROUP NAME>"

$keyVaultName = "<KEYVAULT NAME>"
$sqlServerName = "<SERVER NAME>" # Server name only, not .database.windows.net

$appNames = @(
    "<WEB APP NAME>",
    "<ADMIN APP NAME>"
)
```

## Prerequisites

Install or update the Az PowerShell modules if needed:

```powershell
Install-Module Az -Scope CurrentUser -Repository PSGallery -Force
```

Sign in and select the correct subscription:

```powershell
Connect-AzAccount
Set-AzContext -Subscription $subscriptionName
```

## Why this is needed

App Service outbound traffic does not come from a single obvious IP address. Each app has a set of outbound IP addresses, and Azure also exposes possible outbound IP addresses that may be used after some platform or tier changes.

When Key Vault networking is set to selected networks only, the App Service managed identity still needs a network path to Key Vault. If the Key Vault firewall blocks the app's outbound IP, Key Vault returns a firewall error even if the managed identity has the correct RBAC role.

When Azure SQL public access is restricted by firewall rules, the SQL logical server must allow the App Service outbound IPs before the app can connect.

## Important disclaimer

This script is a practical beta/deployment helper, not the final networking architecture.

App Service outbound IPs can change when apps are recreated, moved, or when some App Service plan/tier/networking changes are made. This script should be rerun after App Service plan changes, app recreation, region changes, or any other infrastructure move that could affect outbound addresses.

This approach also creates one SQL firewall rule per IP address. Azure SQL has firewall-rule limits, so this is acceptable for a small number of App Services but is not a great pattern for large environments.

Long-term alternatives include App Service VNet integration with service endpoints, private endpoints, or NAT Gateway for more controlled outbound networking. Those options are more deliberate infrastructure choices and may have additional cost.

## Get App Service outbound IPs

This script pulls both current outbound IPs and possible outbound IPs from the configured App Services.

```powershell
$subscriptionName = "<SUBSCRIPTION NAME>"
$resourceGroupName = "<RESOURCE GROUP NAME>"

$appNames = @(
    "<WEB APP NAME>",
    "<ADMIN APP NAME>"
)

Connect-AzAccount
Set-AzContext -Subscription $subscriptionName

$ips = foreach ($appName in $appNames) {
    $app = Get-AzWebApp `
        -ResourceGroupName $resourceGroupName `
        -Name $appName

    $app.OutboundIpAddresses -split ","
    $app.PossibleOutboundIpAddresses -split ","
}

$ips = $ips `
    | ForEach-Object { $_.Trim() } `
    | Where-Object { $_ } `
    | Sort-Object -Unique

$ips
```

## Add App Service IPs to Key Vault firewall

Key Vault accepts CIDR ranges. For single IPv4 addresses, normalize them to `/32`.

```powershell
$keyVaultName = "<KEYVAULT NAME>"

$keyVaultIpRules = $ips | ForEach-Object {
    if ($_ -match "/") { $_ } else { "$_/32" }
}

Add-AzKeyVaultNetworkRule `
    -VaultName $keyVaultName `
    -ResourceGroupName $resourceGroupName `
    -IpAddressRange $keyVaultIpRules `
    -PassThru
```

Key Vault does not accept private IP ranges such as `10.0.0.0/8` as IP network rules. Use public outbound IPs for this allowlist pattern.

## Add App Service IPs to Azure SQL firewall

Azure SQL firewall rules use a start IP and end IP. For a single IP, use the same value for both.

```powershell
$sqlServerName = "<SERVER NAME>"

$index = 1

foreach ($ip in $ips) {
    $cleanIp = $ip -replace "/32$", ""
    $ruleName = "appservice-outbound-{0:D3}" -f $index

    $existingRule = Get-AzSqlServerFirewallRule `
        -ResourceGroupName $resourceGroupName `
        -ServerName $sqlServerName `
        -FirewallRuleName $ruleName `
        -ErrorAction SilentlyContinue

    if ($existingRule) {
        Set-AzSqlServerFirewallRule `
            -ResourceGroupName $resourceGroupName `
            -ServerName $sqlServerName `
            -FirewallRuleName $ruleName `
            -StartIpAddress $cleanIp `
            -EndIpAddress $cleanIp
    }
    else {
        New-AzSqlServerFirewallRule `
            -ResourceGroupName $resourceGroupName `
            -ServerName $sqlServerName `
            -FirewallRuleName $ruleName `
            -StartIpAddress $cleanIp `
            -EndIpAddress $cleanIp
    }

    $index++
}
```

## Combined script

This is the normal script to rerun after App Service outbound IPs change.

```powershell
$subscriptionName = "<SUBSCRIPTION NAME>"
$resourceGroupName = "<RESOURCE GROUP NAME>"

$keyVaultName = "<KEYVAULT NAME>"
$sqlServerName = "<SERVER NAME>"

$appNames = @(
    "<WEB APP NAME>",
    "<ADMIN APP NAME>"
)

Connect-AzAccount
Set-AzContext -Subscription $subscriptionName

Write-Host "Collecting App Service outbound IP addresses..."

$ips = foreach ($appName in $appNames) {
    $app = Get-AzWebApp `
        -ResourceGroupName $resourceGroupName `
        -Name $appName

    $app.OutboundIpAddresses -split ","
    $app.PossibleOutboundIpAddresses -split ","
}

$ips = $ips `
    | ForEach-Object { $_.Trim() } `
    | Where-Object { $_ } `
    | Sort-Object -Unique

Write-Host "Found $($ips.Count) unique outbound IP addresses:"
$ips | ForEach-Object { Write-Host "  $_" }

Write-Host "Adding Key Vault firewall rules..."

$keyVaultIpRules = $ips | ForEach-Object {
    if ($_ -match "/") { $_ } else { "$_/32" }
}

Add-AzKeyVaultNetworkRule `
    -VaultName $keyVaultName `
    -ResourceGroupName $resourceGroupName `
    -IpAddressRange $keyVaultIpRules `
    -PassThru

Write-Host "Adding Azure SQL firewall rules..."

$index = 1

foreach ($ip in $ips) {
    $cleanIp = $ip -replace "/32$", ""
    $ruleName = "appservice-outbound-{0:D3}" -f $index

    $existingRule = Get-AzSqlServerFirewallRule `
        -ResourceGroupName $resourceGroupName `
        -ServerName $sqlServerName `
        -FirewallRuleName $ruleName `
        -ErrorAction SilentlyContinue

    if ($existingRule) {
        Write-Host "Updating SQL firewall rule $ruleName -> $cleanIp"

        Set-AzSqlServerFirewallRule `
            -ResourceGroupName $resourceGroupName `
            -ServerName $sqlServerName `
            -FirewallRuleName $ruleName `
            -StartIpAddress $cleanIp `
            -EndIpAddress $cleanIp
    }
    else {
        Write-Host "Creating SQL firewall rule $ruleName -> $cleanIp"

        New-AzSqlServerFirewallRule `
            -ResourceGroupName $resourceGroupName `
            -ServerName $sqlServerName `
            -FirewallRuleName $ruleName `
            -StartIpAddress $cleanIp `
            -EndIpAddress $cleanIp
    }

    $index++
}

Write-Host "Done."
```

## Using a manual comma-separated IP list

If the IPs were copied manually from the Azure Portal, use this instead of `Get-AzWebApp`.

```powershell
$csv = @"
4.236.57.180,1.2.3.4,5.6.7.8
"@

$ips = $csv `
    -split "," `
    | ForEach-Object { $_.Trim() } `
    | Where-Object { $_ } `
    | ForEach-Object { ($_ -replace "/32$", "") } `
    | Sort-Object -Unique
```

Then run the Key Vault and SQL sections normally.

## Verifying Key Vault access

If the app fails during startup with a Key Vault error like this:

```text
ForbiddenByFirewall
Client address is not authorized
```

then managed identity/RBAC may be working, but the Key Vault firewall is blocking the app's outbound IP.

Check:

```text
Key Vault
-> Networking
-> Firewalls and virtual networks
```

Make sure the App Service outbound IPs are present.

Also check:

```text
Key Vault
-> Access control (IAM)
```

The Web App and Admin App managed identities should have:

```text
Key Vault Secrets User
```

## Verifying SQL access

If the app can load Key Vault but fails when connecting to SQL, check:

```text
SQL logical server
-> Networking
-> Firewall rules
```

Make sure the App Service outbound IPs are present.

The public Web App should use a lower-privilege connection string, usually:

```text
ConnectionStrings:AzureSiteDatabase
```

The Admin App should use the admin connection string:

```text
ConnectionStrings:AzureAdminSiteDatabase
```

## Notes

The Web and Admin apps should both have system-assigned managed identity enabled. The managed identity authorizes the app to read Key Vault secrets. The firewall rules authorize the app's network path to reach Key Vault and Azure SQL.

## Cleaning up App Service firewall rules

Use this section to remove the temporary App Service outbound IP firewall rules from Key Vault and Azure SQL.

This is useful when the App Service plan changes, the apps are recreated, the outbound IP list changes, or the beta deployment is being torn down.

### Important warning

Only remove rules that were created for App Service outbound IPs.

Do not remove personal/home IP firewall rules unless that is intentional. If Key Vault or SQL is locked down to selected networks only, removing the wrong rules can lock the developer machine or the App Service out of the resource.

The scripts below assume the App Service-generated SQL firewall rules use this naming pattern:

```powershell
appservice-outbound-001
appservice-outbound-002
appservice-outbound-003
```

If the rule names were changed, adjust the filter before running the cleanup.

## Cleanup variables

```powershell
$subscriptionName = "<SUBSCRIPTION NAME>"
$resourceGroupName = "<RESOURCE GROUP NAME>"

$keyVaultName = "<KEYVAULT NAME>"
$sqlServerName = "<SERVER NAME>" # server name only, not .database.windows.net

$appNames = @(
    "<WEB APP NAME>",
    "<ADMIN APP NAME>"
)
```

## Sign in and select subscription

```powershell
Connect-AzAccount
Set-AzContext -Subscription $subscriptionName
```

## Get the current App Service outbound IPs

This pulls the current and possible outbound IPs for the configured App Services.

```powershell
$ips = foreach ($appName in $appNames) {
    $app = Get-AzWebApp `
        -ResourceGroupName $resourceGroupName `
        -Name $appName

    $app.OutboundIpAddresses -split ","
    $app.PossibleOutboundIpAddresses -split ","
}

$ips = $ips `
    | ForEach-Object { $_.Trim() } `
    | Where-Object { $_ } `
    | Sort-Object -Unique

$ips
```

## Remove App Service IPs from Key Vault firewall

Key Vault network rules use CIDR notation. For single IPs, use `/32`.

```powershell
$keyVaultIpRules = $ips | ForEach-Object {
    if ($_ -match "/") { $_ } else { "$_/32" }
}

foreach ($ipRule in $keyVaultIpRules) {
    Write-Host "Removing Key Vault network rule: $ipRule"

    Remove-AzKeyVaultNetworkRule `
        -VaultName $keyVaultName `
        -ResourceGroupName $resourceGroupName `
        -IpAddressRange $ipRule `
        -PassThru
}
```

## Remove App Service-created SQL firewall rules by name

This removes SQL firewall rules that match the `appservice-outbound-*` naming pattern.

```powershell
$sqlRules = Get-AzSqlServerFirewallRule `
    -ResourceGroupName $resourceGroupName `
    -ServerName $sqlServerName

$appServiceRules = $sqlRules | Where-Object {
    $_.FirewallRuleName -like "appservice-outbound-*"
}

$appServiceRules | Select-Object FirewallRuleName, StartIpAddress, EndIpAddress

foreach ($rule in $appServiceRules) {
    Write-Host "Removing SQL firewall rule: $($rule.FirewallRuleName) [$($rule.StartIpAddress) - $($rule.EndIpAddress)]"

    Remove-AzSqlServerFirewallRule `
        -ResourceGroupName $resourceGroupName `
        -ServerName $sqlServerName `
        -FirewallRuleName $rule.FirewallRuleName
}
```

## Safer SQL cleanup with confirmation

Use this version if you want to review each rule before deleting it.

```powershell
$sqlRules = Get-AzSqlServerFirewallRule `
    -ResourceGroupName $resourceGroupName `
    -ServerName $sqlServerName

$appServiceRules = $sqlRules | Where-Object {
    $_.FirewallRuleName -like "appservice-outbound-*"
}

foreach ($rule in $appServiceRules) {
    Write-Host ""
    Write-Host "Rule:  $($rule.FirewallRuleName)"
    Write-Host "Start: $($rule.StartIpAddress)"
    Write-Host "End:   $($rule.EndIpAddress)"

    $answer = Read-Host "Remove this SQL firewall rule? Type y to remove"

    if ($answer -eq "y") {
        Remove-AzSqlServerFirewallRule `
            -ResourceGroupName $resourceGroupName `
            -ServerName $sqlServerName `
            -FirewallRuleName $rule.FirewallRuleName
    }
}
```

## Manual cleanup from a comma-separated IP list

Use this if the IPs were copied manually from the Azure Portal.

```powershell
$csv = @"
4.236.57.180,1.2.3.4,5.6.7.8
"@

$ips = $csv `
    -split "," `
    | ForEach-Object { $_.Trim() } `
    | Where-Object { $_ } `
    | ForEach-Object { ($_ -replace "/32$", "") } `
    | Sort-Object -Unique
```

Then remove those IPs from Key Vault:

```powershell
$keyVaultIpRules = $ips | ForEach-Object {
    if ($_ -match "/") { $_ } else { "$_/32" }
}

foreach ($ipRule in $keyVaultIpRules) {
    Write-Host "Removing Key Vault network rule: $ipRule"

    Remove-AzKeyVaultNetworkRule `
        -VaultName $keyVaultName `
        -ResourceGroupName $resourceGroupName `
        -IpAddressRange $ipRule `
        -PassThru
}
```

For SQL, removal is by firewall rule name, not IP address. If the rules were created with the `appservice-outbound-*` naming convention, use the SQL cleanup script above.

If the SQL rules were not named consistently, list the rules first:

```powershell
Get-AzSqlServerFirewallRule `
    -ResourceGroupName $resourceGroupName `
    -ServerName $sqlServerName |
    Select-Object FirewallRuleName, StartIpAddress, EndIpAddress
```

Then remove the specific rule by name:

```powershell
Remove-AzSqlServerFirewallRule `
    -ResourceGroupName $resourceGroupName `
    -ServerName $sqlServerName `
    -FirewallRuleName "appservice-outbound-001"
```

## Combined cleanup script

This removes the current App Service outbound IPs from Key Vault and removes SQL firewall rules that match `appservice-outbound-*`.

```powershell
$subscriptionName = "sub-davidbrowning-personal"
$resourceGroupName = "rg-personalwebsite-prod-wus3"

$keyVaultName = "<KEYVAULT NAME>"
$sqlServerName = "<SERVER NAME>"

$appNames = @(
    "app-davidbrowning-web-beta",
    "app-davidbrowning-admin-beta"
)

Connect-AzAccount
Set-AzContext -Subscription $subscriptionName

Write-Host "Collecting App Service outbound IP addresses..."

$ips = foreach ($appName in $appNames) {
    $app = Get-AzWebApp `
        -ResourceGroupName $resourceGroupName `
        -Name $appName

    $app.OutboundIpAddresses -split ","
    $app.PossibleOutboundIpAddresses -split ","
}

$ips = $ips `
    | ForEach-Object { $_.Trim() } `
    | Where-Object { $_ } `
    | Sort-Object -Unique

Write-Host "Found $($ips.Count) unique outbound IP addresses:"
$ips | ForEach-Object { Write-Host "  $_" }

Write-Host ""
Write-Host "Removing Key Vault firewall rules for App Service outbound IPs..."

$keyVaultIpRules = $ips | ForEach-Object {
    if ($_ -match "/") { $_ } else { "$_/32" }
}

foreach ($ipRule in $keyVaultIpRules) {
    Write-Host "Removing Key Vault network rule: $ipRule"

    Remove-AzKeyVaultNetworkRule `
        -VaultName $keyVaultName `
        -ResourceGroupName $resourceGroupName `
        -IpAddressRange $ipRule `
        -PassThru
}

Write-Host ""
Write-Host "Removing Azure SQL firewall rules named appservice-outbound-*..."

$sqlRules = Get-AzSqlServerFirewallRule `
    -ResourceGroupName $resourceGroupName `
    -ServerName $sqlServerName

$appServiceRules = $sqlRules | Where-Object {
    $_.FirewallRuleName -like "appservice-outbound-*"
}

foreach ($rule in $appServiceRules) {
    Write-Host "Removing SQL firewall rule: $($rule.FirewallRuleName) [$($rule.StartIpAddress) - $($rule.EndIpAddress)]"

    Remove-AzSqlServerFirewallRule `
        -ResourceGroupName $resourceGroupName `
        -ServerName $sqlServerName `
        -FirewallRuleName $rule.FirewallRuleName
}

Write-Host "Done."
```

## Verify cleanup

List remaining Key Vault network rules:

```powershell
(Get-AzKeyVault `
    -VaultName $keyVaultName `
    -ResourceGroupName $resourceGroupName).NetworkAcls
```

List remaining SQL firewall rules:

```powershell
Get-AzSqlServerFirewallRule `
    -ResourceGroupName $resourceGroupName `
    -ServerName $sqlServerName |
    Select-Object FirewallRuleName, StartIpAddress, EndIpAddress
```

After cleanup, confirm that any required developer/home IP rules still exist.
