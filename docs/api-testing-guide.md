# API Testing Guide

> Comprehensive guide to exploring, testing, and integrating with the Board Game Café REST APIs

## Table of Contents
- [Getting Started with Swagger UI](#getting-started-with-swagger-ui)
- [Downloading the OpenAPI Specification](#downloading-the-openapi-specification)
- [Generating API Client Code](#generating-api-client-code)
- [Writing Integration Tests](#writing-integration-tests)
- [API Endpoints Reference](#api-endpoints-reference)
- [Authentication Patterns](#authentication-patterns)
- [Common Scenarios](#common-scenarios)

---

## Getting Started with Swagger UI

### Accessing Swagger UI

Once the API is running, navigate to:
```
https://localhost:5001/swagger
```

Or if using HTTP:
```
http://localhost:5000/swagger
```

### Features of Swagger UI

1. **Interactive API Documentation**
   - All endpoints listed with HTTP methods (GET, POST, PUT, PATCH, DELETE)
   - Request/response schemas with property types
   - Example values for requests

2. **Try It Out**
   - Click "Try it out" button on any endpoint
   - Fill in parameter values
   - Click "Execute" to send real requests
   - View actual responses with status codes, headers, and body

3. **Request/Response Examples**
   - See sample request bodies
   - View possible response codes (200, 400, 404, 409, etc.)
   - Understand error response formats

### Example: Testing the Games API

1. Expand **GET /api/v1/games**
2. Click **"Try it out"**
3. Set query parameters:
   - `category`: "Strategy"
   - `availableOnly`: true
   - `page`: 1
   - `pageSize`: 10
4. Click **"Execute"**
5. Review response:
   ```json
   [
     {
       "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
       "title": "Catan",
       "publisher": "Catan Studio",
       "minPlayers": 3,
       "maxPlayers": 4,
       "category": "Strategy",
       "copiesOwned": 3,
       "copiesInUse": 1,
       "isAvailable": true,
       "dailyRentalFee": 5.00
     }
   ]
   ```

---

## Downloading the OpenAPI Specification

### Option 1: Via Swagger UI
1. Navigate to https://localhost:5001/swagger
2. Look for the link at the top: `/swagger/v1/swagger.json`
3. Click to download or view the raw JSON

### Option 2: Direct URL
```bash
curl https://localhost:5001/swagger/v1/swagger.json -o openapi.json
```

### Option 3: Using wget
```bash
wget https://localhost:5001/swagger/v1/swagger.json -O openapi.json
```

### OpenAPI Spec Contents

The spec includes:
- API version and metadata
- All endpoint paths and operations
- Request/response schemas
- Data types and validation rules
- Authentication requirements
- Example values

**Example snippet:**
```json
{
  "openapi": "3.0.1",
  "info": {
    "title": "Board Game Café API",
    "version": "v1"
  },
  "paths": {
    "/api/v1/games": {
      "get": {
        "tags": ["Games"],
        "summary": "Get a list of games",
        "parameters": [...],
        "responses": {
          "200": {
            "description": "Success",
            "content": {
              "application/json": {
                "schema": {
                  "type": "array",
                  "items": { "$ref": "#/components/schemas/GameDto" }
                }
              }
            }
          }
        }
      }
    }
  }
}
```

---

## Generating API Client Code

### Using NSwag (C#)

**Install NSwag CLI:**
```bash
dotnet tool install -g NSwag.ConsoleCore
```

**Generate C# Client:**
```bash
nswag openapi2csclient \
  /input:https://localhost:5001/swagger/v1/swagger.json \
  /output:BoardGameCafeApiClient.cs \
  /namespace:BoardGameCafe.Client \
  /generateClientInterfaces:true
```

**Usage:**
```csharp
var client = new BoardGameCafeApiClient("https://localhost:5001", new HttpClient());
var games = await client.GamesAllAsync(category: "Strategy", availableOnly: true);

foreach (var game in games)
{
    Console.WriteLine($"{game.Title} - {game.CopiesOwned - game.CopiesInUse} available");
}
```

---

### Using OpenAPI Generator (TypeScript/JavaScript)

**Install Generator:**
```bash
npm install -g @openapitools/openapi-generator-cli
```

**Generate TypeScript Client:**
```bash
openapi-generator-cli generate \
  -i https://localhost:5001/swagger/v1/swagger.json \
  -g typescript-fetch \
  -o ./src/api-client
```

**Usage:**
```typescript
import { GamesApi, Configuration } from './api-client';

const config = new Configuration({ basePath: 'https://localhost:5001' });
const gamesApi = new GamesApi(config);

const games = await gamesApi.apiV1GamesGet({
  category: 'Strategy',
  availableOnly: true,
  page: 1,
  pageSize: 10
});

games.forEach(game => {
  console.log(`${game.title} - ${game.copiesOwned - game.copiesInUse} available`);
});
```

---

### Using Postman

**Import OpenAPI Spec:**
1. Open Postman
2. Click **Import**
3. Select **Link** tab
4. Paste: `https://localhost:5001/swagger/v1/swagger.json`
5. Click **Import**

Postman will create a collection with all endpoints pre-configured.

---

## Writing Integration Tests

### Setup with WebApplicationFactory

**Install Packages:**
```bash
dotnet add package Microsoft.AspNetCore.Mvc.Testing
dotnet add package FluentAssertions
dotnet add package xUnit
```

**Create Test Class:**
```csharp
using Microsoft.AspNetCore.Mvc.Testing;
using FluentAssertions;
using System.Net.Http.Json;

public class GamesApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    
    public GamesApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }
    
    [Fact]
    public async Task GetGames_ShouldReturn200_WithGameList()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/games");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var games = await response.Content.ReadFromJsonAsync<List<GameDto>>();
        games.Should().NotBeEmpty();
    }
}
```

---

### Testing POST Endpoints

```csharp
[Fact]
public async Task CreateReservation_WithValidData_ShouldReturn201()
{
    // Arrange
    var request = new CreateReservationRequest
    {
        CustomerId = Guid.NewGuid(),
        TableId = Guid.NewGuid(),
        StartTime = DateTime.UtcNow.AddDays(1),
        EndTime = DateTime.UtcNow.AddDays(1).AddHours(2),
        PartySize = 4
    };
    
    // Act
    var response = await _client.PostAsJsonAsync("/api/v1/reservations", request);
    
    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.Created);
    response.Headers.Location.Should().NotBeNull();
    
    var reservation = await response.Content.ReadFromJsonAsync<ReservationDto>();
    reservation.Id.Should().NotBeEmpty();
    reservation.PartySize.Should().Be(4);
}
```

---

### Testing Error Cases

```csharp
[Fact]
public async Task CreateReservation_WithInvalidPartySize_ShouldReturn400()
{
    // Arrange
    var request = new CreateReservationRequest
    {
        CustomerId = Guid.NewGuid(),
        TableId = Guid.NewGuid(),
        StartTime = DateTime.UtcNow.AddDays(1),
        EndTime = DateTime.UtcNow.AddDays(1).AddHours(2),
        PartySize = 0 // Invalid
    };
    
    // Act
    var response = await _client.PostAsJsonAsync("/api/v1/reservations", request);
    
    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    var error = await response.Content.ReadAsStringAsync();
    error.Should().Contain("party size");
}
```

---

## API Endpoints Reference

### Games API

#### **GET /api/v1/games**
Get a filtered list of games

**Query Parameters:**
- `category` (string, optional): Filter by game category
- `minPlayers` (int, optional): Minimum player count
- `maxPlayers` (int, optional): Maximum player count
- `availableOnly` (bool, optional): Show only available games
- `page` (int, default: 1): Page number
- `pageSize` (int, default: 20): Items per page

**Example Request:**
```bash
curl -X GET "https://localhost:5001/api/v1/games?category=Strategy&availableOnly=true&page=1&pageSize=10"
```

**Example Response (200 OK):**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "title": "Catan",
    "publisher": "Catan Studio",
    "minPlayers": 3,
    "maxPlayers": 4,
    "playTimeMinutes": 90,
    "ageRating": 10,
    "complexity": "Medium",
    "category": "Strategy",
    "copiesOwned": 3,
    "copiesInUse": 1,
    "isAvailable": true,
    "dailyRentalFee": 5.00,
    "description": "Trade and build on the island of Catan",
    "imageUrl": "/images/games/catan.jpg"
  }
]
```

---

#### **GET /api/v1/games/{id}**
Get a specific game by ID

**Example Request:**
```bash
curl -X GET "https://localhost:5001/api/v1/games/3fa85f64-5717-4562-b3fc-2c963f66afa6"
```

**Responses:**
- **200 OK**: Game found
- **404 Not Found**: Game does not exist

---

#### **POST /api/v1/games**
Create a new game (admin only)

**Request Body:**
```json
{
  "title": "Wingspan",
  "publisher": "Stonemaier Games",
  "minPlayers": 1,
  "maxPlayers": 5,
  "playTimeMinutes": 70,
  "ageRating": 10,
  "complexity": "Medium",
  "category": "Strategy",
  "copiesOwned": 2,
  "dailyRentalFee": 6.00,
  "description": "Bird-collection engine-building game"
}
```

**Responses:**
- **201 Created**: Game created successfully (with Location header)
- **400 Bad Request**: Invalid data
- **409 Conflict**: Game with same title already exists

---

### Reservations API

#### **GET /api/v1/reservations/availability**
Query available tables for a time slot

**Query Parameters:**
- `date` (date, required): Reservation date
- `startTime` (time, required): Start time
- `duration` (int, required): Duration in hours
- `partySize` (int, required): Number of guests

**Example Request:**
```bash
curl -X GET "https://localhost:5001/api/v1/reservations/availability?date=2025-12-01&startTime=18:00&duration=2&partySize=4"
```

**Example Response (200 OK):**
```json
[
  {
    "tableId": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
    "tableNumber": "T1",
    "seatingCapacity": 4,
    "isWindowSeat": true,
    "hourlyRate": 5.00
  },
  {
    "tableId": "a1b2c3d4-5e6f-7890-abcd-ef1234567890",
    "tableNumber": "T3",
    "seatingCapacity": 6,
    "isWindowSeat": false,
    "hourlyRate": 7.50
  }
]
```

---

#### **POST /api/v1/reservations**
Create a new reservation

**Request Body:**
```json
{
  "customerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "tableId": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
  "startTime": "2025-12-01T18:00:00Z",
  "endTime": "2025-12-01T20:00:00Z",
  "partySize": 4,
  "specialRequests": "Window seat preferred"
}
```

**Responses:**
- **201 Created**: Reservation created
- **400 Bad Request**: Invalid data (past date, party size > capacity)
- **404 Not Found**: Customer or table not found
- **409 Conflict**: Table unavailable (overlapping reservation)

---

#### **PATCH /api/v1/reservations/{id}/check-in**
Check in a reservation

**Example Request:**
```bash
curl -X PATCH "https://localhost:5001/api/v1/reservations/3fa85f64-5717-4562-b3fc-2c963f66afa6/check-in"
```

**Responses:**
- **200 OK**: Check-in successful
- **404 Not Found**: Reservation not found
- **400 Bad Request**: Already checked in or cancelled

---

### Menu API

#### **GET /api/v1/menu**
Get menu items with filtering

**Query Parameters:**
- `category` (string, optional): Coffee, Tea, Snacks, Meals, Desserts, Alcohol
- `isVegetarian` (bool, optional)
- `isVegan` (bool, optional)
- `isGlutenFree` (bool, optional)
- `minPrice` (decimal, optional)
- `maxPrice` (decimal, optional)

**Example Request:**
```bash
curl -X GET "https://localhost:5001/api/v1/menu?category=Coffee&isVegan=true&maxPrice=5.00"
```

**Example Response:**
```json
[
  {
    "id": "8d1c9b4e-3f6a-4c7d-9e2b-1a5f8c3d6e9b",
    "name": "Meeple Mocha",
    "description": "Rich espresso with steamed oat milk",
    "category": "Coffee",
    "price": 4.50,
    "isAvailable": true,
    "preparationTimeMinutes": 5,
    "isVegetarian": true,
    "isVegan": true,
    "isGlutenFree": true,
    "allergenInfo": "Contains oats"
  }
]
```

---

### Orders API

#### **POST /api/v1/orders**
Create a new order

**Request Body:**
```json
{
  "customerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "reservationId": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
  "items": [
    {
      "menuItemId": "8d1c9b4e-3f6a-4c7d-9e2b-1a5f8c3d6e9b",
      "quantity": 2,
      "specialInstructions": "Extra hot"
    }
  ],
  "loyaltyPointsToRedeem": 100
}
```

**Responses:**
- **201 Created**: Order created
- **400 Bad Request**: Invalid menu items or insufficient loyalty points
- **404 Not Found**: Customer or reservation not found

---

### Events API

#### **GET /api/v1/events**
Get upcoming events

**Example Response:**
```json
[
  {
    "id": "9e8d7c6b-5a4f-3e2d-1c0b-9a8e7d6c5b4a",
    "title": "Catan Tournament",
    "description": "Compete for the title of Catan Champion!",
    "startTime": "2025-12-15T18:00:00Z",
    "endTime": "2025-12-15T22:00:00Z",
    "maxParticipants": 16,
    "currentParticipants": 8,
    "eventType": "Tournament",
    "fee": 10.00,
    "imageUrl": "/images/events/catan-tournament.jpg"
  }
]
```

---

#### **POST /api/v1/events/{id}/register**
Register for an event

**Request Body:**
```json
{
  "customerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

**Responses:**
- **201 Created**: Registration successful
- **404 Not Found**: Event not found
- **409 Conflict**: Event full or already registered

---

### Customers API

#### **GET /api/v1/customers/me/loyalty-points**
Get loyalty points balance and tier

**Example Response:**
```json
{
  "currentPoints": 1250,
  "membershipTier": "Silver",
  "discountPercentage": 10,
  "pointsToNextTier": 750,
  "nextTier": "Gold"
}
```

---

#### **GET /api/v1/customers/me/loyalty-history**
Get loyalty points transaction history

**Example Response:**
```json
[
  {
    "id": "a1b2c3d4-5e6f-7890-1234-567890abcdef",
    "points": 50,
    "transactionType": "Earned",
    "description": "Order #1234",
    "transactionDate": "2025-11-15T10:30:00Z"
  },
  {
    "id": "b2c3d4e5-6f78-9012-3456-7890abcdef12",
    "points": -100,
    "transactionType": "Redeemed",
    "description": "Applied to Order #1235",
    "transactionDate": "2025-11-20T14:15:00Z"
  }
]
```

---

## Authentication Patterns

### Current Implementation (Development)

The API currently uses **implicit authentication** for development purposes. In production, you would implement one of the following:

---

### Option 1: JWT Bearer Tokens

**Login Request:**
```bash
curl -X POST https://localhost:5001/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email": "john@example.com", "password": "SecurePass123"}'
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2025-12-01T18:00:00Z"
}
```

**Using Token:**
```bash
curl -X GET https://localhost:5001/api/v1/customers/me \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

---

### Option 2: API Keys

**Request with API Key:**
```bash
curl -X GET https://localhost:5001/api/v1/games \
  -H "X-API-Key: your-api-key-here"
```

---

### Option 3: OAuth 2.0 / OpenID Connect

**Authorization URL:**
```
https://localhost:5001/connect/authorize?
  client_id=your_client_id&
  redirect_uri=https://yourapp.com/callback&
  response_type=code&
  scope=openid profile api
```

---

## Common Scenarios

### Scenario 1: Complete Reservation Flow

```bash
# 1. Check availability
curl -X GET "https://localhost:5001/api/v1/reservations/availability?date=2025-12-01&startTime=18:00&duration=2&partySize=4"

# 2. Create reservation
curl -X POST "https://localhost:5001/api/v1/reservations" \
  -H "Content-Type: application/json" \
  -d '{
    "customerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "tableId": "7c9e6679-7425-40de-944b-e07fc1f90ae7",
    "startTime": "2025-12-01T18:00:00Z",
    "endTime": "2025-12-01T20:00:00Z",
    "partySize": 4
  }'

# 3. On arrival, check in
curl -X PATCH "https://localhost:5001/api/v1/reservations/{reservationId}/check-in"
```

---

### Scenario 2: Place Order with Loyalty Points

```bash
# 1. Get menu items
curl -X GET "https://localhost:5001/api/v1/menu?category=Coffee"

# 2. Check loyalty points balance
curl -X GET "https://localhost:5001/api/v1/customers/me/loyalty-points"

# 3. Create order with points redemption
curl -X POST "https://localhost:5001/api/v1/orders" \
  -H "Content-Type: application/json" \
  -d '{
    "customerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "items": [
      {"menuItemId": "8d1c9b4e-3f6a-4c7d-9e2b-1a5f8c3d6e9b", "quantity": 2}
    ],
    "loyaltyPointsToRedeem": 100
  }'
```

---

### Scenario 3: Event Registration

```bash
# 1. List upcoming events
curl -X GET "https://localhost:5001/api/v1/events"

# 2. Register for event
curl -X POST "https://localhost:5001/api/v1/events/{eventId}/register" \
  -H "Content-Type: application/json" \
  -d '{"customerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"}'

# 3. Cancel registration (if needed)
curl -X DELETE "https://localhost:5001/api/v1/events/{eventId}/register?customerId=3fa85f64-5717-4562-b3fc-2c963f66afa6"
```

---

## Testing Tools Comparison

| Tool | Best For | Pros | Cons |
|------|----------|------|------|
| **Swagger UI** | Quick exploration | Built-in, no setup | Limited automation |
| **Postman** | Manual testing | Collections, environments | Desktop app required |
| **curl** | Scripting | Lightweight, universal | Verbose syntax |
| **xUnit + WebApplicationFactory** | Integration tests | Automated, CI/CD | C# only |
| **Playwright** | E2E testing | Full browser automation | Slower than API tests |

---

## Tips for Effective API Testing

1. **Use Swagger First**: Understand endpoints before writing tests
2. **Test Happy Path**: Ensure basic functionality works
3. **Test Error Cases**: Validate error handling (400, 404, 409)
4. **Test Edge Cases**: Boundary values, null inputs, concurrent requests
5. **Use Realistic Data**: Test with production-like scenarios
6. **Automate**: Write integration tests for regression coverage
7. **Monitor Performance**: Check response times, especially for complex queries
8. **Version APIs**: Use versioning (e.g., /api/v1/) for compatibility

---

## Additional Resources

- [Swagger/OpenAPI Specification](https://swagger.io/specification/)
- [ASP.NET Core API Testing](https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests)
- [Postman Learning Center](https://learning.postman.com/)
- [curl Documentation](https://curl.se/docs/)
- [NSwag Documentation](https://github.com/RicoSuter/NSwag)

---

**Happy Testing! 🚀**
