using System.Net;
using System.Net.Http.Json;
using BoardGameCafe.Api.Data;
using BoardGameCafe.Api.Features.Customers;
using BoardGameCafe.Api.Features.Orders;
using BoardGameCafe.Domain;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace BoardGameCafe.Tests.Integration;

public class CustomersApiTests : IClassFixture<ReservationsApiTestFixture>
{
    private readonly ReservationsApiTestFixture _fixture;
    private readonly HttpClient _client;

    public CustomersApiTests(ReservationsApiTestFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.CreateClient();
    }

    [Fact]
    public async Task GetCustomerProfile_ShouldReturnProfile()
    {
        // Arrange
        var customer = await SeedCustomerAsync();

        // Act
        var response = await _client.GetAsync($"/api/v1/customers/me?customerId={customer.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var profile = await response.Content.ReadFromJsonAsync<CustomerDto>();
        profile.Should().NotBeNull();
        profile!.Id.Should().Be(customer.Id);
        profile.Email.Should().Be(customer.Email);
        profile.FirstName.Should().Be(customer.FirstName);
        profile.LastName.Should().Be(customer.LastName);
        profile.MembershipTier.Should().Be(customer.MembershipTier.ToString());
    }

    [Fact]
    public async Task GetCustomerProfile_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/v1/customers/me?customerId={invalidId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateCustomerProfile_ShouldUpdateFields()
    {
        // Arrange
        var customer = await SeedCustomerAsync();
        var request = new UpdateCustomerRequest
        {
            FirstName = "UpdatedFirst",
            LastName = "UpdatedLast",
            Phone = "555-9999"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/customers/me?customerId={customer.Id}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await response.Content.ReadFromJsonAsync<CustomerDto>();
        updated.Should().NotBeNull();
        updated!.FirstName.Should().Be("UpdatedFirst");
        updated.LastName.Should().Be("UpdatedLast");
        updated.Phone.Should().Be("555-9999");
    }

    [Fact]
    public async Task GetLoyaltyPoints_ShouldReturnPointsAndTier()
    {
        // Arrange
        var customer = await SeedCustomerWithPointsAsync(0);

        // Act
        var response = await _client.GetAsync($"/api/v1/customers/me/loyalty-points?customerId={customer.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var loyaltyInfo = await response.Content.ReadFromJsonAsync<LoyaltyPointsDto>();
        loyaltyInfo.Should().NotBeNull();
        loyaltyInfo!.CurrentBalance.Should().Be(0);
        loyaltyInfo.CurrentTier.Should().Be("None");
        loyaltyInfo.DiscountPercentage.Should().Be(0);
        loyaltyInfo.NextTier.Should().Be("Bronze");
    }

    [Fact]
    public async Task GetLoyaltyPoints_BronzeTier_ShouldReturn5PercentDiscount()
    {
        // Arrange
        var customer = await SeedCustomerWithPointsAsync(250);

        // Act
        var response = await _client.GetAsync($"/api/v1/customers/me/loyalty-points?customerId={customer.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var loyaltyInfo = await response.Content.ReadFromJsonAsync<LoyaltyPointsDto>();
        loyaltyInfo.Should().NotBeNull();
        loyaltyInfo!.CurrentBalance.Should().Be(250);
        loyaltyInfo.CurrentTier.Should().Be("Bronze");
        loyaltyInfo.DiscountPercentage.Should().Be(5m);
        loyaltyInfo.NextTier.Should().Be("Silver");
        loyaltyInfo.PointsToNextTier.Should().Be(250); // 500 - 250
    }

    [Fact]
    public async Task GetLoyaltyPoints_SilverTier_ShouldReturn10PercentDiscount()
    {
        // Arrange
        var customer = await SeedCustomerWithPointsAsync(1000);

        // Act
        var response = await _client.GetAsync($"/api/v1/customers/me/loyalty-points?customerId={customer.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var loyaltyInfo = await response.Content.ReadFromJsonAsync<LoyaltyPointsDto>();
        loyaltyInfo.Should().NotBeNull();
        loyaltyInfo!.CurrentBalance.Should().Be(1000);
        loyaltyInfo.CurrentTier.Should().Be("Silver");
        loyaltyInfo.DiscountPercentage.Should().Be(10m);
        loyaltyInfo.NextTier.Should().Be("Gold");
        loyaltyInfo.PointsToNextTier.Should().Be(1000); // 2000 - 1000
    }

    [Fact]
    public async Task GetLoyaltyPoints_GoldTier_ShouldReturn15PercentDiscount()
    {
        // Arrange
        var customer = await SeedCustomerWithPointsAsync(2500);

        // Act
        var response = await _client.GetAsync($"/api/v1/customers/me/loyalty-points?customerId={customer.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var loyaltyInfo = await response.Content.ReadFromJsonAsync<LoyaltyPointsDto>();
        loyaltyInfo.Should().NotBeNull();
        loyaltyInfo!.CurrentBalance.Should().Be(2500);
        loyaltyInfo.CurrentTier.Should().Be("Gold");
        loyaltyInfo.DiscountPercentage.Should().Be(15m);
        loyaltyInfo.NextTier.Should().BeNull();
        loyaltyInfo.PointsToNextTier.Should().BeNull();
    }

    [Fact]
    public async Task GetLoyaltyHistory_ShouldReturnEmptyForNewCustomer()
    {
        // Arrange
        var customer = await SeedCustomerAsync();

        // Act
        var response = await _client.GetAsync($"/api/v1/customers/me/loyalty-history?customerId={customer.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var history = await response.Content.ReadFromJsonAsync<List<LoyaltyTransactionDto>>();
        history.Should().NotBeNull();
        history!.Should().BeEmpty();
    }

    [Fact]
    public async Task GetLoyaltyHistory_ShouldReturnTransactions()
    {
        // Arrange
        var customer = await SeedCustomerAsync();
        await SeedLoyaltyHistoryAsync(customer.Id, 100, LoyaltyTransactionType.Earned);
        await SeedLoyaltyHistoryAsync(customer.Id, -50, LoyaltyTransactionType.Redeemed);

        // Act
        var response = await _client.GetAsync($"/api/v1/customers/me/loyalty-history?customerId={customer.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var history = await response.Content.ReadFromJsonAsync<List<LoyaltyTransactionDto>>();
        history.Should().NotBeNull();
        history!.Should().HaveCount(2);
        history.Should().Contain(t => t.PointsChange == 100 && t.TransactionType == "Earned");
        history.Should().Contain(t => t.PointsChange == -50 && t.TransactionType == "Redeemed");
    }

    [Fact]
    public async Task GetVisitStats_ShouldReturnStats()
    {
        // Arrange
        var customer = await SeedCustomerAsync();

        // Act
        var response = await _client.GetAsync($"/api/v1/customers/me/visit-stats?customerId={customer.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var stats = await response.Content.ReadFromJsonAsync<VisitStatsDto>();
        stats.Should().NotBeNull();
        stats!.TotalVisits.Should().Be(customer.TotalVisits);
        stats.GamesPlayed.Should().BeGreaterThanOrEqualTo(0);
        stats.TotalSpent.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task AddFavoriteGame_ShouldReturnNoContent()
    {
        // Arrange
        var customer = await SeedCustomerAsync();
        var game = await SeedGameAsync();

        // Act
        var response = await _client.PostAsync($"/api/v1/customers/me/favorites?customerId={customer.Id}&gameId={game.Id}", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task AddFavoriteGame_WithInvalidGame_ShouldReturnBadRequest()
    {
        // Arrange
        var customer = await SeedCustomerAsync();
        var invalidGameId = Guid.NewGuid();

        // Act
        var response = await _client.PostAsync($"/api/v1/customers/me/favorites?customerId={customer.Id}&gameId={invalidGameId}", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RemoveFavoriteGame_ShouldReturnNoContent()
    {
        // Arrange
        var customer = await SeedCustomerAsync();
        var game = await SeedGameAsync();

        // Act
        var response = await _client.DeleteAsync($"/api/v1/customers/me/favorites/{game.Id}?customerId={customer.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task OrderPayment_ShouldEarnLoyaltyPoints()
    {
        // Arrange
        var customer = await SeedCustomerAsync();
        var menuItem = await SeedMenuItemAsync();
        var order = await CreateOrderAsync(customer.Id);
        await AddOrderItemAsync(order.Id, menuItem.Id, 1);
        await SubmitOrderAsync(order.Id, 0);

        // Act - Pay the order (which earns points)
        var response = await _client.PostAsync($"/api/v1/orders/{order.Id}/pay?paymentMethod=Card", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        // Verify loyalty history was created
        var historyResponse = await _client.GetAsync($"/api/v1/customers/me/loyalty-history?customerId={customer.Id}");
        var history = await historyResponse.Content.ReadFromJsonAsync<List<LoyaltyTransactionDto>>();
        history.Should().NotBeNull();
        history!.Should().ContainSingle(t => t.TransactionType == "Earned");
    }

    [Fact]
    public async Task OrderSubmit_WithPointsRedemption_ShouldTrackRedemption()
    {
        // Arrange
        var customer = await SeedCustomerWithPointsAsync(500);
        var menuItem = await SeedMenuItemAsync();
        var order = await CreateOrderAsync(customer.Id);
        await AddOrderItemAsync(order.Id, menuItem.Id, 1);

        // Act - Submit with points redemption
        var response = await _client.PostAsync($"/api/v1/orders/{order.Id}/submit?loyaltyPointsToRedeem=100", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        // Verify loyalty history was created
        var historyResponse = await _client.GetAsync($"/api/v1/customers/me/loyalty-history?customerId={customer.Id}");
        var history = await historyResponse.Content.ReadFromJsonAsync<List<LoyaltyTransactionDto>>();
        history.Should().NotBeNull();
        history!.Should().ContainSingle(t => t.TransactionType == "Redeemed" && t.PointsChange == -100);
    }

    // Helper methods
    private async Task<Customer> SeedCustomerAsync()
    {
        using var scope = _fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Email = $"test{Guid.NewGuid()}@example.com",
            FirstName = "Test",
            LastName = "Customer",
            Phone = "555-1234",
            MembershipTier = MembershipTier.None,
            LoyaltyPoints = 0,
            JoinedDate = DateTime.UtcNow,
            TotalVisits = 0
        };

        db.Customers.Add(customer);
        await db.SaveChangesAsync();
        return customer;
    }

    private async Task<Customer> SeedCustomerWithPointsAsync(int points)
    {
        using var scope = _fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var tier = points switch
        {
            >= 2000 => MembershipTier.Gold,
            >= 500 => MembershipTier.Silver,
            >= 1 => MembershipTier.Bronze,
            _ => MembershipTier.None
        };

        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Email = $"test{Guid.NewGuid()}@example.com",
            FirstName = "Test",
            LastName = "Customer",
            Phone = "555-1234",
            MembershipTier = tier,
            LoyaltyPoints = points,
            JoinedDate = DateTime.UtcNow,
            TotalVisits = 0
        };

        db.Customers.Add(customer);
        await db.SaveChangesAsync();
        return customer;
    }

    private async Task SeedLoyaltyHistoryAsync(Guid customerId, int pointsChange, LoyaltyTransactionType type)
    {
        using var scope = _fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var history = new LoyaltyPointsHistory
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            PointsChange = pointsChange,
            TransactionType = type,
            TransactionDate = DateTime.UtcNow,
            Description = $"Test {type} transaction"
        };

        db.LoyaltyPointsHistory.Add(history);
        await db.SaveChangesAsync();
    }

    private async Task<Game> SeedGameAsync()
    {
        using var scope = _fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var game = new Game
        {
            Id = Guid.NewGuid(),
            Title = $"Test Game {Guid.NewGuid()}",
            Publisher = "Test Publisher",
            MinPlayers = 2,
            MaxPlayers = 4,
            PlayTimeMinutes = 60,
            AgeRating = 10,
            Complexity = 2.5m,
            Category = GameCategory.Strategy,
            CopiesOwned = 3,
            CopiesInUse = 0,
            DailyRentalFee = 5.00m,
            Description = "Test game description"
        };

        db.Games.Add(game);
        await db.SaveChangesAsync();
        return game;
    }

    private async Task<MenuItem> SeedMenuItemAsync()
    {
        using var scope = _fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var menuItem = new MenuItem
        {
            Id = Guid.NewGuid(),
            Name = $"Test Item {Guid.NewGuid()}",
            Description = "Test menu item",
            Category = MenuCategory.Snacks,
            Price = 10.00m,
            IsAvailable = true,
            PreparationTimeMinutes = 10,
            IsVegetarian = true,
            IsVegan = false,
            IsGlutenFree = false
        };

        db.MenuItems.Add(menuItem);
        await db.SaveChangesAsync();
        return menuItem;
    }

    private async Task<OrderDto> CreateOrderAsync(Guid customerId)
    {
        var request = new CreateOrderRequest { CustomerId = customerId };
        var response = await _client.PostAsJsonAsync("/api/v1/orders", request);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<OrderDto>())!;
    }

    private async Task AddOrderItemAsync(Guid orderId, Guid menuItemId, int quantity)
    {
        var request = new AddOrderItemRequest
        {
            MenuItemId = menuItemId,
            Quantity = quantity
        };
        var response = await _client.PostAsJsonAsync($"/api/v1/orders/{orderId}/items", request);
        response.EnsureSuccessStatusCode();
    }

    private async Task SubmitOrderAsync(Guid orderId, int pointsToRedeem)
    {
        var response = await _client.PostAsync($"/api/v1/orders/{orderId}/submit?loyaltyPointsToRedeem={pointsToRedeem}", null);
        response.EnsureSuccessStatusCode();
    }
}
