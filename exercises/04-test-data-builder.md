# Exercise 4: Test Data Builder Pattern with GitHub Copilot

## Overview
This exercise demonstrates how to use GitHub Copilot to create fluent test data builders that make test setup clean, readable, and maintainable.

**Duration:** 10 minutes  
**Focus:** Builder pattern, fluent API design, reusable test data

---

## Learning Objectives
- Understand the Builder pattern for test data
- Use Copilot to generate fluent builder APIs
- Create expressive test setup code
- Reduce test code duplication

---

## What is a Test Data Builder?

Test data builders help you create test objects with:
- **Fluent APIs** - chainable methods like `.WithCustomer(...).WithDate(...)`
- **Sensible defaults** - tests only specify what's unique
- **Named methods** - express intent like `.ForTomorrow()` or `.DuringBusinessHours()`

**Before (without builder):**
```csharp
var reservation = new Reservation
{
    Id = Guid.NewGuid(),
    CustomerId = Guid.NewGuid(),
    TableId = Guid.NewGuid(),
    ReservationDate = DateTime.Today.AddDays(1),
    StartTime = new TimeSpan(18, 0, 0),
    EndTime = new TimeSpan(20, 0, 0),
    PartySize = 4,
    Status = ReservationStatus.Pending
};
```

**After (with builder):**
```csharp
var reservation = new ReservationBuilder()
    .ForTomorrow()
    .WithPartySize(4)
    .Build();
```

---

## Target: ReservationBuilder Fluent API

**File:** `src/BoardGameCafe.Tests.Unit/Builders/ReservationBuilder.cs`

This builder already exists as a reference implementation. Your task is to:
1. Study the existing ReservationBuilder
2. Create a new builder for Order entities
3. Use Copilot to generate the builder methods

---

## Exercise Steps

### Step 1: Study the Existing ReservationBuilder

**TODO:** Review the ReservationBuilder implementation

```bash
# View the existing builder
cat src/BoardGameCafe.Tests.Unit/Builders/ReservationBuilder.cs
```

**Key patterns to notice:**
- Private fields for all properties
- `With*()` methods that return `this` for chaining
- `Build()` method that creates the final object
- Convenience methods like `ForTomorrow()`, `DuringBusinessHours()`
- Implicit operator for automatic conversion

---

### Step 2: Review the Existing OrderBuilder

**NOTE:** The `OrderBuilder` has already been implemented in this repository as a reference. Your task is to:
1. Study the existing OrderBuilder implementation
2. Understand the builder pattern and fluent API design
3. (Optional) Add additional convenience methods if you want to extend it

```bash
# View the existing OrderBuilder
cat src/BoardGameCafe.Tests.Unit/Builders/OrderBuilder.cs
```

**Key patterns to notice:**
- Private fields for all properties
- `With*()` methods that return `this` for chaining
- `Build()` method that creates the final object
- Convenience methods like `AsCompletedOrder()`, `AsPendingOrder()`
- Implicit operator for automatic conversion

---

### Step 3: (Optional) Extend OrderBuilder with Additional Methods

**TODO:** If you want to practice, use Copilot to add new convenience methods to OrderBuilder

**Additional convenience methods you could add:**
- `WithFoodItems()` - Add typical food items
- `WithAlcoholItems()` - Add alcoholic beverages  
- `WithGoldMemberDiscount()` - Apply gold tier discount
- `AsReadyForPayment()` - Set all required fields for payment

**Copilot Prompt:**
```csharp
// Add new convenience method to OrderBuilder
// Example: WithFoodItems() that adds typical food items to the order
// Should set multiple fields to create a realistic food order scenario
```

---

### Step 4: Understanding Builder Methods

Since OrderBuilder already exists, review these key methods:

**Fluent Builder Methods (already implemented):**
- `WithId(Guid id)` - Set order ID
- `WithCustomer(Customer customer)` - Set customer
- `WithSubtotal(decimal amount)` - Set subtotal
- `WithDiscount(decimal amount)` - Set discount
- `WithTax(decimal amount)` - Set tax
- `WithTotal(decimal amount)` - Set total
- `WithStatus(OrderStatus status)` - Set status

**Convenience Methods (already implemented):**
- `AsCompletedOrder()` - Set status to Completed
- `AsPendingOrder()` - Set status to Pending

