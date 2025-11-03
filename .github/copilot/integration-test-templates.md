# Integration Test Prompt Templates

Use these templates when generating integration tests for API endpoints and database operations.

## Basic API Endpoint Test

```
Generate integration test for {HTTP method} {endpoint}:
- Use WebApplicationFactory<Program>
- Seed database with: {test data}
- Send request: {describe request body}
- Expected response: {status code} with {response body}
- Verify database: {what to check}
```

**Example:**
```
Generate integration test for POST /api/v1/games:
- Use WebApplicationFactory<Program>
- Seed database with: empty Games table
- Send request: { "title": "Catan", "publisher": "Catan Studio", "minPlayers": 3, "maxPlayers": 4 }
- Expected response: 201 Created with Location header, response body contains generated ID
- Verify database: Games table contains 1 record with title "Catan"
```

## Success Scenario Test

```
Generate integration test for successful {operation}:
- Endpoint: {method} {url}
- Setup: {describe initial state}
- Request: {describe request}
- Assert: {list assertions}
```

**Example:**
```
Generate integration test for successful reservation creation:
- Endpoint: POST /api/v1/reservations
- Setup: seed 1 customer, 1 available table, no existing reservations
- Request: { "customerId": "{id}", "tableId": "{id}", "partySize": 4, "startTime": "tomorrow 7pm", "duration": 2 }
- Assert: 
  - Response 201 Created
  - Location header = "/api/v1/reservations/{newId}"
  - Response body contains confirmation number
  - Database has 1 reservation record
  - Table status unchanged (still available)
```

## Error Scenario Tests

```
Generate integration tests for error scenarios on {endpoint}:
- 400 Bad Request: {list validation failures}
- 404 Not Found: {list resource not found cases}
- 409 Conflict: {list business rule violations}
For each: verify error response contains helpful message
```

**Example:**
```
Generate integration tests for error scenarios on POST /api/v1/reservations:
- 400 Bad Request: 
  - Party size is 0 or negative
  - Start time is in the past
  - Duration is 0 or exceeds 4 hours
  - Party size exceeds table seating capacity
- 404 Not Found:
  - Customer ID doesn't exist
  - Table ID doesn't exist
- 409 Conflict:
  - Table already booked at that time (overlapping reservation)
For each: verify error response contains field name and descriptive message
```

## CRUD Operation Test Suite

```
Generate complete CRUD integration tests for {Resource}:
- Create: POST {endpoint} with valid data returns 201
- Read single: GET {endpoint}/{id} returns 200 with correct data
- Read list: GET {endpoint} returns 200 with array of items
- Update: PUT {endpoint}/{id} with changes returns 200
- Delete: DELETE {endpoint}/{id} returns 204
- Get after delete: returns 404
```

**Example:**
```
Generate complete CRUD integration tests for Games:
- Create: POST /api/v1/games with game data returns 201, Location header, generated ID
- Read single: GET /api/v1/games/{id} returns 200 with matching game data
- Read list: GET /api/v1/games returns 200 with array, verify count matches seeded data
- Update: PUT /api/v1/games/{id} with title change returns 200, verify title updated
- Delete: DELETE /api/v1/games/{id} returns 204, game removed from database
- Get after delete: GET /api/v1/games/{id} returns 404
```

## Filtering and Pagination Tests

```
Generate integration tests for {endpoint} filtering:
- Filter by {field1}: verify results match criteria
- Filter by {field2}: verify results match criteria
- Pagination: test page size, page number, total count
- Combined filters: verify AND logic works
- No results: verify empty array returned
```

**Example:**
```
Generate integration tests for GET /api/v1/games filtering:
- Filter by category=Strategy: verify all results have Category="Strategy"
- Filter by minPlayers=2&maxPlayers=4: verify results where 2 is between MinPlayers and MaxPlayers
- Filter by isAvailable=true: verify CopiesOwned > CopiesInUse for all results
- Pagination: test page=1&pageSize=5 returns 5 items, page=2 returns next 5
- Combined filters: category=Strategy&isAvailable=true returns only available strategy games
- No results: category=InvalidCategory returns empty array with 200 status
```

## Database Relationship Tests

```
Generate integration test verifying relationships for {Entity}:
- Test cascade delete: when {parent} deleted, {children} are also deleted
- Test restrict delete: when {parent} has {children}, delete fails with 409
- Test eager loading: navigation properties are populated
- Test query includes: related data is loaded in single query
```

**Example:**
```
Generate integration test verifying relationships for Customer:
- Test cascade delete: when Customer deleted, all Reservations and Orders are deleted
- Test restrict delete: when Customer has active GameSessions, delete fails with 409
- Test eager loading: GET /api/v1/customers/{id} includes Reservations and Orders
- Test query includes: single database query loads customer with all related data (verify with SQL logging)
```

## Transaction and Concurrency Tests

