# Exercise 1: Unit Testing with GitHub Copilot

**Duration:** 10-15 minutes  
**Difficulty:** Beginner  
**Focus:** Using Copilot to generate unit tests for service layer business logic

---

## Learning Objectives

By the end of this exercise, you will:
- Generate unit tests using Copilot prompts
- Understand test structure: Arrange, Act, Assert
- Use FluentAssertions for readable test assertions
- Create parameterized tests with `[Theory]` and `[InlineData]`

---

## Prerequisites

- ✅ Backend API running (`dotnet run` in `src/BoardGameCafe.Api`)
- ✅ Issue #17 (Unit Test Infrastructure) completed
- ✅ xUnit and FluentAssertions packages installed

---

## Scenario

You need to write comprehensive unit tests for the `OrderCalculationService` which handles:
- **Subtotal calculation**: Quantity × Unit Price for each item
- **Tax calculation**: 8% on food, 10% on alcohol
- **Member discounts**: Bronze 5%, Silver 10%, Gold 15%
- **Total calculation**: Subtotal + Tax - Discount

---

## Part 1: Generate Basic Test Structure

### TODO 1.1: Create Test Class

**Your task**: Use Copilot to generate a test class skeleton.

**Copilot Prompt**:
```csharp
// Create xUnit test class for OrderCalculationService
// Include test data builders and setup
```

**Expected Output**:
```csharp
public class OrderCalculationServiceTests
{
    private readonly OrderCalculationService _service;
    
    public OrderCalculationServiceTests()
    {
        _service = new OrderCalculationService();
    }
    
    // Test methods will go here
}
```

**Verification**: File created in `tests/BoardGameCafe.Tests.Unit/Services/OrderCalculationServiceTests.cs`

---

## Part 2: Test Subtotal Calculation

### TODO 2.1: Write Simple Subtotal Test

**Your task**: Generate a test for subtotal calculation with single item.

**Copilot Prompt**:
```csharp
// Test: CalculateSubtotal with single item should return quantity * unit price
// Arrange: 1 item, quantity 2, price $10
// Assert using FluentAssertions
```

**Expected Test**:
```csharp
[Fact]
public void CalculateSubtotal_WithSingleItem_ReturnsCorrectTotal()
{
    // Arrange
    var orderItems = new List<OrderItem>
    {
        new() { Quantity = 2, UnitPrice = 10.00m }
    };
    
    // Act
    var result = _service.CalculateSubtotal(orderItems);
    
    // Assert
    result.Should().Be(20.00m);
}
```

**Run Test**:
```bash
dotnet test --filter "FullyQualifiedName~OrderCalculationServiceTests"
```

**Expected**: ✅ Test passes

### TODO 2.2: Test Multiple Items

**Your task**: Use Copilot to generate test with multiple items.

**Copilot Prompt**:
```csharp
// Test: CalculateSubtotal with multiple items
// Arrange: 3 items with different quantities and prices
// Expected: sum of all (quantity * price)
```

**Practice Prompt Engineering**:
Try different ways to ask Copilot:
- "Write a test for calculating subtotal with 3 order items"
- "// Test multiple order items subtotal calculation"
- "Generate parameterized test for various item combinations"

**Challenge**: Which prompt gives the best result?

---

## Part 3: Test Tax Calculation

### TODO 3.1: Parameterized Tax Tests

**Your task**: Create parameterized tests for different food/alcohol tax rates.

**Copilot Prompt**:
```csharp
// Test: CalculateTax using Theory with InlineData
// Food items: 8% tax
// Alcohol items: 10% tax
// Test cases:
//   - Food only: $100 subtotal → $8 tax
//   - Alcohol only: $100 subtotal → $10 tax
//   - Mixed: $50 food + $50 alcohol → $4 + $5 = $9 tax
```

**Expected Pattern**:
```csharp
[Theory]
[InlineData(100.00, MenuCategory.Snacks, 8.00)]
[InlineData(100.00, MenuCategory.Alcohol, 10.00)]
[InlineData(50.00, MenuCategory.Meals, 4.00)]
public void CalculateTax_ForDifferentCategories_ReturnsCorrectAmount(
    decimal subtotal, 
    MenuCategory category, 
    decimal expectedTax)
{
    // Arrange
    var order = new Order { Subtotal = subtotal, Category = category };
    
    // Act
    var result = _service.CalculateTax(order);
    
    // Assert
    result.Should().Be(expectedTax);
}
```

**Practice**:
- Add more test cases
- Test edge cases: $0 subtotal, negative values (should throw?)
- Test decimal precision: $99.99 → $7.9992 rounds to?

---

## Part 4: Test Discount Calculation

### TODO 4.1: Test Membership Tier Discounts

**Your task**: Test each membership tier discount percentage.

**Copilot Prompt**:
```csharp
// Test: CalculateDiscount for each MembershipTier
// Theory with InlineData:
//   - None: 0% ($100 → $0 discount)
//   - Bronze: 5% ($100 → $5 discount)
//   - Silver: 10% ($100 → $10 discount)
//   - Gold: 15% ($100 → $15 discount)
```

