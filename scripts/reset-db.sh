#!/usr/bin/env bash
set -euo pipefail
DB_FILE="data/dev.db"
if [ -f "$DB_FILE" ]; then
  rm "$DB_FILE"
  echo "Removed $DB_FILE"
fi
bash scripts/seed-db.sh
