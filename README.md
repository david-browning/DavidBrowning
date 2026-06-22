# DavidBrowning

Personal website and authoring/admin system for David Browning.

This repository contains the public website, the local admin dashboard, the shared domain model, and the infrastructure code needed to render, cache, store, and edit site content.

The project is source-available for portfolio review and educational reading only. It is not open source. See the copyright section at the end of this file.

## Project summary

This is an ASP.NET Core MVC personal website with a separate admin site.

The public site is designed to be read-oriented, fast, cache-friendly, and cheap to host. It presents writing, projects, work history, interests, assets, and site metadata.

The admin site is the local authoring tool. It is used to edit database-backed content, upload content assets, manage ordered lists, and prepare content that will eventually be deployed to production.

The solution is split into four projects:

```text
DavidBrowning.Admin
    Local admin dashboard for editing site content.

DavidBrowning.Web
    Public website.

DavidBrowning.Core
    Shared models, constants, options, interfaces, and helpers.

DavidBrowning.Infrastructure
    EF Core, stores, content pipeline, caching, rendering, middleware, and shared service registration.
```

The main architectural idea is separation between content, storage, rendering, and presentation.

Database records describe site entities such as posts, projects, interests, credentials, assets, and relationships between them. Content files such as Markdown, JSON, images, documents, and other assets are accessed through a content store. The content pipeline loads assets, renders text content, emits image/document links where appropriate, and caches expensive results.

## Design goals

The project is guided by a few practical goals:

```text
Fast
    The public site should render quickly and avoid unnecessary repeated work.

Flexible
    Content should be editable without hardcoding everything into views.

Familiar
    The site should be ordinary ASP.NET Core MVC, ordinary SQL Server, ordinary Bootstrap, and ordinary HTML.

Friendly
    The public site should be readable, accessible, and easy to maintain after long breaks.
```

More concretely:

* The public site should be cheap to run.
* The admin site should make editing content safer and less repetitive.
* The content pipeline should allow local files now and cloud-backed storage later.
* SQL Server should be the source of truth for structured content.
* Content assets should use provider-neutral keys such as `images/profile.jpg`, not machine-specific paths.
* The project should be recoverable from source control plus a database/content backup.

## Repository layout

```text
DavidBrowning.sln

DavidBrowning.Core/
    Shared models, interfaces, constants, helpers, and options.

DavidBrowning.Infrastructure/
    EF Core DbContext, SQL stores, content stores, renderers, cache helpers,
    middleware, and dependency injection setup.

DavidBrowning.Web/
    Public MVC website.
    ContentAssets/
        Local content root for public-site content.
    Data/Seeding/
        JSON seed data used for development/bootstrap loading.
    SQL/
        SQL database setup scripts.
    Views/
    ViewModels/
    Controllers/
    wwwroot/

DavidBrowning.Admin/
    Local MVC admin dashboard.
    ContentAssets/
        Local content root for admin support files, such as picker catalogs.
    Controllers/
    ViewModels/
    Views/
    wwwroot/
```

## Required tools

Install these on a new development machine:

```text
Required
    Git
    Visual Studio 2022
    .NET 8 SDK
    PowerShell 7 or newer
    SQL Server client tooling
        SQL Server Management Studio, Azure Data Studio, or sqlcmd
    Docker
        Docker Desktop on Windows, or Docker Engine/Compose on a Linux server

Recommended
    Azure Storage Explorer
    Azure Data Studio SQL Server extension
```

This project currently targets .NET 8.

Check the installed SDK:

```powershell
dotnet --list-sdks
```

The output should include an `8.x` SDK.

## Local and LAN services

The two development services to remember are:

```text
SQL Server
    Required for realistic development and admin editing.

Azurite
    Optional right now if the content store is Local.
    Useful for future Azure Blob Storage development and deployment parity.
```

Both services can run locally on the development PC, or they can run on a LAN server. In the current LAN-server setup, SQL Server runs in Docker under `/opt/mssql-dev`, and Azurite can run in Docker under `/opt/azurite`.

