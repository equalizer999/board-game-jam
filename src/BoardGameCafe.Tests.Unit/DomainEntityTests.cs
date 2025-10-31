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
}