---

### Step 5: Use the Builder in Tests

**TODO:** Find tests that use OrderBuilder to see examples

**Find a test that uses OrderBuilder:**
```bash
# Search for test files that use OrderBuilder
grep -r "OrderBuilder" src/BoardGameCafe.Tests.Unit/
```

**Example usage from existing tests:**
```csharp
// Using OrderBuilder in a test
[Fact]
public void CalculateTotal_WithDiscount_ReducesTotal()
{
    var order = new OrderBuilder()
        .WithSubtotal(100m)
        .WithDiscount(10m)
        .Build();
    
    // test continues...
    order.TotalAmount.Should().Be(98m);
}
```

Notice how the builder makes the test intent clear - we only specify what matters (subtotal and discount), and the builder provides sensible defaults for everything else.

---

## Expected Test Coverage

After completing all steps:
- **OrderBuilder class** already exists with 10+ fluent methods
- **Multiple convenience methods** already implemented (AsCompletedOrder, AsPendingOrder, etc.)
- **Implicit operator** already exists for automatic conversion
- **Understanding** of builder pattern and how it improves test readability
- **(Optional)** Extended the builder with additional methods if you chose to practice

---

## Running the Tests

```bash
# Navigate to test project
cd src/BoardGameCafe.Tests.Unit

# Run all unit tests
dotnet test

# Run tests that use builders
dotnet test --filter "FullyQualifiedName~OrderBuilder"
```

---

## Verification Checklist

- [ ] Reviewed ReservationBuilder class structure
- [ ] Reviewed OrderBuilder implementation
- [ ] Understand all fluent methods and how they return `this` for chaining
- [ ] Understand convenience methods and how they make tests more expressive
- [ ] Reviewed Build() method and how it creates valid Order objects
- [ ] Understand implicit operator and how it enables automatic conversion
- [ ] Found examples of OrderBuilder usage in existing tests
- [ ] (Optional) Added new convenience methods to extend the builder
- [ ] All tests still pass

---

## Common Copilot Tips

### Creating Builder Classes
✅ **Good Prompt:**
```
// Create fluent builder for Order entity
// Follow ReservationBuilder pattern
// Include: WithCustomer(), WithTotal(), convenience methods, implicit operator
```

❌ **Vague Prompt:**
```
// Make a builder
```

### Generating Multiple Similar Methods
```
// Generate With* methods for all Order properties
// Each should set field and return this
// Properties: Id, CustomerId, Subtotal, DiscountAmount, TaxAmount, TotalAmount
```

### Creating Convenience Methods
```
// Generate convenience method AsCompletedOrder()
// Should set Status = Completed, CompletedAt = now
// Return this for chaining
```

---

## Extension Challenges

### Challenge 1: Add Validation
Add validation to builder methods:
```csharp
public OrderBuilder WithSubtotal(decimal amount)
{
    if (amount < 0)
        throw new ArgumentException("Subtotal cannot be negative");
    _subtotal = amount;
    return this;
}
```

### Challenge 2: Add Calculated Defaults
Auto-calculate fields when Build() is called:
```csharp
public Order Build()
{
    // Auto-calculate total if not explicitly set
    if (_totalAmount == 0)
    {
        _totalAmount = _subtotal - _discountAmount + _taxAmount;
    }
    return new Order { ... };
}
```

### Challenge 3: Create More Builders
Create builders for:
- `CustomerBuilder`
- `GameBuilder`
- `MenuItemBuilder`

---

## Success Criteria

You've completed this exercise when:
1. ✅ Reviewed and understand OrderBuilder class functionality
2. ✅ Understand builder pattern with at least 10 fluent methods
3. ✅ Understand the convenience methods that exist
4. ✅ Found and reviewed tests that use the builder
5. ✅ (Optional) Extended the builder with new methods
6. ✅ All tests pass
7. ✅ You understand how builders improve test readability

---

## Why This Matters

Test data builders:
- **Reduce duplication** - default values defined once
- **Improve readability** - tests express only what's unique
- **Enable refactoring** - changes to entities only require updating the builder
- **Document intent** - method names like `ForTomorrow()` explain the scenario

---

## Next Steps

Continue to [Exercise 5: Bug Fix with Test](./05-bug-fix-with-test.md) to practice debugging with Copilot.
