using BoardGameCafe.Domain;
using BoardGameCafe.Tests.Unit.Builders;
using BoardGameCafe.Tests.Unit.TestUtilities;
using FluentAssertions;

namespace BoardGameCafe.Tests.Unit.Services;

/// <summary>
/// Example tests that verify Exercise 1 patterns work correctly
/// These demonstrate the testing patterns that workshop participants will use
/// </summary>
public class Exercise1ValidationTests
{
    private readonly OrderCalculationService _service = new();

    #region CalculateTax Tests (Exercise 1 - Step 2)

    [Theory]
    [InlineData(100, 8)] // $100 food = $8 tax
    [InlineData(50, 4)]  // $50 food = $4 tax
    [InlineData(0, 0)]   // $0 food = $0 tax
    public void CalculateTax_FoodItems_Returns8Percent(decimal itemTotal, decimal expectedTax)
    {
        // Arrange
        var menuItem = new MenuItem
        {
            Id = Guid.NewGuid(),
            Name = "Test Food",
            Category = MenuCategory.Meals, // Non-alcohol category gets 8% tax
            Price = itemTotal
        };
        var items = new List<OrderItem>
        {
            new OrderItem
            {
                Id = Guid.NewGuid(),
                MenuItem = menuItem,
                MenuItemId = menuItem.Id,
                Quantity = 1,
                UnitPrice = itemTotal
            }
        };

        // Act
        var tax = _service.CalculateTax(items);

        // Assert
        tax.Should().Be(expectedTax);
    }

    [Theory]
    [InlineData(100, 10)] // $100 alcohol = $10 tax
    [InlineData(50, 5)]   // $50 alcohol = $5 tax
    public void CalculateTax_AlcoholItems_Returns10Percent(decimal itemTotal, decimal expectedTax)
    {
        // Arrange
        var menuItem = new MenuItem
        {
            Id = Guid.NewGuid(),
            Name = "Test Alcohol",
            Category = MenuCategory.Alcohol,
            Price = itemTotal
        };
        var items = new List<OrderItem>
        {
            new OrderItem
            {
                Id = Guid.NewGuid(),
                MenuItem = menuItem,
                MenuItemId = menuItem.Id,
                Quantity = 1,
                UnitPrice = itemTotal
            }
        };

        // Act
        var tax = _service.CalculateTax(items);

        // Assert
        tax.Should().Be(expectedTax);
    }

    [Fact]
    public void CalculateTax_MixedCart_CalculatesCorrectly()
    {
        // Arrange - $100 food (8% = $8) + $50 alcohol (10% = $5) = $13 total tax
        var foodItem = new MenuItem
        {
            Id = Guid.NewGuid(),
            Name = "Food",
            Category = MenuCategory.Meals, // Non-alcohol category
            Price = 100
        };
        var alcoholItem = new MenuItem
        {
            Id = Guid.NewGuid(),
            Name = "Alcohol",
            Category = MenuCategory.Alcohol,
            Price = 50
        };
        var items = new List<OrderItem>
        {
            new OrderItem { Id = Guid.NewGuid(), MenuItem = foodItem, MenuItemId = foodItem.Id, Quantity = 1, UnitPrice = 100 },
            new OrderItem { Id = Guid.NewGuid(), MenuItem = alcoholItem, MenuItemId = alcoholItem.Id, Quantity = 1, UnitPrice = 50 }
        };

        // Act
        var tax = _service.CalculateTax(items);

        // Assert
        tax.Should().Be(13m);
    }

    #endregion

    #region CalculateDiscount Tests (Exercise 1 - Step 3)

