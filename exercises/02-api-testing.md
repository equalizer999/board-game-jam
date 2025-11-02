# Exercise 2: Web API Testing with Swagger and GitHub Copilot

**Duration:** 8-12 minutes  
**Difficulty:** Beginner to Intermediate  
**Focus:** Using Copilot to explore and test REST APIs via Swagger/OpenAPI documentation

---

## Learning Objectives

By the end of this exercise, you will:
- Use Swagger UI to explore API endpoints
- Write integration tests for REST APIs
- Use Copilot to generate HTTP request tests
- Validate API contracts (request/response schemas)
- Test error scenarios (400, 404, 409 status codes)

---

## Prerequisites

- ✅ Backend API running (`dotnet run` in `src/BoardGameCafe.Api`)
- ✅ Swagger UI accessible at `https://localhost:5001/swagger`
- ✅ Issue #10 (Games API) and Issue #4 (Reservations API) completed
- ✅ Integration test project created

---

## Scenario

The Board Game Café has REST APIs for managing games, reservations, and orders. You need to write integration tests to ensure:
- Endpoints return correct status codes
- Response data matches expected schema
- Validation rules are enforced
- Error handling works properly

---

## Part 1: Explore APIs with Swagger

### TODO 1.1: Review Games API

**Your task**: Open Swagger and explore the Games API endpoints.

**Steps**:
1. Navigate to `https://localhost:5001/swagger`
2. Expand **Games** section
3. Review available endpoints:
   - `GET /api/v1/games` - List games with filters
   - `GET /api/v1/games/{id}` - Get single game
   - `POST /api/v1/games` - Create game
   - `PUT /api/v1/games/{id}` - Update game
   - `DELETE /api/v1/games/{id}` - Delete game

**Observation Questions**:
- What query parameters are available for filtering? (`category`, `minPlayers`, `maxPlayers`, `isAvailable`)
- What properties are in the `GameDto` schema?
- What status codes can each endpoint return?

### TODO 1.2: Test Endpoint Manually

**Your task**: Use Swagger UI to test `GET /api/v1/games`.

**Steps**:
1. Click **GET /api/v1/games**
2. Click **Try it out**
3. Leave filters empty
4. Click **Execute**

**Expected Response**:
```json
{
  "games": [
    {
      "id": "uuid-here",
      "title": "Catan",
      "publisher": "Catan Studio",
      "minPlayers": 3,
      "maxPlayers": 4,
      "category": "Strategy",
      "isAvailable": true,
      "dailyRentalFee": 5.00
    },
    // ... more games
  ],
  "totalCount": 5,
  "page": 1,
  "pageSize": 10
}
```

**Status Code**: `200 OK`

---

## Part 2: Write Integration Tests

### TODO 2.1: Test GET All Games

**Your task**: Use Copilot to generate an integration test for listing games.

**Copilot Prompt**:
```csharp
// Integration test for GET /api/v1/games
// Setup: Seed database with 5 games
// Act: Send GET request to /api/v1/games
// Assert: Status 200, response contains 5 games
// Use WebApplicationFactory and HttpClient
```

**Expected Test Structure**:
```csharp
public class GamesApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    
    public GamesApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }
    
    [Fact]
    public async Task GetGames_WithNoFilters_ReturnsAllGames()
    {
        // Arrange
        // (Database seeded via migrations)
        
        // Act
        var response = await _client.GetAsync("/api/v1/games");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<GamesListResponse>(content);
        
        result.Games.Should().HaveCountGreaterThan(0);
        result.Games.Should().AllSatisfy(g => 
        {
            g.Title.Should().NotBeNullOrEmpty();
            g.MinPlayers.Should().BeGreaterThan(0);
            g.MaxPlayers.Should().BeGreaterOrEqualTo(g.MinPlayers);
        });
    }
}
```

**Run Test**:
```bash
dotnet test --filter "FullyQualifiedName~GamesApiTests"
```

### TODO 2.2: Test Query Filters

**Your task**: Test filtering games by category.

**Copilot Prompt**:
```csharp
// Test: GET /api/v1/games?category=Strategy
// Assert: All returned games have category "Strategy"
// Assert: Count matches expected (seed data has 2 strategy games)
```

