using BoardGameCafe.Api.Features.Orders;
using BoardGameCafe.Domain;
using FluentAssertions;

namespace BoardGameCafe.Tests.Unit;

public class OrderCalculationServiceTests
{
    private readonly OrderCalculationService _service = new();

    [Fact]
    public void CalculateLoyaltyPointsDiscount_ShouldReturn1DollarPer100Points()
    {
        // Arrange & Act
        var discount = _service.CalculateLoyaltyPointsDiscount(100);

        // Assert
        discount.Should().Be(1.00m);
    }

    [Fact]
    public void CalculateLoyaltyPointsDiscount_ShouldReturn10DollarsPer1000Points()
    {
        // Arrange & Act
        var discount = _service.CalculateLoyaltyPointsDiscount(1000);

        // Assert
        discount.Should().Be(10.00m);
    }

    [Fact]
    public void CalculateLoyaltyPointsDiscount_ShouldReturnProportionalAmountForPartialPoints()
    {
        // Arrange & Act
        var discount = _service.CalculateLoyaltyPointsDiscount(250);

        // Assert
        discount.Should().Be(2.50m);
    }

    [Fact]
    public void CalculateLoyaltyPointsDiscount_ShouldThrowExceptionForNegativePoints()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentException>(() => _service.CalculateLoyaltyPointsDiscount(-100));
    }

    [Fact]
    public void CalculateLoyaltyPointsEarned_ShouldReturn1PointPerDollar()
    {
        // Arrange & Act
        var points = _service.CalculateLoyaltyPointsEarned(50.00m);

        // Assert
        points.Should().Be(50);
    }

    [Fact]
    public void CalculateLoyaltyPointsEarned_ShouldRoundDown()
    {
        // Arrange & Act
        var points = _service.CalculateLoyaltyPointsEarned(50.99m);

        // Assert
        points.Should().Be(50); // Should round down
    }

    [Fact]
    public void CalculateLoyaltyPointsEarned_ShouldReturn0ForNegativeAmount()
    {
        // Arrange & Act
        var points = _service.CalculateLoyaltyPointsEarned(-10.00m);

        // Assert
        points.Should().Be(0);
    }

    [Fact]
    public void ValidateLoyaltyPointsRedemption_ShouldReturnTrueWhenCustomerHasEnoughPoints()
    {
        // Arrange
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Email = "test@test.com",
            FirstName = "Test",
            LastName = "User",
            LoyaltyPoints = 500
        };

        // Act
        var isValid = _service.ValidateLoyaltyPointsRedemption(customer, 300);

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public void ValidateLoyaltyPointsRedemption_ShouldReturnFalseWhenCustomerDoesNotHaveEnoughPoints()
    {
        // Arrange
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Email = "test@test.com",
            FirstName = "Test",
            LastName = "User",
            LoyaltyPoints = 200
        };

        // Act
        var isValid = _service.ValidateLoyaltyPointsRedemption(customer, 300);

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateLoyaltyPointsRedemption_ShouldReturnFalseForNegativePoints()
    {
        // Arrange
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Email = "test@test.com",
            FirstName = "Test",
            LastName = "User",
            LoyaltyPoints = 500
        };

        // Act
        var isValid = _service.ValidateLoyaltyPointsRedemption(customer, -100);

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public void GetMemberDiscountPercentage_ShouldReturn5PercentForBronze()
    {
        // Arrange & Act
        var percentage = _service.GetMemberDiscountPercentage(MembershipTier.Bronze);

        // Assert
        percentage.Should().Be(0.05m);
    }

    [Fact]
    public void GetMemberDiscountPercentage_ShouldReturn10PercentForSilver()
    {
        // Arrange & Act
        var percentage = _service.GetMemberDiscountPercentage(MembershipTier.Silver);

        // Assert
        percentage.Should().Be(0.10m);
    }

    [Fact]
    public void GetMemberDiscountPercentage_ShouldReturn15PercentForGold()
    {
        // Arrange & Act
        var percentage = _service.GetMemberDiscountPercentage(MembershipTier.Gold);

        // Assert
        percentage.Should().Be(0.15m);
    }

    [Fact]
    public void GetMemberDiscountPercentage_ShouldReturn0ForNone()
    {
        // Arrange & Act
        var percentage = _service.GetMemberDiscountPercentage(MembershipTier.None);

        // Assert
        percentage.Should().Be(0m);
    }

    [Fact]
    public void CalculateOrderTotals_ShouldCalculateSubtotal()
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

        var menuItem = new MenuItem
        {
            Id = Guid.NewGuid(),
            Name = "Coffee",
            Category = MenuCategory.Coffee,
            Price = 5.00m
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
                    Quantity = 2, 
                    UnitPrice = 5.00m,
                    MenuItem = menuItem 
                }
            }
        };

        // Act
        _service.CalculateOrderTotals(order);

        // Assert
        order.Subtotal.Should().Be(10.00m);
    }

    [Fact]
    public void CalculateOrderTotals_ShouldApplyMemberDiscount()
    {
        // Arrange
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Email = "test@test.com",
            FirstName = "Test",
            LastName = "User",
            MembershipTier = MembershipTier.Gold // 15% discount
        };

        var menuItem = new MenuItem
        {
            Id = Guid.NewGuid(),
            Name = "Pizza",
            Category = MenuCategory.Meals,
            Price = 20.00m
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
                    MenuItem = menuItem 
                }
            }
        };

        // Act
        _service.CalculateOrderTotals(order);

        // Assert
        order.DiscountAmount.Should().Be(3.00m); // 20 * 0.15
    }

    [Fact]
    public void CalculateOrderTotals_ShouldApplyLoyaltyPointsDiscount()
    {
        // Arrange
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Email = "test@test.com",
            FirstName = "Test",
            LastName = "User",
            MembershipTier = MembershipTier.None,
            LoyaltyPoints = 500
        };

        var menuItem = new MenuItem
        {
            Id = Guid.NewGuid(),
            Name = "Pizza",
            Category = MenuCategory.Meals,
            Price = 20.00m
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
                    MenuItem = menuItem 
                }
            }
        };

        // Act
        _service.CalculateOrderTotals(order, loyaltyPointsToRedeem: 200);

        // Assert
        // Member discount: 0
        // Loyalty discount: 200 * 0.01 = 2.00
        // Total discount: 2.00
        order.DiscountAmount.Should().Be(2.00m);
    }

    [Fact]
    public void CalculateOrderTotals_ShouldCombineMemberAndLoyaltyDiscounts()
    {
        // Arrange
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Email = "test@test.com",
            FirstName = "Test",
            LastName = "User",
            MembershipTier = MembershipTier.Silver, // 10% discount
            LoyaltyPoints = 500
        };

        var menuItem = new MenuItem
        {
            Id = Guid.NewGuid(),
            Name = "Pizza",
            Category = MenuCategory.Meals,
            Price = 100.00m
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
                    UnitPrice = 100.00m,
                    MenuItem = menuItem 
                }
            }
        };

        // Act
        _service.CalculateOrderTotals(order, loyaltyPointsToRedeem: 300);

        // Assert
        // Member discount: 100 * 0.10 = 10.00
        // Loyalty discount: 300 * 0.01 = 3.00
        // Total discount: 13.00
        order.DiscountAmount.Should().Be(13.00m);
    }

    [Fact]
    public void CalculateOrderTotals_ShouldCalculateTaxCorrectly()
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
            Price = 10.00m
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
                    Quantity = 1, 
                    UnitPrice = 10.00m,
                    MenuItem = alcoholItem 
                }
            }
        };

        // Act
        _service.CalculateOrderTotals(order);

        // Assert
        // Food tax: 20 * 0.08 = 1.60
        // Alcohol tax: 10 * 0.10 = 1.00
        // Total tax: 2.60
        order.TaxAmount.Should().Be(2.60m);
    }

    [Fact]
    public void CalculateOrderTotals_ShouldPreventNegativeTotals()
    {
        // Arrange
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Email = "test@test.com",
            FirstName = "Test",
            LastName = "User",
            MembershipTier = MembershipTier.Gold, // 15% discount
            LoyaltyPoints = 10000
        };

        var menuItem = new MenuItem
        {
            Id = Guid.NewGuid(),
            Name = "Coffee",
            Category = MenuCategory.Coffee,
            Price = 5.00m
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
                    UnitPrice = 5.00m,
                    MenuItem = menuItem 
                }
            }
        };

        // Act - Try to redeem way more points than the order is worth
        _service.CalculateOrderTotals(order, loyaltyPointsToRedeem: 10000);

        // Assert - Should prevent negative total
        order.TotalAmount.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public void CalculateOrderTotals_ShouldCalculateCorrectFinalTotal()
    {
        // Arrange
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Email = "test@test.com",
            FirstName = "Test",
            LastName = "User",
            MembershipTier = MembershipTier.Silver, // 10% discount
            LoyaltyPoints = 500
        };

        var menuItem = new MenuItem
        {
            Id = Guid.NewGuid(),
            Name = "Pizza",
            Category = MenuCategory.Meals,
            Price = 50.00m
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
                    UnitPrice = 50.00m,
                    MenuItem = menuItem 
                }
            }
        };

        // Act
        _service.CalculateOrderTotals(order, loyaltyPointsToRedeem: 200);

        // Assert
        // Subtotal: 50.00
        // Member discount: 50 * 0.10 = 5.00
        // Loyalty discount: 200 * 0.01 = 2.00
        // Total discount: 7.00
        // After discount: 50 - 7 = 43.00
        // Tax: 43 * 0.08 = 3.44 (but tax is calculated on original items, not discounted amount)
        // Tax: 50 * 0.08 = 4.00
        // Total: 50 - 7 + 4 = 47.00
        order.Subtotal.Should().Be(50.00m);
        order.DiscountAmount.Should().Be(7.00m);
        order.TaxAmount.Should().Be(4.00m);
        order.TotalAmount.Should().Be(47.00m);
    }
}
