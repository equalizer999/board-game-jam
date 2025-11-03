# Exercise 1: Unit Testing with GitHub Copilot

## Overview
This exercise demonstrates how to use GitHub Copilot to generate comprehensive unit tests for C# backend business logic.

**Duration:** 10 minutes  
**Focus:** Service layer testing with xUnit and FluentAssertions

---

## Learning Objectives
- Generate unit tests from existing code
- Use Copilot to create test data and arrange-act-assert patterns
- Write parameterized tests with `[Theory]` and `[InlineData]`
- Mock dependencies with NSubstitute

---

## Target Code: Order Calculation Service

**File:** `src/BoardGameCafe.Domain/Services/OrderCalculationService.cs`

This service calculates order totals with:
- Tax calculation (8% on food, 10% on alcohol)
- Member discounts (Bronze 5%, Silver 10%, Gold 15%)
- Loyalty points redemption (100 points = $1 discount)

---

## Exercise Steps

### Step 1: Generate Test Class Skeleton

**Copilot Prompt:**
```
// Create unit test class for OrderCalculationService
// Use xUnit, FluentAssertions, NSubstitute
// Test naming: MethodName_Scenario_ExpectedResult
```

**Expected Output:**
- Test class in `tests/BoardGameCafe.Tests.Unit/Services/OrderCalculationServiceTests.cs`
- Basic test setup with proper namespaces
- Constructor for test fixtures

---

### Step 2: Test Tax Calculation

**TODO:** Use Copilot to generate tests for `CalculateTax()` method

**Scenarios to cover:**
- Food items (8% tax)
- Alcohol items (10% tax)
- Mixed cart (food + alcohol)
- Zero subtotal

**Copilot Prompt:**
```
// Generate [Theory] tests for CalculateTax method
// Test cases: food only, alcohol only, mixed, zero amount
// Use InlineData for parameterization
```

**Expected Test Names:**
- `CalculateTax_FoodItems_Returns8Percent`
- `CalculateTax_AlcoholItems_Returns10Percent`
- `CalculateTax_MixedCart_CalculatesCorrectly`
- `CalculateTax_ZeroSubtotal_ReturnsZero`

---

### Step 3: Test Member Discounts

**TODO:** Use Copilot to generate tests for `CalculateDiscount()` method

**Scenarios to cover:**
- Bronze member (5% discount)
- Silver member (10% discount)
- Gold member (15% discount)
- Non-member (0% discount)

**Copilot Prompt:**
```
// Generate tests for member discount calculation
// Test all membership tiers: None, Bronze, Silver, Gold
// Verify correct discount percentages
```

**Expected Assertions:**
```csharp
result.Should().Be(5.00m); // Bronze on $100
result.Should().Be(10.00m); // Silver on $100
result.Should().Be(15.00m); // Gold on $100
```

---

### Step 4: Test Loyalty Points Redemption

**TODO:** Use Copilot to generate tests for `ApplyLoyaltyPoints()` method

**Edge cases:**
- Customer has sufficient points
- Customer has insufficient points (should throw)
- Redeeming 0 points
- Redeeming more than order total

**Copilot Prompt:**
```
// Generate tests for loyalty points redemption
// Edge cases: sufficient points, insufficient points, zero points, exceeds total
// Use FluentAssertions .Should().Throw<>() for validation errors
```

---

### Step 5: Test Complete Order Calculation

**TODO:** Use Copilot to generate integration test for `CalculateOrderTotal()`

**Full workflow test:**
1. Create order with items
2. Apply member discount
3. Calculate tax
4. Redeem loyalty points
5. Verify final total

**Copilot Prompt:**
```
// Generate end-to-end test for CalculateOrderTotal
// Create sample order with food and alcohol
// Apply Gold member discount
// Redeem 500 loyalty points
// Assert final total matches expected calculation
```

---

## Expected Test Coverage

After completing all steps:
- **Total Tests:** 15-20 test methods
- **Code Coverage:** >85%
- **Test Categories:**
  - Tax calculation: 4 tests
  - Discount calculation: 4 tests
  - Loyalty points: 5 tests
  - End-to-end: 3 tests
  - Edge cases: 4 tests

---

## Running the Tests

```bash
# Navigate to test project
cd tests/BoardGameCafe.Tests.Unit

# Run all unit tests
dotnet test

# Run with coverage (if configured)
dotnet test --collect:"XPlat Code Coverage"

# Run specific test class
dotnet test --filter "FullyQualifiedName~OrderCalculationServiceTests"
```

---

## Verification Checklist

- [ ] All tests pass (`dotnet test` shows green)
- [ ] Tests use FluentAssertions (`.Should()` syntax)
- [ ] Parameterized tests use `[Theory]` and `[InlineData]`
- [ ] Test names follow convention: `MethodName_Scenario_ExpectedResult`
- [ ] Edge cases are covered (nulls, zeros, boundaries)
- [ ] No hardcoded test data (use test builders or fixtures)

---

## Common Copilot Tips

### Getting Better Test Suggestions
✅ **Good Prompt:**
```
// Generate [Theory] test for CalculateTax with InlineData
// Test cases: $100 food (expect $8), $50 alcohol (expect $5), $0 (expect $0)
```

❌ **Vague Prompt:**
```
// Test the tax method
```

### Generating Test Data
```
// Create test data builder for Order with fluent API
// Methods: WithItems(), WithMember(), WithSubtotal()
```

### Asserting Collections
```
// Assert that result collection has 3 items
// First item should have category "Coffee"
// Use FluentAssertions collection assertions
```

---

## Extension Challenges

### Challenge 1: Negative Cases
Add tests for invalid inputs:
- Negative prices
- Null orders
- Invalid membership tiers

### Challenge 2: Test Data Builders
Create fluent builders for test objects:
```csharp
var order = new OrderBuilder()
    .WithItems(5)
    .WithMember(MembershipTier.Gold)
    .Build();
```

### Challenge 3: Theory Data from Methods
Replace `[InlineData]` with `[MemberData]` for complex test cases

---

## Success Criteria

You've completed this exercise when:
1. ✅ All generated tests pass
2. ✅ Code coverage is >80%
3. ✅ Tests are readable and maintainable
4. ✅ You understand how Copilot generates test patterns
5. ✅ You can guide Copilot to create the tests you need

---

## Next Steps

Continue to [Exercise 2: API Endpoint Creation](./02-api-endpoint-creation.md) to learn how to create new REST endpoints with Copilot.
