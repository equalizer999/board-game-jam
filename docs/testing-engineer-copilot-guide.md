# Testing Engineer's Guide to GitHub Copilot

> A practical guide for QA and testing engineers to leverage GitHub Copilot for test automation, API testing, and quality assurance tasks.

## Table of Contents
- [Quick Start for Testing Engineers](#quick-start-for-testing-engineers)
- [Test Generation Workflows](#test-generation-workflows)
- [API Testing with Copilot](#api-testing-with-copilot)
- [E2E Testing with Playwright](#e2e-testing-with-playwright)
- [Test Data Management](#test-data-management)
- [Bug Hunting & Regression Testing](#bug-hunting--regression-testing)
- [Common Testing Scenarios](#common-testing-scenarios)

---

## Quick Start for Testing Engineers

### Your First Test with Copilot

**Scenario:** You need to test the order calculation logic that applies tax and discounts.

**Step 1:** Open the file you want to test:
- Navigate to `src/BoardGameCafe.Api/Services/OrderCalculationService.cs`

**Step 2:** In GitHub Copilot Chat, type:
```
Generate comprehensive xUnit tests for this OrderCalculationService.
Include tests for:
- Tax calculation (8% on food, 10% on alcohol)
- Member discounts (Bronze 5%, Silver 10%, Gold 15%)
- Loyalty points redemption
- Edge cases: null orders, empty items, zero amounts
Use FluentAssertions and Arrange-Act-Assert pattern
```

**Step 3:** Review and refine:
- Copilot generates test class with multiple test methods
- Review each test for completeness
- Add any missing edge cases
- Run tests: `dotnet test`

**Result:** Complete test suite in 2-3 minutes instead of 30+ minutes of manual work!

---

## Test Generation Workflows

### Unit Testing Workflow

#### 1. Service/Business Logic Testing

**When:** Testing calculation, validation, or business rule logic

**Prompt Template:**
```
Generate xUnit unit tests for {ClassName} in {FilePath}:
- Test all public methods
- Include edge cases: {list specific cases}
- Use [Theory] with [InlineData] for data-driven tests
- Use FluentAssertions for assertions
- Mock dependencies with NSubstitute
- Test naming: MethodName_Scenario_ExpectedResult
```

**Example:**
```
Generate xUnit unit tests for ReservationValidator in tests/BoardGameCafe.Tests.Unit/Validators/ReservationValidatorTests.cs:
- Test ValidateReservation method
- Include edge cases: party size exceeds capacity, past date, overlapping times, duration too long
- Use [Theory] with [InlineData] for different party sizes and table capacities
- Use FluentAssertions: result.Errors.Should().Contain()
- Mock dependencies: IReservationRepository
- Test naming: ValidateReservation_PartySizeExceedsCapacity_ReturnsValidationError
```

#### 2. Domain Entity Testing

**When:** Testing entity validation, computed properties, business rules

**Prompt Template:**
```
Generate unit tests for {Entity} entity:
- Test computed property: {PropertyName}
- Test validation: {list validation rules}
- Test business rules: {list rules}
- Use entity builder for test data setup
```

**Example:**
```
Generate unit tests for Game entity:
- Test computed property: IsAvailable (should be true when CopiesOwned > CopiesInUse)
- Test validation: Title required, MinPlayers <= MaxPlayers, CopiesInUse <= CopiesOwned
- Test business rules: can't checkout more copies than owned, title must be unique
- Use GameBuilder for test data setup
```

### Integration Testing Workflow

#### 1. API Endpoint Testing

**When:** Testing REST API endpoints end-to-end

**Prompt Template:**
```
Generate integration tests for {Feature} API:
- Endpoint: {HTTP method} {url}
- Test scenarios: {list scenarios}
- Expected status codes: {list codes}
- Verify response body: {describe expected data}
- Use WebApplicationFactory
- Seed test database with: {test data}
```

**Example:**
```
Generate integration tests for Reservations API:
- Endpoints: POST /api/v1/reservations, GET /api/v1/reservations/{id}, DELETE /api/v1/reservations/{id}
- Test scenarios: create valid reservation, get existing reservation, get non-existent reservation (404), delete reservation, create with invalid party size (400), create with past date (400), create with table already booked (409)
- Expected status codes: 201, 200, 404, 400, 409
- Verify response body: returned ReservationDto matches input, includes generated ID and confirmation number
- Use WebApplicationFactory<Program>
- Seed test database with: 3 tables, 2 customers, 1 existing reservation
```

#### 2. Database Query Testing

**When:** Verifying data access logic and query performance

**Prompt Template:**
```
Generate integration tests for {Repository/Query}:
- Test method: {MethodName}
- Verify: {expected behavior}
- Test data: {describe seed data}
- Assert: {what to check}
```

**Example:**
```
Generate integration tests for GetAvailableGames query:
- Test method: GetAvailableGamesAsync with filters (category, player count, availability)
- Verify: only returns games with CopiesOwned > CopiesInUse, filters work correctly
- Test data: 10 games with different categories, player counts, some fully checked out
- Assert: result count is correct, all results match filter criteria, unavailable games excluded
```

---

## API Testing with Copilot

### REST API Test Generation

#### Positive Test Cases

**Prompt:**
```
Generate integration test for successful {operation}:
- Endpoint: {method} {url}
- Request body: {example}
- Expected response: 200/201 with {describe data}
- Verify: {what to check in response}
```

**Example:**
```
Generate integration test for successful game creation:
- Endpoint: POST /api/v1/games
- Request body: { "title": "Catan", "publisher": "Catan Studio", "minPlayers": 3, "maxPlayers": 4, "copiesOwned": 5 }
- Expected response: 201 Created with Location header
- Verify: response body contains generated ID, Title is "Catan", Location header points to GET endpoint
```

#### Negative Test Cases

**Prompt:**
```
Generate integration tests for error scenarios:
- 400 Bad Request when: {list validation failures}
- 404 Not Found when: {when resource doesn't exist}
- 409 Conflict when: {when business rule violated}
```

**Example:**
```
Generate integration tests for reservation creation errors:
- 400 Bad Request when: party size is 0, party size exceeds table capacity, start time is in past, duration is 0 hours
- 404 Not Found when: table ID doesn't exist, customer ID doesn't exist
- 409 Conflict when: table already has reservation at that time (include 15-minute buffer)
```

### Swagger/OpenAPI Testing

**Prompt:**
```
Generate test to verify Swagger documentation for {Feature} API:
- Check all endpoints appear in /swagger
- Verify request/response schemas are correct
- Check that XML documentation is included
- Verify example values are present
```

**Example:**
```
Generate test to verify Swagger documentation for Games API:
- Check endpoints: GET /api/v1/games, GET /api/v1/games/{id}, POST /api/v1/games appear
- Verify GameDto schema includes: id, title, publisher, minPlayers, maxPlayers, isAvailable
- Check that XML summary and parameter descriptions are in OpenAPI spec
- Verify example values show realistic game data
```

---

## E2E Testing with Playwright

### Page Object Model Generation

**Prompt:**
```
Create Playwright Page Object Model for {PageName}:
- File: client/tests/e2e/pages/{PageName}Page.ts
- Selectors: {list elements with data-testid}
- Methods: {list user actions}
- Properties: {list page elements}
```

**Example:**
```
Create Playwright Page Object Model for GameCatalogPage:
- File: client/tests/e2e/pages/GameCatalogPage.ts
- Selectors: game-card, filter-category, filter-players, search-input, game-detail-modal
- Methods: goto(), searchGames(query), filterByCategory(category), filterByPlayers(count), clickGameCard(index), closeDetailModal()
- Properties: gameCards, categoryFilter, playerCountFilter, searchBox, detailModal, gameTitle, gameDescription
```

### User Journey Testing

**Prompt:**
```
Create Playwright E2E test for {user journey}:
- Journey: {step-by-step description}
- Starting point: {url}
- User actions: {list actions}
- Assertions: {what to verify at each step}
- Use Page Object: {PageName}Page
```

**Example:**
```
Create Playwright E2E test for reservation booking journey:
- Journey: customer browses games, selects one, makes a reservation to play it
- Starting point: /games
- User actions:
  1. Browse game catalog
  2. Click "Catan" game card
  3. View game details
  4. Click "Reserve Table to Play" button
  5. Select date (tomorrow), party size (4)
  6. Click "Find Available Tables"
  7. Select first available table
  8. Fill in name and email
  9. Click "Confirm Reservation"
- Assertions: games load, game detail shows, available tables appear, success message displays with confirmation number
- Use Page Objects: GameCatalogPage, ReservationPage
```

### Cross-Browser Testing

**Prompt:**
```
Create Playwright test that runs on multiple browsers:
- Test: {describe test}
- Browsers: chromium, firefox, webkit
- Verify consistent behavior across all browsers
- Take screenshots on failure
```

**Example:**
```
Create Playwright test that runs on multiple browsers:
- Test: game filtering and sorting works consistently
- Browsers: chromium, firefox, webkit
- Verify: filter by Strategy category shows only strategy games, sort by player count orders correctly, search finds matching games
- Take screenshots on failure to debug browser-specific issues
```

### Visual Testing

**Prompt:**
```
Create Playwright visual regression test:
- Component: {component name}
- Screenshot: {what to capture}
- Viewport sizes: {list sizes}
- Compare against baseline
```

**Example:**
```
Create Playwright visual regression test:
- Component: Game catalog grid layout
- Screenshot: full page after games load with filters applied
- Viewport sizes: 1920x1080 (desktop), 768x1024 (tablet), 375x667 (mobile)
- Compare against baseline screenshots to detect unintended UI changes
```

---

## Test Data Management

### Test Data Builder Pattern

**Prompt:**
```
Create test data builder for {Entity}:
- Default values: {list sensible defaults}
- Fluent methods: With{Property}() for each property
- Convenience methods: {special configurations}
- Build() returns {Entity} instance
```

**Example:**
```
Create test data builder for Reservation:
- Default values: PartySize=4, StartTime=tomorrow 7pm, Duration=2 hours, Status=Pending
- Fluent methods: WithPartySize(int), WithStartTime(DateTime), WithTable(Table), WithCustomer(Customer)
- Convenience methods: AsPastReservation() sets StartTime to yesterday, AsConfirmed() sets Status=Confirmed, AsLargeParty() sets PartySize=10
- Build() returns Reservation instance with all relationships set
```

### Test Fixtures and Seed Data

**Prompt:**
```
Create test fixture with seed data for {scenario}:
- Entities: {list entities to create}
- Relationships: {describe connections}
- Variations: {different states/types}
- Export as: {format}
```

**Example:**
```
Create test fixture with seed data for reservation conflict testing:
- Entities: 5 tables (different sizes), 3 customers, 8 reservations at various times
- Relationships: each reservation links to table and customer
- Variations: some tables fully booked, some available, some with gaps, different party sizes
- Export as: JSON array for E2E tests, builder methods for integration tests
```

### Realistic Test Data

**Prompt:**
```
Generate realistic test data for {entity}:
- Quantity: {number}
- Variety: {describe variations}
- Realistic values: {describe realistic data}
```

**Example:**
```
Generate realistic test data for menu items:
- Quantity: 25 items
- Variety: mix of Coffee (5), Tea (3), Snacks (5), Meals (7), Desserts (3), Alcohol (2)
- Realistic values: game-themed names ("Catan Cappuccino", "Meeple Mocha"), prices $2-15, preparation times 5-20 minutes, allergen info, dietary flags
```

---

## Bug Hunting & Regression Testing

### Reproducing Bugs with Tests

**Prompt:**
```
Create failing test that reproduces bug:
- Bug description: {describe the bug}
- Steps to reproduce: {list steps}
- Expected behavior: {what should happen}
- Actual behavior: {what actually happens}
- Write test that fails demonstrating the bug
```

**Example:**
```
Create failing test that reproduces bug:
- Bug description: loyalty points not deducted when order is cancelled
- Steps to reproduce: 
  1. Customer has 1000 points
  2. Place order earning 50 points (now 1050)
  3. Cancel the order
  4. Check points balance
- Expected behavior: points should be 1000 (50 points reversed)
- Actual behavior: points remain at 1050
- Write test that fails: CreateOrder_ThenCancel_ShouldReversePoints
```

### Regression Test Suites

**Prompt:**
```
Create regression test suite for {bug or feature}:
- Test all scenarios that were broken
- Test edge cases that could break again
- Test related functionality that might be affected
- Mark with [Trait("Category", "Regression")]
```

**Example:**
```
Create regression test suite for reservation time overlap bug:
- Test scenarios: exact time overlap, 5-minute overlap, 14-minute overlap (within buffer), 16-minute overlap (outside buffer), back-to-back reservations
- Test edge cases: midnight boundary, daylight saving time transition, different time zones
- Test related: reservation updates don't create conflicts, cancellation frees up time slot
- Mark with [Trait("Category", "Regression")] for easy filtering
```

### Bug Fix Verification

**Prompt:**
```
After fixing {bug}, create tests to verify:
- Bug is actually fixed
- Fix doesn't break existing functionality
- Edge cases around the fix work correctly
```

**Example:**
```
After fixing double discount bug, create tests to verify:
- Member discount and loyalty points redemption don't stack incorrectly
- Negative totals are impossible
- Each discount type works correctly in isolation
- Combined discounts never exceed 100%
- Order totals are always >= 0
```

---

## Common Testing Scenarios

### Testing Async Operations

**Prompt:**
```
Test async method {MethodName}:
- Verify it completes successfully
- Test with delays/timeouts
- Verify concurrent calls don't cause issues
- Check cancellation token handling
```

**Example:**
```
Test async method ProcessOrderAsync:
- Verify order status changes from Pending to Processing to Completed
- Test with simulated 5-second processing delay
- Verify 10 concurrent orders don't cause race conditions on inventory
- Check that cancellation token stops processing and rolls back changes
```

### Testing Error Handling

**Prompt:**
```
Test error handling in {Component}:
- Null inputs: {what should happen}
- Invalid inputs: {what should happen}
- Exception scenarios: {list exceptions}
- Verify error messages are user-friendly
```

**Example:**
```
Test error handling in OrderCalculationService:
- Null order: throw ArgumentNullException with message "Order cannot be null"
- Order with no items: throw InvalidOperationException "Order must have at least one item"
- Negative quantity: throw ArgumentException "Quantity must be positive"
- Item not found: throw NotFoundException "Menu item {id} not found"
- Verify error messages are clear and actionable
```

### Testing Authorization

**Prompt:**
```
Test authorization for {endpoint}:
- Anonymous users: should return 401
- Authenticated non-members: should return 403
- Authenticated members: should return 200
- Admin users: should have full access
```

**Example:**
```
Test authorization for DELETE /api/v1/games/{id}:
- Anonymous users: should return 401 Unauthorized
- Authenticated customers: should return 403 Forbidden
- Authenticated staff: should return 403 Forbidden
- Admin users: should return 200 OK and delete game
```

### Testing Pagination

**Prompt:**
```
Test pagination for {endpoint}:
- First page returns correct items
- Page size is respected
- Last page has remaining items
- Out of range page returns empty
- Total count is correct
```

**Example:**
```
Test pagination for GET /api/v1/games?page=1&pageSize=10:
- First page (page=1) returns games 1-10
- Page size of 10 is respected
- If 25 total games, page 3 returns games 21-25
- Page 10 (out of range) returns empty array
- Response includes totalCount=25, pageCount=3, currentPage
```

---

## Copilot Testing Best Practices

### 1. Be Specific About Test Frameworks

❌ **Vague:** "Write tests for the service"

✅ **Specific:** "Generate xUnit tests with FluentAssertions for OrderCalculationService"

### 2. Describe Expected Behavior

❌ **Vague:** "Test the validation"

✅ **Specific:** "Test ReservationValidator ensures party size is between 1 and table capacity, returns error message 'Party size exceeds table capacity of {capacity}' when exceeded"

### 3. Request Edge Cases

❌ **Basic:** "Test CreateOrder"

✅ **Comprehensive:** "Test CreateOrder with scenarios: valid order, empty items list, null customer, zero quantities, negative prices, duplicate items, non-existent menu items"

### 4. Use Existing Patterns

❌ **Generic:** "Create tests"

✅ **Pattern-aware:** "Create tests following the pattern in GameRepositoryTests.cs with similar setup and assertions"

### 5. Include Verification Steps

❌ **Incomplete:** "Test the endpoint"

✅ **Complete:** "Test POST /api/v1/games endpoint verifying: 201 status, Location header, response body contains ID, database contains new record, audit log created"

---

## Quick Reference: Common Test Assertions

### FluentAssertions (C#)
```csharp
// Equality
result.Should().Be(expected);
result.Should().BeEquivalentTo(expected);

// Null checks
result.Should().NotBeNull();
result.Should().BeNull();

// Collections
collection.Should().HaveCount(5);
collection.Should().Contain(item);
collection.Should().NotContain(item);
collection.Should().BeEmpty();
collection.Should().OnlyContain(x => x.IsActive);

// Exceptions
action.Should().Throw<ArgumentException>()
    .WithMessage("*invalid*");
action.Should().NotThrow();

// Comparisons
number.Should().BeGreaterThan(0);
number.Should().BeLessThanOrEqualTo(100);
text.Should().StartWith("Hello");
text.Should().Contain("world");

// Booleans
flag.Should().BeTrue();
flag.Should().BeFalse();
```

### Playwright (TypeScript)
```typescript
// Visibility
await expect(element).toBeVisible();
await expect(element).toBeHidden();

// Text content
await expect(element).toHaveText('Expected text');
await expect(element).toContainText('partial');

// Count
await expect(elements).toHaveCount(5);

// Attributes
await expect(element).toHaveAttribute('href', '/games');
await expect(element).toHaveClass('active');

// State
await expect(button).toBeEnabled();
await expect(button).toBeDisabled();
await expect(checkbox).toBeChecked();

// URL
await expect(page).toHaveURL(/.*games/);
await expect(page).toHaveTitle('Board Games');
```

---

## Measuring Test Effectiveness

### Code Coverage Prompts

```
Analyze code coverage for {ClassName}:
- Show which lines are covered
- Identify uncovered branches
- Suggest tests for uncovered code
- Target: >80% line coverage, >70% branch coverage
```

### Test Quality Check

```
Review these tests for quality:
- Do they test behavior, not implementation?
- Are edge cases covered?
- Are tests independent and isolated?
- Do they have clear assertions?
- Are test names descriptive?
```

### Performance Testing

```
Create performance test for {operation}:
- Measure execution time for {n} iterations
- Verify time is under {threshold} ms
- Test with {data size} records
- Identify bottlenecks
```

---

## Troubleshooting with Copilot

### When Tests Fail

```
This test is failing: {test name}
Error: {error message}
Expected: {expected value}
Actual: {actual value}

Help me:
1. Understand why it's failing
2. Identify the root cause
3. Fix the implementation or test
```

### When Tests Are Flaky

```
Test {name} sometimes passes, sometimes fails:
- Failure rate: ~30%
- Error when it fails: {error}
- Suspect issue: {timing/race condition/external dependency}

Help me:
1. Identify source of non-determinism
2. Make test deterministic
3. Add appropriate waits or retries
```

---

## Resources for Testing Engineers

- [Copilot Quick Prompts](./copilot-quick-prompts.md) - Copy-paste ready prompts
- [Copilot Prompts Guide](./copilot-prompts-guide.md) - Detailed prompt engineering
- [API Testing Guide](./api-testing-guide.md) - REST API testing strategies
- [Bug Hunting Guide](./bug-hunting-guide.md) - Finding and fixing bugs
- [Workshop Exercises](../exercises/README.md) - Hands-on practice

---

## Success Metrics

Track your effectiveness with Copilot:

- **Time Saved:** How much faster are you writing tests?
- **Coverage Improved:** Has code coverage increased?
- **Bugs Found:** Are tests catching more issues?
- **Confidence:** Do you feel more confident in test quality?

**Target Goals:**
- 5x faster test writing
- >80% code coverage
- 90%+ of bugs caught by tests
- High confidence in regression suites

---

**Remember:** Copilot is your testing assistant. The more specific and context-rich your prompts, the better the tests it generates. Happy testing! 🧪
