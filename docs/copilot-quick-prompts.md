# GitHub Copilot Quick Prompts Reference

> **Copy-paste ready prompts** for common tasks in the Board Game Café project. Optimized for GitHub Copilot Chat and inline suggestions.

## Table of Contents
- [Backend Development](#backend-development)
- [Testing](#testing)
- [Frontend Development](#frontend-development)
- [Database & Migrations](#database--migrations)
- [Debugging & Troubleshooting](#debugging--troubleshooting)

---

## Backend Development

### Create a New REST API Endpoint

```
Create a REST API endpoint at POST /api/v1/{resource} with:
- Request DTO: Create{Resource}Request with properties: {list properties}
- Response DTO: {Resource}Dto
- Validation: {validation rules}
- Business logic: {describe logic}
- Response codes: 201 Created, 400 Bad Request, 409 Conflict
- XML documentation for Swagger
- Register in Program.cs
```

**Example:**
```
Create a REST API endpoint at POST /api/v1/games/{id}/checkout with:
- Request DTO: CheckoutGameRequest with properties: customerId (Guid), reservationId (Guid)
- Response DTO: GameDto
- Validation: game must exist, must have available copies
- Business logic: increment CopiesInUse, create GameSession record
- Response codes: 200 OK, 404 Not Found, 409 Conflict if no copies available
- XML documentation for Swagger
- Register in Program.cs
```

### Create DTOs

```
Create DTOs for {Entity}:
1. {Entity}Dto - for API responses with properties: {list properties}
2. Create{Entity}Request - for POST requests with properties: {list properties}
3. Update{Entity}Request - for PUT requests with properties: {list properties}
Include data annotations for validation
```

**Example:**
```
Create DTOs for Event:
1. EventDto - for API responses with properties: Id, Title, Description, StartTime, EndTime, MaxParticipants, CurrentParticipants, EventType
2. CreateEventRequest - for POST requests with properties: Title, Description, StartTime, EndTime, MaxParticipants, EventType
3. UpdateEventRequest - for PUT requests with properties: Title, Description, StartTime, EndTime, MaxParticipants
Include data annotations: Title max 200 chars, StartTime and EndTime required, MaxParticipants between 5 and 100
```

### Add Validation Logic

```
Add validation to {Entity} using data annotations:
- {Property}: Required, MaxLength {n}
- {Property}: Range {min} to {max}
- {Property}: RegularExpression {pattern}
Create custom validation for: {custom rule}
```

**Example:**
```
Add validation to CreateReservationRequest using data annotations:
- PartySize: Required, Range 1 to 20
- StartTime: Required
- Duration: Required, Range 1 to 4 hours
Create custom validation for: StartTime must be in the future, EndTime must be after StartTime
```

### Implement Business Logic Service

```
Create service class {ServiceName} in src/BoardGameCafe.Api/Services/ with:
- Constructor dependency: BoardGameCafeDbContext
- Method: {MethodName} that {describe behavior}
- Business rules: {list rules}
- Return type: {type}
- Error handling: throw {ExceptionType} when {condition}
```

**Example:**
```
Create service class OrderCalculationService in src/BoardGameCafe.Api/Services/ with:
- Constructor dependency: BoardGameCafeDbContext
- Method: CalculateTotal(Order order) that computes subtotal, tax, discount, and final total
- Business rules: tax is 8% on food, 10% on alcohol; apply member discount (Bronze 5%, Silver 10%, Gold 15%)
- Return type: decimal
- Error handling: throw ArgumentNullException when order is null, throw InvalidOperationException when order has no items
```

---

## Testing

### Generate Unit Tests

```
Generate xUnit unit tests for {ClassName} in tests/BoardGameCafe.Tests.Unit/{Category}/{ClassName}Tests.cs with FluentAssertions:
- Test method {MethodName} with scenarios: {list scenarios}
- Use [Theory] with [InlineData] for parameterized tests
- Test edge cases: null inputs, empty collections, boundary values, negative numbers
- Use {Builder}Builder for test data setup
- Follow Arrange-Act-Assert pattern
- Test naming: MethodName_Scenario_ExpectedResult
```

**Example:**
```
Generate xUnit unit tests for OrderCalculationService in tests/BoardGameCafe.Tests.Unit/Services/OrderCalculationServiceTests.cs with FluentAssertions:
- Test method CalculateTotal with scenarios: food-only order, alcohol-only order, mixed order, order with Bronze/Silver/Gold discount
- Use [Theory] with [InlineData] for tax rate tests (8% food, 10% alcohol)
- Test edge cases: null order, empty order items, zero quantity, negative prices
- Use OrderBuilder for test data setup
- Follow Arrange-Act-Assert pattern
- Test naming: CalculateTotal_FoodOnlyOrder_Returns8PercentTax
```

### Generate Integration Tests

```
Generate integration tests for {Feature} API in tests/BoardGameCafe.Tests.Integration/Features/{Feature}/{Feature}EndpointsTests.cs:
- Use WebApplicationFactory<Program>
- Test endpoints: {list endpoints with methods}
- Test scenarios: success case, validation errors, not found, conflict
- Seed database with: {test data description}
- Use realistic test data
- Verify response status codes and body content
```

**Example:**
```
Generate integration tests for Games API in tests/BoardGameCafe.Tests.Integration/Features/Games/GamesEndpointsTests.cs:
- Use WebApplicationFactory<Program>
- Test endpoints: GET /api/v1/games, GET /api/v1/games/{id}, POST /api/v1/games, PUT /api/v1/games/{id}
- Test scenarios: list returns all games, get by ID returns correct game, create returns 201 with location header, update returns 200, get non-existent returns 404
- Seed database with: 5 sample games (Catan, Pandemic, Ticket to Ride, Codenames, Azul)
- Use realistic test data with different categories and player counts
- Verify response status codes and that returned DTOs match expected values
```

### Generate Playwright E2E Tests

```
Create Playwright E2E test for {feature} in client/tests/e2e/specs/{feature}.spec.ts:
- Use Page Object Model: {Page}Page
- User journey: {describe step-by-step workflow}
- Verify: {list assertions}
- Test on: chromium, firefox, webkit
- Use data-testid selectors for stability
- Include auto-waiting for API responses
```

**Example:**
```
Create Playwright E2E test for game checkout in client/tests/e2e/specs/game-checkout.spec.ts:
- Use Page Object Model: GameCatalogPage, ReservationPage
- User journey: 
  1. Navigate to /games
  2. Browse available games
  3. Select "Catan" game card
  4. Click "Checkout Game" button
  5. Fill in reservation ID
  6. Confirm checkout
  7. Verify success message appears
  8. Verify game shows "0 available" or reduced count
- Verify: success message contains game title, game availability is updated
- Test on: chromium, firefox, webkit
- Use data-testid selectors: data-testid="game-card", data-testid="checkout-button"
- Include auto-waiting for checkout API call to complete
```

### Create Test Data Builder

```
Create fluent test data builder for {Entity} in tests/BoardGameCafe.Tests.Unit/Builders/{Entity}Builder.cs:
- Sensible defaults: {list default values}
- Fluent methods: With{Property}(value) for each property
- Special methods: {list convenience methods}
- Build() method returns {Entity} instance
- Follow existing builder pattern in Builders/ folder
```

**Example:**
```
Create fluent test data builder for Order in tests/BoardGameCafe.Tests.Unit/Builders/OrderBuilder.cs:
- Sensible defaults: OrderDate = DateTime.UtcNow, Status = OrderStatus.Pending, empty OrderItems list
- Fluent methods: WithCustomer(Customer), WithOrderItems(List<OrderItem>), WithStatus(OrderStatus), WithDiscount(decimal)
- Special methods: AsCompletedOrder() sets Status to Completed and OrderDate to past, WithSmallTotal() adds 1-2 cheap items, WithLargeTotal() adds 5+ expensive items
- Build() method returns Order instance with calculated totals
- Follow existing builder pattern in ReservationBuilder and GameBuilder
```

---

## Frontend Development

### Create React Component

```
Create React component {ComponentName} in client/src/components/{Category}/{ComponentName}.tsx:
- Props interface: {list props with types}
- State: {list state variables}
- Functionality: {describe behavior}
- UI elements: {describe layout}
- Event handlers: {list handlers}
- Use TypeScript
- Use functional component with hooks
```

**Example:**
```
Create React component GameCard in client/src/components/Games/GameCard.tsx:
- Props interface: game (Game type), onSelect (function)
- State: isHovered (boolean)
- Functionality: displays game title, image, player count, availability
- UI elements: card with image, title, metadata section, "View Details" button
- Event handlers: onClick calls onSelect(game), onMouseEnter/Leave updates isHovered
- Use TypeScript
- Use functional component with useState hook
```

### Create API Hook

```
Create custom React Query hook use{Feature} in client/src/hooks/use{Feature}.ts:
- Endpoint: {HTTP method} {url}
- Query key: ['{resource}', {parameters}]
- Transforms response to: {type}
- Error handling: {describe error cases}
- Return: { data, isLoading, error, refetch }
```

**Example:**
```
Create custom React Query hook useGames in client/src/hooks/useGames.ts:
- Endpoint: GET /api/v1/games with optional filters (category, minPlayers, maxPlayers)
- Query key: ['games', filters]
- Transforms response to: Game[] array
- Error handling: show error toast on network failure
- Return: { games, isLoading, error, refetch }
```

### Create Page Component

```
Create page component {Page}Page in client/src/pages/{Page}Page.tsx:
- Route path: {path}
- Fetch data: {describe data fetching}
- State management: {describe state}
- Sections: {list page sections}
- Navigation: {describe nav}
- Use TypeScript and React Router
```

**Example:**
```
Create page component ReservationsPage in client/src/pages/ReservationsPage.tsx:
- Route path: /reservations
- Fetch data: useReservations hook to get customer's reservations
- State management: selectedDate (useState), filters (useState)
- Sections: header with "My Reservations" title, date filter, reservation list, "New Reservation" button
- Navigation: clicking reservation navigates to /reservations/{id}, "New Reservation" button navigates to /reservations/new
- Use TypeScript and React Router (useNavigate)
```

---

## Database & Migrations

### Create New Entity

```
Create domain entity {Entity} in src/BoardGameCafe.Domain/Entities/{Entity}.cs:
- Properties: {list properties with types}
- Relationships: {describe foreign keys and navigation properties}
- Computed properties: {list computed properties}
- Business rules: {list rules}
```

**Example:**
```
Create domain entity GameSession in src/BoardGameCafe.Domain/Entities/GameSession.cs:
- Properties: Id (Guid), GameId (Guid), ReservationId (Guid), CheckoutTime (DateTime), ReturnTime (DateTime?), IsReturned (bool)
- Relationships: Game (navigation property), Reservation (navigation property)
- Computed properties: DurationHours = (ReturnTime ?? DateTime.UtcNow) - CheckoutTime in hours
- Business rules: CheckoutTime cannot be in the future, ReturnTime must be after CheckoutTime if set
```

### Configure Entity in DbContext

```
Configure {Entity} in BoardGameCafeDbContext.OnModelCreating:
- Table name: {name}
- Primary key: {field}
- Indexes: {list indexed fields}
- Foreign keys: {describe relationships}
- Check constraints: {list constraints}
- Cascade delete: {describe cascade rules}
- Default values: {list defaults}
```

**Example:**
```
Configure GameSession in BoardGameCafeDbContext.OnModelCreating:
- Table name: GameSessions
- Primary key: Id
- Indexes: GameId, ReservationId, CheckoutTime
- Foreign keys: GameId references Games(Id) on delete restrict, ReservationId references Reservations(Id) on delete cascade
- Check constraints: CheckoutTime <= current time, ReturnTime > CheckoutTime if not null
- Cascade delete: cascade on Reservation delete, restrict on Game delete
- Default values: IsReturned = false
```

### Create Migration

```
Create EF Core migration for {description}:
1. Add entity to DbContext
2. Configure in OnModelCreating
3. Run: dotnet ef migrations add {MigrationName} --project src/BoardGameCafe.Api
4. Review generated migration
5. Add seed data if needed
```

**Example:**
```
Create EF Core migration for game sessions tracking:
1. Add DbSet<GameSession> GameSessions to BoardGameCafeDbContext
2. Configure GameSession entity in OnModelCreating with indexes, foreign keys, and constraints
3. Run: dotnet ef migrations add AddGameSessionsTracking --project src/BoardGameCafe.Api
4. Review migration file to ensure CreateTable includes all columns and constraints
5. Add seed data: 3 sample game sessions (1 returned, 2 active)
```

---

## Debugging & Troubleshooting

### Debug Failing Test

```
Analyze why test {TestName} is failing:
1. Show the test code
2. Show the implementation being tested
3. Identify the mismatch between expected and actual
4. Suggest fix with explanation
```

**Example:**
```
Analyze why test CalculateTotal_WithGoldMemberDiscount_Applies15PercentDiscount is failing:
1. Show the test code
2. Show the OrderCalculationService.CalculateTotal implementation
3. Identify why discount is 10% instead of 15%
4. Suggest fix: check that Gold tier discount is set to 0.15m not 0.10m
```

### Fix Build Error

```
Fix build error: {error message}
Context: {describe what you were doing}
Files involved: {list files}
```

**Example:**
```
Fix build error: "CS0246: The type or namespace name 'GameDto' could not be found"
Context: trying to build API project after adding Games endpoints
Files involved: src/BoardGameCafe.Api/Features/Games/GamesEndpoints.cs
```

### Optimize Query Performance

```
Optimize query for {operation}:
- Current query: {show current code}
- Problem: {describe performance issue}
- Expected result: {describe desired outcome}
- Consider: eager loading, indexing, pagination
```

**Example:**
```
Optimize query for loading customer reservations:
- Current query: var reservations = await db.Reservations.Where(r => r.CustomerId == customerId).ToListAsync();
- Problem: N+1 query loading related Table and Customer data
- Expected result: single query with eager loading
- Consider: use .Include() for Table and Customer navigation properties
```

### Explain Error

```
Explain this error and suggest fix:
{paste error message}
Context: {what were you doing when error occurred}
```

**Example:**
```
Explain this error and suggest fix:
"SqliteException: SQLite Error 19: 'FOREIGN KEY constraint failed'."
Context: trying to delete a Game that has GameSession records referencing it
```

---

## Quick Snippets

### Test Method Names
```
{MethodName}_When{Condition}_Should{ExpectedOutcome}
{MethodName}_{Scenario}_{ExpectedResult}
```

### API Endpoint Patterns
```csharp
app.MapGet("/api/v1/{resource}", async (BoardGameCafeDbContext db) => { })
    .WithName("{OperationName}")
    .WithTags("{Resource}")
    .Produces<{Dto}>(200);
```

### FluentAssertions Common
```csharp
result.Should().Be(expected);
result.Should().NotBeNull();
result.Should().BeEquivalentTo(expected);
action.Should().Throw<ArgumentException>();
collection.Should().HaveCount(5);
collection.Should().Contain(x => x.Property == value);
```

### Playwright Selectors
```typescript
await page.getByTestId('game-card').click();
await page.getByRole('button', { name: 'Submit' }).click();
await page.getByText('Success').waitFor();
```

---

## Chaining Prompts for Complex Tasks

For complex implementations, chain multiple prompts in sequence:

**Example: Complete Feature Implementation**

1. **Entity:**
   ```
   Create domain entity Event with properties: Id, Title, Description, StartTime, EndTime, MaxParticipants, EventType (enum: Tournament, GameNight, Workshop)
   ```

2. **DTOs:**
   ```
   Create DTOs for Event: EventDto, CreateEventRequest, UpdateEventRequest with data annotations for validation
   ```

3. **Endpoints:**
   ```
   Create REST API endpoints for Events at /api/v1/events with GET (list), GET /{id}, POST, PUT /{id}, POST /{id}/register
   ```

4. **Unit Tests:**
   ```
   Generate unit tests for EventValidator with validation scenarios: required fields, date range validation, max participants range
   ```

5. **Integration Tests:**
   ```
   Generate integration tests for Events API testing all endpoints with success and error scenarios
   ```

---

## Pro Tips

### Get Better Suggestions

1. **Be specific:** Include types, libraries, patterns
2. **Provide context:** Mention related files and existing patterns
3. **Request examples:** Ask for realistic test data
4. **Iterate:** Refine prompt based on initial output

### Test-Driven Development

```
1. Write failing test for {feature}
2. Generate minimal implementation to make test pass
3. Generate additional tests for edge cases
4. Refactor implementation for clarity
```

### Code Review

```
Review this code for:
- Correctness: does it meet requirements?
- Testing: are edge cases covered?
- Performance: any N+1 queries or inefficiencies?
- Security: any vulnerabilities?
- Style: follows project conventions?
```

---

## Additional Resources

- [Full Copilot Prompts Guide](./copilot-prompts-guide.md)
- [Copilot Agent Assignment Guide](./copilot-agent-assignment-guide.md)
- [Testing Engineer Guide](./testing-engineer-copilot-guide.md)
- [API Testing Guide](./api-testing-guide.md)
- [GitHub Copilot Docs](https://docs.github.com/copilot)

---

**Remember:** These are starting points. Customize prompts for your specific needs and iterate based on Copilot's output!
