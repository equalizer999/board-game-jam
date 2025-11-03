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

### Step 2: Create OrderBuilder Skeleton

**TODO:** Use Copilot to generate a new OrderBuilder class

**Copilot Prompt:**
```csharp
// Create test data builder for Order entity using fluent API
// File: src/BoardGameCafe.Tests.Unit/Builders/OrderBuilder.cs
// Pattern: same as ReservationBuilder
// Include: WithCustomer(), WithItems(), WithTotal(), Build()
```

**Expected class structure:**
```csharp
public class OrderBuilder
{
    private Guid _id = Guid.NewGuid();
    private Guid _customerId = Guid.NewGuid();
    private decimal _subtotal = 100m;
    // ... more fields
    
    public OrderBuilder WithCustomer(Customer customer) { ... }
    public OrderBuilder WithSubtotal(decimal amount) { ... }
    public Order Build() { ... }
}
```

---

### Step 3: Add Fluent Builder Methods

**TODO:** Use Copilot to add chainable methods

**Methods to create:**
- `WithId(Guid id)` - Set order ID
- `WithCustomer(Customer customer)` - Set customer
- `WithSubtotal(decimal amount)` - Set subtotal
- `WithDiscount(decimal amount)` - Set discount
- `WithTax(decimal amount)` - Set tax
- `WithTotal(decimal amount)` - Set total
- `WithStatus(OrderStatus status)` - Set status

**Copilot Prompt:**
```csharp
// Generate fluent builder methods for OrderBuilder
// Each method should set the corresponding field and return this
// Follow the pattern from ReservationBuilder
```

**Expected method:**
```csharp
public OrderBuilder WithSubtotal(decimal amount)
{
    _subtotal = amount;
    return this;
}
```

---

### Step 4: Add Convenience Methods

**TODO:** Use Copilot to add expressive convenience methods

**Scenarios to support:**
- `AsCompletedOrder()` - Set status to Completed
- `AsPendingOrder()` - Set status to Pending
- `WithSmallTotal()` - Set total to $20
- `WithLargeTotal()` - Set total to $500
- `WithMemberDiscount()` - Apply typical member discount

**Copilot Prompt:**
```csharp
// Generate convenience methods for common order scenarios
// Examples: AsCompletedOrder(), WithSmallTotal(), WithMemberDiscount()
// These should set multiple fields to create realistic test scenarios
```

**Expected method:**
```csharp
public OrderBuilder AsCompletedOrder()
{
    _status = OrderStatus.Completed;
    _completedAt = DateTime.UtcNow;
    return this;
}
```

---

### Step 5: Add Collection Builder Methods

**TODO:** Use Copilot to add methods for building order items

**Method to create:**
```csharp
public OrderBuilder WithItems(params OrderItem[] items)
public OrderBuilder WithMenuItem(string name, decimal price, int quantity = 1)
```

**Copilot Prompt:**
```csharp
// Generate methods to add order items to the builder
// WithItems() should accept multiple OrderItem objects
// WithMenuItem() should create an OrderItem and add it
// Both should return this for chaining
```

---

### Step 6: Add Build() and Implicit Operator

**TODO:** Use Copilot to generate the final build methods

**Copilot Prompt:**
```csharp
// Generate Build() method that creates Order from builder fields
// Generate implicit operator to convert OrderBuilder to Order automatically
// Follow ReservationBuilder pattern
```

**Expected:**
```csharp
public Order Build()
{
    return new Order
    {
        Id = _id,
        CustomerId = _customerId,
        Subtotal = _subtotal,
        // ... all other fields
    };
}

public static implicit operator Order(OrderBuilder builder) => builder.Build();
```

---

### Step 7: Use the Builder in Tests

**TODO:** Refactor existing tests to use OrderBuilder

**Find a test that creates an Order:**
```bash
# Search for test files that create Order objects
grep -r "new Order" src/BoardGameCafe.Tests.Unit/
```

**Copilot Prompt:**
```csharp
// Refactor this test to use OrderBuilder instead of new Order()
// Make the test more readable by using fluent methods
// Only specify properties that matter for this test
```

**Before:**
```csharp
[Fact]
public void CalculateTotal_WithDiscount_ReducesTotal()
{
    var order = new Order
    {
        Id = Guid.NewGuid(),
        Subtotal = 100m,
        DiscountAmount = 10m,
        TaxAmount = 8m,
        TotalAmount = 98m
    };
    // test continues...
}
```

**After:**
```csharp
[Fact]
public void CalculateTotal_WithDiscount_ReducesTotal()
{
    var order = new OrderBuilder()
        .WithSubtotal(100m)
        .WithDiscount(10m)
        .Build();
    // test continues...
}
```

---

## Expected Test Coverage

After completing all steps:
- **OrderBuilder class** with 10+ fluent methods
- **At least 3 convenience methods** for common scenarios
- **Implicit operator** for automatic conversion
- **2-3 existing tests refactored** to use the builder

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

- [ ] OrderBuilder class created
- [ ] All fluent methods return `this` for chaining
- [ ] Convenience methods make tests more expressive
- [ ] Build() method creates valid Order objects
- [ ] Implicit operator works (can assign builder to Order variable)
- [ ] At least 2 tests refactored to use builder
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
1. ✅ OrderBuilder class is fully functional
2. ✅ Builder has at least 10 fluent methods
3. ✅ At least 3 convenience methods exist
4. ✅ 2+ existing tests refactored to use builder
5. ✅ All tests pass
6. ✅ You understand how builders improve test readability

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
