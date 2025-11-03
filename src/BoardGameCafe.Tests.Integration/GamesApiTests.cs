using BoardGameCafe.Api.Data;
using BoardGameCafe.Api.Features.Games;
using BoardGameCafe.Domain;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace BoardGameCafe.Tests.Integration;

public class GamesApiTests : IClassFixture<ReservationsApiTestFixture>, IAsyncLifetime
{
    private readonly ReservationsApiTestFixture _factory;
    private HttpClient _client = null!;

    public GamesApiTests(ReservationsApiTestFixture factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        _client = _factory.CreateClient();

        // Clean up any existing data from previous tests
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BoardGameCafeDbContext>();
        db.Games.RemoveRange(db.Games);
        await db.SaveChangesAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetGames_WithNoGames_ReturnsEmptyList()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/games");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var games = await response.Content.ReadFromJsonAsync<List<GameDto>>();
        games.Should().NotBeNull();
        games.Should().BeEmpty();
    }

    [Fact]
    public async Task GetGames_WithGames_ReturnsAllGames()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BoardGameCafeDbContext>();

        var game1 = new Game
        {
            Id = Guid.NewGuid(),
            Title = "Catan",
            Publisher = "Catan Studio",
            MinPlayers = 3,
            MaxPlayers = 4,
            PlayTimeMinutes = 90,
            AgeRating = 10,
            Complexity = 2.5m,
            Category = GameCategory.Strategy,
            CopiesOwned = 3,
            CopiesInUse = 1,
            DailyRentalFee = 5.00m
        };

        var game2 = new Game
        {
            Id = Guid.NewGuid(),
            Title = "Pandemic",
            Publisher = "Z-Man Games",
            MinPlayers = 2,
            MaxPlayers = 4,
            PlayTimeMinutes = 45,
            AgeRating = 8,
            Complexity = 2.0m,
            Category = GameCategory.Cooperative,
            CopiesOwned = 2,
            CopiesInUse = 0,
            DailyRentalFee = 4.50m
        };