The development configuration can still use the in-memory database for the public web project, but serious development should use SQL Server so schema, relationships, queries, and seed behavior are real.

## SQL Server local or LAN setup

You can run SQL Server either directly on a machine or through Docker. The recommended setup is Docker on a LAN server or development machine.

### Option A: Use the existing LAN server script

A helper script exists in the `ServerScripts` repository:

```text
SQLDockerSetup.sh
```

That script creates `/opt/mssql-dev`, writes a Docker Compose file, configures the SQL Server container, and prints the commands needed to start SQL Server and create the development database.

On the Linux server:

```bash
chmod +x SQLDockerSetup.sh
./SQLDockerSetup.sh
```

Then start SQL Server:

```bash
cd /opt/mssql-dev
docker compose up -d
```

Create the development database after the container is running:

```bash
cd /opt/mssql-dev
source .env

docker exec -it sql2025-dev /opt/mssql-tools18/bin/sqlcmd \
   -S localhost \
   -U sa \
   -P "$SA_PASSWORD" \
   -C \
   -Q "CREATE DATABASE WebsiteDev;"
```

Important: do not expose SQL Server directly to the public internet. Keep port `1433` LAN/VPN-only.

### Option B: Create the SQL Server Compose file manually

Create a folder for local infrastructure.

On Windows:

```powershell
mkdir C:\DevServices\mssql-dev
cd C:\DevServices\mssql-dev
```

On a Linux LAN server:

```bash
sudo mkdir -p /opt/mssql-dev
sudo chown "$USER":"$USER" /opt/mssql-dev
cd /opt/mssql-dev
```

Create `.env`:

```text
SA_PASSWORD=replace-with-a-long-strong-password
DATABASE_NAME=WebsiteDev
```

Create `docker-compose.yml`:

```yaml
services:
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2025-latest
    container_name: sql2025-dev
    environment:
      ACCEPT_EULA: "Y"
      MSSQL_PID: "Developer"
      MSSQL_SA_PASSWORD: "${SA_PASSWORD}"
    ports:
      - "1433:1433"
    volumes:
      - sql2025-dev-data:/var/opt/mssql
    restart: unless-stopped

volumes:
  sql2025-dev-data:
```

Start SQL Server:

```powershell
docker compose up -d
```

Watch logs until SQL Server is ready:

```powershell
docker logs -f sql2025-dev
```

Create the database:

```powershell
docker exec -it sql2025-dev /opt/mssql-tools18/bin/sqlcmd `
   -S localhost `
   -U sa `
   -P "replace-with-sa-password" `
   -C `
   -Q "CREATE DATABASE WebsiteDev;"
```

On Linux, the same command can be written as:

```bash
docker exec -it sql2025-dev /opt/mssql-tools18/bin/sqlcmd \
   -S localhost \
   -U sa \
   -P "replace-with-sa-password" \
   -C \
   -Q "CREATE DATABASE WebsiteDev;"
```

## SQL Server accounts

There are two categories of SQL accounts:

```text
sa
    Container/database administrator.
    Used for setup, troubleshooting, and creating the application login.

DavidBrowningApp
    Application login.
    Used by the website/admin connection string.
