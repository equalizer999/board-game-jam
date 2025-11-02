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

    #region Order Pricing Tests

    [Fact]
    public void Order_CalculateSubtotal_ShouldSumAllItems()
    {
        // Arrange
        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            Items = new List<OrderItem>
            {
                new OrderItem { Quantity = 2, UnitPrice = 5.50m },
                new OrderItem { Quantity = 1, UnitPrice = 8.50m },
                new OrderItem { Quantity = 3, UnitPrice = 4.00m }
            }
        };

        // Act
        order.CalculateSubtotal();

        // Assert
        order.Subtotal.Should().Be(31.50m); // (2*5.50) + (1*8.50) + (3*4.00)
    }

    [Fact]
    public void Order_CalculateTax_ShouldApply8PercentOnFood()
    {
        // Arrange
        var menuItem = new MenuItem
        {
            Id = Guid.NewGuid(),
            Name = "Pizza",
            Category = MenuCategory.Meals,
            Price = 10.00m
        };

        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            Items = new List<OrderItem>
            {
                new OrderItem 
                { 
                    Quantity = 1, 
                    UnitPrice = 10.00m,
                    MenuItem = menuItem 
                }
            }
        };

        // Act
        order.CalculateTax();

        // Assert
        order.TaxAmount.Should().Be(0.80m); // 10.00 * 0.08
    }

    [Fact]
    public void Order_CalculateTax_ShouldApply10PercentOnAlcohol()
    {
        // Arrange
        var menuItem = new MenuItem
        {
            Id = Guid.NewGuid(),
            Name = "Beer",
            Category = MenuCategory.Alcohol,
            Price = 7.50m
        };

        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            Items = new List<OrderItem>
            {
                new OrderItem 
                { 
                    Quantity = 2, 
                    UnitPrice = 7.50m,
                    MenuItem = menuItem 
                }
            }
        };

        // Act
        order.CalculateTax();

        // Assert
        order.TaxAmount.Should().Be(1.50m); // (2 * 7.50) * 0.10
    }

    [Fact]
    public void Order_CalculateTax_ShouldHandleMixedCategories()
    {
        // Arrange
        var foodItem = new MenuItem
        {
            Id = Guid.NewGuid(),
            Name = "Pizza",
            Category = MenuCategory.Meals,
            Price = 15.00m
        };

        var alcoholItem = new MenuItem
        {
            Id = Guid.NewGuid(),
            Name = "Wine",
            Category = MenuCategory.Alcohol,
            Price = 9.00m
        };

        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            Items = new List<OrderItem>
            {
                new OrderItem 
                { 
                    Quantity = 1, 
                    UnitPrice = 15.00m,
                    MenuItem = foodItem 
                },
                new OrderItem 
                { 
                    Quantity = 2, 
                    UnitPrice = 9.00m,
                    MenuItem = alcoholItem 
                }
            }
        };

        // Act
        order.CalculateTax();

        // Assert
        // Food tax: 15.00 * 0.08 = 1.20
        // Alcohol tax: (2 * 9.00) * 0.10 = 1.80
        // Total: 3.00
        order.TaxAmount.Should().Be(3.00m);
    }

    [Fact]
    public void Order_CalculateMemberDiscount_BronzeMemberGets5Percent()
    {
        // Arrange
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Email = "test@test.com",
            FirstName = "Test",
            LastName = "User",
            MembershipTier = MembershipTier.Bronze
        };

        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customer.Id,
            Customer = customer,
            Subtotal = 100.00m
        };

        // Act
        order.CalculateMemberDiscount();

        // Assert
        order.DiscountAmount.Should().Be(5.00m); // 100 * 0.05
    }

    [Fact]
    public void Order_CalculateMemberDiscount_SilverMemberGets10Percent()
    {
        // Arrange
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Email = "test@test.com",
            FirstName = "Test",
            LastName = "User",
            MembershipTier = MembershipTier.Silver
        };

        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customer.Id,
            Customer = customer,
            Subtotal = 100.00m
        };

        // Act
        order.CalculateMemberDiscount();

        // Assert
        order.DiscountAmount.Should().Be(10.00m); // 100 * 0.10
    }

    [Fact]
    public void Order_CalculateMemberDiscount_GoldMemberGets15Percent()
    {
        // Arrange
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Email = "test@test.com",
            FirstName = "Test",
            LastName = "User",
            MembershipTier = MembershipTier.Gold
        };

        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customer.Id,
            Customer = customer,
            Subtotal = 100.00m
        };

        // Act
        order.CalculateMemberDiscount();

        // Assert
        order.DiscountAmount.Should().Be(15.00m); // 100 * 0.15
    }

    [Fact]
    public void Order_CalculateMemberDiscount_NonMemberGetsNoDiscount()
    {
        // Arrange
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Email = "test@test.com",
            FirstName = "Test",
            LastName = "User",
            MembershipTier = MembershipTier.None
        };

        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customer.Id,
            Customer = customer,
            Subtotal = 100.00m
        };

        // Act
        order.CalculateMemberDiscount();

        // Assert
        order.DiscountAmount.Should().Be(0.00m);
    }

    [Fact]
    public void Order_CalculateTotal_ShouldBeSubtotalMinusDiscountPlusTax()
    {
        // Arrange
        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            Subtotal = 100.00m,
            DiscountAmount = 10.00m,
            TaxAmount = 7.20m
        };

        // Act
        order.CalculateTotal();

        // Assert
        order.TotalAmount.Should().Be(97.20m); // 100 - 10 + 7.20
    }

    [Fact]
    public void Order_RecalculateTotals_ShouldCalculateAllAmountsCorrectly()
    {
        // Arrange
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Email = "test@test.com",
            FirstName = "Test",
            LastName = "User",
            MembershipTier = MembershipTier.Silver // 10% discount
        };

        var foodItem = new MenuItem
        {
            Id = Guid.NewGuid(),
            Name = "Pizza",
            Category = MenuCategory.Meals,
            Price = 20.00m
        };

        var alcoholItem = new MenuItem
        {
            Id = Guid.NewGuid(),
            Name = "Beer",
            Category = MenuCategory.Alcohol,
            Price = 8.00m
        };

        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customer.Id,
            Customer = customer,
            Items = new List<OrderItem>
            {
                new OrderItem 
                { 
                    Quantity = 1, 
                    UnitPrice = 20.00m,
                    MenuItem = foodItem 
                },
                new OrderItem 
                { 
                    Quantity = 2, 
                    UnitPrice = 8.00m,
                    MenuItem = alcoholItem 
                }
            }
        };

        // Act
        order.RecalculateTotals();

        // Assert
        order.Subtotal.Should().Be(36.00m); // 20 + (2*8)
        order.DiscountAmount.Should().Be(3.60m); // 36 * 0.10
        // Tax: Food 20 * 0.08 = 1.60, Alcohol 16 * 0.10 = 1.60, Total = 3.20
        order.TaxAmount.Should().Be(3.20m);
        order.TotalAmount.Should().Be(35.60m); // 36 - 3.60 + 3.20
    }

    #endregion
}

