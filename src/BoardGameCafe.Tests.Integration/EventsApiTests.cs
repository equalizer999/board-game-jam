using BoardGameCafe.Api.Data;
using BoardGameCafe.Api.Features.Events;
using BoardGameCafe.Domain;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace BoardGameCafe.Tests.Integration;

public class EventsApiTests : IClassFixture<ReservationsApiTestFixture>, IAsyncLifetime
{
    private readonly ReservationsApiTestFixture _factory;
    private HttpClient _client = null!;

    // Test data IDs
    private Guid _customerId1;
    private Guid _customerId2;
    private Guid _customerId3;
    private Guid _eventId1;

    public EventsApiTests(ReservationsApiTestFixture factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        _client = _factory.CreateClient();

        // Create a scope to seed test data for this test
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BoardGameCafeDbContext>();

        // Clean up any existing data from previous tests
        db.EventRegistrations.RemoveRange(db.EventRegistrations);
        db.Events.RemoveRange(db.Events);
        db.Customers.RemoveRange(db.Customers);
        await db.SaveChangesAsync();

        // Seed test customers
        var customer1 = new Customer
        {
            Id = Guid.NewGuid(),
            Email = "event-test1@example.com",
            FirstName = "Alice",
            LastName = "Johnson",
            JoinedDate = DateTime.UtcNow
        };

        var customer2 = new Customer
        {
            Id = Guid.NewGuid(),
            Email = "event-test2@example.com",
            FirstName = "Bob",
            LastName = "Smith",
            JoinedDate = DateTime.UtcNow
        };

        var customer3 = new Customer
        {
            Id = Guid.NewGuid(),
            Email = "event-test3@example.com",
            FirstName = "Charlie",
            LastName = "Brown",
            JoinedDate = DateTime.UtcNow
        };

        db.Customers.AddRange(customer1, customer2, customer3);
        await db.SaveChangesAsync();

        _customerId1 = customer1.Id;
        _customerId2 = customer2.Id;
        _customerId3 = customer3.Id;

        // Seed a test event
        var event1 = new Event
        {
            Id = Guid.NewGuid(),
            Title = "Catan Tournament",
            Description = "Competitive Catan tournament",
            EventDate = DateTime.UtcNow.AddDays(7),
            DurationMinutes = 180,
            MaxParticipants = 2, // Small capacity for testing
            TicketPrice = 15.00m,
            EventType = EventType.Tournament,
            RequiresRegistration = true,
            ImageUrl = "https://example.com/catan.jpg"
        };

        db.Events.Add(event1);
        await db.SaveChangesAsync();

        _eventId1 = event1.Id;
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetUpcomingEvents_ReturnsEvents()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/events");
        var events = await response.Content.ReadFromJsonAsync<List<EventDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        events.Should().NotBeNull();
        events.Should().ContainSingle();
        events![0].Title.Should().Be("Catan Tournament");
        events[0].CurrentParticipants.Should().Be(0);
    }

    [Fact]
    public async Task GetEvent_WithValidId_ReturnsEvent()
    {
        // Act
        var response = await _client.GetAsync($"/api/v1/events/{_eventId1}");
        var eventDto = await response.Content.ReadFromJsonAsync<EventDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        eventDto.Should().NotBeNull();
        eventDto!.Title.Should().Be("Catan Tournament");
        eventDto.MaxParticipants.Should().Be(2);
        eventDto.CurrentParticipants.Should().Be(0);
    }