```

Use `sa` only for setup. The applications should connect as `DavidBrowningApp`.

Connect as `sa`, then run the SQL code in DavidBrowning.Web/SQL/AzureSetup/001_CreateUsers.sql_

During early development, `db_datareader` and `db_datawriter` are acceptable. If the application needs to create/drop tables during setup, use the `sa` setup connection for that operation instead of giving the app account permanent DDL permissions.

## Connection strings and user secrets

Do not commit real passwords to `appsettings.json` or `appsettings.Development.json`.

The applications support multiple named SQL Server connection strings. The active SQL connection string is selected by:

    Database:ConnectionName

The currently expected connection string names are:

    LocalSiteDatabase
        Local or LAN SQL Server used for development.

    AzureSiteDatabase
        Azure SQL Database used for production-like testing or Azure deployment.

By default, Development uses:

    Database:ConnectionName = LocalSiteDatabase

Production-style settings use:

    Database:ConnectionName = AzureSiteDatabase

### Public website secrets

From the repository root:

    dotnet user-secrets set `
       --project .\DavidBrowning.Web\DavidBrowning.Web.csproj `
       "ConnectionStrings:LocalSiteDatabase" `
       "Server=<IP OR HOSTNAME>,1433;Database=DavidBrowning;User Id=DavidBrowningApp;Password=<PASSWORD>;TrustServerCertificate=True;"

    dotnet user-secrets set `
       --project .\DavidBrowning.Web\DavidBrowning.Web.csproj `
       "ConnectionStrings:AzureSiteDatabase" `
       "Server=tcp:<SQL SERVER>.database.windows.net,1433;Initial Catalog=DavidBrowning;Persist Security Info=False;User ID=DavidBrowningApp;Password=<PASSWORD>;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

### Admin site secrets

The admin site normally uses a more privileged database account because it edits content and may perform setup or maintenance tasks.

    dotnet user-secrets set `
       --project .\DavidBrowning.Admin\DavidBrowning.Admin.csproj `
       "ConnectionStrings:LocalSiteDatabase" `
       "Server=<IP OR HOSTNAME>,1433;Database=DavidBrowning;User Id=DavidBrowningAdmin;Password=<PASSWORD>;TrustServerCertificate=True;"

    dotnet user-secrets set `
       --project .\DavidBrowning.Admin\DavidBrowning.Admin.csproj `
       "ConnectionStrings:AzureSiteDatabase" `
       "Server=tcp:<SQL SERVER>.database.windows.net,1433;Initial Catalog=DavidBrowning;Persist Security Info=False;User ID=DavidBrowningAdmin;Password=<PASSWORD>;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

### Switching databases during development

To temporarily run the public website against Azure SQL while still using the Development environment:

    dotnet user-secrets set `
       --project .\DavidBrowning.Web\DavidBrowning.Web.csproj `
       "Database:ConnectionName" `
       "AzureSiteDatabase"

To switch back to the local or LAN SQL Server:

    dotnet user-secrets set `
       --project .\DavidBrowning.Web\DavidBrowning.Web.csproj `
       "Database:ConnectionName" `
       "LocalSiteDatabase"

Use the same pattern for the admin project:

    dotnet user-secrets set `
       --project .\DavidBrowning.Admin\DavidBrowning.Admin.csproj `
       "Database:ConnectionName" `
       "AzureSiteDatabase"

    dotnet user-secrets set `
       --project .\DavidBrowning.Admin\DavidBrowning.Admin.csproj `
       "Database:ConnectionName" `
       "LocalSiteDatabase"

User secrets are per project. Setting a secret for `DavidBrowning.Web` does not set it for `DavidBrowning.Admin`.

## Content storage connection strings

The content store can use either local files or an Azure Blob-compatible store. For production parity, development can use Azurite while production uses Azure Blob Storage.

The active blob connection string is selected by:

    Stores:ContentStore:AzureStorageBlobs:ConnectionName

Expected connection string names:

    LocalContentStorage
        LAN or local Azurite.

    AzureContentStorage
        Real Azure Storage Account.

Development defaults to:

    Stores:ContentStore:AzureStorageBlobs:ConnectionName = LocalContentStorage

Production-style settings use:

    Stores:ContentStore:AzureStorageBlobs:ConnectionName = AzureContentStorage

Both environments should use the same logical asset keys and usually the same container name:

    content

Asset keys should remain provider-neutral:

    images/profile.jpg
    documents/resume.pdf
    writing/four-fs.md

Do not store blob URLs or machine-specific paths in database records.

### Local or LAN Azurite secrets

If Azurite is running on the LAN server:

    dotnet user-secrets set `
       --project .\DavidBrowning.Web\DavidBrowning.Web.csproj `
       "ConnectionStrings:LocalContentStorage" `
       "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=<ACCOUNT KEY>;BlobEndpoint=http://<IP OR HOSTNAME>:10000/devstoreaccount1;"

    dotnet user-secrets set `
       --project .\DavidBrowning.Admin\DavidBrowning.Admin.csproj `
       "ConnectionStrings:LocalContentStorage" `
       "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=<ACCOUNT KEY>;BlobEndpoint=http://<IP OR HOSTNAME>:10000/devstoreaccount1;"

The account key for devstoreaccount1 is public knowledge. Look it up.       

If Azurite is running on the development PC, `UseDevelopmentStorage=true` may be enough:

    dotnet user-secrets set `
       --project .\DavidBrowning.Web\DavidBrowning.Web.csproj `
       "ConnectionStrings:LocalContentStorage" `
       "UseDevelopmentStorage=true"

    dotnet user-secrets set `
       --project .\DavidBrowning.Admin\DavidBrowning.Admin.csproj `
       "ConnectionStrings:LocalContentStorage" `
       "UseDevelopmentStorage=true"

### Azure Storage secrets

After the Azure Storage Account is created, store the production-like connection string as:

    dotnet user-secrets set `
       --project .\DavidBrowning.Web\DavidBrowning.Web.csproj `
       "ConnectionStrings:AzureContentStorage" `
       "DefaultEndpointsProtocol=https;AccountName=<ACCOUNT NAME>;AccountKey=<KEY>;EndpointSuffix=core.windows.net"

    dotnet user-secrets set `
       --project .\DavidBrowning.Admin\DavidBrowning.Admin.csproj `
       "ConnectionStrings:AzureContentStorage" `
       "DefaultEndpointsProtocol=https;AccountName=<ACCOUNT NAME>;AccountKey=<KEY>;EndpointSuffix=core.windows.net"

User secrets are per project. Setting a storage secret for `DavidBrowning.Web` does not set it for `DavidBrowning.Admin`.

### Switching blob storage during development

To temporarily use real Azure Blob Storage while running locally:

    dotnet user-secrets set `
       --project .\DavidBrowning.Web\DavidBrowning.Web.csproj `
       "Stores:ContentStore:AzureStorageBlobs:ConnectionName" `
       "AzureContentStorage"