        db.Games.AddRange(game1, game2);
        await db.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync("/api/v1/games");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var games = await response.Content.ReadFromJsonAsync<List<GameDto>>();
        games.Should().NotBeNull();
        games.Should().HaveCount(2);
        games![0].Title.Should().Be("Catan"); // Ordered by title
        games[1].Title.Should().Be("Pandemic");
    }

    [Fact]
    public async Task GetGames_WithCategoryFilter_ReturnsFilteredGames()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BoardGameCafeDbContext>();

        db.Games.AddRange(
            new Game { Title = "Catan", Publisher = "Test", MinPlayers = 3, MaxPlayers = 4, PlayTimeMinutes = 90, AgeRating = 10, Complexity = 2.5m, Category = GameCategory.Strategy, CopiesOwned = 1, DailyRentalFee = 5.00m },
            new Game { Title = "Pandemic", Publisher = "Test", MinPlayers = 2, MaxPlayers = 4, PlayTimeMinutes = 45, AgeRating = 8, Complexity = 2.0m, Category = GameCategory.Cooperative, CopiesOwned = 1, DailyRentalFee = 4.50m }
        );
        await db.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync("/api/v1/games?Category=0"); // Strategy

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var games = await response.Content.ReadFromJsonAsync<List<GameDto>>();
        games.Should().NotBeNull();
        games.Should().HaveCount(1);
        games![0].Title.Should().Be("Catan");
        games[0].Category.Should().Be("Strategy");
    }

    [Fact]
    public async Task GetGames_WithPlayerCountFilter_ReturnsGamesMatchingPlayerCount()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BoardGameCafeDbContext>();

        db.Games.AddRange(
            new Game { Title = "Game 2-4 Players", Publisher = "Test", MinPlayers = 2, MaxPlayers = 4, PlayTimeMinutes = 60, AgeRating = 10, Complexity = 2.0m, Category = GameCategory.Family, CopiesOwned = 1, DailyRentalFee = 5.00m },
            new Game { Title = "Game 4-6 Players", Publisher = "Test", MinPlayers = 4, MaxPlayers = 6, PlayTimeMinutes = 90, AgeRating = 12, Complexity = 3.0m, Category = GameCategory.Strategy, CopiesOwned = 1, DailyRentalFee = 6.00m }
        );
        await db.SaveChangesAsync();

        // Act - filter for games that support exactly 3 players
        var response = await _client.GetAsync("/api/v1/games?PlayerCount=3");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var games = await response.Content.ReadFromJsonAsync<List<GameDto>>();
        games.Should().NotBeNull();
        games.Should().HaveCount(1);
        games![0].Title.Should().Be("Game 2-4 Players"); // Min 2, Max 4 supports 3 players
    }

    [Fact]
    public async Task GetGames_WithAvailableOnlyFilter_ReturnsOnlyAvailableGames()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BoardGameCafeDbContext>();

        db.Games.AddRange(
            new Game { Title = "Available Game", Publisher = "Test", MinPlayers = 2, MaxPlayers = 4, PlayTimeMinutes = 60, AgeRating = 10, Complexity = 2.0m, Category = GameCategory.Family, CopiesOwned = 2, CopiesInUse = 0, DailyRentalFee = 5.00m },
            new Game { Title = "Unavailable Game", Publisher = "Test", MinPlayers = 2, MaxPlayers = 4, PlayTimeMinutes = 60, AgeRating = 10, Complexity = 2.0m, Category = GameCategory.Family, CopiesOwned = 1, CopiesInUse = 1, DailyRentalFee = 5.00m }
        );
        await db.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync("/api/v1/games?AvailableOnly=true");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var games = await response.Content.ReadFromJsonAsync<List<GameDto>>();
        games.Should().NotBeNull();
        games.Should().HaveCount(1);
        games![0].Title.Should().Be("Available Game");
        games[0].IsAvailable.Should().BeTrue();
    }

    [Fact]
    public async Task GetGames_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BoardGameCafeDbContext>();

        for (int i = 1; i <= 15; i++)
        {
            db.Games.Add(new Game
            {
                Title = $"Game {i:D2}",
                Publisher = "Test",
                MinPlayers = 2,
                MaxPlayers = 4,
                PlayTimeMinutes = 60,
                AgeRating = 10,
                Complexity = 2.0m,
                Category = GameCategory.Family,
                CopiesOwned = 1,
                DailyRentalFee = 5.00m
            });
        }
        await db.SaveChangesAsync();

        // Act - Get page 2 with page size 10
        var response = await _client.GetAsync("/api/v1/games?Page=2&PageSize=10");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var games = await response.Content.ReadFromJsonAsync<List<GameDto>>();
        games.Should().NotBeNull();
        games.Should().HaveCount(5); // 15 total, page 2 should have 5
        games![0].Title.Should().Be("Game 11");
    }

    [Fact]
    public async Task GetGameById_WithValidId_ReturnsGame()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BoardGameCafeDbContext>();

        var gameId = Guid.NewGuid();
        var game = new Game
        {
            Id = gameId,
            Title = "Catan",
            Publisher = "Catan Studio",
            MinPlayers = 3,
            MaxPlayers = 4,
            PlayTimeMinutes = 90,
            AgeRating = 10,
            Complexity = 2.5m,
            Category = GameCategory.Strategy,
            CopiesOwned = 3,
            CopiesInUse = 1,
            DailyRentalFee = 5.00m,
            Description = "Classic strategy game",
            ImageUrl = "http://example.com/catan.jpg"
        };
        db.Games.Add(game);
        await db.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"/api/v1/games/{gameId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await response.Content.ReadFromJsonAsync<GameDto>();
        dto.Should().NotBeNull();
        dto!.Id.Should().Be(gameId);
        dto.Title.Should().Be("Catan");
        dto.Publisher.Should().Be("Catan Studio");
        dto.IsAvailable.Should().BeTrue();
    }

    [Fact]
    public async Task GetGameById_WithInvalidId_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync($"/api/v1/games/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateGame_WithValidData_ReturnsCreated()
    {
        // Arrange
        var request = new CreateGameRequest
        {
            Title = "New Game",
            Publisher = "Test Publisher",
            MinPlayers = 2,
            MaxPlayers = 4,
            PlayTimeMinutes = 60,
            AgeRating = 10,
            Complexity = 2.5m,
            Category = 0, // Strategy
            CopiesOwned = 3,
            DailyRentalFee = 5.00m,
            Description = "A new game",
            ImageUrl = "http://example.com/game.jpg"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/games", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();

        var dto = await response.Content.ReadFromJsonAsync<GameDto>();
        dto.Should().NotBeNull();
        dto!.Title.Should().Be("New Game");
        dto.CopiesInUse.Should().Be(0); // Should start at 0
        dto.IsAvailable.Should().BeTrue();
    }

    [Fact]
    public async Task CreateGame_WithInvalidPlayerCount_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateGameRequest
        {
            Title = "Invalid Game",
            Publisher = "Test",
            MinPlayers = 5,
            MaxPlayers = 3, // Invalid: min > max
            PlayTimeMinutes = 60,
            AgeRating = 10,
            Complexity = 2.0m,
            Category = 0,
            CopiesOwned = 1,
            DailyRentalFee = 5.00m
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/games", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateGame_WithDuplicateTitle_ReturnsConflict()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BoardGameCafeDbContext>();

        var existingGame = new Game
        {
            Title = "Existing Game",
            Publisher = "Test",
            MinPlayers = 2,
            MaxPlayers = 4,
            PlayTimeMinutes = 60,
            AgeRating = 10,
            Complexity = 2.0m,
            Category = GameCategory.Family,
            CopiesOwned = 1,
            DailyRentalFee = 5.00m
        };
        db.Games.Add(existingGame);
        await db.SaveChangesAsync();

        var request = new CreateGameRequest
        {
            Title = "existing game", // Case-insensitive duplicate
            Publisher = "Test",
            MinPlayers = 2,
            MaxPlayers = 4,
            PlayTimeMinutes = 60,
            AgeRating = 10,
            Complexity = 2.0m,
            Category = 0,
            CopiesOwned = 1,
            DailyRentalFee = 5.00m
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/games", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task UpdateGame_WithValidData_ReturnsUpdated()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BoardGameCafeDbContext>();

        var gameId = Guid.NewGuid();
        var game = new Game
        {
            Id = gameId,
            Title = "Original Title",
            Publisher = "Original Publisher",
            MinPlayers = 2,
            MaxPlayers = 4,
            PlayTimeMinutes = 60,
            AgeRating = 10,
            Complexity = 2.0m,
            Category = GameCategory.Family,
            CopiesOwned = 2,
            CopiesInUse = 1,
            DailyRentalFee = 5.00m
        };
        db.Games.Add(game);
        await db.SaveChangesAsync();

        var request = new UpdateGameRequest
        {
            Title = "Updated Title",
            Publisher = "Updated Publisher",
            MinPlayers = 3,
            MaxPlayers = 6,
            PlayTimeMinutes = 90,
            AgeRating = 12,
            Complexity = 3.0m,
            Category = 0, // Strategy
            CopiesOwned = 3,
            CopiesInUse = 1,
            DailyRentalFee = 6.00m
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/games/{gameId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await response.Content.ReadFromJsonAsync<GameDto>();
        dto.Should().NotBeNull();
        dto!.Title.Should().Be("Updated Title");
        dto.Publisher.Should().Be("Updated Publisher");
        dto.MaxPlayers.Should().Be(6);
    }

    [Fact]
    public async Task UpdateGame_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var request = new UpdateGameRequest
        {
            Title = "Game",
            Publisher = "Test",
            MinPlayers = 2,
            MaxPlayers = 4,
            PlayTimeMinutes = 60,
            AgeRating = 10,
            Complexity = 2.0m,
            Category = 0,
            CopiesOwned = 1,
            CopiesInUse = 0,
            DailyRentalFee = 5.00m
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/games/{Guid.NewGuid()}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateGame_WithCopiesInUseExceedingOwned_ReturnsBadRequest()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BoardGameCafeDbContext>();

        var gameId = Guid.NewGuid();
        var game = new Game
        {
            Id = gameId,
            Title = "Game",
            Publisher = "Test",
            MinPlayers = 2,
            MaxPlayers = 4,
            PlayTimeMinutes = 60,
            AgeRating = 10,
            Complexity = 2.0m,
            Category = GameCategory.Family,
            CopiesOwned = 2,
            CopiesInUse = 1,
            DailyRentalFee = 5.00m
        };
        db.Games.Add(game);
        await db.SaveChangesAsync();

        var request = new UpdateGameRequest
        {
            Title = "Game",
            Publisher = "Test",
            MinPlayers = 2,
            MaxPlayers = 4,
            PlayTimeMinutes = 60,
            AgeRating = 10,
            Complexity = 2.0m,
            Category = 0,
            CopiesOwned = 2,
            CopiesInUse = 3, // Invalid: exceeds CopiesOwned
            DailyRentalFee = 5.00m
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/games/{gameId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteGame_WithValidId_ReturnsNoContent()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BoardGameCafeDbContext>();

        var gameId = Guid.NewGuid();
        var game = new Game
        {
            Id = gameId,
            Title = "Game to Delete",
            Publisher = "Test",
            MinPlayers = 2,
            MaxPlayers = 4,
            PlayTimeMinutes = 60,
            AgeRating = 10,
            Complexity = 2.0m,
            Category = GameCategory.Family,
            CopiesOwned = 2,
            CopiesInUse = 0, // No copies in use
            DailyRentalFee = 5.00m
        };
        db.Games.Add(game);
        await db.SaveChangesAsync();

        // Act
        var response = await _client.DeleteAsync($"/api/v1/games/{gameId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify deletion with a fresh scope
        using var verifyScope = _factory.Services.CreateScope();
        var verifyDb = verifyScope.ServiceProvider.GetRequiredService<BoardGameCafeDbContext>();
        var deletedGame = await verifyDb.Games.FindAsync(gameId);
        deletedGame.Should().BeNull();
    }

    [Fact]
    public async Task DeleteGame_WithInvalidId_ReturnsNotFound()
    {
        // Act
        var response = await _client.DeleteAsync($"/api/v1/games/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteGame_WithCopiesInUse_ReturnsConflict()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BoardGameCafeDbContext>();

        var gameId = Guid.NewGuid();
        var game = new Game
        {
            Id = gameId,
            Title = "Game In Use",
            Publisher = "Test",
            MinPlayers = 2,
            MaxPlayers = 4,
            PlayTimeMinutes = 60,
            AgeRating = 10,
            Complexity = 2.0m,
            Category = GameCategory.Family,
            CopiesOwned = 2,
            CopiesInUse = 1, // Has copies in use
            DailyRentalFee = 5.00m
        };
        db.Games.Add(game);
        await db.SaveChangesAsync();

        // Act
        var response = await _client.DeleteAsync($"/api/v1/games/{gameId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}