```
Generate integration test for concurrent {operation}:
- Setup: {describe initial state}
- Action: two simultaneous requests for {operation}
- Expected: {describe expected outcome}
- Verify: database state is consistent, no race conditions
```

**Example:**
```
Generate integration test for concurrent event registration:
- Setup: event with MaxParticipants=10, CurrentParticipants=9 (1 spot left)
- Action: two customers simultaneously POST /api/v1/events/{id}/register
- Expected: one gets 201 Created, one gets 409 Conflict "Event is full"
- Verify: database has exactly 10 registrations, not 11 (no race condition)
```

## Authentication/Authorization Tests

```
Generate integration tests for {endpoint} authorization:
- Anonymous request: returns 401 Unauthorized
- Authenticated as {Role1}: {expected behavior}
- Authenticated as {Role2}: {expected behavior}
- Token expired: returns 401
```

**Example:**
```
Generate integration tests for DELETE /api/v1/games/{id} authorization:
- Anonymous request: returns 401 Unauthorized
- Authenticated as Customer: returns 403 Forbidden
- Authenticated as Staff: returns 403 Forbidden
- Authenticated as Admin: returns 204 No Content, game deleted
- Token expired: returns 401 with "Token expired" message
```

## Data Validation Tests

```
Generate data validation integration tests for {endpoint}:
- Required fields: omit each required field, verify 400 with field name in error
- Field length: exceed max length for string fields, verify 400
- Range validation: send values outside valid range, verify 400
- Format validation: send invalid formats (email, URL, date), verify 400
```

**Example:**
```
Generate data validation integration tests for POST /api/v1/customers:
- Required fields: omit FirstName, LastName, Email individually → 400 "Field is required"
- Field length: 
  - FirstName with 201 chars (max 200) → 400 "exceeds maximum length"
  - Email with 101 chars → 400
- Range validation: 
  - Phone with 5 digits (min 10) → 400 "must be at least 10 digits"
- Format validation: 
  - Email = "notanemail" → 400 "invalid email format"
```

## Idempotency Tests

```
Generate idempotency test for {operation}:
- Send same request twice
- Verify: both return same response
- Verify: second request doesn't create duplicate data
- Check: database contains only one record
```

**Example:**
```
Generate idempotency test for POST /api/v1/orders:
- Send same order request twice (same customerId, items, timestamp)
- Verify: first returns 201 Created, second returns 200 OK or 409 Conflict
- Verify: second request doesn't create duplicate order
- Check: database contains exactly 1 order, not 2
```

---

## Test Setup Helpers

### Database Seeding Template
```csharp
private async Task SeedDatabaseAsync(WebApplicationFactory<Program> factory)
{
    using var scope = factory.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<BoardGameCafeDbContext>();
    
    // Add test data
    db.{Entity}.AddRange(
        new {Entity} { /* properties */ },
        new {Entity} { /* properties */ }
    );
    
    await db.SaveChangesAsync();
}
```

### HTTP Client Helper Template
```csharp
private HttpClient CreateClient(WebApplicationFactory<Program> factory)
{
    return factory.CreateClient(new WebApplicationFactoryClientOptions
    {
        AllowAutoRedirect = false
    });
}

private async Task<HttpResponseMessage> PostAsJsonAsync<T>(
    HttpClient client, string url, T content)
{
    return await client.PostAsync(url, 
        JsonContent.Create(content, mediaType: MediaTypeHeaderValue.Parse("application/json")));
}
```

### Response Verification Template
```csharp
private async Task VerifyResponseAsync<T>(
    HttpResponseMessage response,
    HttpStatusCode expectedStatus,
    Action<T> verifyContent = null)
{
    response.StatusCode.Should().Be(expectedStatus);
    
    if (verifyContent != null && response.Content != null)
    {
        var content = await response.Content.ReadFromJsonAsync<T>();
        verifyContent(content);
    }
}
```

---

## Usage Tips

1. **Seed realistic data** - helps catch real-world issues
2. **Test both success and failure paths** - don't just test happy path
3. **Verify database state** - ensure data is persisted correctly
4. **Check response headers** - Location, Content-Type, Cache-Control
5. **Use meaningful assertions** - verify actual values, not just "not null"

## Common Patterns

### WebApplicationFactory Setup
```csharp
public class GamesEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    
    public GamesEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }
    
    [Fact]
    public async Task GetGames_ReturnsSuccessStatusCode()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/v1/games");
        response.EnsureSuccessStatusCode();
    }
}
```

### In-Memory Database
```csharp
var factory = new WebApplicationFactory<Program>()
    .WithWebHostBuilder(builder =>
    {
        builder.ConfigureServices(services =>
        {
            // Remove existing DbContext
            var descriptor = services.Single(
                d => d.ServiceType == typeof(DbContextOptions<BoardGameCafeDbContext>));
            services.Remove(descriptor);
            
            // Add in-memory database
            services.AddDbContext<BoardGameCafeDbContext>(options =>
            {
                options.UseSqlite("DataSource=:memory:");
            });
        });
    });
```