To switch back to Azurite:

    dotnet user-secrets set `
       --project .\DavidBrowning.Web\DavidBrowning.Web.csproj `
       "Stores:ContentStore:AzureStorageBlobs:ConnectionName" `
       "LocalContentStorage"

## Windows, Linux hostnames, DNS, and shares

Windows may not automatically resolve Linux server hostnames. This can affect connection strings, browser URLs, file shares, and development tooling.

For example, this may fail on Windows:

```text
Server=linux-server,1433
```

even though this works:

```text
Server=192.168.1.50,1433
```

There are three practical solutions.

### Recommended: add local DNS records

If the router, Pi-hole, or local DNS server supports static host records, add entries there.

Example:

```text
192.168.1.50    dev-server.lan
192.168.1.50    sql-dev.lan
192.168.1.50    azurite-dev.lan
```

This is the best setup because every device on the LAN can resolve the same names.

Example SQL connection string:

```text
Server=sql-dev.lan,1433;Database=WebsiteDev;User Id=DavidBrowningApp;Password=replace-with-password;TrustServerCertificate=True;
```

Example Azurite blob endpoint:

```text
http://azurite-dev.lan:10000/devstoreaccount1
```

### Alternative: add entries to the Windows hosts file

Open Notepad as Administrator.

Open:

```text
C:\Windows\System32\drivers\etc\hosts
```

Add entries:

```text
192.168.1.50    dev-server
192.168.1.50    dev-server.lan
192.168.1.50    sql-dev.lan
192.168.1.50    azurite-dev.lan
```

Flush DNS:

```powershell
ipconfig /flushdns
```

Test resolution:

```powershell
ping sql-dev.lan
Test-NetConnection sql-dev.lan -Port 1433
Test-NetConnection azurite-dev.lan -Port 10000
```

The hosts-file approach is simple and reliable for one development machine, but it must be repeated on every machine.

### Alternative: use Samba for Linux file shares

If the goal is browsing Linux-hosted folders from Windows, install and configure Samba on the Linux server.

Samba is for SMB file shares such as:

```text
\\dev-server\projects
\\dev-server\content
```

It does not replace DNS for SQL Server, Azurite, or HTTP service hostnames. Even with Samba, it is still better to use local DNS or hosts-file entries so the server name resolves consistently.

If hostname discovery is unreliable, connect by IP:

```text
\\192.168.1.50\projects
```

But for long-term sanity, use local DNS.

## Switch development to SQL Server

The Admin project already uses SQL Server in development.

The Web project may still be configured to use the in-memory provider. To use SQL Server for the public site too, update `DavidBrowning.Web/appsettings.Development.json`:

```json
{
  "Database": {
    "Provider": "SqlServer"
  }
}
```

The stores should already be configured for SQL Server in development:

```json
{
  "Stores": {
    "WritingStore": {
      "Provider": "SqlServer"
    },
    "ProjectStore": {
      "Provider": "SqlServer"
    },
    "ErrorStore": {
      "Provider": "SqlServer"
    },
    "LookupStore": {
      "Provider": "SqlServer"
    },
    "ContentStore": {
      "Provider": "Local",
      "LocalRoot": "ContentAssets"
    }
  }
}
```

## Database setup scripts

The SQL setup scripts live under:

```text
DavidBrowning.Web/SQL/
```

The table scripts live under:

```text
DavidBrowning.Web/SQL/Tables/TableSetup/
```

Helper scripts live under:

```text
DavidBrowning.Web/SQL/Tables/
```

Typical table setup:

```powershell
$connectionString = "Server=localhost,1433;Database=WebsiteDev;User Id=sa;Password=replace-with-sa-password;TrustServerCertificate=True;"