    [Fact]
    public async Task GetEvent_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/v1/events/{invalidId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateEvent_WithValidData_ReturnsCreated()
    {
        // Arrange
        var request = new CreateEventRequest
        {
            Title = "Game Night",
            Description = "Casual game night",
            EventDate = DateTime.UtcNow.AddDays(14),
            DurationMinutes = 240,
            MaxParticipants = 20,
            TicketPrice = 5.00m,
            EventType = EventType.GameNight,
            RequiresRegistration = true,
            ImageUrl = "https://example.com/gamenight.jpg"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/events", request);
        var eventDto = await response.Content.ReadFromJsonAsync<EventDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        eventDto.Should().NotBeNull();
        eventDto!.Title.Should().Be("Game Night");
        eventDto.MaxParticipants.Should().Be(20);
        eventDto.CurrentParticipants.Should().Be(0);
    }

    [Fact]
    public async Task CreateEvent_WithPastDate_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateEventRequest
        {
            Title = "Past Event",
            Description = "This is in the past",
            EventDate = DateTime.UtcNow.AddDays(-1),
            DurationMinutes = 120,
            MaxParticipants = 10,
            TicketPrice = 10.00m,
            EventType = EventType.Workshop,
            RequiresRegistration = true
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/events", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RegisterForEvent_WithValidData_ReturnsCreated()
    {
        // Arrange
        var request = new RegisterForEventRequest
        {
            CustomerId = _customerId1
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/v1/events/{_eventId1}/register", request);
        var registration = await response.Content.ReadFromJsonAsync<EventRegistrationDto>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        registration.Should().NotBeNull();
        registration!.EventId.Should().Be(_eventId1);
        registration.CustomerId.Should().Be(_customerId1);
        registration.Status.Should().Be(RegistrationStatus.Registered);
    }

    [Fact]
    public async Task RegisterForEvent_TwiceForSameCustomer_ReturnsConflict()
    {
        // Arrange
        var request = new RegisterForEventRequest
        {
            CustomerId = _customerId1
        };

        // Act - Register once
        await _client.PostAsJsonAsync($"/api/v1/events/{_eventId1}/register", request);

        // Act - Try to register again
        var response = await _client.PostAsJsonAsync($"/api/v1/events/{_eventId1}/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task RegisterForEvent_WhenEventFull_ReturnsConflict()
    {
        // Arrange - Register first two customers (max capacity is 2)
        await _client.PostAsJsonAsync($"/api/v1/events/{_eventId1}/register",
            new RegisterForEventRequest { CustomerId = _customerId1 });
        await _client.PostAsJsonAsync($"/api/v1/events/{_eventId1}/register",
            new RegisterForEventRequest { CustomerId = _customerId2 });

        // Act - Try to register third customer
        var request = new RegisterForEventRequest
        {
            CustomerId = _customerId3
        };
        var response = await _client.PostAsJsonAsync($"/api/v1/events/{_eventId1}/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task CancelRegistration_AfterCancel_OpensSpot()
    {
        // Arrange - Fill the event to capacity
        await _client.PostAsJsonAsync($"/api/v1/events/{_eventId1}/register",
            new RegisterForEventRequest { CustomerId = _customerId1 });
        await _client.PostAsJsonAsync($"/api/v1/events/{_eventId1}/register",
            new RegisterForEventRequest { CustomerId = _customerId2 });

        // Act - Cancel one registration
        var cancelResponse = await _client.DeleteAsync($"/api/v1/events/{_eventId1}/register?customerId={_customerId1}");

        // Assert - Cancel succeeded
        cancelResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Act - Try to register third customer (should now succeed)
        var registerResponse = await _client.PostAsJsonAsync($"/api/v1/events/{_eventId1}/register",
            new RegisterForEventRequest { CustomerId = _customerId3 });

        // Assert - Registration succeeded
        registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task GetParticipants_ReturnsRegistrations()
    {
        // Arrange - Register two customers
        await _client.PostAsJsonAsync($"/api/v1/events/{_eventId1}/register",
            new RegisterForEventRequest { CustomerId = _customerId1 });
        await _client.PostAsJsonAsync($"/api/v1/events/{_eventId1}/register",
            new RegisterForEventRequest { CustomerId = _customerId2 });

        // Act
        var response = await _client.GetAsync($"/api/v1/events/{_eventId1}/participants");
        var participants = await response.Content.ReadFromJsonAsync<List<EventRegistrationDto>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        participants.Should().NotBeNull();
        participants.Should().HaveCount(2);
        participants.Should().Contain(p => p.CustomerId == _customerId1);
        participants.Should().Contain(p => p.CustomerId == _customerId2);
    }

    [Fact]
    public async Task GetParticipants_ForNonExistentEvent_ReturnsNotFound()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/v1/events/{invalidId}/participants");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetEvent_AfterRegistration_ShowsUpdatedParticipantCount()
    {
        // Act - Get event before registration
        var responseBefore = await _client.GetAsync($"/api/v1/events/{_eventId1}");
        var eventBefore = await responseBefore.Content.ReadFromJsonAsync<EventDto>();

        // Arrange - Register a customer
        await _client.PostAsJsonAsync($"/api/v1/events/{_eventId1}/register",
            new RegisterForEventRequest { CustomerId = _customerId1 });

        // Act - Get event after registration
        var responseAfter = await _client.GetAsync($"/api/v1/events/{_eventId1}");
        var eventAfter = await responseAfter.Content.ReadFromJsonAsync<EventDto>();

        // Assert
        eventBefore!.CurrentParticipants.Should().Be(0);
        eventAfter!.CurrentParticipants.Should().Be(1);
    }

    [Fact]
    public async Task CancelRegistration_WithInvalidCustomer_ReturnsNotFound()
    {
        // Arrange
        var invalidCustomerId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/api/v1/events/{_eventId1}/register?customerId={invalidCustomerId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
