# GitHub Copilot Custom Instructions for Board Game Café Project

## Project Context

This is a **Board Game Café Management System** demo repository designed for testing automation workshops. The project showcases GitHub Copilot's capabilities in test generation, API development, and E2E testing.

## Tech Stack

**Backend:**
- .NET 9.0 with Minimal APIs
- Entity Framework Core 9 with SQLite
- xUnit + FluentAssertions for testing
- Swashbuckle for OpenAPI/Swagger

**Frontend:**
- React 18+ with TypeScript
- Vite for development
- Playwright for E2E testing
- TanStack Query for state management

## Code Conventions

### Backend (.NET/C#)

**Project Structure:**
- Use **Vertical Slice Architecture** for features
- Feature folders: `src/BoardGameCafe.Api/Features/{FeatureName}/`
- Each feature contains: DTOs, endpoints, validators
- Domain entities: `src/BoardGameCafe.Domain/Entities/`
- Tests: `tests/BoardGameCafe.Tests.Unit/` and `tests/BoardGameCafe.Tests.Integration/`

**Naming Conventions:**
- Entities: `PascalCase` (e.g., `Game`, `Reservation`, `Customer`)
- DTOs: `{Entity}Dto` or `{Action}{Entity}Request` (e.g., `GameDto`, `CreateReservationRequest`)
- Endpoints: `{Feature}Endpoints.cs` (e.g., `GamesEndpoints.cs`)
- Tests: `{Class}Tests.cs` with method naming: `MethodName_Scenario_ExpectedResult`

**Testing Standards:**
- Use xUnit with `[Fact]` for single tests, `[Theory]` with `[InlineData]` for parameterized tests
- Use FluentAssertions for all assertions: `result.Should().Be(expected)`
- Follow Arrange-Act-Assert pattern
- Use test data builders from `tests/BoardGameCafe.Tests.Unit/Builders/`
- Integration tests use `WebApplicationFactory` with in-memory SQLite

**API Patterns:**
- Use Minimal APIs with typed `Results<>` return types
- Add XML documentation comments for Swagger
- Include `.WithName()`, `.WithTags()`, `.Produces<>()` for all endpoints
- Validate requests using data annotations or FluentValidation
- Return proper HTTP status codes: 200/201 (success), 400 (validation), 404 (not found), 409 (conflict)

**Database:**
- Use EF Core fluent API in `OnModelCreating`, not data annotations
- Create indexes for commonly queried fields
- Use check constraints for business rules
- Create migrations: `dotnet ef migrations add {MigrationName}`

### Frontend (React/TypeScript)

**Component Structure:**
- Functional components with TypeScript
- Use hooks: `useState`, `useEffect`, `useQuery`, `useMutation`
- Page components in `client/src/pages/`
- Reusable components in `client/src/components/`

**Playwright Testing:**
- Use Page Object Model pattern in `client/tests/e2e/pages/`
- Use `data-testid` attributes for stable selectors
- Test fixtures in `client/tests/e2e/fixtures/`
- Test specs in `client/tests/e2e/specs/`

## Common Tasks

### Creating a New API Endpoint

1. Create feature folder: `src/BoardGameCafe.Api/Features/{Feature}/`
2. Create DTOs for request/response
3. Create `{Feature}Endpoints.cs` with Minimal API endpoints
4. Add XML documentation for Swagger
5. Register endpoints in `Program.cs`: `app.MapGroup("/api/v1/{feature}").Add{Feature}Endpoints()`
6. Create integration tests in `tests/BoardGameCafe.Tests.Integration/Features/{Feature}/`

### Creating a New Domain Entity

1. Create entity in `src/BoardGameCafe.Domain/Entities/{Entity}.cs`
2. Add `DbSet<{Entity}>` to `BoardGameCafeDbContext.cs`
3. Configure entity in `OnModelCreating()` with fluent API
4. Create migration: `dotnet ef migrations add Add{Entity}`
5. Apply migration: `dotnet ef database update`
6. Add seed data if needed

### Writing Unit Tests