cd .\DavidBrowning.Web\SQL\Tables

.\Get-TableSetupScripts.ps1 -TableSetupPath "TableSetup" |
   .\Invoke-SqlSetupScript.ps1 -ConnectionString $connectionString -Verbose
```

For a LAN SQL Server, use the LAN hostname or IP:

```powershell
$connectionString = "Server=sql-dev.lan,1433;Database=WebsiteDev;User Id=sa;Password=replace-with-sa-password;TrustServerCertificate=True;"
```

Use the `sa` setup connection for initial DDL unless the application login has been deliberately granted schema setup permissions.

If the setup scripts fail, check:

* the database exists;
* the connection string points to the correct server and database;
* SQL Server is running;
* `TrustServerCertificate=True` is present for local/container development;
* foreign-key table creation order is correct;
* old tables from a previous schema are not still present;
* Windows can resolve the LAN hostname if one is used.

## Database seeding

The development seeder loads JSON files into EF Core entities.

Seed files live under:

```text
DavidBrowning.Web/Data/Seeding/DataFiles/
```

The seeder scans the configured seed root folder for `*.json` files and matches each file to an EF Core entity type. A file may match:

```text
DbSet property name
    Projects.json

Physical table name
    db_Projects.json

Physical table name without the configured prefix
    Projects.json

CLR model type name
    Project.json
```

The default table prefix is:

```text
db_
```

The seeder inspects EF Core foreign keys and inserts principal entities before dependent entities. That means lookup/reference tables are inserted before tables that depend on them.

For SQL Server, if a seed file contains explicit integer identity keys, the seeder can temporarily enable identity insert for that table:

```sql
SET IDENTITY_INSERT [table] ON;
```

Seed JSON should use C# property names exactly:

```json
{
  "ProjectStatusId": 1
}
```

not camelCase:

```json
{
  "projectStatusId": 1
}
```

Enums are represented as strings. `DateOnly` values use:

```text
yyyy-MM-dd
```

The seeder is for development/bootstrap data. It is not a production migration tool and should not be treated as a replacement for schema migrations, BACPAC export/import, or deliberate content migration.

## Azurite setup

Azurite is the local Azure Storage emulator. It is optional while the content store uses `Local`, but useful if the project adds or tests an Azure Blob-backed content store.

Azurite can run on the development PC or on the LAN server. The LAN-server setup mirrors SQL Server: a Docker Compose service under `/opt/azurite`.

### Option A: run Azurite on the LAN server

On the Linux LAN server:

```bash
sudo mkdir -p /opt/azurite
sudo chown "$USER":"$USER" /opt/azurite
cd /opt/azurite
```

Create `docker-compose.yml`:

```yaml
services:
  azurite:
    image: mcr.microsoft.com/azure-storage/azurite:latest
    container_name: azurite-dev
    command: >
      azurite
      --blobHost 0.0.0.0
      --queueHost 0.0.0.0
      --tableHost 0.0.0.0
      --location /data
      --debug /data/debug.log
    ports:
      - "10000:10000"
      - "10001:10001"
      - "10002:10002"
    volumes:
      - azurite-dev-data:/data
    restart: unless-stopped