    [Theory]
    [InlineData(100, MembershipTier.Bronze, 5)]  // 5% of $100
    [InlineData(100, MembershipTier.Silver, 10)] // 10% of $100
    [InlineData(100, MembershipTier.Gold, 15)]   // 15% of $100
    [InlineData(100, MembershipTier.None, 0)]    // 0% for non-members
    public void CalculateDiscount_AllMembershipTiers_ReturnsCorrectPercentage(
        decimal subtotal, 
        MembershipTier tier, 
        decimal expectedDiscount)
    {
        // Act
        var discount = _service.CalculateDiscount(subtotal, tier);

        // Assert
        discount.Should().Be(expectedDiscount);
    }

    #endregion

    #region ApplyLoyaltyPoints Tests (Exercise 1 - Step 4)

    [Fact]
    public void ApplyLoyaltyPoints_SufficientPoints_ReturnsDiscount()
    {
        // Arrange
        var customer = new CustomerBuilder()
            .WithLoyaltyPoints(1000)
            .Build();

        // Act
        var discount = _service.ApplyLoyaltyPoints(customer, 500);

        // Assert
        discount.Should().Be(5.00m); // 500 points = $5
    }

    [Fact]
    public void ApplyLoyaltyPoints_InsufficientPoints_ThrowsException()
    {
        // Arrange
        var customer = new CustomerBuilder()
            .WithLoyaltyPoints(100)
            .Build();

        // Act & Assert
        _service.Invoking(s => s.ApplyLoyaltyPoints(customer, 500))
            .Should().Throw<InvalidOperationException>()
            .WithMessage("*does not have enough loyalty points*");
    }

    [Fact]
    public void ApplyLoyaltyPoints_ZeroPoints_ReturnsZeroDiscount()
    {
        // Arrange
        var customer = new CustomerBuilder()
            .WithLoyaltyPoints(1000)
            .Build();

        // Act
        var discount = _service.ApplyLoyaltyPoints(customer, 0);

        // Assert
        discount.Should().Be(0m);
    }

    [Fact]
    public void ApplyLoyaltyPoints_NegativePoints_ThrowsException()
    {
        // Arrange
        var customer = new CustomerBuilder()
            .WithLoyaltyPoints(1000)
            .Build();

        // Act & Assert
        _service.Invoking(s => s.ApplyLoyaltyPoints(customer, -100))
            .Should().Throw<ArgumentException>()
            .WithMessage("*cannot be negative*");
    }

    #endregion

    #region Complete Order Calculation (Exercise 1 - Step 5)

    [Fact]
    public void CalculateOrderTotal_CompleteWorkflow_AllCalculationsCorrect()
    {
        // Arrange - Create order with food and alcohol
        var foodItem = new MenuItem
        {
            Id = Guid.NewGuid(),
            Name = "Sandwich",
            Category = MenuCategory.Meals, // Non-alcohol category
            Price = 10
        };
        var alcoholItem = new MenuItem
        {
            Id = Guid.NewGuid(),
            Name = "Beer",
            Category = MenuCategory.Alcohol,
            Price = 5
        };
        
        var customer = new CustomerBuilder()
            .WithMembershipTier(MembershipTier.Gold)
            .WithLoyaltyPoints(500)
            .Build();

        var order = new OrderBuilder()
            .WithCustomer(customer)
            .WithItem(foodItem, 2)    // $20 food
            .WithItem(alcoholItem, 2) // $10 alcohol
            .Build();

        // Act - Calculate full order
        // Subtotal: $30
        // Gold discount (15%): $4.50
        // Tax: $20 * 0.08 + $10 * 0.10 = $1.60 + $1.00 = $2.60
        // Loyalty points (500 points): $5.00
        // Total: $30 - $4.50 + $2.60 - $5.00 = $23.10
        _service.CalculateOrderTotals(order, loyaltyPointsToRedeem: 500);

        // Assert
        order.Subtotal.Should().Be(30m);
        order.DiscountAmount.Should().Be(9.50m); // $4.50 member + $5.00 loyalty
        order.TaxAmount.Should().Be(2.60m);
        order.TotalAmount.Should().Be(23.10m);
    }

    #endregion
}
