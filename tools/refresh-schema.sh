#!/usr/bin/env bash
# tools/refresh-schema.sh
set -euo pipefail
: "${SHOPPINGCART_DEV_CONNECTION:?Set SHOPPINGCART_DEV_CONNECTION to the DEV database connection string}"
db="$(cd "$(dirname "$0")/.." && pwd)/Database"; mkdir -p "$db"

sqlpackage /Action:Extract "/SourceConnectionString:$SHOPPINGCART_DEV_CONNECTION" "/TargetFile:$db/schema.dacpac" \
  /p:IgnorePermissions=True /p:IgnoreUserLoginMappings=True /p:VerifyExtraction=True

rm -rf "$db/schema"
sqlpackage /Action:Extract "/SourceConnectionString:$SHOPPINGCART_DEV_CONNECTION" "/TargetFile:$db/schema" \
  /p:ExtractTarget=SchemaObjectType /p:IgnorePermissions=True /p:IgnoreUserLoginMappings=True