volumes:
  azurite-dev-data:
```

Start Azurite:

```bash
docker compose up -d
```

Check the container:

```bash
docker ps
docker logs azurite-dev
```

If local DNS is configured, use a stable hostname such as:

```text
azurite-dev.lan
```

Blob endpoint:

```text
http://azurite-dev.lan:10000/devstoreaccount1
```

Queue endpoint:

```text
http://azurite-dev.lan:10001/devstoreaccount1
```

Table endpoint:

```text
http://azurite-dev.lan:10002/devstoreaccount1
```

If DNS is not configured, use the server IP:

```text
http://192.168.1.50:10000/devstoreaccount1
```

### Option B: run Azurite on the development PC

Create a folder:

```powershell
mkdir C:\DevServices\azurite
cd C:\DevServices\azurite
```

Create `docker-compose.yml`:

```yaml
services:
  azurite:
    image: mcr.microsoft.com/azure-storage/azurite:latest
    container_name: azurite-dev
    command: >
      azurite
      --blobHost 0.0.0.0
      --queueHost 0.0.0.0
      --tableHost 0.0.0.0
      --location /data
      --debug /data/debug.log
    ports:
      - "10000:10000"
      - "10001:10001"
      - "10002:10002"
    volumes:
      - azurite-dev-data:/data
    restart: unless-stopped

volumes:
  azurite-dev-data:
```

Start Azurite:

```powershell
docker compose up -d
```

Default local endpoints:

```text
Blob service
    http://127.0.0.1:10000/devstoreaccount1

Queue service
    http://127.0.0.1:10001/devstoreaccount1

Table service
    http://127.0.0.1:10002/devstoreaccount1
```

Common development connection string:

```text
UseDevelopmentStorage=true
```

If Azurite is running locally, this is usually enough:

```powershell
dotnet user-secrets set `
   --project .\DavidBrowning.Web\DavidBrowning.Web.csproj `
   "ConnectionStrings:ContentStorage" `
   "UseDevelopmentStorage=true"

dotnet user-secrets set `
   --project .\DavidBrowning.Admin\DavidBrowning.Admin.csproj `
   "ConnectionStrings:ContentStorage" `
   "UseDevelopmentStorage=true"
```

If Azurite is running on a LAN server, prefer explicit endpoints instead of `UseDevelopmentStorage=true`.

Store those values in user secrets when the Azure Blob content store is enabled.

## HTTPS development certificate

ASP.NET Core development uses a local HTTPS certificate.

Trust the development certificate:

```powershell
dotnet dev-certs https --trust
```

If local HTTPS starts failing after moving machines, reinstalling Visual Studio, or changing SDKs, clean and recreate the certificate:

```powershell
dotnet dev-certs https --clean
dotnet dev-certs https --trust
```

Then restart Visual Studio and the browser.

## Restore, build, and run

From the repository root:

```powershell
dotnet restore .\DavidBrowning.sln
dotnet build .\DavidBrowning.sln
```

Run the public site:

```powershell
dotnet run --project .\DavidBrowning.Web\DavidBrowning.Web.csproj
```

Run the admin site:

```powershell
dotnet run --project .\DavidBrowning.Admin\DavidBrowning.Admin.csproj
```

In Visual Studio, set startup projects as needed:

```text
DavidBrowning.Web
    Public website.

