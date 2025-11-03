# Unit Test Prompt Templates

Use these templates when generating unit tests for backend services and business logic.

## Basic Service Test

```
Generate xUnit unit tests for {ServiceName} in tests/BoardGameCafe.Tests.Unit/Services/{ServiceName}Tests.cs:
- Test method: {MethodName}
- Scenarios: {list scenarios}
- Use FluentAssertions for assertions
- Follow Arrange-Act-Assert pattern
- Test naming: MethodName_Scenario_ExpectedResult
```

## Parameterized Test with Theory

```
Generate [Theory] unit test for {MethodName} with [InlineData]:
- Test cases: {list test cases with inputs and expected outputs}
- Use FluentAssertions
- Cover edge cases: null, empty, zero, negative, boundary values
```

**Example:**
```
Generate [Theory] unit test for CalculateTax with [InlineData]:
- Test cases:
  - Food item $10 → tax $0.80 (8%)
  - Alcohol item $10 → tax $1.00 (10%)
  - Food item $0 → tax $0
  - Mixed order $20 food + $10 alcohol → tax $2.60
- Use FluentAssertions: result.Should().Be(expected)
- Cover edge cases: null items, empty order, zero amounts
```

## Validation Logic Test

```
Generate validation tests for {ValidatorName}:
- Valid scenarios: {describe valid inputs}
- Invalid scenarios with expected errors:
  - {scenario 1}: {expected error message}
  - {scenario 2}: {expected error message}
- Use FluentAssertions to check error messages
```

**Example:**
```
Generate validation tests for ReservationValidator:
- Valid scenarios: party size within table capacity, future date, duration 1-4 hours
- Invalid scenarios with expected errors:
  - Party size 0: "Party size must be at least 1"
  - Party size > capacity: "Party size exceeds table capacity of {capacity}"
  - Past date: "Reservation must be in the future"
  - Duration 5 hours: "Duration cannot exceed 4 hours"
- Use FluentAssertions: validationResult.Errors.Should().ContainSingle(e => e.ErrorMessage.Contains("Party size"))
```

## Exception Testing

```
Generate exception tests for {MethodName}:
- Should throw {ExceptionType} when {condition}
- Exception message should contain: {expected message}
- Use FluentAssertions: action.Should().Throw<{Exception}>().WithMessage("*{keyword}*")
```

**Example:**
```
Generate exception tests for CheckoutGame:
- Should throw ArgumentNullException when game is null
- Should throw InvalidOperationException when no copies available
- Should throw NotFoundException when game ID doesn't exist
- Exception message should contain game title or ID
- Use FluentAssertions: action.Should().Throw<InvalidOperationException>().WithMessage("*no copies available*")
```

## Test with Mocked Dependencies

```
Generate unit test for {MethodName} with mocked dependencies:
- Mock: {DependencyName} using NSubstitute
- Setup mock to return: {describe mock behavior}
- Verify: mock was called with {expected parameters}
- Assert: {expected result}
```

**Example:**
```
Generate unit test for ProcessOrderAsync with mocked dependencies:
- Mock: IOrderRepository using NSubstitute
- Setup mock GetByIdAsync to return: test order with 3 items
- Setup mock UpdateAsync to return: true (successful update)
- Verify: UpdateAsync was called exactly once with updated order
- Assert: order status is "Processing", updated timestamp is set
```

## Computed Property Test

```
Generate test for computed property {PropertyName} on {EntityName}:
- Test scenarios where property should be {value1}
- Test scenarios where property should be {value2}
- Use entity builder for test data setup
```

**Example:**
```
Generate test for computed property IsAvailable on Game:
- Test scenarios where IsAvailable should be true: CopiesOwned=5, CopiesInUse=3
- Test scenarios where IsAvailable should be false: CopiesOwned=5, CopiesInUse=5
- Use GameBuilder for test data setup: new GameBuilder().WithAvailability(5, 3).Build()
```

## Business Rule Test

```
Generate test for business rule: {describe rule}
- Test when rule is satisfied: {scenario}
- Test when rule is violated: {scenario}
- Expected outcome: {describe expected behavior}
```

**Example:**
```
Generate test for business rule: loyalty tier upgrades automatically when points threshold reached
- Test when rule is satisfied: 
  - Customer with 499 points earns 1 point → tier upgrades from None to Bronze
  - Customer with 1999 points earns 1 point → tier upgrades from Silver to Gold
- Test when rule is violated: 
  - Customer with 500 points loses 1 point → tier downgrades from Bronze to None
- Expected outcome: tier property reflects correct level, discount percentage updates accordingly
```

## Edge Case Test Suite

```
Generate comprehensive edge case tests for {MethodName}:
- Boundary values: {list boundaries}
- Null/empty inputs
- Maximum/minimum values
- Invalid combinations
- Concurrent access scenarios
```

**Example:**
```
Generate comprehensive edge case tests for CreateReservation:
- Boundary values: party size exactly equals table capacity, duration exactly 4 hours, start time exactly at opening time
- Null inputs: null customer, null table, null start time
- Maximum values: party size 100, duration 24 hours
- Invalid combinations: end time before start time, table ID with customer ID from different accounts
- Concurrent access: two reservations for same table created simultaneously
```

---

## Usage Tips

1. **Replace placeholders** in curly braces `{...}` with actual values
2. **Customize test cases** based on your specific requirements
3. **Add context** from existing code when prompting
4. **Iterate** - refine prompts based on generated output
5. **Review carefully** - Copilot suggestions are starting points

## Common Test Patterns

### Arrange-Act-Assert Template
```csharp
[Fact]
public void MethodName_Scenario_ExpectedResult()
{
    // Arrange
    var service = new ServiceName(dependencies);
    var input = new InputBuilder().Build();
    
    // Act
    var result = service.MethodName(input);
    
    // Assert
    result.Should().Be(expectedValue);
}
```

### Theory with InlineData Template
```csharp
[Theory]
[InlineData(input1, expected1)]
[InlineData(input2, expected2)]
public void MethodName_VariousInputs_ReturnsExpected(int input, int expected)
{
    // Arrange
    var service = new ServiceName();
    
    // Act
    var result = service.MethodName(input);
    
    // Assert
    result.Should().Be(expected);
}
```

### Exception Test Template
```csharp
[Fact]
public void MethodName_InvalidInput_ThrowsException()
{
    // Arrange
    var service = new ServiceName();
    var invalidInput = null;
    
    // Act
    Action act = () => service.MethodName(invalidInput);
    
    // Assert
    act.Should().Throw<ArgumentNullException>()
        .WithMessage("*parameter*");
}
```
