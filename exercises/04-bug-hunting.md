# Exercise 4: Bug Hunting and Regression Testing with GitHub Copilot

**Duration:** 12-18 minutes  
**Difficulty:** Intermediate to Advanced  
**Focus:** Finding intentional bugs, writing regression tests, and using Copilot to debug issues

---

## Learning Objectives

By the end of this exercise, you will:
- Use Copilot to help identify bugs in existing code
- Write regression tests to prevent bugs from recurring
- Debug async timing issues and race conditions
- Test edge cases and boundary conditions
- Use Copilot to suggest bug fixes

---

## Prerequisites

- ✅ All previous exercises completed (Unit, API, UI testing)
- ✅ Backend and frontend running
- ✅ Familiarity with the codebase structure
- ✅ Bug branches created (see Issue #21)

---

## Scenario

The Board Game Café codebase contains **8 intentional bugs** for workshop practice. Your task is to:
1. **Reproduce** the bug
2. **Write a failing test** that captures the bug
3. **Fix** the bug
4. **Verify** the test now passes
5. **Write additional regression tests** to prevent similar bugs

---

## Part 1: Bug #1 - Midnight Reservation Timezone Issue

### Bug Description

**Issue**: Reservations made around midnight (23:00-01:00) are sometimes created for the wrong day due to timezone conversion.

**Affected Code**: `src/BoardGameCafe.Api/Features/Reservations/ReservationEndpoints.cs`

**Reproduction Steps**:
1. Navigate to `/reservations/new`
2. Select date: Today
3. Select time: 23:30
4. Create reservation
5. **Expected**: Reservation for today at 23:30
6. **Actual**: Reservation created for tomorrow at 23:30

### TODO 1.1: Write Failing Test

**Your task**: Use Copilot to write a test that reproduces this bug.

**Copilot Prompt**:
```csharp
// Integration test: Create reservation at 23:30
// Bug: Timezone conversion shifts reservation to next day
// Expected: Reservation date matches input date
// Actual: Reservation date is next day
// Test should FAIL until bug is fixed
```

**Expected Test**:
```csharp
[Fact]
public async Task CreateReservation_At2330Hours_CreatesOnCorrectDate()
{
    // Arrange
    var today = DateTime.UtcNow.Date;
    var request = new CreateReservationRequest
    {
        CustomerId = _testCustomer.Id,
        TableId = _testTable.Id,
        ReservationDate = today,
        ReservationTime = new TimeSpan(23, 30, 0),
        DurationHours = 2,
        PartySize = 4
    };
    
    // Act
    var response = await _client.PostAsJsonAsync("/api/v1/reservations", request);
    
    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.Created);
    
    var reservation = await response.Content.ReadFromJsonAsync<ReservationDto>();
    reservation.ReservationDate.Date.Should().Be(today); // This will FAIL
}
```

**Run Test**:
```bash
dotnet test --filter "CreateReservation_At2330Hours"
```

**Expected Result**: ❌ Test FAILS - Bug confirmed

### TODO 1.2: Use Copilot to Identify Root Cause

**Your task**: Ask Copilot to analyze the code and suggest where the bug might be.

**Copilot Prompt in code**:
```csharp
// BUG: When creating reservation near midnight, date shifts to next day
// Analyze this code for timezone conversion issues:
// [Paste ReservationEndpoints.cs CreateReservation method]
```

**Expected Copilot Suggestion**:
```csharp
// Issue: DateTime.UtcNow is being used instead of local timezone
// When user selects 23:30 local time, UTC conversion may shift to next day
// Fix: Use DateTimeOffset or store date and time separately
```

### TODO 1.3: Fix the Bug

**Your task**: Apply Copilot's suggested fix.

**Copilot Prompt**:
```csharp
// Fix timezone bug: Store reservation date and time as separate fields
// Update Reservation entity:
//   - Change ReservationDate from DateTime to DateOnly
//   - Change ReservationTime from DateTime to TimeOnly
// Update CreateReservation to avoid UTC conversion issues
```

**Expected Fix**:
```csharp
// Before:
var reservation = new Reservation
{
    ReservationDate = request.ReservationDate.ToUniversalTime() // BUG: shifts date
};

// After:
var reservation = new Reservation
{
    ReservationDate = DateOnly.FromDateTime(request.ReservationDate),
    ReservationTime = TimeOnly.FromTimeSpan(request.ReservationTime)
};
```

### TODO 1.4: Verify Fix

**Run Test Again**:
```bash
dotnet test --filter "CreateReservation_At2330Hours"
```

**Expected Result**: ✅ Test PASSES - Bug fixed!

### TODO 1.5: Write Regression Tests

**Your task**: Add more tests to prevent similar timezone bugs.

**Copilot Prompt**:
```csharp
// Regression tests for reservation time boundaries
// Theory with InlineData:
//   - 00:00 (midnight) 
//   - 01:00 (1 AM)
//   - 12:00 (noon)
//   - 23:59 (one minute before midnight)
// Assert: All times stored correctly without date shift
```

---

## Part 2: Bug #2 - Double Discount Application

### Bug Description

**Issue**: Loyalty discount is applied twice if customer submits order twice quickly, resulting in negative total.

**Affected Code**: `src/BoardGameCafe.Api/Features/Orders/OrderEndpoints.cs`

### TODO 2.1: Reproduce the Bug

**Copilot Prompt**:
```csharp
// Integration test: Submit order twice rapidly
// Bug: Discount applied multiple times
// Expected: Discount applied once, subsequent submissions rejected
// Actual: Discount stacks, total becomes negative
```

**Expected Test**:
```csharp
[Fact]
public async Task SubmitOrder_TwiceRapidly_OnlyAppliesDiscountOnce()
{
    // Arrange
    var order = await CreateTestOrder(subtotal: 50.00m);
    var goldCustomer = new Customer { MembershipTier = MembershipTier.Gold }; // 15% discount
    
    // Act: Submit order twice in rapid succession
    var task1 = _client.PostAsync($"/api/v1/orders/{order.Id}/submit?customerId={goldCustomer.Id}", null);
    var task2 = _client.PostAsync($"/api/v1/orders/{order.Id}/submit?customerId={goldCustomer.Id}", null);
    
    await Task.WhenAll(task1, task2);
    
    // Assert
    var response = await _client.GetAsync($"/api/v1/orders/{order.Id}");
    var result = await response.Content.ReadFromJsonAsync<OrderDto>();
    
    // Discount should be $7.50 (15% of $50), not $15 (30%)
    result.DiscountAmount.Should().Be(7.50m);
    result.TotalAmount.Should().BeGreaterThan(0); // Should not be negative
}
```

**Expected Result**: ❌ Test FAILS - Total is negative

### TODO 2.2: Fix with Idempotency Check

**Copilot Prompt**:
```csharp
// Fix double discount bug with idempotency
// Check if order already submitted
// Return 409 Conflict if order already processed
// Use database transaction to prevent race condition
```

**Expected Fix**:
```csharp
// Check order status before applying discount
if (order.Status != OrderStatus.Draft)
{
    return Results.Conflict(new { error = "Order already submitted" });
}

// Update status atomically
order.Status = OrderStatus.Submitted;
order.DiscountAmount = CalculateDiscount(order, customer);

await _context.SaveChangesAsync();
```

### TODO 2.3: Add Concurrency Tests

**Copilot Prompt**:
```csharp
// Test concurrent order submissions
// Use Parallel.For or Task.WhenAll
// Assert: Only one submission succeeds, others return 409
```

---

## Part 3: Bug #3 - Vanishing Game After Checkout

### Bug Description

**Issue**: Game disappears from catalog after being checked out, even when copies are still available.

**Affected Code**: `src/BoardGameCafe.Api/Features/Games/GameEndpoints.cs`

### TODO 3.1: Write Test for Bug

**Copilot Prompt**:
```csharp
// Integration test: Check out game
// Setup: Game has 3 copies, 0 in use
// Act: Check out 1 copy
// Assert: Game still appears in catalog (isAvailable should be true)
// Bug: Game incorrectly marked as unavailable when CopiesInUse < CopiesOwned
```

**Expected**:
```csharp
[Fact]
public async Task CheckoutGame_WithCopiesRemaining_StillAppearsInCatalog()
{
    // Arrange
    var game = await CreateTestGame(copiesOwned: 3, copiesInUse: 0);
    
    // Act: Checkout 1 copy
    await _client.PostAsync($"/api/v1/games/{game.Id}/checkout", null);
    
    // Assert: Game should still be available
    var response = await _client.GetAsync($"/api/v1/games/{game.Id}");
    var result = await response.Content.ReadFromJsonAsync<GameDto>();
    
    result.IsAvailable.Should().BeTrue(); // This FAILS
    result.CopiesInUse.Should().Be(1);
    result.CopiesOwned.Should().Be(3);
}
```

### TODO 3.2: Use Copilot to Find the Bug

**Copilot Prompt**:
```csharp
// Analyze IsAvailable computed property:
public bool IsAvailable => CopiesOwned > CopiesInUse;

// What's wrong with this logic?
// Hint: Should it be > or >= ?
```

**Expected Answer**: Bug is using `>` instead of `>=` - last copy is never available!

### TODO 3.3: Fix and Add Boundary Tests

**Fix**:
```csharp
// Correct logic:
public bool IsAvailable => CopiesInUse < CopiesOwned;
```

**Regression Tests**:
```csharp
// Test boundary conditions:
// - 3 owned, 0 in use → available ✅
// - 3 owned, 2 in use → available ✅  
// - 3 owned, 3 in use → NOT available ✅
// - 3 owned, 4 in use → NOT available (invalid state) ❌
```

---

## Part 4: Bug #4 - Table Time Traveler

### Bug Description

**Issue**: Customers can make reservations for past dates.

**Affected Code**: `src/BoardGameCafe.Api/Features/Reservations/ReservationEndpoints.cs`

### TODO 4.1: Write Validation Test

**Copilot Prompt**:
```csharp
// Test: Create reservation for yesterday
// Expected: 400 Bad Request "Cannot book past dates"
// Actual: Reservation created successfully (BUG)
```

**Expected Test**:
```csharp
[Fact]
public async Task CreateReservation_ForPastDate_ReturnsBadRequest()
{
    // Arrange
    var yesterday = DateTime.UtcNow.AddDays(-1).Date;
    var request = new CreateReservationRequest
    {
        CustomerId = _testCustomer.Id,
        TableId = _testTable.Id,
        ReservationDate = yesterday,
        ReservationTime = new TimeSpan(18, 0, 0),
        PartySize = 4
    };
    
    // Act
    var response = await _client.PostAsJsonAsync("/api/v1/reservations", request);
    
    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.BadRequest); // FAILS - no validation
}
```

### TODO 4.2: Add Validation

**Copilot Prompt**:
```csharp
// Add validation: ReservationDate must be >= today
// Return 400 Bad Request if date is in past
// Use FluentValidation or manual check
```

---

## Part 5: Bug #5 - Order Item Duplication Race Condition

### Bug Description

**Issue**: Rapidly clicking "Add to Cart" creates duplicate order items instead of incrementing quantity.

**Affected Code**: `src/BoardGameCafe.Api/Features/Orders/OrderEndpoints.cs`

### TODO 5.1: Write Concurrency Test

**Copilot Prompt**:
```csharp
// Test: Add same item to cart 5 times concurrently
// Expected: 1 order item with quantity 5
// Actual: 5 separate order items with quantity 1 each (BUG)
// Use Task.WhenAll for concurrent requests
```

**Expected**:
```csharp
[Fact]
public async Task AddItemToCart_ConcurrentRequests_IncrementsQuantity()
{
    // Arrange
    var order = await CreateTestOrder();
    var menuItem = _testMenuItems.First();
    
    // Act: Add item 5 times concurrently
    var tasks = Enumerable.Range(0, 5)
        .Select(_ => _client.PostAsync($"/api/v1/orders/{order.Id}/items", 
            JsonContent.Create(new { menuItemId = menuItem.Id, quantity = 1 })))
        .ToArray();
    
    await Task.WhenAll(tasks);
    
    // Assert
    var response = await _client.GetAsync($"/api/v1/orders/{order.Id}");
    var result = await response.Content.ReadFromJsonAsync<OrderDto>();
    
    result.OrderItems.Should().HaveCount(1); // FAILS - has 5 items
    result.OrderItems.First().Quantity.Should().Be(5);
}
```

### TODO 5.2: Fix with Optimistic Concurrency

**Copilot Prompt**:
```csharp
// Fix race condition:
// Option 1: Use database row locking (FOR UPDATE)
// Option 2: Check for existing item and update quantity
// Option 3: Use optimistic concurrency token (RowVersion)
```

---

## Part 6: Debugging with Copilot

### TODO 6.1: Use Copilot Chat for Debugging

**Practice asking Copilot to help debug**:

```
User: I have a bug where reservations at midnight shift to the next day. 
Can you help me identify the issue in this code?

[Paste code]

Copilot: [Analyzes code, suggests UTC/local time conversion issue]

User: How should I fix this using DateOnly and TimeOnly?

Copilot: [Provides code example]
```

### TODO 6.2: Ask for Test Suggestions

```
User: I just fixed a double discount bug. What regression tests should I write to prevent this in the future?

Copilot: [Suggests idempotency tests, concurrency tests, boundary tests]
```

---

## Reflection Questions

1. **Bug Patterns**: What common patterns did you notice across these bugs? (Timezone, race conditions, validation)

2. **Test-First**: How did writing the failing test first help you understand and fix the bug?

3. **Copilot Debugging**: Was Copilot helpful in identifying root causes? What worked well? What didn't?

4. **Regression Tests**: How many additional tests did you write beyond just the bug fix?

5. **Real-World**: Have you encountered similar bugs in production? How would these tests have helped?

---

## Success Criteria

- [ ] All 5 bugs identified and reproduced
- [ ] Failing test written for each bug
- [ ] All bugs fixed with code changes
- [ ] All tests now pass
- [ ] At least 2 regression tests per bug
- [ ] Used Copilot to suggest fixes
- [ ] Documented root cause for each bug

---

## Bonus Challenges

### Challenge 1: Bug #6 - Case-Sensitive Email
```csharp
// Bug: Customers can create duplicate accounts with same email (different casing)
// Example: "john@example.com" and "JOHN@example.com" treated as different users
// Fix: Use case-insensitive email comparison
```

### Challenge 2: Bug #7 - Event Registration Race
```csharp
// Bug: Last event spot can be double-booked due to race condition
// Fix: Use database transaction isolation level or row locking
```

### Challenge 3: Bug #8 - Loyalty Points Not Reversed
```csharp
// Bug: Cancelled orders don't deduct loyalty points that were earned
// Fix: Add loyalty transaction for reversal
```

### Challenge 4: Add Mutation Testing
```bash
# Install Stryker.NET for mutation testing
dotnet tool install -g dotnet-stryker
cd tests/BoardGameCafe.Tests.Unit
dotnet stryker

# Mutation testing verifies your tests catch bugs
# Are your tests strong enough to catch injected bugs?
```

---

## Next Steps

- Review all bug branches in repository
- Practice finding bugs in peer's code
- Learn about static analysis tools (SonarQube, CodeQL)
- Explore fuzzing techniques for edge case discovery

---

**Instructor Notes**:
- Walk through Bug #1 together as example
- Students work on Bugs #2-5 independently or in pairs
- Discussion: Share interesting bugs found
- Compare different fix approaches
- Discuss how to prevent bugs in original development
- Review best practices for concurrency and validation
- Emphasize importance of regression tests