DavidBrowning.Admin
    Local admin dashboard.
```

Running both at once is useful when editing content in Admin and checking the public rendering in Web.

## Content assets

The current development content store is local file based.

The public site uses:

```text
DavidBrowning.Web/ContentAssets/
```

The admin site uses:

```text
DavidBrowning.Admin/ContentAssets/
```

Content asset keys should be provider-neutral:

```text
images/profile.jpg
documents/resume.pdf
blurbs/projects.md
```

Avoid machine-specific paths:

```text
C:\Users\David\...
/home/david/...
```

Avoid provider-specific URLs in database records:

```text
https://storage-account.blob.core.windows.net/...
```

That keeps the project movable between local files, Azure Blob Storage, and future content stores.

## Cache and content pipeline

The content system is intentionally layered.

```text
Content store
    Reads raw assets.
    Examples: local file store now, Azure Blob later.

Content renderer
    Converts raw content into renderable output.
    Examples: Markdown to HTML, plain text to paragraphs, image assets to img/link markup.

Content pipeline
    Coordinates store + renderer.
    Owns caching behavior.

Caches
    Avoid repeated file reads, JSON parsing, Markdown rendering, and slug lookups.
```

The cache layer is intended to make the public site cheap and fast without leaking cache responsibilities into every store and controller.

Development cache settings live in configuration under:

```json
{
  "Cache": {
    "EnableContentCache": true,
    "SlugCache": {
      "CacheDuration": 60
    },
    "JsonCache": {
      "CacheDuration": 60
    },
    "RenderTagCache": {
      "CacheDuration": 60
    }
  }
}
```

If content changes do not appear immediately during development, restart the app or temporarily reduce/disable caching.

## Admin dashboard notes

The admin site is a local authoring tool. It is not currently intended to be a public multi-user CMS.

The admin site manages database-backed content such as:

```text
Interests
Work/credentials
Projects
Writing metadata
Assets and asset metadata
Ordered lists
```

Many admin views use partials, Bootstrap, HTMX-style partial replacement, offcanvas edit panels, and small JavaScript helpers.

Sortable lists are edited through dedicated reorder endpoints. Ordinary create/edit view models should not expose `SortOrder`; the insert path or database context should assign a new record to the end of the list, and reorder operations should explicitly rewrite order.

## Moving to a new machine

Use this checklist:

```text
1. Install required tools.
2. Clone the repository.
3. Trust the ASP.NET Core development certificate.
4. Start SQL Server.
5. Create the development database.
6. Create the SQL application login/user.
7. Configure LAN hostnames or hosts-file entries if services run on a Linux server.
8. Set user secrets for Web and Admin.
9. Run SQL table setup scripts.
10. Run or verify seed data.
11. Start Web and Admin.
12. Verify content assets are present.
13. If needed, start Azurite.
```

Commands:

```powershell
git clone https://github.com/david-browning/DavidBrowning.git
cd DavidBrowning

dotnet dev-certs https --trust

dotnet restore .\DavidBrowning.sln
dotnet build .\DavidBrowning.sln
```

Set user secrets:

```powershell
dotnet user-secrets set `
   --project .\DavidBrowning.Web\DavidBrowning.Web.csproj `
   "ConnectionStrings:SiteDatabase" `
   "Server=sql-dev.lan,1433;Database=WebsiteDev;User Id=DavidBrowningApp;Password=replace-with-password;TrustServerCertificate=True;"

dotnet user-secrets set `
   --project .\DavidBrowning.Admin\DavidBrowning.Admin.csproj `
   "ConnectionStrings:SiteDatabase" `
   "Server=sql-dev.lan,1433;Database=WebsiteDev;User Id=DavidBrowningApp;Password=replace-with-password;TrustServerCertificate=True;"
```

Run:

```powershell
dotnet run --project .\DavidBrowning.Web\DavidBrowning.Web.csproj
dotnet run --project .\DavidBrowning.Admin\DavidBrowning.Admin.csproj
```

## Moving data to Azure later

The easiest first production move is:

```text
Local SQL Server
    Export BACPAC

