using System.Net;
using System.Net.Http.Json;
using BoardGameCafe.Api.Data;
using BoardGameCafe.Api.Features.Orders;
using BoardGameCafe.Domain;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace BoardGameCafe.Tests.Integration;

public class OrdersApiTests : IClassFixture<ReservationsApiTestFixture>
{
    private readonly ReservationsApiTestFixture _fixture;
    private readonly HttpClient _client;

    public OrdersApiTests(ReservationsApiTestFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.CreateClient();
    }

    [Fact]
    public async Task CreateOrder_ShouldReturnCreatedOrder()
    {
        // Arrange
        var customer = await SeedCustomerAsync();
        var request = new CreateOrderRequest
        {
            CustomerId = customer.Id
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/orders", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var order = await response.Content.ReadFromJsonAsync<OrderDto>();
        order.Should().NotBeNull();
        order!.CustomerId.Should().Be(customer.Id);
        order.Status.Should().Be("Draft");
        order.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateOrder_WithInvalidCustomer_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            CustomerId = Guid.NewGuid() // Non-existent customer
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/orders", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetOrders_ShouldReturnCustomerOrders()
    {
        // Arrange
        var customer = await SeedCustomerAsync();
        var order1 = await CreateOrderAsync(customer.Id);
        var order2 = await CreateOrderAsync(customer.Id);

        // Act
        var response = await _client.GetAsync($"/api/v1/orders?customerId={customer.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var orders = await response.Content.ReadFromJsonAsync<List<OrderDto>>();
        orders.Should().NotBeNull();
        orders!.Should().HaveCountGreaterThanOrEqualTo(2);
        orders.Should().Contain(o => o.Id == order1.Id);
        orders.Should().Contain(o => o.Id == order2.Id);
    }

    [Fact]
    public async Task GetOrder_ShouldReturnOrderWithItems()
    {
        // Arrange
        var customer = await SeedCustomerAsync();
        var menuItem = await SeedMenuItemAsync();
        var order = await CreateOrderAsync(customer.Id);
        await AddOrderItemAsync(order.Id, menuItem.Id, 2);

        // Act
        var response = await _client.GetAsync($"/api/v1/orders/{order.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var orderDto = await response.Content.ReadFromJsonAsync<OrderDto>();
        orderDto.Should().NotBeNull();
        orderDto!.Id.Should().Be(order.Id);
        orderDto.Items.Should().HaveCount(1);
        orderDto.Items[0].MenuItemId.Should().Be(menuItem.Id);
        orderDto.Items[0].Quantity.Should().Be(2);
    }

    [Fact]
    public async Task GetOrder_WithInvalidId_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync($"/api/v1/orders/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AddOrderItem_ShouldAddItemToDraftOrder()
    {
        // Arrange
        var customer = await SeedCustomerAsync();
        var menuItem = await SeedMenuItemAsync();
        var order = await CreateOrderAsync(customer.Id);
        var request = new AddOrderItemRequest
        {
            MenuItemId = menuItem.Id,
            Quantity = 3,
            SpecialInstructions = "Extra cheese"
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/v1/orders/{order.Id}/items", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var orderDto = await response.Content.ReadFromJsonAsync<OrderDto>();
        orderDto.Should().NotBeNull();
        orderDto!.Items.Should().HaveCount(1);
        orderDto.Items[0].MenuItemId.Should().Be(menuItem.Id);
        orderDto.Items[0].Quantity.Should().Be(3);
        orderDto.Items[0].SpecialInstructions.Should().Be("Extra cheese");
        orderDto.Subtotal.Should().Be(menuItem.Price * 3);
    }

    [Fact]
    public async Task AddOrderItem_ToSubmittedOrder_ShouldReturnBadRequest()
    {
        // Arrange
        var customer = await SeedCustomerAsync();
        var menuItem = await SeedMenuItemAsync();
        var order = await CreateOrderAsync(customer.Id);
        await AddOrderItemAsync(order.Id, menuItem.Id, 1);
        await SubmitOrderAsync(order.Id);

        var request = new AddOrderItemRequest
        {
            MenuItemId = menuItem.Id,
            Quantity = 1
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/v1/orders/{order.Id}/items", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AddOrderItem_WithInvalidQuantity_ShouldReturnBadRequest()
    {
        // Arrange
        var customer = await SeedCustomerAsync();
        var menuItem = await SeedMenuItemAsync();
        var order = await CreateOrderAsync(customer.Id);
        var request = new AddOrderItemRequest
        {
            MenuItemId = menuItem.Id,
            Quantity = 0 // Invalid
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/v1/orders/{order.Id}/items", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RemoveOrderItem_ShouldRemoveItemFromDraftOrder()
    {
        // Arrange
        var customer = await SeedCustomerAsync();
        var menuItem = await SeedMenuItemAsync();
        var order = await CreateOrderAsync(customer.Id);
        var orderWithItem = await AddOrderItemAsync(order.Id, menuItem.Id, 2);
        var itemId = orderWithItem.Items[0].Id;

        // Act
        var response = await _client.DeleteAsync($"/api/v1/orders/{order.Id}/items/{itemId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify item was removed
        var getResponse = await _client.GetAsync($"/api/v1/orders/{order.Id}");
        var orderDto = await getResponse.Content.ReadFromJsonAsync<OrderDto>();
        orderDto!.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateOrder_ShouldUpdateItemQuantities()
    {
        // Arrange
        var customer = await SeedCustomerAsync();
        var menuItem = await SeedMenuItemAsync();
        var order = await CreateOrderAsync(customer.Id);
        var orderWithItem = await AddOrderItemAsync(order.Id, menuItem.Id, 2);
        var itemId = orderWithItem.Items[0].Id;

        var request = new UpdateOrderRequest
        {
            ItemUpdates = new List<OrderItemUpdate>
            {
                new OrderItemUpdate { OrderItemId = itemId, Quantity = 5 }
            }
        };

        // Act
        var response = await _client.PatchAsync($"/api/v1/orders/{order.Id}", JsonContent.Create(request));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var orderDto = await response.Content.ReadFromJsonAsync<OrderDto>();
        orderDto.Should().NotBeNull();
        orderDto!.Items[0].Quantity.Should().Be(5);
        orderDto.Subtotal.Should().Be(menuItem.Price * 5);
    }

    [Fact]
    public async Task UpdateOrder_WithQuantityZero_ShouldRemoveItem()
    {
        // Arrange
        var customer = await SeedCustomerAsync();
        var menuItem = await SeedMenuItemAsync();
        var order = await CreateOrderAsync(customer.Id);
        var orderWithItem = await AddOrderItemAsync(order.Id, menuItem.Id, 2);
        var itemId = orderWithItem.Items[0].Id;

        var request = new UpdateOrderRequest
        {
            ItemUpdates = new List<OrderItemUpdate>
            {
                new OrderItemUpdate { OrderItemId = itemId, Quantity = 0 }
            }
        };

        // Act
        var response = await _client.PatchAsync($"/api/v1/orders/{order.Id}", JsonContent.Create(request));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var orderDto = await response.Content.ReadFromJsonAsync<OrderDto>();
        orderDto!.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task SubmitOrder_ShouldCalculateTotalsAndUpdateStatus()
    {
        // Arrange
        var customer = await SeedCustomerWithMembershipAsync(MembershipTier.Gold); // 15% discount
        var menuItem = await SeedMenuItemAsync(MenuCategory.Meals, 100.00m);
        var order = await CreateOrderAsync(customer.Id);
        await AddOrderItemAsync(order.Id, menuItem.Id, 1);

        // Act
        var response = await _client.PostAsync($"/api/v1/orders/{order.Id}/submit", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var summary = await response.Content.ReadFromJsonAsync<OrderSummaryDto>();
        summary.Should().NotBeNull();
        summary!.Status.Should().Be("Submitted");
        summary.Subtotal.Should().Be(100.00m);
        summary.MemberDiscountAmount.Should().Be(15.00m); // 15% of 100
        summary.TaxAmount.Should().Be(8.00m); // 8% of 100 (food)
        summary.TotalAmount.Should().Be(93.00m); // 100 - 15 + 8
    }

    [Fact]
    public async Task SubmitOrder_WithLoyaltyPointsRedemption_ShouldApplyDiscount()
    {
        // Arrange
        var customer = await SeedCustomerWithLoyaltyPointsAsync(500);
        var menuItem = await SeedMenuItemAsync(MenuCategory.Coffee, 10.00m);
        var order = await CreateOrderAsync(customer.Id);
        await AddOrderItemAsync(order.Id, menuItem.Id, 1);

        // Act
        var response = await _client.PostAsync($"/api/v1/orders/{order.Id}/submit?loyaltyPointsToRedeem=200", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var summary = await response.Content.ReadFromJsonAsync<OrderSummaryDto>();
        summary.Should().NotBeNull();
        summary!.LoyaltyPointsRedeemed.Should().Be(200);
        summary.LoyaltyPointsDiscountAmount.Should().Be(2.00m); // 200 points = $2
    }

    [Fact]
    public async Task SubmitOrder_WithEmptyOrder_ShouldReturnBadRequest()
    {
        // Arrange
        var customer = await SeedCustomerAsync();
        var order = await CreateOrderAsync(customer.Id);

        // Act
        var response = await _client.PostAsync($"/api/v1/orders/{order.Id}/submit", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PayOrder_ShouldUpdateCustomerLoyaltyPoints()
    {
        // Arrange
        var customer = await SeedCustomerAsync();
        var menuItem = await SeedMenuItemAsync(MenuCategory.Coffee, 50.00m);
        var order = await CreateOrderAsync(customer.Id);
        await AddOrderItemAsync(order.Id, menuItem.Id, 1);
        await SubmitOrderAsync(order.Id);

        // Act
        var response = await _client.PostAsync($"/api/v1/orders/{order.Id}/pay?paymentMethod=Card", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var summary = await response.Content.ReadFromJsonAsync<OrderSummaryDto>();
        summary.Should().NotBeNull();
        summary!.Status.Should().Be("Completed");
        summary.LoyaltyPointsEarned.Should().BeGreaterThan(0);

        // Verify customer's loyalty points were updated
        using var scope = _fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BoardGameCafeDbContext>();
        var updatedCustomer = await db.Customers.FindAsync(customer.Id);
        updatedCustomer!.LoyaltyPoints.Should().Be(summary.LoyaltyPointsEarned);
    }

    [Fact]
    public async Task PayOrder_WithLoyaltyPointsRedeemedInSubmit_ShouldOnlyAddEarnedPoints()
    {
        // Arrange
        var customer = await SeedCustomerWithLoyaltyPointsAsync(500);
        var menuItem = await SeedMenuItemAsync(MenuCategory.Meals, 100.00m);
        var order = await CreateOrderAsync(customer.Id);
        await AddOrderItemAsync(order.Id, menuItem.Id, 1);

        // Submit with loyalty points redemption
        var submitResponse = await _client.PostAsync(
            $"/api/v1/orders/{order.Id}/submit?loyaltyPointsToRedeem=200",
            null);
        submitResponse.EnsureSuccessStatusCode();
        var submitSummary = await submitResponse.Content.ReadFromJsonAsync<OrderSummaryDto>();

        // Act - Pay the order
        var response = await _client.PostAsync(
            $"/api/v1/orders/{order.Id}/pay?paymentMethod=Card",
            null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var paySummary = await response.Content.ReadFromJsonAsync<OrderSummaryDto>();
        paySummary.Should().NotBeNull();

        // Verify customer's loyalty points: 500 (initial) - 200 (redeemed in submit) + earned
        using var scope = _fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BoardGameCafeDbContext>();
        var updatedCustomer = await db.Customers.FindAsync(customer.Id);
        var expectedPoints = 500 - 200 + paySummary!.LoyaltyPointsEarned;
        updatedCustomer!.LoyaltyPoints.Should().Be(expectedPoints);
    }

    // Helper methods
    private async Task<Customer> SeedCustomerAsync()
    {
        using var scope = _fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BoardGameCafeDbContext>();

        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Email = $"test{Guid.NewGuid()}@test.com",
            FirstName = "Test",
            LastName = "User",
            MembershipTier = MembershipTier.None,
            LoyaltyPoints = 0,
            JoinedDate = DateTime.UtcNow
        };

        db.Customers.Add(customer);
        await db.SaveChangesAsync();
        return customer;
    }

    private async Task<Customer> SeedCustomerWithMembershipAsync(MembershipTier tier)
    {
        using var scope = _fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BoardGameCafeDbContext>();

        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Email = $"test{Guid.NewGuid()}@test.com",
            FirstName = "Test",
            LastName = "User",
            MembershipTier = tier,
            LoyaltyPoints = 0,
            JoinedDate = DateTime.UtcNow
        };

        db.Customers.Add(customer);
        await db.SaveChangesAsync();
        return customer;
    }

    private async Task<Customer> SeedCustomerWithLoyaltyPointsAsync(int points)
    {
        using var scope = _fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BoardGameCafeDbContext>();

        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Email = $"test{Guid.NewGuid()}@test.com",
            FirstName = "Test",
            LastName = "User",
            MembershipTier = MembershipTier.None,
            LoyaltyPoints = points,
            JoinedDate = DateTime.UtcNow
        };

        db.Customers.Add(customer);
        await db.SaveChangesAsync();
        return customer;
    }

    private async Task<MenuItem> SeedMenuItemAsync(MenuCategory category = MenuCategory.Coffee, decimal price = 5.00m)
    {
        using var scope = _fixture.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BoardGameCafeDbContext>();

        var menuItem = new MenuItem
        {
            Id = Guid.NewGuid(),
            Name = $"Test Item {Guid.NewGuid()}",
            Description = "Test description",
            Category = category,
            Price = price,
            IsAvailable = true,
            PreparationTimeMinutes = 5
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

    private async Task<OrderDto> AddOrderItemAsync(Guid orderId, Guid menuItemId, int quantity)
    {
        var request = new AddOrderItemRequest
        {
            MenuItemId = menuItemId,
            Quantity = quantity
        };
        var response = await _client.PostAsJsonAsync($"/api/v1/orders/{orderId}/items", request);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<OrderDto>())!;
    }

    private async Task<OrderSummaryDto> SubmitOrderAsync(Guid orderId)
    {
        var response = await _client.PostAsync($"/api/v1/orders/{orderId}/submit", null);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<OrderSummaryDto>())!;
    }
}
