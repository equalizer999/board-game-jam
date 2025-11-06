# Exercise 2: API Endpoint Creation with GitHub Copilot

## Overview
This exercise demonstrates how to use GitHub Copilot to create a new REST API endpoint for game recommendations, including validation, business logic, and integration tests.

**Duration:** 12 minutes  
**Focus:** API endpoint creation, request/response DTOs, validation, testing

---

## Learning Objectives
- Create a new REST API endpoint with Copilot
- Design request/response DTOs
- Implement validation rules
- Generate integration tests for the new endpoint
- Test the endpoint with Swagger

---

## Target API: Game Recommendations Endpoint

**New Endpoint to Create:** `POST /api/v1/games/recommendations`

**Purpose:** Recommend board games based on player preferences

**Input:**
```json
{
  "playerCount": 4,
  "preferredCategory": "Strategy",
  "maxDuration": 120,
  "experienceLevel": "Intermediate"
}
```

**Output:**
```json
{
  "recommendations": [
    {
      "gameId": "guid",
      "title": "Catan",
      "category": "Strategy",
      "minPlayers": 3,
      "maxPlayers": 4,
      "averageDuration": 90,
      "matchScore": 95
    }
  ],
  "totalMatches": 5
}
```

**Validation Rules:**
- `playerCount`: Required, 1-12
- `preferredCategory`: Optional, must match existing categories
- `maxDuration`: Optional, 15-480 minutes
- `experienceLevel`: Optional, one of: Beginner, Intermediate, Advanced

---

## Exercise Steps

### Step 1: Create Request DTO

**TODO:** Use Copilot to create the request model

**File:** `src/BoardGameCafe.Api/Features/Games/GetRecommendationsRequest.cs`

**Copilot Prompt:**
```csharp
// Create request DTO for game recommendations endpoint
// Properties: PlayerCount (int, required), PreferredCategory (string, optional),
//   MaxDuration (int, optional), ExperienceLevel (string, optional)
// Add data annotations for validation
// PlayerCount: Range 1-12, Required
// MaxDuration: Range 15-480
// ExperienceLevel: must be one of Beginner, Intermediate, Advanced
```

**Expected Output:**
```csharp
public class GetRecommendationsRequest
{
    [Required]
    [Range(1, 12)]
    public int PlayerCount { get; set; }
    
    public string? PreferredCategory { get; set; }
    
    [Range(15, 480)]
    public int? MaxDuration { get; set; }
    
    [RegularExpression("Beginner|Intermediate|Advanced")]
    public string? ExperienceLevel { get; set; }
}
```

---

### Step 2: Create Response DTO

**TODO:** Use Copilot to create the response model

**File:** `src/BoardGameCafe.Api/Features/Games/GetRecommendationsResponse.cs`

**Copilot Prompt:**
```csharp
// Create response DTO for game recommendations
// Include: list of recommended games with match score
// Properties: GameId, Title, Category, MinPlayers, MaxPlayers, AverageDuration, MatchScore
// Add TotalMatches property
```

**Expected Output:**
```csharp
public class GetRecommendationsResponse
{
    public List<GameRecommendation> Recommendations { get; set; } = new();
    public int TotalMatches { get; set; }
}

public class GameRecommendation
{
    public Guid GameId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int MinPlayers { get; set; }
    public int MaxPlayers { get; set; }
    public int AverageDuration { get; set; }
    public int MatchScore { get; set; } // 0-100
}
```

---

### Step 3: Create the Endpoint

**TODO:** Use Copilot to create the endpoint in GamesEndpoints.cs

**File:** `src/BoardGameCafe.Api/Features/Games/GamesEndpoints.cs`

**Copilot Prompt:**
```csharp
// Add POST endpoint /api/v1/games/recommendations
// Accept GetRecommendationsRequest, return GetRecommendationsResponse
// TODO: Implementation should filter games by criteria and calculate match scores
// Return 200 OK with recommendations
// Return 400 Bad Request if validation fails
```

**Expected Method Signature:**
```csharp
public static async Task<IResult> GetRecommendations(
    GetRecommendationsRequest request,
    IGameRepository repository,
    ILogger<GamesEndpoints> logger)
{
    // TODO: Implementation goes here
    return Results.Ok(new GetRecommendationsResponse());
}
```

---

### Step 4: Implement Recommendation Logic

**TODO:** Use Copilot to implement the filtering and scoring logic

**Copilot Prompt:**
```csharp
// Implement game recommendation logic:
// 1. Get all games from repository
// 2. Filter by player count (game.MinPlayers <= playerCount <= game.MaxPlayers)
// 3. Filter by category if provided
// 4. Filter by duration if provided
// 5. Calculate match score based on how well game matches criteria
// 6. Sort by match score descending
// 7. Return top 10 recommendations
```

**Example Implementation:**
```csharp
var allGames = await repository.GetAllAsync();

var filtered = allGames
    .Where(g => g.MinPlayers <= request.PlayerCount && g.MaxPlayers >= request.PlayerCount)
    .Where(g => string.IsNullOrEmpty(request.PreferredCategory) || 
                g.Category == request.PreferredCategory)
    .Where(g => !request.MaxDuration.HasValue || 
                g.AverageDuration <= request.MaxDuration.Value);

var recommendations = filtered
    .Select(g => new GameRecommendation
    {
        GameId = g.Id,
        Title = g.Title,
        Category = g.Category,
        MinPlayers = g.MinPlayers,
        MaxPlayers = g.MaxPlayers,
        AverageDuration = g.AverageDuration,
        MatchScore = CalculateMatchScore(g, request)
    })
    .OrderByDescending(r => r.MatchScore)
    .Take(10)
    .ToList();

return Results.Ok(new GetRecommendationsResponse
{
    Recommendations = recommendations,
    TotalMatches = recommendations.Count
});
```

