using BoardGameCafe.Api.Data;
using BoardGameCafe.Api.Features.Reservations;
using BoardGameCafe.Domain;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace BoardGameCafe.Tests.Integration;

public class ReservationsApiTests : IClassFixture<ReservationsApiTestFixture>, IAsyncLifetime
{
    private readonly ReservationsApiTestFixture _factory;
    private HttpClient _client = null!;

    // Test data IDs
    private Guid _customerId1;
    private Guid _customerId2;
    private Guid _tableId1;
    private Guid _tableId2;

    public ReservationsApiTests(ReservationsApiTestFixture factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        _client = _factory.CreateClient();
        
        // Create a scope to seed test data for this test
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Clean up any existing data from previous tests
        db.Reservations.RemoveRange(db.Reservations);
        db.Customers.RemoveRange(db.Customers);
        db.Tables.RemoveRange(db.Tables);
        await db.SaveChangesAsync();

        // Seed test data
        var customer1 = new Customer
        {
            Id = Guid.NewGuid(),
            Email = "test1@example.com",
            FirstName = "John",
            LastName = "Doe",
            JoinedDate = DateTime.UtcNow
        };
        
        var customer2 = new Customer
        {
            Id = Guid.NewGuid(),
            Email = "test2@example.com",
            FirstName = "Jane",
            LastName = "Smith",
            JoinedDate = DateTime.UtcNow
        };

        var table1 = new Table
        {
            Id = Guid.NewGuid(),
            TableNumber = "T1",
            SeatingCapacity = 4,
            HourlyRate = 15.00m,
            Status = TableStatus.Available
        };

        var table2 = new Table
        {
            Id = Guid.NewGuid(),
            TableNumber = "T2",
            SeatingCapacity = 6,
            HourlyRate = 20.00m,
            IsWindowSeat = true,
            Status = TableStatus.Available
        };

        db.Customers.AddRange(customer1, customer2);
        db.Tables.AddRange(table1, table2);
        await db.SaveChangesAsync();

        _customerId1 = customer1.Id;
        _customerId2 = customer2.Id;
        _tableId1 = table1.Id;
        _tableId2 = table2.Id;
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task CreateReservation_WithValidData_ReturnsCreated()
    {
        // Arrange
        var tomorrow = DateTime.UtcNow.Date.AddDays(1);
        var request = new CreateReservationRequest
        {
            CustomerId = _customerId1,
            TableId = _tableId1,
            ReservationDate = tomorrow,
            StartTime = new TimeSpan(14, 0, 0),
            EndTime = new TimeSpan(16, 0, 0),
            PartySize = 4,
            SpecialRequests = "Window seat preferred"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/reservations", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var reservation = await response.Content.ReadFromJsonAsync<ReservationDto>();
        reservation.Should().NotBeNull();
        reservation!.CustomerId.Should().Be(_customerId1);
        reservation.TableId.Should().Be(_tableId1);
        reservation.PartySize.Should().Be(4);
        reservation.Status.Should().Be("Confirmed");
        reservation.SpecialRequests.Should().Be("Window seat preferred");
    }

    [Fact]
    public async Task CreateReservation_WithPastDate_ReturnsBadRequest()
    {
        // Arrange
        var yesterday = DateTime.UtcNow.Date.AddDays(-1);
        var request = new CreateReservationRequest
        {
            CustomerId = _customerId1,
            TableId = _tableId1,
            ReservationDate = yesterday,
            StartTime = new TimeSpan(14, 0, 0),
            EndTime = new TimeSpan(16, 0, 0),
            PartySize = 4
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/reservations", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateReservation_WithInvalidTimeRange_ReturnsBadRequest()
    {
        // Arrange
        var tomorrow = DateTime.UtcNow.Date.AddDays(1);
        var request = new CreateReservationRequest
        {
            CustomerId = _customerId1,
            TableId = _tableId1,
            ReservationDate = tomorrow,
            StartTime = new TimeSpan(16, 0, 0),
            EndTime = new TimeSpan(14, 0, 0), // End before start
            PartySize = 4
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/reservations", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateReservation_ExceedingTableCapacity_ReturnsBadRequest()
    {
        // Arrange
        var tomorrow = DateTime.UtcNow.Date.AddDays(1);
        var request = new CreateReservationRequest
        {
            CustomerId = _customerId1,
            TableId = _tableId1, // Table with capacity 4
            ReservationDate = tomorrow,
            StartTime = new TimeSpan(14, 0, 0),
            EndTime = new TimeSpan(16, 0, 0),
            PartySize = 5 // Exceeds capacity
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/reservations", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateReservation_WithConflict_ReturnsConflict()
    {
        // Arrange - Create first reservation
        var tomorrow = DateTime.UtcNow.Date.AddDays(1);
        var firstRequest = new CreateReservationRequest
        {
            CustomerId = _customerId1,
            TableId = _tableId1,
            ReservationDate = tomorrow,
            StartTime = new TimeSpan(14, 0, 0),
            EndTime = new TimeSpan(16, 0, 0),
            PartySize = 4
        };
        await _client.PostAsJsonAsync("/api/v1/reservations", firstRequest);

        // Act - Try to create overlapping reservation
        var conflictingRequest = new CreateReservationRequest
        {
            CustomerId = _customerId2,
            TableId = _tableId1, // Same table
            ReservationDate = tomorrow, // Same date
            StartTime = new TimeSpan(15, 0, 0), // Overlaps with first reservation
            EndTime = new TimeSpan(17, 0, 0),
            PartySize = 3
        };
        var response = await _client.PostAsJsonAsync("/api/v1/reservations", conflictingRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task CreateReservation_WithBufferViolation_ReturnsConflict()
    {
        // Arrange - Create first reservation ending at 16:00
        var tomorrow = DateTime.UtcNow.Date.AddDays(1);
        var firstRequest = new CreateReservationRequest
        {
            CustomerId = _customerId1,
            TableId = _tableId1,
            ReservationDate = tomorrow,
            StartTime = new TimeSpan(14, 0, 0),
            EndTime = new TimeSpan(16, 0, 0),
            PartySize = 4
        };
        await _client.PostAsJsonAsync("/api/v1/reservations", firstRequest);

        // Act - Try to create reservation starting at 16:05 (within 15-minute buffer)
        var bufferViolationRequest = new CreateReservationRequest
        {
            CustomerId = _customerId2,
            TableId = _tableId1,
            ReservationDate = tomorrow,
            StartTime = new TimeSpan(16, 5, 0),
            EndTime = new TimeSpan(18, 0, 0),
            PartySize = 3
        };
        var response = await _client.PostAsJsonAsync("/api/v1/reservations", bufferViolationRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task CreateReservation_AfterBuffer_Succeeds()
    {
        // Arrange - Create first reservation ending at 16:00
        var tomorrow = DateTime.UtcNow.Date.AddDays(1);
        var firstRequest = new CreateReservationRequest
        {
            CustomerId = _customerId1,
            TableId = _tableId1,
            ReservationDate = tomorrow,
            StartTime = new TimeSpan(14, 0, 0),
            EndTime = new TimeSpan(16, 0, 0),
            PartySize = 4
        };
        await _client.PostAsJsonAsync("/api/v1/reservations", firstRequest);

        // Act - Create reservation starting at 16:15 (exactly at buffer boundary)
        var validRequest = new CreateReservationRequest
        {
            CustomerId = _customerId2,
            TableId = _tableId1,
            ReservationDate = tomorrow,
            StartTime = new TimeSpan(16, 15, 0),
            EndTime = new TimeSpan(18, 0, 0),
            PartySize = 3
        };
        var response = await _client.PostAsJsonAsync("/api/v1/reservations", validRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task GetReservation_WithValidId_ReturnsReservation()
    {
        // Arrange - Create a reservation
        var tomorrow = DateTime.UtcNow.Date.AddDays(1);
        var request = new CreateReservationRequest
        {
            CustomerId = _customerId1,
            TableId = _tableId1,
            ReservationDate = tomorrow,
            StartTime = new TimeSpan(14, 0, 0),
            EndTime = new TimeSpan(16, 0, 0),
            PartySize = 4
        };
        var createResponse = await _client.PostAsJsonAsync("/api/v1/reservations", request);
        var created = await createResponse.Content.ReadFromJsonAsync<ReservationDto>();

        // Act
        var response = await _client.GetAsync($"/api/v1/reservations/{created!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var reservation = await response.Content.ReadFromJsonAsync<ReservationDto>();
        reservation.Should().NotBeNull();
        reservation!.Id.Should().Be(created.Id);
    }

    [Fact]
    public async Task GetReservation_WithInvalidId_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync($"/api/v1/reservations/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetReservations_ForCustomer_ReturnsAllReservations()
    {
        // Arrange - Create multiple reservations
        var tomorrow = DateTime.UtcNow.Date.AddDays(1);
        var request1 = new CreateReservationRequest
        {
            CustomerId = _customerId1,
            TableId = _tableId1,
            ReservationDate = tomorrow,
            StartTime = new TimeSpan(14, 0, 0),
            EndTime = new TimeSpan(16, 0, 0),
            PartySize = 4
        };
        var request2 = new CreateReservationRequest
        {
            CustomerId = _customerId1,
            TableId = _tableId2,
            ReservationDate = tomorrow.AddDays(1),
            StartTime = new TimeSpan(18, 0, 0),
            EndTime = new TimeSpan(20, 0, 0),
            PartySize = 5
        };
        await _client.PostAsJsonAsync("/api/v1/reservations", request1);
        await _client.PostAsJsonAsync("/api/v1/reservations", request2);

        // Act
        var response = await _client.GetAsync($"/api/v1/reservations?customerId={_customerId1}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var reservations = await response.Content.ReadFromJsonAsync<List<ReservationDto>>();
        reservations.Should().NotBeNull();
        reservations!.Count.Should().Be(2);
        reservations.All(r => r.CustomerId == _customerId1).Should().BeTrue();
    }

    [Fact]
    public async Task UpdateReservation_WithValidData_ReturnsUpdated()
    {
        // Arrange - Create a reservation
        var tomorrow = DateTime.UtcNow.Date.AddDays(1);
        var createRequest = new CreateReservationRequest
        {
            CustomerId = _customerId1,
            TableId = _tableId1,
            ReservationDate = tomorrow,
            StartTime = new TimeSpan(14, 0, 0),
            EndTime = new TimeSpan(16, 0, 0),
            PartySize = 4
        };
        var createResponse = await _client.PostAsJsonAsync("/api/v1/reservations", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<ReservationDto>();

        // Act - Update the reservation
        var updateRequest = new UpdateReservationRequest
        {
            StartTime = new TimeSpan(15, 0, 0),
            EndTime = new TimeSpan(17, 0, 0),
            PartySize = 3
        };
        var response = await _client.PutAsJsonAsync($"/api/v1/reservations/{created!.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await response.Content.ReadFromJsonAsync<ReservationDto>();
        updated.Should().NotBeNull();
        updated!.StartTime.Should().Be(new TimeSpan(15, 0, 0));
        updated.EndTime.Should().Be(new TimeSpan(17, 0, 0));
        updated.PartySize.Should().Be(3);
    }

    [Fact]
    public async Task UpdateReservation_WithConflict_ReturnsConflict()
    {
        // Arrange - Create two reservations
        var tomorrow = DateTime.UtcNow.Date.AddDays(1);
        var request1 = new CreateReservationRequest
        {
            CustomerId = _customerId1,
            TableId = _tableId1,
            ReservationDate = tomorrow,
            StartTime = new TimeSpan(14, 0, 0),
            EndTime = new TimeSpan(16, 0, 0),
            PartySize = 4
        };
        var request2 = new CreateReservationRequest
        {
            CustomerId = _customerId2,
            TableId = _tableId1,
            ReservationDate = tomorrow,
            StartTime = new TimeSpan(18, 0, 0),
            EndTime = new TimeSpan(20, 0, 0),
            PartySize = 3
        };
        await _client.PostAsJsonAsync("/api/v1/reservations", request1);
        var response2 = await _client.PostAsJsonAsync("/api/v1/reservations", request2);
        var reservation2 = await response2.Content.ReadFromJsonAsync<ReservationDto>();

        // Act - Try to update second reservation to overlap with first
        var updateRequest = new UpdateReservationRequest
        {
            StartTime = new TimeSpan(15, 0, 0), // Would overlap with first reservation
            EndTime = new TimeSpan(17, 0, 0)
        };
        var response = await _client.PutAsJsonAsync($"/api/v1/reservations/{reservation2!.Id}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task CancelReservation_WithValidId_ReturnsNoContent()
    {
        // Arrange - Create a reservation
        var tomorrow = DateTime.UtcNow.Date.AddDays(1);
        var request = new CreateReservationRequest
        {
            CustomerId = _customerId1,
            TableId = _tableId1,
            ReservationDate = tomorrow,
            StartTime = new TimeSpan(14, 0, 0),
            EndTime = new TimeSpan(16, 0, 0),
            PartySize = 4
        };
        var createResponse = await _client.PostAsJsonAsync("/api/v1/reservations", request);
        var created = await createResponse.Content.ReadFromJsonAsync<ReservationDto>();

        // Act
        var response = await _client.DeleteAsync($"/api/v1/reservations/{created!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify reservation is cancelled
        var getResponse = await _client.GetAsync($"/api/v1/reservations/{created.Id}");
        var cancelled = await getResponse.Content.ReadFromJsonAsync<ReservationDto>();
        cancelled!.Status.Should().Be("Cancelled");
    }

    [Fact]
    public async Task CheckInReservation_WithValidId_ReturnsUpdated()
    {
        // Arrange - Create a reservation
        var tomorrow = DateTime.UtcNow.Date.AddDays(1);
        var request = new CreateReservationRequest
        {
            CustomerId = _customerId1,
            TableId = _tableId1,
            ReservationDate = tomorrow,
            StartTime = new TimeSpan(14, 0, 0),
            EndTime = new TimeSpan(16, 0, 0),
            PartySize = 4
        };
        var createResponse = await _client.PostAsJsonAsync("/api/v1/reservations", request);
        var created = await createResponse.Content.ReadFromJsonAsync<ReservationDto>();

        // Act
        var response = await _client.PostAsync($"/api/v1/reservations/{created!.Id}/check-in", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var checkedIn = await response.Content.ReadFromJsonAsync<ReservationDto>();
        checkedIn!.Status.Should().Be("CheckedIn");
    }

    [Fact]
    public async Task CheckInReservation_AlreadyCheckedIn_ReturnsBadRequest()
    {
        // Arrange - Create and check in a reservation
        var tomorrow = DateTime.UtcNow.Date.AddDays(1);
        var request = new CreateReservationRequest
        {
            CustomerId = _customerId1,
            TableId = _tableId1,
            ReservationDate = tomorrow,
            StartTime = new TimeSpan(14, 0, 0),
            EndTime = new TimeSpan(16, 0, 0),
            PartySize = 4
        };
        var createResponse = await _client.PostAsJsonAsync("/api/v1/reservations", request);
        var created = await createResponse.Content.ReadFromJsonAsync<ReservationDto>();
        await _client.PostAsync($"/api/v1/reservations/{created!.Id}/check-in", null);

        // Act - Try to check in again
        var response = await _client.PostAsync($"/api/v1/reservations/{created.Id}/check-in", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetAvailability_WithValidParameters_ReturnsAvailableTables()
    {
        // Arrange
        var tomorrow = DateTime.UtcNow.Date.AddDays(1);
        
        // Act
        var response = await _client.GetAsync(
            $"/api/v1/reservations/availability?date={tomorrow:yyyy-MM-dd}&startTime=14:00:00&endTime=16:00:00&partySize=4");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var tables = await response.Content.ReadFromJsonAsync<List<AvailableTableDto>>();
        tables.Should().NotBeNull();
        tables!.Count.Should().BeGreaterThan(0);
        tables.All(t => t.SeatingCapacity >= 4).Should().BeTrue();
    }

    [Fact]
    public async Task GetAvailability_ExcludesConflictingTables_ReturnsOnlyAvailable()
    {
        // Arrange - Create a reservation for table 1
        var tomorrow = DateTime.UtcNow.Date.AddDays(1);
        var request = new CreateReservationRequest
        {
            CustomerId = _customerId1,
            TableId = _tableId1,
            ReservationDate = tomorrow,
            StartTime = new TimeSpan(14, 0, 0),
            EndTime = new TimeSpan(16, 0, 0),
            PartySize = 4
        };
        await _client.PostAsJsonAsync("/api/v1/reservations", request);

        // Act - Query availability for overlapping time
        var response = await _client.GetAsync(
            $"/api/v1/reservations/availability?date={tomorrow:yyyy-MM-dd}&startTime=15:00:00&endTime=17:00:00&partySize=4");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var tables = await response.Content.ReadFromJsonAsync<List<AvailableTableDto>>();
        tables.Should().NotBeNull();
        tables!.Should().NotContain(t => t.Id == _tableId1); // Table 1 should not be available
        tables.Should().Contain(t => t.Id == _tableId2); // Table 2 should still be available
    }

    [Fact]
    public async Task GetAvailability_WithPastDate_ReturnsBadRequest()
    {
        // Arrange
        var yesterday = DateTime.UtcNow.Date.AddDays(-1);

        // Act
        var response = await _client.GetAsync(
            $"/api/v1/reservations/availability?date={yesterday:yyyy-MM-dd}&startTime=14:00:00&endTime=16:00:00&partySize=4");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
