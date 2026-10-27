# tools/refresh-schema.ps1
param([string]$Connection = $env:SHOPPINGCART_DEV_CONNECTION)

if (-not $Connection) {
    throw "Set SHOPPINGCART_DEV_CONNECTION (or pass -Connection) to the DEV database connection string."
}

$database = Join-Path (Split-Path $PSScriptRoot -Parent) "Database"
New-Item -ItemType Directory -Force $database | Out-Null

# 1. The package the tests deploy
sqlpackage /Action:Extract "/SourceConnectionString:$Connection" "/TargetFile:$database/schema.dacpac" `
    /p:IgnorePermissions=True /p:IgnoreUserLoginMappings=True /p:VerifyExtraction=True
if ($LASTEXITCODE -ne 0) { throw "dacpac extract failed" }

# 2. A readable copy, one .sql file per object, so pull requests show what changed
if (Test-Path "$database/schema") { Remove-Item "$database/schema" -Recurse -Force }
sqlpackage /Action:Extract "/SourceConnectionString:$Connection" "/TargetFile:$database/schema" `
    /p:ExtractTarget=SchemaObjectType /p:IgnorePermissions=True /p:IgnoreUserLoginMappings=True
if ($LASTEXITCODE -ne 0) { throw "readable extract failed" }

Write-Host "Done. Review 'git diff Database' before committing."