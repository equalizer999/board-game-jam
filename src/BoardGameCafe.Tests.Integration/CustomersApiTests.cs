using BoardGameCafe.Api.Data;
using BoardGameCafe.Api.Features.Customers;
using BoardGameCafe.Domain;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace BoardGameCafe.Tests.Integration;

public class CustomersApiTests : IClassFixture<ReservationsApiTestFixture>, IAsyncLifetime
{
    private readonly ReservationsApiTestFixture _factory;
    private HttpClient _client = null!;
    private Guid _testCustomerId;

    public CustomersApiTests(ReservationsApiTestFixture factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        _client = _factory.CreateClient();
        
        // Clean up and seed test data
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BoardGameCafeDbContext>();

        // Clean existing data
        db.LoyaltyPointsHistory.RemoveRange(db.LoyaltyPointsHistory);
        db.Customers.RemoveRange(db.Customers);
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
        await db.SaveChangesAsync();

        _testCustomerId = customer.Id;
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
        loyaltyPoints!.CurrentBalance.Should().Be(250);
        loyaltyPoints.CurrentTier.Should().Be("Bronze");
        loyaltyPoints.DiscountPercentage.Should().Be(0.05m);
        loyaltyPoints.NextTier.Should().Be("Silver");
        loyaltyPoints.PointsToNextTier.Should().Be(250); // 500 - 250
    }
}