1. Create test class in `tests/BoardGameCafe.Tests.Unit/{Category}/{Class}Tests.cs`
2. Use test data builders for setup
3. Use `[Theory]` with `[InlineData]` for multiple test cases
4. Use FluentAssertions: `.Should().Be()`, `.Should().Throw<>()`, `.Should().NotBeNull()`
5. Test happy path, edge cases, and error conditions

### Writing E2E Tests

1. Create Page Object Model in `client/tests/e2e/pages/{Page}Page.ts`
2. Add selectors using `data-testid` attributes
3. Create test spec in `client/tests/e2e/specs/{feature}.spec.ts`
4. Use fixtures for test data setup
5. Run tests: `npx playwright test`

## Important Notes

### Testing Focus
This repository is a **testing automation demo**. When suggesting code:
- Always include comprehensive tests (unit, integration, E2E as appropriate)
- Demonstrate Copilot's testing capabilities
- Include edge cases and error scenarios
- Use test data builders for clean test setup

### Business Rules to Know

**Reservations:**
- Party size must not exceed table seating capacity
- Reservations must be in the future
- 15-minute buffer between reservations on same table
- Status: Pending → Confirmed → CheckedIn → Completed/Cancelled

**Orders:**
- Tax rates: 8% on food, 10% on alcohol
- Member discounts: Bronze 5%, Silver 10%, Gold 15%
- Loyalty points: 1 point per $1 spent (when order is Paid)
- Points redemption: 100 points = $1 discount

**Games:**
- CopiesInUse cannot exceed CopiesOwned
- IsAvailable computed property: CopiesOwned > CopiesInUse
- Categories: Strategy, Party, Family, Cooperative, Abstract

**Loyalty Tiers:**
- None: 0 points (0% discount)
- Bronze: 1-499 points (5% discount)
- Silver: 500-1999 points (10% discount)
- Gold: 2000+ points (15% discount)

### Do NOT

- Don't use data annotations for EF configuration (use fluent API)
- Don't use lazy loading (use eager loading with `.Include()`)
- Don't create `[Authorize]` attributes (auth not implemented yet)
- Don't hardcode dates/times (use `DateTime.UtcNow` or test clock)
- Don't skip error handling in API endpoints

### Helpful Patterns

**Test Data Builder Example:**
```csharp
public class GameBuilder
{
    private string _title = "Test Game";
    private int _copiesOwned = 3;
    
    public GameBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }
    
    public Game Build() => new()
    {
        Id = Guid.NewGuid(),
        Title = _title,
        CopiesOwned = _copiesOwned
    };
}
```

**Minimal API Example:**
```csharp
app.MapGet("/api/v1/games/{id}", async (Guid id, BoardGameCafeDbContext db) =>
{
    var game = await db.Games.FindAsync(id);
    return game is null ? Results.NotFound() : Results.Ok(game.ToDto());
})
.WithName("GetGame")
.WithTags("Games")
.Produces<GameDto>(200)
.Produces(404);
```

**E2E Test Example:**
```typescript
test('should browse games and view details', async ({ page }) => {
  const catalogPage = new GameCatalogPage(page);
  await catalogPage.goto();
  
  await expect(catalogPage.gameCards).toHaveCountGreaterThan(0);
  await catalogPage.clickGameCard(0);
  await expect(catalogPage.gameDetailModal).toBeVisible();
});
```

## References

- [Copilot Prompts Guide](../docs/copilot-prompts-guide.md)
- [Copilot Agent Assignment Guide](../docs/copilot-agent-assignment-guide.md)
- [API Testing Guide](../docs/api-testing-guide.md)
- [Testing Engineer Guide](../docs/testing-engineer-copilot-guide.md)

## When in Doubt

Follow existing patterns in the codebase. Examples:
- API endpoints: `src/BoardGameCafe.Api/Features/Games/GamesEndpoints.cs`
- Domain entities: `src/BoardGameCafe.Domain/Entities/Game.cs`
- Unit tests: `tests/BoardGameCafe.Tests.Unit/Services/OrderCalculationServiceTests.cs`
- Integration tests: `tests/BoardGameCafe.Tests.Integration/Features/Games/GamesEndpointsTests.cs`
- E2E tests: `client/tests/e2e/specs/game-browsing.spec.ts`
