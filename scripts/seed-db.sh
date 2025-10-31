#!/usr/bin/env bash
set -euo pipefail
DB_FILE="data/dev.db"
mkdir -p data
if [ ! -f "$DB_FILE" ]; then
  echo "Creating SQLite database at $DB_FILE"
fi
# Placeholder for EF migrations
# dotnet ef database update --project src/BoardGameCafe.Api/BoardGameCafe.Api.csproj
echo "Seeding initial data (placeholder)"
# Future: use dotnet run seed command or SQL inserts.