**Expected**:
```csharp
[Fact]
public async Task GetGames_FilterByCategory_ReturnsOnlyMatchingGames()
{
    // Arrange
    var category = "Strategy";
    
    // Act
    var response = await _client.GetAsync($"/api/v1/games?category={category}");
    
    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
    
    var result = await response.Content.ReadFromJsonAsync<GamesListResponse>();
    
    result.Games.Should().NotBeEmpty();
    result.Games.Should().AllSatisfy(g => 
        g.Category.Should().Be(category));
}
```

### TODO 2.3: Test Player Count Filter

**Your task**: Use Copilot to test the `minPlayers` and `maxPlayers` filters.

**Copilot Prompt**:
```csharp
// Test: GET /api/v1/games?minPlayers=2&maxPlayers=4
// Assert: All games support 2-4 players
// Theory with InlineData for different player count ranges
```

**Practice**: Generate tests for:
- `minPlayers=2&maxPlayers=2` (exactly 2 players)
- `minPlayers=4&maxPlayers=8` (4-8 players)
- `minPlayers=1&maxPlayers=10` (solo to party)

---

## Part 3: Test POST Endpoint

### TODO 3.1: Test Creating a Game

**Your task**: Write test for creating a new game.

**Copilot Prompt**:
```csharp
// Integration test: POST /api/v1/games
// Arrange: CreateGameRequest with valid data
// Act: POST to endpoint
// Assert: 
//   - Status 201 Created
//   - Location header set
//   - Response contains created game with Id
//   - Can GET the created game by Id
```

**Expected**:
```csharp
[Fact]
public async Task CreateGame_WithValidData_ReturnsCreatedGame()
{
    // Arrange
    var request = new CreateGameRequest
    {
        Title = "Wingspan",
        Publisher = "Stonemaier Games",
        MinPlayers = 1,
        MaxPlayers = 5,
        PlayTimeMinutes = 70,
        AgeRating = 10,
        Complexity = 2.5m,
        Category = "Strategy",
        CopiesOwned = 3,
        DailyRentalFee = 8.00m,
        Description = "Beautiful bird-themed engine-building game",
        ImageUrl = "https://example.com/wingspan.jpg"
    };
    
    // Act
    var response = await _client.PostAsJsonAsync("/api/v1/games", request);
    
    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.Created);
    response.Headers.Location.Should().NotBeNull();
    
    var game = await response.Content.ReadFromJsonAsync<GameDto>();
    game.Id.Should().NotBeEmpty();
    game.Title.Should().Be("Wingspan");
    game.IsAvailable.Should().BeTrue();
    
    // Verify can retrieve by Id
    var getResponse = await _client.GetAsync(response.Headers.Location);
    getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
}
```

### TODO 3.2: Test Validation

**Your task**: Test that invalid requests are rejected.

**Copilot Prompt**:
```csharp
// Test: POST /api/v1/games with invalid data
// Scenarios:
//   - Title missing → 400 Bad Request
//   - MinPlayers > MaxPlayers → 400 Bad Request
//   - Negative price → 400 Bad Request
//   - PlayTime = 0 → 400 Bad Request
// Use Theory with InlineData
```

**Expected Pattern**:
```csharp
[Theory]
[InlineData("", "Missing title")]
[InlineData(null, "Null title")]
public async Task CreateGame_WithInvalidTitle_ReturnsBadRequest(
    string title, 
    string reason)
{
    // Arrange
    var request = new CreateGameRequest
    {
        Title = title,
        Publisher = "Publisher",
        MinPlayers = 2,
        MaxPlayers = 4
        // ... other required fields
    };
    
    // Act
    var response = await _client.PostAsJsonAsync("/api/v1/games", request);
    
    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
}
```

---

## Part 4: Test Reservations API

### TODO 4.1: Test Availability Query

**Your task**: Test the reservation availability endpoint.

**Reference Swagger**: `GET /api/v1/reservations/availability`

**Copilot Prompt**:
```csharp
// Integration test: GET /api/v1/reservations/availability
// Query params: date=2025-01-15, time=18:00, duration=2, partySize=4
// Assert: Returns list of available tables
// Assert: All tables have seatingCapacity >= partySize
// Assert: No conflicting reservations
```