---

### Step 5: Add Match Score Calculation

**TODO:** Use Copilot to create the scoring algorithm

**Copilot Prompt:**
```csharp
// Create CalculateMatchScore method
// Score 0-100 based on:
// - Player count match (30 points): higher if playerCount is in optimal range
// - Category match (30 points): full points if exact match
// - Duration match (20 points): higher if duration is close to maxDuration
// - Experience level match (20 points): if provided and matches
```

---

### Step 6: Test with Swagger

1. Start the API:
```bash
cd src/BoardGameCafe.Api
dotnet run
```

2. Open Swagger UI: `http://localhost:5000/swagger`

3. Find the new endpoint: `POST /api/v1/games/recommendations`

4. Click "Try it out"

5. Test with sample data:
```json
{
  "playerCount": 4,
  "preferredCategory": "Strategy",
  "maxDuration": 120
}
```

6. Verify:
   - Status code 200
   - Response has recommendations array
   - Games match the criteria
   - Match scores are reasonable (0-100)

---

### Step 7: Test Validation Errors

**TODO:** Use Copilot to test edge cases in Swagger

Test these scenarios:
- `playerCount: 0` → Should return 400 (below range)
- `playerCount: 20` → Should return 400 (above range)
- `maxDuration: 500` → Should return 400 (above range)
- `experienceLevel: "Expert"` → Should return 400 (invalid value)

---

### Step 8: Create Integration Tests

**TODO:** Use Copilot to generate tests for the new endpoint

**File:** `src/BoardGameCafe.Tests.Integration/Features/Games/GamesEndpointsTests.cs`

**Copilot Prompt:**
```csharp
// Generate integration test for POST /api/v1/games/recommendations
// Test: GetRecommendations_ValidRequest_ReturnsMatchingGames
// Create request with playerCount=4, category="Strategy"
// Assert: 200 OK
// Assert: recommendations list is not empty
// Assert: all games have minPlayers <= 4 <= maxPlayers
// Assert: all games have category "Strategy"
```

**Expected Test:**
```csharp
[Fact]
public async Task GetRecommendations_ValidRequest_ReturnsMatchingGames()
{
    // Arrange
    var request = new GetRecommendationsRequest
    {
        PlayerCount = 4,
        PreferredCategory = "Strategy",
        MaxDuration = 120
    };
    
    // Act
    var response = await _client.PostAsJsonAsync("/api/v1/games/recommendations", request);
    
    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.OK);
    var result = await response.Content.ReadFromJsonAsync<GetRecommendationsResponse>();
    result.Should().NotBeNull();
    result!.Recommendations.Should().NotBeEmpty();
    result.Recommendations.Should().AllSatisfy(g => 
    {
        g.MinPlayers.Should().BeLessOrEqualTo(4);
        g.MaxPlayers.Should().BeGreaterOrEqualTo(4);
        g.Category.Should().Be("Strategy");
    });
}
```

---

### Step 9: Test Validation in Integration Tests

**TODO:** Use Copilot to generate validation tests

**Copilot Prompt:**
```csharp
// Generate integration test for validation
// Test: GetRecommendations_InvalidPlayerCount_ReturnsBadRequest
// Send request with playerCount = 0
// Assert: 400 Bad Request
```

---

## Verification Checklist

- [ ] GetRecommendationsRequest DTO created with validation attributes
- [ ] GetRecommendationsResponse DTO created
- [ ] Endpoint added to GamesEndpoints.cs
- [ ] Recommendation logic filters by player count, category, duration
- [ ] Match score calculation implemented
- [ ] Endpoint works in Swagger UI
- [ ] Validation errors return 400 Bad Request
- [ ] Integration tests pass

---

## Success Criteria

You've completed this exercise when:
1. ✅ New endpoint created and working
2. ✅ Validation rules enforced
3. ✅ Match scoring algorithm implemented
4. ✅ Tested manually in Swagger
5. ✅ Integration tests pass
6. ✅ You can create new API endpoints with Copilot

---

## Common Copilot Tips

### Creating DTOs
✅ **Good Prompt:**
```
// Create request DTO with properties: PlayerCount (required, 1-12),
//   PreferredCategory (optional string), MaxDuration (optional, 15-480)
// Add data annotation validation attributes
```

❌ **Vague Prompt:**
```
// Create a request class
```

### Implementing Business Logic
```
// Implement filtering logic:
// 1. Filter games where minPlayers <= playerCount <= maxPlayers
// 2. If category provided, filter by exact category match
// 3. If maxDuration provided, filter where game duration <= maxDuration
```

### Calculating Scores
```
// Create scoring algorithm that returns 0-100:
// - 30 points for player count in optimal range
// - 30 points for category exact match
// - 20 points for duration close to preferred
// - 20 points for experience level match
```

---

## Extension Challenges

### Challenge 1: Add Complexity Filtering
Add complexity level to the recommendation criteria:
```csharp
public string? ComplexityLevel { get; set; } // Light, Medium, Heavy
```

### Challenge 2: Improve Match Score
Enhance scoring to consider:
- Game popularity (rating count)
- Average rating
- Availability (copies owned vs reserved)

### Challenge 3: Add Caching
Cache recommendation results for 5 minutes:
```csharp
// Use IMemoryCache to cache results
// Cache key based on request parameters
```

---

## Next Steps

Continue to [Exercise 3: UI Testing](./03-playwright-test-writing.md) to learn E2E testing with Playwright.
