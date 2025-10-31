# Board Game Café Demo (Copilot Testing Environment)

A fork-friendly demonstration project for Test Automation Engineers showcasing GitHub Copilot assisting with:
- .NET 9 Minimal REST API development (Games, Reservations, Orders, Events)
- React + TypeScript frontend
- Automated test layers: unit, integration, Playwright E2E
- Intentional bug branches for regression scenarios
- DevContainer (VS Code) environment + OpenAPI (Swagger)

## Tech Stack
Backend: .NET 9 Minimal APIs, EF Core (SQLite default)  
Frontend: React 18 + TypeScript + Vite + TanStack Query  
Testing: xUnit, FluentAssertions, WebApplicationFactory, Playwright  
Dev Environment: VS Code DevContainer (Node LTS + .NET 9)  
CI/CD: GitHub Actions (build, tests, e2e)  

## Project Goals
Provide a realistic yet engaging domain (Board Game Café) to explore:
- Copilot test generation (edge cases, parameterized tests)
- REST API contract validation via Swagger/OpenAPI
- UI flows with Playwright across multiple browsers
- Bug hunting & regression test patterns
- Seed data + test data builders

## Repository Layout (Initial)
```
src/BoardGameCafe.Api/      # Backend API (Minimal APIs)
client/                     # React + TS frontend
scripts/                    # Utility scripts (seed, reset, openapi)
.docs/ (future)             # Architecture diagrams, deep dives
.devcontainer/              # Codespaces / DevContainer config
exercises/                  # Workshop exercise descriptions
playwright/ (future)        # E2E tests root
``` 

## Getting Started (Local DevContainer or Codespaces)
1. Open in VS Code (local with DevContainer extension or GitHub Codespaces)
2. Container builds and runs post-create script (restore, migrate, seed)
3. Launch API: `dotnet run` (if not auto-started) → Swagger at `/swagger`
4. Launch frontend: `npm run dev` inside `client`

## OpenAPI / Swagger
The REST API auto-generates full OpenAPI spec using Swashbuckle. Client regeneration script: `scripts/generate-openapi-client.sh`.

## Seed Data
Will include sample Games, Tables, Menu Items, Customers, Events. See issue tracking for details.

## Roadmap
Structured issues drive implementation: environment setup → domain slices → endpoints → tests → bug branches → CI/CD.

## Contributing
Fork the repo or spin up a Codespace. Use issues labeled `exercise` for workshop tasks. Apply tests before PR submission. Refer to `docs/COPILOT_PROMPTS.md` (coming soon) for prompt examples.

## License
MIT License.
