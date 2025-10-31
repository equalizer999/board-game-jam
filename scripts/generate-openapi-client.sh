#!/usr/bin/env bash
set -euo pipefail
OUTPUT_DIR="client/src/generated"
SPEC_URL="http://localhost:5000/swagger/v1/swagger.json"
mkdir -p "$OUTPUT_DIR"
# Placeholder: use openapi-generator-cli or NSwag
# npx @openapitools/openapi-generator-cli generate -i $SPEC_URL -g typescript-fetch -o $OUTPUT_DIR || echo "Generation skipped (API not running)"
echo "(Placeholder) OpenAPI client generation step"
