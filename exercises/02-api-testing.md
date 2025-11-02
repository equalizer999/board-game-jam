# Exercise 2: API Testing with GitHub Copilot

## Overview
This exercise demonstrates how to use GitHub Copilot to generate integration tests for REST API endpoints using Swagger/OpenAPI documentation.

**Duration:** 8 minutes  
**Focus:** REST endpoint testing, HTTP client testing, contract validation

---

## Learning Objectives
- Generate API integration tests from Swagger documentation
- Test HTTP endpoints with WebApplicationFactory
- Validate request/response contracts
- Test error scenarios (400, 404, 409)

---

## Target API: Games REST Endpoints

**Base URL:** `/api/v1/games`  
**Swagger URL:** `https://localhost:5001/swagger`

**Endpoints:**
- `GET /api/v1/games` - List games with filters
- `GET /api/v1/games/{id}` - Get single game
- `POST /api/v1/games` - Create game (admin)
- `PUT /api/v1/games/{id}` - Update game (admin)
- `DELETE /api/v1/games/{id}` - Delete game (admin)

---

## Exercise Steps

### Step 1: Explore Swagger Documentation

1. Start the API:
```bash
cd src/BoardGameCafe.Api
dotnet run
```

2. Open Swagger UI: `https://localhost:5001/swagger`

3. Explore the `Games` endpoints:
   - Click "GET /api/v1/games"
   - View request parameters (category, minPlayers, maxPlayers)
   - View response schema (GameDto)
   - Try "Execute" to see sample data

**TODO:** Take note of:
- Request parameter types and formats
- Response status codes (200, 400, 404)
- DTO property names and types

---

### Step 2: Generate Test Class with WebApplicationFactory

**Copilot Prompt:**
```
// Create integration test class for Games API endpoints
// Use WebApplicationFactory<Program> for in-memory testing
// Use HttpClient for API calls
// Test class name: GamesApiTests
```

**Expected Output:**
```csharp
public class GamesApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    
    public GamesApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }
}
```

**File:** `tests/BoardGameCafe.Tests.Integration/Api/GamesApiTests.cs`

---

### Step 3: Test GET /api/v1/games (List Games)

**TODO:** Use Copilot to generate test for listing games

**Scenarios:**
- Get all games (no filters)
- Filter by category
- Filter by player count
- Multiple filters combined

**Copilot Prompt:**
```
// Generate test: GET /api/v1/games returns 200 OK with list of games
// Assert: status code is 200
// Assert: response contains array of GameDto objects
// Assert: each game has Id, Title, Publisher properties
// Use System.Net.Http.Json for deserialization
```

**Expected Test:**
```csharp
[Fact]
public async Task GetGames_ReturnsOkWithGames()
{
    // Act
    var response = await _client.GetAsync("/api/v1/games");
    
    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
    var games = await response.Content.ReadFromJsonAsync<List<GameDto>>();
    games.Should().NotBeNull();
    games.Should().NotBeEmpty();
}
```

---

### Step 4: Test POST /api/v1/games (Create Game)

**TODO:** Use Copilot to generate test for creating a new game

**Copilot Prompt:**
```
// Generate test: POST /api/v1/games creates new game
// Create CreateGameRequest DTO with valid data
// Assert: status code 201 Created
// Assert: Location header contains new game ID
// Assert: response body contains created game
```

---

### Step 5: Test Validation (400 Bad Request)

**TODO:** Use Copilot to generate tests for request validation

**Invalid scenarios:**
- Title is empty or null
- MinPlayers > MaxPlayers
- Negative CopiesOwned

**Copilot Prompt:**
```
// Generate test: POST /api/v1/games with invalid data returns 400
// Test cases: empty title, MinPlayers > MaxPlayers, negative price
// Assert: status code 400 Bad Request
```

---

## Success Criteria

You've completed this exercise when:
1. ✅ All CRUD operations have passing tests
2. ✅ Error scenarios (400, 404, 409) are covered
3. ✅ Tests use Swagger docs as source of truth
4. ✅ You can generate new API tests quickly with Copilot

---

## Next Steps

Continue to [Exercise 3: UI Testing](./03-ui-testing.md) to learn E2E testing with Playwright.