Azure SQL Database
    Import BACPAC
```

That moves schema and table data together.

Remember that database export does not automatically move local content files. If production uses Azure Blob Storage, also upload the content assets and verify that database `AssetKey` values still match the blob keys.

The full move is:

```text
1. Export/import SQL database.
2. Upload content files to production content storage.
3. Set production connection strings/secrets.
4. Verify public pages.
5. Verify admin can still edit content if admin is enabled for that environment.
```

## Troubleshooting

### SQL Server container is not reachable

Check the container:

```powershell
docker ps
docker logs sql2025-dev
```

Check the port:

```powershell
Test-NetConnection localhost -Port 1433
```

For a LAN server:

```powershell
Test-NetConnection sql-dev.lan -Port 1433
```

or:

```powershell
Test-NetConnection 192.168.1.50 -Port 1433
```

Make sure the server firewall allows TCP `1433` only from trusted LAN/VPN clients.

### Azurite container is not reachable

Check the container:

```powershell
docker ps
docker logs azurite-dev
```

Check the blob endpoint:

```powershell
Test-NetConnection azurite-dev.lan -Port 10000
```

or:

```powershell
Test-NetConnection 192.168.1.50 -Port 10000
```

If Azurite runs on the LAN server, make sure Docker maps ports `10000`, `10001`, and `10002`, and make sure the server firewall allows those ports from trusted LAN/VPN clients.

### Windows cannot resolve the Linux server name

Try the IP address first:

```powershell
Test-NetConnection 192.168.1.50 -Port 1433
```

If the IP works but the hostname fails:

```powershell
Test-NetConnection sql-dev.lan -Port 1433
```

then this is name resolution, not SQL Server.

Preferred fix: add local DNS records on the router or Pi-hole.

Example:

```text
192.168.1.50    dev-server.lan
192.168.1.50    sql-dev.lan
192.168.1.50    azurite-dev.lan
```

Alternative fix: add entries to the Windows hosts file.

Open Notepad as Administrator, edit:

```text
C:\Windows\System32\drivers\etc\hosts
```

Add:

```text
192.168.1.50    dev-server.lan
192.168.1.50    sql-dev.lan
192.168.1.50    azurite-dev.lan
```

Flush DNS:

```powershell
ipconfig /flushdns
```

Test again:

```powershell
ping sql-dev.lan
Test-NetConnection sql-dev.lan -Port 1433
Test-NetConnection azurite-dev.lan -Port 10000
```

If the issue is browsing Linux folders from Windows, configure Samba on the Linux server or connect by IP:

```text
\\192.168.1.50\share-name
```

Samba is useful for file shares, but SQL Server and Azurite still need a resolvable hostname or IP address in connection strings.

### Login failed for user

Confirm:

* the login exists in `master`;
* the database user exists in `WebsiteDev`;
* the user is mapped to the login;
* the user has `db_datareader` and `db_datawriter`;
* the password in user secrets matches the SQL login;
* the connection string points to the correct database.

### HTTPS warning or certificate error

Run:

```powershell
dotnet dev-certs https --clean
dotnet dev-certs https --trust
```

Restart Visual Studio and the browser.

### Seed data does not load

Check:

* `Database:Seed:Enabled`;
* the configured seed root folder;
* JSON filenames match entity/table names;
* JSON property names are C# property names;
* the target table is empty if `SkipFileWhenTargetTableHasRows` is true;
* principal tables are seeded before dependent tables;
* enum values are strings;
* `DateOnly` values use `yyyy-MM-dd`.

### Content does not update

Check:

* the correct `ContentAssets` folder;
* the content store provider;
* asset keys are relative and provider-neutral;
* content cache settings;
* whether the app needs a restart.

## Copyright

Copyright © 2026 David Browning. All rights reserved.

This repository is source-available for portfolio review and educational reading only. It is not open source. No permission is granted to copy, modify, redistribute, deploy, rehost, or reuse this website, its source code, design, content, or assets without explicit written permission.

Most files have a copyright header specified. If a file does not, then this copyright notice applies to that file.
