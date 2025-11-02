# GitHub Copilot Prompt Engineering Guide for Testing Automation

> Effective prompts for generating high-quality tests, API endpoints, and refactoring code in the Board Game Café demo environment.

## Table of Contents
- [Unit Test Generation](#unit-test-generation)
- [API Endpoint Generation](#api-endpoint-generation)
- [Playwright E2E Test Generation](#playwright-e2e-test-generation)
- [Test Data Builder Prompts](#test-data-builder-prompts)
- [Refactoring Prompts](#refactoring-prompts)
- [Prompt Engineering Best Practices](#prompt-engineering-best-practices)

---

## Unit Test Generation

### Example 1: Service Layer Testing (xUnit + FluentAssertions)

**Good Prompt:**
```
Generate xUnit tests for OrderCalculationService with FluentAssertions. Include:
- Tax calculation: 8% on food, 10% on alcohol
- Member discount calculation: Bronze 5%, Silver 10%, Gold 15%
- Edge cases: zero amount, null member tier, negative prices
- Use Arrange-Act-Assert pattern
```

**Why It Works:**
- ✅ Specifies testing framework (xUnit) and assertion library (FluentAssertions)
- ✅ Provides business rules explicitly
- ✅ Requests edge cases
- ✅ Mentions desired test pattern

**Bad Prompt:**
```
Write tests for order calculations
```

**Why It Fails:**
- ❌ No framework specified
- ❌ No business rules context
- ❌ No edge case guidance
- ❌ Too vague

---

### Example 2: Domain Entity Validation

**Good Prompt:**
```
Create unit tests for Reservation entity validation using xUnit and FluentAssertions. Test:
- Party size must be between 1 and table seating capacity
- Start time must be in the future
- End time must be after start time
- Duration must be between 1 and 4 hours
- Use ReservationBuilder test data builder for setup
```

**Generated Test Example:**
```csharp
[Fact]
public void Create_ShouldThrowException_WhenPartySizeExceedsTableCapacity()
{
    // Arrange
    var table = new TableBuilder().WithSeatingCapacity(4).Build();
    
    // Act
    Action act = () => new ReservationBuilder()
        .WithTable(table)
        .WithPartySize(5)
        .Build();
    
    // Assert
    act.Should().Throw<ArgumentException>()
        .WithMessage("*party size*capacity*");
}
```

---

### Example 3: Repository Pattern Testing

**Good Prompt:**
```
Generate integration tests for GameRepository using WebApplicationFactory and in-memory SQLite. Include:
- GetAvailableGamesAsync should return only games with CopiesInUse < CopiesOwned
- FilterByPlayerCountAsync should include games where MinPlayers <= input <= MaxPlayers
- FilterByCategoryAsync should use case-insensitive comparison
- Seed test data with GameBuilder
```

---

## API Endpoint Generation

### Example 1: REST CRUD Endpoint

**Good Prompt:**
```
Create REST endpoint for game checkout with validation at POST /api/v1/games/{id}/checkout.
Request: { customerId: guid, reservationId: guid }
Response: 200 OK with updated game, 404 if game not found, 409 if no copies available
Include:
- XML documentation for Swagger
- Fluent validation
- Business rule: CopiesInUse cannot exceed CopiesOwned
```

**Generated Code Structure:**
```csharp
/// <summary>
/// Checks out a game to a customer's reservation
/// </summary>
app.MapPost("/api/v1/games/{id}/checkout", async (
    Guid id,
    CheckoutGameRequest request,
    BoardGameCafeDbContext db) =>
{
    var game = await db.Games.FindAsync(id);
    if (game == null) return Results.NotFound();
    
    if (game.CopiesInUse >= game.CopiesOwned)
        return Results.Conflict(new { error = "No copies available" });
    
    game.CopiesInUse++;
    await db.SaveChangesAsync();
    
    return Results.Ok(game.ToDto());
})
.WithName("CheckoutGame")
.WithTags("Games")
.Produces<GameDto>(200)
.Produces(404)
.Produces(409);
```

---

### Example 2: Query Endpoint with Filtering

**Good Prompt:**
```
Generate GET /api/v1/reservations/availability endpoint that returns available tables.
Query params: date (required), partySize (required), startTime, duration (hours)
Logic:
- Find tables with SeatingCapacity >= partySize
- Exclude tables with overlapping reservations (include 15-minute buffer)
- Return list of AvailableTableDto with { tableId, number, capacity, hourlyRate }
- Include Swagger documentation
```

---

## Playwright E2E Test Generation

### Example 1: Reservation Booking Flow

**Good Prompt:**
```
Write Playwright E2E test for reservation booking flow using Page Object Model:
1. Navigate to /reservations
2. Select date using date picker (data-testid="date-picker")
3. Select party size from dropdown (data-testid="party-size-select")
4. Click "Find Tables" button
5. Verify available tables are displayed
6. Click first available table card
7. Fill in customer name and email
8. Click "Confirm Reservation" button
9. Assert success message appears with confirmation number
Include auto-waiting for API responses
```

**Generated Test Example:**
```typescript
test('should complete reservation booking end-to-end', async ({ page }) => {
  const reservationPage = new ReservationPage(page);
  await reservationPage.goto();
  
  await reservationPage.selectDate('2025-12-01');
  await reservationPage.selectPartySize(4);
  await reservationPage.clickFindTables();
  
  await expect(reservationPage.availableTablesSection).toBeVisible();
  
  await reservationPage.selectFirstTable();
  await reservationPage.fillCustomerInfo('John Doe', 'john@example.com');
  await reservationPage.confirmReservation();
  
  await expect(reservationPage.successMessage).toContainText('Confirmation');
});
```

---

### Example 2: Visual Regression Test

**Good Prompt:**
```
Create Playwright visual regression test for game catalog page:
- Navigate to /games
- Wait for games to load (data-testid="game-card")
- Apply filter: Category = "Strategy"
- Take screenshot of filtered results
- Compare with baseline screenshot
- Test on chromium, firefox, and webkit
```

---

## Test Data Builder Prompts

### Example 1: Fluent Builder

**Good Prompt:**
```
Create fluent builder for Game entity with:
- Sensible defaults: Title="Test Game", Publisher="Test Publisher", MinPlayers=2, MaxPlayers=4
- With methods for each property: WithTitle(), WithPublisher(), WithPlayerCount(min, max)
- Build() method that returns Game instance
- BuildDto() method that returns GameDto
- Follow existing builder pattern in Builders/ folder
```

**Generated Code:**
```csharp
public class GameBuilder
{
    private string _title = "Test Game";
    private string _publisher = "Test Publisher";
    private int _minPlayers = 2;
    private int _maxPlayers = 4;
    private int _copiesOwned = 3;
    private int _copiesInUse = 0;
    
    public GameBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }
    
    public GameBuilder WithAvailability(int owned, int inUse)
    {
        _copiesOwned = owned;
        _copiesInUse = inUse;
        return this;
    }
    
    public Game Build() => new()
    {
        Id = Guid.NewGuid(),
        Title = _title,
        Publisher = _publisher,
        MinPlayers = _minPlayers,
        MaxPlayers = _maxPlayers,
        CopiesOwned = _copiesOwned,
        CopiesInUse = _copiesInUse
    };
}
```

---

### Example 2: Test Data Fixture

**Good Prompt:**
```
Generate test data fixture with sample games for E2E tests:
- 3 Strategy games (Catan, Ticket to Ride, Pandemic)
- 2 Party games (Codenames, Exploding Kittens)
- 1 Cooperative game (Pandemic)
- Export as JSON-serializable array
- Include realistic player counts, complexity, and pricing
```

---

## Refactoring Prompts

### Example 1: Extract Validation Logic

**Good Prompt:**
```
Extract reservation validation logic into ReservationValidator class using FluentValidation.
Rules to extract:
- Party size between 1 and table capacity
- Start time must be future date/time
- End time after start time
- No overlapping reservations (check against database)
Update ReservationsEndpoints.cs to use validator
```

**Before:**
```csharp
// Validation mixed with endpoint logic
if (request.PartySize < 1 || request.PartySize > table.SeatingCapacity)
    return Results.BadRequest("Invalid party size");
if (request.StartTime < DateTime.UtcNow)
    return Results.BadRequest("Start time must be in the future");
```

**After:**
```csharp
// Clean separation of concerns
public class ReservationValidator : AbstractValidator<CreateReservationRequest>
{
    public ReservationValidator(BoardGameCafeDbContext db)
    {
        RuleFor(x => x.PartySize)
            .GreaterThan(0)
            .LessThanOrEqualTo(x => x.Table.SeatingCapacity);
            
        RuleFor(x => x.StartTime)
            .GreaterThan(DateTime.UtcNow);
    }
}
```

---

### Example 2: Introduce Repository Pattern

**Good Prompt:**
```
Refactor direct DbContext usage in GamesEndpoints.cs into IGameRepository interface.
Methods needed:
- GetByIdAsync(Guid id)
- GetAvailableGamesAsync(GameFilterRequest filter)
- CreateAsync(Game game)
- UpdateAsync(Game game)
- DeleteAsync(Guid id)
Use dependency injection to inject repository into endpoints
```

---

## Prompt Engineering Best Practices

### ✅ Do's

1. **Be Specific About Frameworks**
   - ❌ "Write tests"
   - ✅ "Generate xUnit tests with FluentAssertions"

2. **Provide Business Context**
   - ❌ "Test the order service"
   - ✅ "Test order calculation with 8% tax on food, 10% on alcohol, and member discounts (Bronze 5%, Silver 10%, Gold 15%)"

3. **Request Edge Cases**
   - ❌ "Test the validation"
   - ✅ "Test validation including null inputs, empty strings, negative numbers, and boundary values"

4. **Specify Patterns**
   - ❌ "Create an endpoint"
   - ✅ "Create REST endpoint following Minimal API pattern with Swagger documentation and FluentValidation"

5. **Reference Existing Code**
   - ❌ "Build a test"
   - ✅ "Build a test following the pattern in GameAvailabilityServiceTests.cs"

6. **Include Expected Outcomes**
   - ❌ "Test the API"
   - ✅ "Test POST /api/games returns 201 Created with location header on success, 400 Bad Request for invalid input, 409 Conflict for duplicate titles"

---

### ❌ Don'ts

1. **Don't Be Vague**
   - ❌ "Make it better"
   - ✅ "Refactor to extract validation into FluentValidation validators"

2. **Don't Omit Test Data Strategy**
   - ❌ "Test the reservation logic"
   - ✅ "Test reservation logic using ReservationBuilder and TableBuilder test data builders"

3. **Don't Forget Error Cases**
   - ❌ "Test successful creation"
   - ✅ "Test successful creation AND failure cases: 404 not found, 400 validation errors, 409 conflicts"

4. **Don't Skip Documentation**
   - ❌ "Generate endpoint"
   - ✅ "Generate endpoint with XML documentation for Swagger, including param descriptions and response examples"

5. **Don't Ignore Existing Conventions**
   - ❌ "Add new tests anywhere"
   - ✅ "Add tests in src/BoardGameCafe.Tests.Unit/Services/ following existing namespace and file structure"

---

## Multi-Step Prompts for Complex Tasks

### Example: Complete Feature with Tests

**Step 1: Domain Model**
```
Create Event domain entity with properties: Id, Title, Description, StartTime, EndTime, MaxParticipants, CurrentParticipants (computed), EventType enum (Tournament, GameNight, Workshop)
```

**Step 2: Repository**
```
Create IEventRepository with GetUpcomingEventsAsync, GetByIdAsync, RegisterParticipantAsync, CancelRegistrationAsync
Implement with EventRepository using DbContext
```

**Step 3: API Endpoint**
```
Create POST /api/v1/events/{id}/register endpoint
Request: { customerId: guid }
Response: 201 Created, 409 Conflict if full or already registered
Include capacity validation and concurrency handling
```

**Step 4: Unit Tests**
```
Generate xUnit tests for event capacity validation
Test scenarios: successful registration, event full, duplicate registration, event not found
```

**Step 5: Integration Tests**
```
Generate integration tests for events API using WebApplicationFactory
Test full registration workflow end-to-end with real database
```

**Step 6: E2E Tests**
```
Create Playwright test for event registration user flow
Navigate to /events, select event, fill form, verify confirmation
```

---

## Context-Aware Prompts

When working in a specific file, Copilot uses context. Enhance with:

```csharp
// In ReservationEndpoints.cs

// TODO: Generate endpoint for checking in a reservation
// PATCH /api/v1/reservations/{id}/check-in
// Updates Status from Confirmed to CheckedIn
// Returns 200 OK with updated reservation
// Returns 404 if not found
// Returns 400 if already checked in or cancelled

// Copilot will generate the endpoint using surrounding code patterns
```

---

## Testing-Specific Prompt Templates

### Template 1: Unit Test Class
```
Generate unit test class for [ClassName] using xUnit and FluentAssertions.
Include tests for:
- [Method1]: [expected behavior]
- [Method2]: [expected behavior]
- Edge cases: [list specific scenarios]
Use [BuilderName] for test data setup.
Follow Arrange-Act-Assert pattern.
```

### Template 2: Integration Test Class
```
Generate integration test class for [API endpoint] using WebApplicationFactory.
Test scenarios:
- Happy path: [describe]
- Validation errors: [list invalid inputs]
- Not found cases: [when]
- Conflict cases: [when]
Seed database with [test data description].
Use realistic test data.
```

### Template 3: E2E Test Suite
```
Create Playwright E2E test suite for [feature name].
User journey:
1. [Step 1]
2. [Step 2]
3. [Step 3]
Use Page Object Model with [PageName]Page class.
Test on chromium, firefox, webkit.
Include screenshots on failure.
```

---

## Tips for Workshop Participants

1. **Start Simple**: Begin with small, focused prompts and iterate
2. **Use Examples**: Reference existing code patterns in your prompts
3. **Be Incremental**: Generate tests one method at a time, not entire classes
4. **Review & Refine**: Copilot's first attempt may need tweaking
5. **Provide Feedback**: If the output isn't right, refine your prompt with more context
6. **Learn Patterns**: Study generated code to understand best practices
7. **Test Coverage**: Use prompts to ensure edge cases are covered
8. **Consistency**: Reference existing builders, fixtures, and patterns

---

## Additional Resources

- [GitHub Copilot Documentation](https://docs.github.com/en/copilot)
- [xUnit Documentation](https://xunit.net/)
- [FluentAssertions Documentation](https://fluentassertions.com/)
- [Playwright Documentation](https://playwright.dev/)
- [Minimal APIs Documentation](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis)