**Expected**:
```csharp
[Theory]
[InlineData(MembershipTier.None, 100.00, 0.00)]
[InlineData(MembershipTier.Bronze, 100.00, 5.00)]
[InlineData(MembershipTier.Silver, 100.00, 10.00)]
[InlineData(MembershipTier.Gold, 100.00, 15.00)]
public void CalculateDiscount_ForMembershipTier_ReturnsCorrectPercentage(
    MembershipTier tier, 
    decimal subtotal, 
    decimal expectedDiscount)
{
    // Arrange
    var customer = new Customer { MembershipTier = tier };
    var order = new Order { Subtotal = subtotal, CustomerId = customer.Id };
    
    // Act
    var result = _service.CalculateDiscount(order, customer);
    
    // Assert
    result.Should().Be(expectedDiscount);
}
```

---

## Part 5: Integration Tests

### TODO 5.1: End-to-End Order Total Test

**Your task**: Test complete order calculation flow.

**Copilot Prompt**:
```csharp
// Test: CalculateOrderTotal end-to-end
// Scenario: Silver member orders:
//   - 2x Coffee @ $5 = $10
//   - 1x Pizza @ $15 = $15
// Subtotal: $25
// Tax (8% on food): $2.00
// Discount (10% Silver): $2.50
// Total: $25 + $2.00 - $2.50 = $24.50
```

**Expected Test Structure**:
```csharp
[Fact]
public void CalculateOrderTotal_SilverMemberWithFoodOrder_ReturnsCorrectTotal()
{
    // Arrange
    var customer = new Customer { MembershipTier = MembershipTier.Silver };
    var orderItems = new List<OrderItem>
    {
        new() { MenuItem = new MenuItem { Name = "Coffee", Category = MenuCategory.Coffee }, 
                Quantity = 2, UnitPrice = 5.00m },
        new() { MenuItem = new MenuItem { Name = "Pizza", Category = MenuCategory.Meals }, 
                Quantity = 1, UnitPrice = 15.00m }
    };
    
    // Act
    var result = _service.CalculateOrderTotal(orderItems, customer);
    
    // Assert
    result.Subtotal.Should().Be(25.00m);
    result.Tax.Should().Be(2.00m);
    result.Discount.Should().Be(2.50m);
    result.Total.Should().Be(24.50m);
}
```

---

## Part 6: Edge Cases and Error Handling

### TODO 6.1: Test Error Scenarios

**Your task**: Use Copilot to generate tests for error conditions.

**Copilot Prompts**:

```csharp
// Test: CalculateSubtotal with null orderItems throws ArgumentNullException

// Test: CalculateSubtotal with negative quantity throws ArgumentException

// Test: CalculateDiscount with null customer throws ArgumentNullException

// Test: CalculateTax with negative subtotal throws ArgumentException
```

**Expected Pattern**:
```csharp
[Fact]
public void CalculateSubtotal_WithNullOrderItems_ThrowsArgumentNullException()
{
    // Arrange
    List<OrderItem> orderItems = null;
    
    // Act
    Action act = () => _service.CalculateSubtotal(orderItems);
    
    // Assert
    act.Should().Throw<ArgumentNullException>()
       .WithParameterName("orderItems");
}
```

---

## Reflection Questions

After completing the exercise:

1. **Prompt Quality**: Which Copilot prompts generated the best tests? What made them effective?

2. **Test Coverage**: Run `dotnet test /p:CollectCoverage=true`. What's your code coverage percentage?

3. **Edge Cases**: What edge cases did you discover while writing tests? Did Copilot suggest any you hadn't thought of?

4. **Naming Conventions**: Did Copilot follow the `MethodName_Scenario_ExpectedResult` naming pattern consistently?

5. **Assertions**: How readable are FluentAssertions compared to traditional `Assert.Equal()`?

---

## Success Criteria

- [ ] All tests pass (`dotnet test`)
- [ ] At least 10 unit tests created
- [ ] Code coverage >80% for OrderCalculationService
- [ ] All edge cases tested (null, negative, boundary values)
- [ ] Tests use FluentAssertions
- [ ] Parameterized tests for multiple scenarios

---

## Bonus Challenges

### Challenge 1: Test Data Builders
Use Copilot to generate a test data builder pattern:

```csharp
// Create OrderTestBuilder class with fluent API
// Example usage:
//   var order = new OrderTestBuilder()
//       .WithCustomer(MembershipTier.Gold)
//       .WithItem("Coffee", 2, 5.00m)
//       .WithItem("Pizza", 1, 15.00m)
//       .Build();
```

### Challenge 2: Property-Based Testing
Explore property-based testing with FsCheck:

```csharp
// Property: Subtotal should always equal sum of (quantity * price) for any order
// Use FsCheck to generate random orders
```

### Challenge 3: Code Coverage Report
Generate HTML coverage report:

```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
reportgenerator -reports:coverage.opencover.xml -targetdir:coverage-report
```

---

## Next Steps

- Exercise 2: API Testing with Swagger
- Exercise 3: UI Testing with Playwright
- Exercise 4: Bug Hunting and Regression Tests

---

**Instructor Notes**: 
- Walk through first test together
- Encourage experimentation with different Copilot prompts
- Share best/worst prompts as group learning
- Discuss test naming conventions
- Review FluentAssertions syntax
