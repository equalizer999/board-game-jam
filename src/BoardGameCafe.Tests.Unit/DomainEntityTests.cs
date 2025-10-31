using BoardGameCafe.Domain;
using FluentAssertions;

namespace BoardGameCafe.Tests.Unit;

public class DomainEntityTests
{
    [Fact]
    public void Game_IsAvailable_ReturnsTrue_WhenCopiesAvailable()
    {
        // Arrange
        var game = new Game
        {
            Id = Guid.NewGuid(),
            Title = "Catan",
            CopiesOwned = 3,
            CopiesInUse = 1
        };

        // Act
        var isAvailable = game.IsAvailable;

        // Assert
        isAvailable.Should().BeTrue();
    }

    [Fact]
    public void Game_IsAvailable_ReturnsFalse_WhenNoCopiesAvailable()
    {
        // Arrange
        var game = new Game
        {
            Id = Guid.NewGuid(),
            Title = "Pandemic",
            CopiesOwned = 2,
            CopiesInUse = 2
        };

        // Act
        var isAvailable = game.IsAvailable;

        // Assert
        isAvailable.Should().BeFalse();
    }

    [Fact]
    public void Customer_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com"
        };

        // Assert
        customer.FirstName.Should().Be("John");
        customer.LastName.Should().Be("Doe");
        customer.Email.Should().Be("john@example.com");
        customer.LoyaltyPoints.Should().Be(0);
    }

    [Fact]
    public void MenuItem_ShouldInitializeWithDefaults()
    {
        // Arrange & Act
        var menuItem = new MenuItem
        {
            Id = Guid.NewGuid(),
            Name = "Espresso",
            Description = "Strong black coffee",
            Category = MenuCategory.Coffee,
            Price = 3.50m,
            IsAvailable = true
        };

        // Assert
        menuItem.Name.Should().Be("Espresso");
        menuItem.Category.Should().Be(MenuCategory.Coffee);
        menuItem.Price.Should().Be(3.50m);
        menuItem.IsAvailable.Should().BeTrue();
    }

    [Fact]
    public void Order_ShouldInitializeCollections()
    {
        // Arrange & Act
        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Draft
        };

        // Assert
        order.Items.Should().NotBeNull();
        order.Items.Should().BeEmpty();
    }

    [Fact]
    public void Event_ShouldInitializeCollections()
    {
        // Arrange & Act
        var evt = new Event
        {
            Id = Guid.NewGuid(),
            Title = "Game Tournament",
            Description = "Monthly board game tournament",
            EventDate = DateTime.UtcNow.AddDays(7),
            MaxParticipants = 20,
            EventType = EventType.Tournament
        };

        // Assert
        evt.Registrations.Should().NotBeNull();
        evt.Registrations.Should().BeEmpty();
        evt.CurrentParticipants.Should().Be(0);
    }

    [Fact]
    public void GameSession_ShouldTrackReturnStatus()
    {
        // Arrange & Act
        var session = new GameSession
        {
            Id = Guid.NewGuid(),
            GameId = Guid.NewGuid(),
            ReservationId = Guid.NewGuid(),
            CheckedOutAt = DateTime.UtcNow,
            DueBackAt = DateTime.UtcNow.AddHours(2),
            Condition = GameCondition.Excellent
        };

        // Assert
        session.ReturnedAt.Should().BeNull();
        session.LateFeeApplied.Should().Be(0);
    }
}
