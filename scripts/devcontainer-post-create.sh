#!/usr/bin/env bash
set -euo pipefail

echo "[devcontainer] Restoring .NET & installing Node deps if present"
if [ -f "src/BoardGameCafe.Api/BoardGameCafe.Api.csproj" ]; then
  dotnet restore src/BoardGameCafe.Api/BoardGameCafe.Api.csproj
fi
if [ -f "client/package.json" ]; then
  cd client && npm install && cd -
fi

if [ -f "scripts/seed-db.sh" ]; then
  bash scripts/seed-db.sh || echo "Seed script not ready yet"
fi

if [ -f "scripts/generate-openapi-client.sh" ]; then
  bash scripts/generate-openapi-client.sh || echo "OpenAPI client not generated yet"
fi

echo "[devcontainer] Completed post-create tasks"
