using BoardGameCafe.Api.Data;
using BoardGameCafe.Api.Features.Customers;
using BoardGameCafe.Domain;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;

namespace BoardGameCafe.Tests.Integration;

public class CustomersApiTests : IClassFixture<ReservationsApiTestFixture>, IAsyncLifetime
{
    private readonly ReservationsApiTestFixture _factory;
    private HttpClient _client = null!;
    private Guid _testCustomerId;
    private Guid _testGameId;

    public CustomersApiTests(ReservationsApiTestFixture factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        _client = _factory.CreateClient();

        // Clean up and seed test data
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Clean existing data in correct order (from most dependent to least)
        db.GameSessions.RemoveRange(db.GameSessions);
        db.Reservations.RemoveRange(db.Reservations);
        db.Tables.RemoveRange(db.Tables);
        db.OrderItems.RemoveRange(db.OrderItems);
        db.Orders.RemoveRange(db.Orders);
        db.LoyaltyPointsHistory.RemoveRange(db.LoyaltyPointsHistory);
        
        // Clear many-to-many relationship using EF operations
        var existingCustomers = await db.Customers.Include(c => c.FavoriteGames).ToListAsync();
        foreach (var existingCustomer in existingCustomers)
        {
            existingCustomer.FavoriteGames.Clear();
        }
        
        db.Customers.RemoveRange(db.Customers);
        db.Games.RemoveRange(db.Games);
        await db.SaveChangesAsync();

        // Create test customer
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            Phone = "555-1234",
            MembershipTier = MembershipTier.Bronze,
            LoyaltyPoints = 250,
            JoinedDate = DateTime.UtcNow.AddMonths(-6),
            TotalVisits = 10
        };
        db.Customers.Add(customer);

        // Create test game for favorites
        var game = new Game
        {
            Id = Guid.NewGuid(),
            Title = "Test Game",
            Publisher = "Test Publisher",
            MinPlayers = 2,
            MaxPlayers = 4,
            PlayTimeMinutes = 60,
            AgeRating = 10,
            Complexity = 2.5m,
            Category = GameCategory.Strategy,
            CopiesOwned = 2,
            CopiesInUse = 0,
            DailyRentalFee = 5.00m
        };
        db.Games.Add(game);

        await db.SaveChangesAsync();

