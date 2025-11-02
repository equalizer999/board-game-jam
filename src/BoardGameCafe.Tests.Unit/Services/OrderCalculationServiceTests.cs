using BoardGameCafe.Domain;
using BoardGameCafe.Tests.Unit.Builders;
using BoardGameCafe.Tests.Unit.TestUtilities;
using FluentAssertions;

namespace BoardGameCafe.Tests.Unit.Services;

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

    #region Additional Comprehensive Tests with Builders and Edge Cases

    [Theory]
    [InlineData(0, 0.00)]
    [InlineData(1, 0.01)]
    [InlineData(50, 0.50)]
    [InlineData(999, 9.99)]
    public void CalculateLoyaltyPointsDiscount_VariousAmounts_ReturnsCorrectDiscount(int points, decimal expected)
    {
        // Act
        var discount = _service.CalculateLoyaltyPointsDiscount(points);

        // Assert
        discount.Should().Be(expected);
    }

    [Theory]
    [InlineData(0.00, 0)]
    [InlineData(0.99, 0)]
    [InlineData(1.00, 1)]
    [InlineData(1.99, 1)]
    [InlineData(99.99, 99)]
    [InlineData(100.00, 100)]
    [InlineData(100.50, 100)]
    public void CalculateLoyaltyPointsEarned_VariousAmounts_ReturnsCorrectPoints(decimal amount, int expected)
    {
        // Act
        var points = _service.CalculateLoyaltyPointsEarned(amount);

        // Assert
        points.Should().Be(expected);
    }

    [Fact]
    public void CalculateSubtotal_WithBuilder_CalculatesCorrectly()
    {
        // Arrange
        var customer = new CustomerBuilder().Build();
        var menuItem1 = new MenuItem { Id = Guid.NewGuid(), Name = "Item1", Category = MenuCategory.Coffee, Price = 5.00m };
        var menuItem2 = new MenuItem { Id = Guid.NewGuid(), Name = "Item2", Category = MenuCategory.Meals, Price = 12.00m };

        var order = new OrderBuilder()
            .WithCustomer(customer)
            .WithItem(menuItem1, quantity: 2)
            .WithItem(menuItem2, quantity: 1)
            .Build();

        // Act
        order.CalculateSubtotal();

        // Assert
        order.Subtotal.Should().Be(22.00m); // (2 * 5.00) + (1 * 12.00)
    }

    [Fact]
    public void ApplyMemberDiscount_Bronze_Applies5PercentDiscount()
    {
        // Arrange
        var customer = new CustomerBuilder()
            .AsBronzeMember()
            .Build();

        var order = new OrderBuilder()
            .WithCustomer(customer)
            .WithSubtotal(100.00m)
            .Build();

        // Act
        order.CalculateMemberDiscount();

        // Assert
        order.DiscountAmount.Should().Be(5.00m);
    }

    [Fact]
    public void ApplyMemberDiscount_Silver_Applies10PercentDiscount()
    {
        // Arrange
        var customer = new CustomerBuilder()
            .AsSilverMember()
            .Build();

        var order = new OrderBuilder()
            .WithCustomer(customer)
            .WithSubtotal(100.00m)
            .Build();

        // Act
        order.CalculateMemberDiscount();

        // Assert
        order.DiscountAmount.Should().Be(10.00m);
    }

    [Fact]
    public void ApplyMemberDiscount_Gold_Applies15PercentDiscount()
    {
        // Arrange
        var customer = new CustomerBuilder()
            .AsGoldMember()
            .Build();

        var order = new OrderBuilder()
            .WithCustomer(customer)
            .WithSubtotal(100.00m)
            .Build();

        // Act
        order.CalculateMemberDiscount();

        // Assert
        order.DiscountAmount.Should().Be(15.00m);
    }

    [Fact]
    public void CalculateTax_Food_Applies8PercentTax()
    {
        // Arrange
        var customer = new CustomerBuilder().Build();
        var menuItem = new MenuItem { Id = Guid.NewGuid(), Name = "Pizza", Category = MenuCategory.Meals, Price = 25.00m };

        var order = new OrderBuilder()
            .WithCustomer(customer)
            .WithItem(menuItem, quantity: 1)
            .Build();

        // Act
        order.CalculateTax();

        // Assert
        order.TaxAmount.Should().Be(2.00m); // 25.00 * 0.08
    }

    [Fact]
    public void CalculateTax_Alcohol_Applies10PercentTax()
    {
        // Arrange
        var customer = new CustomerBuilder().Build();
        var menuItem = new MenuItem { Id = Guid.NewGuid(), Name = "Wine", Category = MenuCategory.Alcohol, Price = 30.00m };

        var order = new OrderBuilder()
            .WithCustomer(customer)
            .WithItem(menuItem, quantity: 1)
            .Build();

        // Act
        order.CalculateTax();

        // Assert
        order.TaxAmount.Should().Be(3.00m); // 30.00 * 0.10
    }

    [Fact]
    public void CalculateTax_MixedItems_AppliesDifferentRatesCorrectly()
    {
        // Arrange
        var customer = new CustomerBuilder().Build();
        var foodItem = new MenuItem { Id = Guid.NewGuid(), Name = "Burger", Category = MenuCategory.Meals, Price = 15.00m };
        var alcoholItem = new MenuItem { Id = Guid.NewGuid(), Name = "Beer", Category = MenuCategory.Alcohol, Price = 8.00m };
        var coffeeItem = new MenuItem { Id = Guid.NewGuid(), Name = "Coffee", Category = MenuCategory.Coffee, Price = 4.00m };

        var order = new OrderBuilder()
            .WithCustomer(customer)
            .WithItem(foodItem, quantity: 1)
            .WithItem(alcoholItem, quantity: 2)
            .WithItem(coffeeItem, quantity: 1)
            .Build();

        // Act
        order.CalculateTax();

        // Assert
        // Food: 15.00 * 0.08 = 1.20
        // Alcohol: 16.00 * 0.10 = 1.60
        // Coffee: 4.00 * 0.08 = 0.32
        // Total: 3.12
        order.TaxAmount.Should().Be(3.12m);
    }

    [Fact]
    public void RedeemLoyaltyPoints_100Points_Reduces1Dollar()
    {
        // Arrange
        var customer = new CustomerBuilder()
            .WithLoyaltyPoints(500)
            .Build();

        var menuItem = new MenuItem { Id = Guid.NewGuid(), Name = "Coffee", Category = MenuCategory.Coffee, Price = 5.00m };

        var order = new OrderBuilder()
            .WithCustomer(customer)
            .WithItem(menuItem, quantity: 1)
            .Build();

        // Act
        _service.CalculateOrderTotals(order, loyaltyPointsToRedeem: 100);

        // Assert
        order.DiscountAmount.Should().Be(1.00m);
    }

    [Fact]
    public void RedeemLoyaltyPoints_PreventsNegativeTotal()
    {
        // Arrange
        var customer = new CustomerBuilder()
            .WithLoyaltyPoints(10000)
            .AsGoldMember() // 15% discount
            .Build();

        var menuItem = new MenuItem { Id = Guid.NewGuid(), Name = "Coffee", Category = MenuCategory.Coffee, Price = 5.00m };

        var order = new OrderBuilder()
            .WithCustomer(customer)
            .WithItem(menuItem, quantity: 1)
            .Build();

        // Act - Try to redeem more than order is worth
        _service.CalculateOrderTotals(order, loyaltyPointsToRedeem: 10000);

        // Assert
        order.TotalAmount.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public void CalculateLoyaltyPointsEarned_1PointPerDollarSpent()
    {
        // Act
        var points = _service.CalculateLoyaltyPointsEarned(45.00m);

        // Assert
        points.Should().Be(45);
    }

    [Fact]
    public void CalculateLoyaltyPointsEarned_RoundsDown()
    {
        // Act
        var points = _service.CalculateLoyaltyPointsEarned(45.99m);

        // Assert
        points.Should().Be(45); // Should round down, not up
    }

    [Theory]
    [InlineData(0.00)]
    [InlineData(0.01)]
    [InlineData(0.99)]
    public void CalculateOrderTotals_ZeroAmounts_HandlesCorrectly(decimal itemPrice)
    {
        // Arrange
        var customer = new CustomerBuilder().Build();
        var menuItem = new MenuItem { Id = Guid.NewGuid(), Name = "Free Item", Category = MenuCategory.Snacks, Price = itemPrice };

        var order = new OrderBuilder()
            .WithCustomer(customer)
            .WithItem(menuItem, quantity: 1)
            .Build();

        // Act
        _service.CalculateOrderTotals(order);

        // Assert
        order.TotalAmount.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public void CalculateOrderTotals_MaxDiscount_DoesNotExceedSubtotal()
    {
        // Arrange
        var customer = new CustomerBuilder()
            .AsGoldMember() // 15% discount
            .WithLoyaltyPoints(10000)
            .Build();

        var menuItem = new MenuItem { Id = Guid.NewGuid(), Name = "Item", Category = MenuCategory.Meals, Price = 10.00m };

        var order = new OrderBuilder()
            .WithCustomer(customer)
            .WithItem(menuItem, quantity: 1)
            .Build();

        // Act
        _service.CalculateOrderTotals(order, loyaltyPointsToRedeem: 10000);

        // Assert
        // Even with huge discount, total should not be negative
        order.TotalAmount.Should().BeGreaterThanOrEqualTo(0);
        // Tax should still be calculated
        order.TaxAmount.Should().BeGreaterThan(0);
    }

    [Theory]
    [InlineData(10.00, 10.00, 1.80)]  // Food: 10 * 0.08 = 0.80, Alcohol: 10 * 0.10 = 1.00, Total = 1.80
    [InlineData(10.00, 0.00, 0.80)]   // Food only
    [InlineData(0.00, 10.00, 1.00)]   // Alcohol only: 10 * 0.10
    [InlineData(25.50, 15.75, 3.615)] // Mixed: 25.50 * 0.08 + 15.75 * 0.10 = 2.04 + 1.575 = 3.615
    public void CalculateTax_Rounding_HandlesCorrectly(decimal foodPrice, decimal alcoholPrice, decimal expectedTax)
    {
        // Arrange
        var customer = new CustomerBuilder().Build();
        var orderBuilder = new OrderBuilder().WithCustomer(customer);

        if (foodPrice > 0)
        {
            var foodItem = new MenuItem { Id = Guid.NewGuid(), Name = "Food", Category = MenuCategory.Meals, Price = foodPrice };
            orderBuilder.WithItem(foodItem, quantity: 1);
        }

        if (alcoholPrice > 0)
        {
            var alcoholItem = new MenuItem { Id = Guid.NewGuid(), Name = "Alcohol", Category = MenuCategory.Alcohol, Price = alcoholPrice };
            orderBuilder.WithItem(alcoholItem, quantity: 1);
        }

        var order = orderBuilder.Build();

        // Act
        order.CalculateTax();

        // Assert
        order.TaxAmount.Should().Be(expectedTax);
    }

    #endregion
}
