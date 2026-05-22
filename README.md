## Devloper Setup
### Adding Secrets
```
dotnet user-secrets set "ConnectionStrings:SiteDatabase" "Server=...,1433;Database=DavidBrowning;User Id=DavidBrowningApp;Password=...;"
```

### Add tables to database
```
PS> $connectionString = ""
PS DavidBrowning\SQL\Tables> .\Get-TableSetupScripts.ps1 -TableSetupPath "TableSetup" | .\Invoke-SqlSetupScript.ps1 -ConnectionString $connectionString -Verbose
```

### Database Seeding

This package contains a reflection-heavy EF Core seed loader plus starter seed JSON files.

#### How file matching works

The seeder scans the seed root folder for `*.json` files and matches each file to an EF Core entity type.

A file may match:

- The `DbSet<TEntity>` property name, e.g. `Projects.json`
- The physical EF table name, e.g. `db_Projects.json`
- The physical EF table name after removing the configured prefix, e.g. `Projects.json`
- The CLR model type name, e.g. `Project.json`

The default table prefix is:

```csharp
"db_"
```

but it is configurable through `JsonSeedDatabaseSeederOptions.TablePrefix`.

#### Insert order

The seeder inspects EF Core foreign keys and inserts principal entities before dependent entities.

That means lookup tables such as statuses/types/origins should be inserted before `Projects`, and `Projects` should be inserted before join/link tables.

#### Identity insert

If SQL Server is the provider and a seed file contains explicit integer identity keys, the seeder can temporarily run:

```sql
SET IDENTITY_INSERT [table] ON;
```

for that one table while inserting that file.

This is enabled by default through:

```csharp
UseSqlServerIdentityInsertWhenNeeded = true
```

#### Serialization rules

Seed files should use C# property names exactly. Example:

```json
{
  "ProjectStatusId": 1
}
```

not:

```json
{
  "projectStatusId": 1
}
```

The default serializer options are intentionally strict:

```csharp
PropertyNameCaseInsensitive = false
```

Enums are represented as strings, and `DateOnly` values use `yyyy-MM-dd`.

#### Important limitation

This is development/bootstrap seeding, not a replacement for schema migrations or a production data migration tool. It is intentionally magic for convenience, but it still depends on the EF model being accurate.




## Copyright

Copyright © 2026 David Browning. All rights reserved.

This repository is source-available for portfolio review and educational reading
only. It is not open source. No permission is granted to copy, modify,
redistribute, deploy, rehost, or reuse this website, its source code, design,
content, or assets without explicit written permission.

Most files have a copyright header specified. If a file does not, then this copyright notice applies to that file.