        _testCustomerId = customer.Id;
        _testGameId = game.Id;
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetCustomerProfile_ReturnsCustomerData()
    {
        // Act
        var response = await _client.GetAsync($"/api/v1/customers/me?customerId={_testCustomerId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var customer = await response.Content.ReadFromJsonAsync<CustomerDto>();
        customer.Should().NotBeNull();
        customer!.Id.Should().Be(_testCustomerId);
        customer.FirstName.Should().Be("John");
        customer.LastName.Should().Be("Doe");
        customer.Email.Should().Be("test@example.com");
        customer.MembershipTier.Should().Be("Bronze");
        customer.LoyaltyPoints.Should().Be(250);
        customer.TotalVisits.Should().Be(10);
    }

    [Fact]
    public async Task GetCustomerProfile_WithInvalidId_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync($"/api/v1/customers/me?customerId={Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateCustomerProfile_UpdatesData()
    {
        // Arrange
        var request = new UpdateCustomerRequest
        {
            FirstName = "Jane",
            LastName = "Smith",
            Phone = "555-9999"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/customers/me?customerId={_testCustomerId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var customer = await response.Content.ReadFromJsonAsync<CustomerDto>();
        customer.Should().NotBeNull();
        customer!.FirstName.Should().Be("Jane");
        customer.LastName.Should().Be("Smith");
        customer.Phone.Should().Be("555-9999");
        customer.Email.Should().Be("test@example.com"); // Email should not change
    }

    [Fact]
    public async Task GetLoyaltyPoints_ReturnsTierInfo()
    {
        // Act
        var response = await _client.GetAsync($"/api/v1/customers/me/loyalty-points?customerId={_testCustomerId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var loyaltyPoints = await response.Content.ReadFromJsonAsync<LoyaltyPointsDto>();
        loyaltyPoints.Should().NotBeNull();
        loyaltyPoints!.CurrentPoints.Should().Be(250);
        loyaltyPoints.CurrentTier.Should().Be("Bronze");
        loyaltyPoints.CurrentDiscount.Should().Be(0.05m);
        loyaltyPoints.NextTier.Should().Be("Silver");
        loyaltyPoints.PointsToNextTier.Should().Be(250); // 500 - 250
    }

    [Fact]
    public async Task GetLoyaltyPoints_WithGoldTier_ShowsNoNextTier()
    {
        // Arrange - Update customer to Gold tier
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var customer = await db.Customers.FindAsync(_testCustomerId);
        customer!.LoyaltyPoints = 2500;
        customer.MembershipTier = MembershipTier.Gold;
        await db.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"/api/v1/customers/me/loyalty-points?customerId={_testCustomerId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var loyaltyPoints = await response.Content.ReadFromJsonAsync<LoyaltyPointsDto>();
        loyaltyPoints.Should().NotBeNull();
        loyaltyPoints!.CurrentPoints.Should().Be(2500);
        loyaltyPoints.CurrentTier.Should().Be("Gold");
        loyaltyPoints.CurrentDiscount.Should().Be(0.15m);
        loyaltyPoints.NextTier.Should().BeNull();
        loyaltyPoints.PointsToNextTier.Should().BeNull();
    }

    [Fact]
    public async Task GetLoyaltyHistory_ReturnsEmptyList_WhenNoTransactions()
    {
        // Act
        var response = await _client.GetAsync($"/api/v1/customers/me/loyalty-history?customerId={_testCustomerId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var history = await response.Content.ReadFromJsonAsync<List<LoyaltyTransactionDto>>();
        history.Should().NotBeNull();
        history.Should().BeEmpty();
    }

    [Fact]
    public async Task GetLoyaltyHistory_ReturnsTransactions_WhenExists()
    {
        // Arrange - Add some transactions
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var transaction1 = new LoyaltyPointsHistory
        {
            Id = Guid.NewGuid(),
            CustomerId = _testCustomerId,
            PointsChange = 100,
            Description = "Points earned from order",
            TransactionDate = DateTime.UtcNow.AddDays(-2),
            TransactionType = LoyaltyTransactionType.Earned
        };

        var transaction2 = new LoyaltyPointsHistory
        {
            Id = Guid.NewGuid(),
            CustomerId = _testCustomerId,
            PointsChange = -50,
            Description = "Points redeemed",
            TransactionDate = DateTime.UtcNow.AddDays(-1),
            TransactionType = LoyaltyTransactionType.Redeemed
        };

        db.LoyaltyPointsHistory.AddRange(transaction1, transaction2);
        await db.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"/api/v1/customers/me/loyalty-history?customerId={_testCustomerId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var history = await response.Content.ReadFromJsonAsync<List<LoyaltyTransactionDto>>();
        history.Should().NotBeNull();
        history.Should().HaveCount(2);
        // Should be ordered by date descending
        history![0].PointsChange.Should().Be(-50);
        history[0].TransactionType.Should().Be("Redeemed");
        history[1].PointsChange.Should().Be(100);
        history[1].TransactionType.Should().Be("Earned");
    }

    [Fact]
    public async Task AddFavoriteGame_AddsGameToFavorites()
    {
        // Act
        var response = await _client.PostAsync($"/api/v1/customers/me/favorites?customerId={_testCustomerId}&gameId={_testGameId}", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify it was added
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var customer = await db.Customers
            .Include(c => c.FavoriteGames)
            .FirstOrDefaultAsync(c => c.Id == _testCustomerId);
        customer!.FavoriteGames.Should().HaveCount(1);
        customer.FavoriteGames[0].Id.Should().Be(_testGameId);
    }

    [Fact]
    public async Task AddFavoriteGame_WhenAlreadyFavorited_ReturnsConflict()
    {
        // Arrange - Add game to favorites first
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var customer = await db.Customers
            .Include(c => c.FavoriteGames)
            .FirstOrDefaultAsync(c => c.Id == _testCustomerId);
        var game = await db.Games.FindAsync(_testGameId);
        customer!.FavoriteGames.Add(game!);
        await db.SaveChangesAsync();

        // Act
        var response = await _client.PostAsync($"/api/v1/customers/me/favorites?customerId={_testCustomerId}&gameId={_testGameId}", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task AddFavoriteGame_WithInvalidGameId_ReturnsNotFound()
    {
        // Act
        var response = await _client.PostAsync($"/api/v1/customers/me/favorites?customerId={_testCustomerId}&gameId={Guid.NewGuid()}", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RemoveFavoriteGame_RemovesGameFromFavorites()
    {
        // Arrange - Add game to favorites first
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var customer = await db.Customers
            .Include(c => c.FavoriteGames)
            .FirstOrDefaultAsync(c => c.Id == _testCustomerId);
        var game = await db.Games.FindAsync(_testGameId);
        customer!.FavoriteGames.Add(game!);
        await db.SaveChangesAsync();

        // Act
        var response = await _client.DeleteAsync($"/api/v1/customers/me/favorites/{_testGameId}?customerId={_testCustomerId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify it was removed
        using var scope2 = _factory.Services.CreateScope();
        var db2 = scope2.ServiceProvider.GetRequiredService<AppDbContext>();
        var updatedCustomer = await db2.Customers
            .Include(c => c.FavoriteGames)
            .FirstOrDefaultAsync(c => c.Id == _testCustomerId);
        updatedCustomer!.FavoriteGames.Should().BeEmpty();
    }

    [Fact]
    public async Task RemoveFavoriteGame_WhenNotFavorited_ReturnsNotFound()
    {
        // Act
        var response = await _client.DeleteAsync($"/api/v1/customers/me/favorites/{_testGameId}?customerId={_testCustomerId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetVisitStats_ReturnsStatistics()
    {
        // Act
        var response = await _client.GetAsync($"/api/v1/customers/me/visit-stats?customerId={_testCustomerId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var stats = await response.Content.ReadFromJsonAsync<VisitStatsDto>();
        stats.Should().NotBeNull();
        stats!.TotalVisits.Should().Be(10);
        stats.GamesPlayed.Should().Be(0); // No game sessions yet
        stats.TotalSpending.Should().Be(0); // No completed orders yet
        stats.LastVisit.Should().BeNull(); // No reservations yet
    }

    [Fact]
    public async Task GetVisitStats_WithOrdersAndSessions_ReturnsAccurateData()
    {
        // Arrange - Add orders and game sessions
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Create an order
        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = _testCustomerId,
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Completed,
            Subtotal = 100m,
            TaxAmount = 8m,
            DiscountAmount = 5m,
            TotalAmount = 103m,
            PaymentMethod = PaymentMethod.Card
        };
        db.Orders.Add(order);

        // Create a table and reservation for game session
        var table = new Table
        {
            Id = Guid.NewGuid(),
            TableNumber = "T1",
            SeatingCapacity = 4,
            HourlyRate = 10m,
            IsWindowSeat = false,
            IsAccessible = true,
            Status = TableStatus.Available
        };
        db.Tables.Add(table);

        var reservation = new Reservation
        {
            Id = Guid.NewGuid(),
            CustomerId = _testCustomerId,
            TableId = table.Id,
            ReservationDate = DateTime.Today,
            StartTime = TimeSpan.FromHours(14), // 2 PM
            EndTime = TimeSpan.FromHours(16), // 4 PM
            PartySize = 3,
            Status = ReservationStatus.Confirmed
        };
        db.Reservations.Add(reservation);

        // Create a game session
        var gameSession = new GameSession
        {
            Id = Guid.NewGuid(),
            GameId = _testGameId,
            ReservationId = reservation.Id,
            CheckedOutAt = DateTime.UtcNow.AddHours(-2),
            ReturnedAt = null
        };
        db.GameSessions.Add(gameSession);

        await db.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"/api/v1/customers/me/visit-stats?customerId={_testCustomerId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var stats = await response.Content.ReadFromJsonAsync<VisitStatsDto>();
        stats.Should().NotBeNull();
        stats!.TotalVisits.Should().Be(10);
        stats.GamesPlayed.Should().Be(1);
        stats.TotalSpending.Should().Be(103m);
        stats.LastVisit.Should().Be(DateTime.Today);
    }
}
