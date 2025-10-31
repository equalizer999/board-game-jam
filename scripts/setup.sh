#!/bin/bash
set -e

echo "🎮 Board Game Café - Initial Setup"
echo "==================================="

# Create data directory
mkdir -p data

# Backend setup (will be added when solution exists)
if [ -f "src/BoardGameCafe.Api/BoardGameCafe.Api.csproj" ]; then
  echo "📦 Running EF migrations..."
  cd src/BoardGameCafe.Api
  dotnet ef database update
  cd ../..
else
  echo "⏭️  Skipping migrations (project not yet created)"
fi

# Seed database (will be added)
if [ -f "scripts/seed-db.sh" ]; then
  echo "🌱 Seeding database..."
  bash scripts/seed-db.sh
fi

# Install Playwright browsers
if [ -f "client/package.json" ]; then
  echo "🎭 Installing Playwright browsers..."
  cd client
  npx playwright install --with-deps chromium firefox webkit
  cd ..
else
  echo "⏭️  Skipping Playwright (client not yet created)"
fi

echo ""
echo "✅ Setup complete!"
echo ""
echo "Next steps:"
echo "  1. Start API: cd src/BoardGameCafe.Api && dotnet run"
echo "  2. Start Frontend: cd client && npm run dev"
echo "  3. Open Swagger: https://localhost:5001/swagger"
echo ""