**Expected**:
```csharp
[Fact]
public async Task GetAvailability_ForValidDateTime_ReturnsAvailableTables()
{
    // Arrange
    var date = DateTime.UtcNow.AddDays(7).ToString("yyyy-MM-dd");
    var time = "18:00";
    var duration = 2;
    var partySize = 4;
    
    // Act
    var response = await _client.GetAsync(
        $"/api/v1/reservations/availability?date={date}&time={time}&duration={duration}&partySize={partySize}");
    
    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
    
    var tables = await response.Content.ReadFromJsonAsync<List<AvailableTableDto>>();
    
    tables.Should().NotBeEmpty();
    tables.Should().AllSatisfy(t => 
        t.SeatingCapacity.Should().BeGreaterOrEqualTo(partySize));
}
```

### TODO 4.2: Test Reservation Conflict

**Your task**: Test that double-booking is prevented.

**Copilot Prompt**:
```csharp
// Integration test: POST /api/v1/reservations (conflict scenario)
// Setup: Create reservation for Table 1, 2024-01-15 18:00-20:00
// Act: Try to create overlapping reservation (same table, 19:00-21:00)
// Assert: 409 Conflict status
// Assert: Error message mentions "already reserved"
```

---

## Part 5: Test Error Scenarios

### TODO 5.1: Test 404 Not Found

**Your task**: Test getting a non-existent game.

**Copilot Prompt**:
```csharp
// Test: GET /api/v1/games/{nonExistentId}
// Assert: 404 Not Found
// Assert: Error message helpful
```

**Expected**:
```csharp
[Fact]
public async Task GetGame_WithNonExistentId_ReturnsNotFound()
{
    // Arrange
    var nonExistentId = Guid.NewGuid();
    
    // Act
    var response = await _client.GetAsync($"/api/v1/games/{nonExistentId}");
    
    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.NotFound);
}
```

### TODO 5.2: Test 400 Bad Request

**Your task**: Test invalid query parameters.

**Copilot Prompt**:
```csharp
// Test: GET /api/v1/games?minPlayers=-1
// Assert: 400 Bad Request (negative player count invalid)
```

---

## Part 6: Test Response Schema Validation

### TODO 6.1: Validate OpenAPI Contract

**Your task**: Use Copilot to verify response matches OpenAPI schema.

**Copilot Prompt**:
```csharp
// Test: Validate GameDto schema from OpenAPI spec
// Properties required: Id, Title, Publisher, MinPlayers, MaxPlayers, IsAvailable
// Properties optional: Description, ImageUrl
// Use FluentAssertions to verify schema compliance
```

**Bonus**: Use libraries like `Swashbuckle.AspNetCore.Annotations` or `NJsonSchema` to programmatically validate against OpenAPI spec.

---

## Reflection Questions

1. **Swagger Exploration**: How helpful was Swagger for understanding API contracts before writing tests?

2. **Integration vs Unit**: What's the difference between integration tests (this exercise) and unit tests (Exercise 1)?

3. **Status Codes**: Which HTTP status codes did you test? Are there others you should cover (401, 403, 500)?

4. **Test Data**: How did you handle test data setup? Did you use in-memory database? Seed data?

5. **Copilot Effectiveness**: Did Copilot generate correct HTTP client code? Did it know ASP.NET testing patterns?

---

## Success Criteria

- [ ] All integration tests pass
- [ ] Tested successful scenarios (200, 201 status codes)
- [ ] Tested error scenarios (400, 404, 409)
- [ ] Validated response schemas
- [ ] Tests isolated and repeatable
- [ ] Used WebApplicationFactory for test server

---

## Bonus Challenges

### Challenge 1: Test PUT Endpoint
```csharp
// Test: PUT /api/v1/games/{id}
// Scenarios: Update game title, update price, partial update
```

### Challenge 2: Test DELETE Endpoint
```csharp
// Test: DELETE /api/v1/games/{id}
// Scenarios: Soft delete (IsAvailable=false), cannot delete if in use
```

### Challenge 3: Test Pagination
```csharp
// Test: GET /api/v1/games?page=1&pageSize=10
// Test: GET /api/v1/games?page=2&pageSize=5
// Assert: Correct page data returned
```

### Challenge 4: Performance Test
```csharp
// Test: GET /api/v1/games should respond within 200ms
// Use Stopwatch or BenchmarkDotNet
```

---

## Next Steps

- Exercise 3: UI Testing with Playwright
- Exercise 4: Bug Hunting and Regression Tests

---

**Instructor Notes**:
- Demo Swagger UI exploration first
- Show how to use "Try it out" feature
- Explain WebApplicationFactory setup
- Discuss test database strategies (in-memory vs SQLite)
- Review HTTP status code meanings
- Compare integration vs unit testing trade-offs
