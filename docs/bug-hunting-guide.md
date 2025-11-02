# Bug Hunting Guide

> Workshop guide for finding and fixing intentional bugs with GitHub Copilot

## Overview

This repository contains **8 intentional bug branches** designed to help you practice:
- Writing regression tests to catch bugs
- Using GitHub Copilot to generate test cases
- Debugging with test-driven approaches
- Creating comprehensive test coverage

Each bug has a linked GitHub Issue with reproduction steps. Your goal is to:
1. Understand the bug from the issue description
2. Check out the bug branch
3. Write a failing test that demonstrates the bug
4. Fix the bug to make the test pass
5. Ensure no other tests break

---

## Bug Branches

### 🐛 Bug #1: Midnight Reservation Bug
**Branch:** `bug/midnight-reservation`  
**Issue:** [#XX](https://github.com/equalizer999/board-game-jam/issues/XX)  
**Severity:** High  
**Category:** Date/Time Handling

**Description:**  
Reservations made for midnight (00:00) are being stored in the wrong timezone, causing them to appear on the wrong day when displayed to users in different timezones.

**Reproduction Steps:**
1. Create a reservation for 12:00 AM (midnight)
2. Check the database - time is stored correctly in UTC
3. Display the reservation in the UI
4. **Bug:** Reservation shows on the previous day for users in PST/EST

**Hints (Don't Read Until You've Tried!):**
<details>
<summary>Click to reveal hint 1</summary>
Check how DateTime values are being converted between UTC and local time in the API response.
</details>

<details>
<summary>Click to reveal hint 2</summary>
Look at the ReservationDto mapping - is the timezone conversion happening correctly?
</details>

<details>
<summary>Click to reveal hint 3</summary>
The bug is in Features/Reservations/ReservationDto.cs - DateTime.SpecifyKind or ToLocalTime might be missing.
</details>

**Suggested Copilot Prompts:**
```
Generate integration test for reservation creation at midnight UTC.
Verify the reservation appears on the correct day when converted to PST timezone.
Use DateTimeOffset for timezone-aware comparisons.
```

**Expected Test Structure:**
```csharp
[Fact]
public async Task CreateReservation_AtMidnight_ShouldDisplayCorrectDay()
{
    // Arrange
    var midnightUtc = new DateTime(2025, 12, 01, 0, 0, 0, DateTimeKind.Utc);
    var request = new CreateReservationRequest
    {
        StartTime = midnightUtc,
        // ... other properties
    };
    
    // Act
    var response = await _client.PostAsJsonAsync("/api/v1/reservations", request);
    var reservation = await response.Content.ReadFromJsonAsync<ReservationDto>();
    
    // Assert
    reservation.StartTime.Date.Should().Be(new DateTime(2025, 12, 01));
}
```

---

### 🐛 Bug #2: Double Discount Bug
**Branch:** `bug/double-discount`  
**Issue:** [#XX](https://github.com/equalizer999/board-game-jam/issues/XX)  
**Severity:** Critical  
**Category:** Business Logic

**Description:**  
When a Gold member (15% discount) applies loyalty points (e.g., 500 points = $5 discount) to an order, both discounts are being applied incorrectly, resulting in negative order totals.

**Reproduction Steps:**
1. Create customer with Gold membership tier (15% discount)
2. Customer has 1000 loyalty points
3. Place order for $50 worth of items
4. Apply 1000 points discount ($10 off)
5. **Bug:** Final total is -$2.50 instead of $32.50

**Hints:**
<details>
<summary>Click to reveal hint 1</summary>
Check the order calculation logic - are discounts being applied to the already-discounted price?
</details>

<details>
<summary>Click to reveal hint 2</summary>
The order of operations matters: member discount should apply first, then loyalty points.
</details>

<details>
<summary>Click to reveal hint 3</summary>
Look in OrderCalculationService - the loyalty discount is being calculated from the original subtotal instead of the post-member-discount amount.
</details>

**Suggested Copilot Prompts:**
```
Generate unit test for order calculation with both member discount and loyalty points.
Test case: $50 order, Gold member (15% off = $42.50), then $10 loyalty discount.
Expected total: $32.50 (not negative).
Use OrderBuilder and CustomerBuilder with fluent syntax.
```

**Expected Test Structure:**
```csharp
[Fact]
public void CalculateTotal_WithMemberDiscountAndLoyaltyPoints_ShouldNotGoNegative()
{
    // Arrange
    var customer = new CustomerBuilder()
        .WithMembershipTier(MembershipTier.Gold)
        .WithLoyaltyPoints(1000)
        .Build();
    
    var order = new OrderBuilder()
        .WithCustomer(customer)
        .WithSubtotal(50.00m)
        .Build();
    
    // Act
    var total = _orderCalculationService.CalculateTotal(order, loyaltyPointsToRedeem: 1000);
    
    // Assert
    total.Should().Be(32.50m);
    total.Should().BePositive();
}
```

---

### 🐛 Bug #3: Vanishing Game Bug
**Branch:** `bug/vanishing-game`  
**Issue:** [#XX](https://github.com/equalizer999/board-game-jam/issues/XX)  
**Severity:** Medium  
**Category:** Caching / State Management

**Description:**  
When a game is checked out, it disappears from the "Available Games" list in the UI, even though there are still copies available (e.g., 3 owned, 1 in use, should show as available with 2 copies).

**Reproduction Steps:**
1. View game catalog - game shows as available (e.g., "Catan - 3 copies")
2. Checkout 1 copy of the game
3. Refresh the available games list
4. **Bug:** Game completely disappears instead of showing "2 copies available"

**Hints:**
<details>
<summary>Click to reveal hint 1</summary>
Check the filter logic for available games - what condition determines if a game is "available"?
</details>

<details>
<summary>Click to reveal hint 2</summary>
Look at the IsAvailable property - is it using > or >= in the comparison?
</details>

<details>
<summary>Click to reveal hint 3</summary>
The bug is in Game entity's IsAvailable computed property or the GetAvailableGames query filter.
</details>

**Suggested Copilot Prompts:**
```
Generate integration test for game availability after partial checkout.
Scenario: Game has 3 copies, checkout 1 copy, should still appear as available.
Verify IsAvailable returns true when CopiesInUse < CopiesOwned.
```

**Expected Test Structure:**
```csharp
[Fact]
public async Task GetAvailableGames_AfterPartialCheckout_ShouldStillShowGame()
{
    // Arrange
    var game = new GameBuilder()
        .WithCopiesOwned(3)
        .WithCopiesInUse(0)
        .Build();
    await _db.Games.AddAsync(game);
    await _db.SaveChangesAsync();
    
    // Checkout 1 copy
    game.CopiesInUse = 1;
    await _db.SaveChangesAsync();
    
    // Act
    var response = await _client.GetAsync("/api/v1/games?availableOnly=true");
    var games = await response.Content.ReadFromJsonAsync<List<GameDto>>();
    
    // Assert
    games.Should().Contain(g => g.Id == game.Id);
}
```

---

### 🐛 Bug #4: Table Time Traveler Bug
**Branch:** `bug/table-time-traveler`  
**Issue:** [#XX](https://github.com/equalizer999/board-game-jam/issues/XX)  
**Severity:** Medium  
**Category:** Validation

**Description:**  
Users can create reservations for dates in the past through the API, even though the UI has date validation. This causes confusion and invalid data in the database.

**Reproduction Steps:**
1. Send POST request to `/api/v1/reservations` with StartTime in the past
2. **Bug:** Request succeeds with 201 Created
3. Reservation appears in the system with past date
4. Cannot be checked in because it's already "expired"

**Hints:**
<details>
<summary>Click to reveal hint 1</summary>
Client-side validation exists, but server-side validation is missing.
</details>

<details>
<summary>Click to reveal hint 2</summary>
Check the CreateReservationRequest validation - is there a rule for future dates only?
</details>

<details>
<summary>Click to reveal hint 3</summary>
Add FluentValidation rule: RuleFor(x => x.StartTime).GreaterThan(DateTime.UtcNow)
</details>

**Suggested Copilot Prompts:**
```
Generate integration test for reservation creation with past date.
Should return 400 Bad Request with validation error.
Test both API endpoint and FluentValidation rules.
Include error message verification.
```

**Expected Test Structure:**
```csharp
[Fact]
public async Task CreateReservation_WithPastDate_ShouldReturn400()
{
    // Arrange
    var pastDate = DateTime.UtcNow.AddDays(-1);
    var request = new CreateReservationRequest
    {
        StartTime = pastDate,
        // ... other valid properties
    };
    
    // Act
    var response = await _client.PostAsJsonAsync("/api/v1/reservations", request);
    
    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    var error = await response.Content.ReadAsStringAsync();
    error.Should().Contain("future");
}
```

---

### 🐛 Bug #5: Order Item Duplication Bug
**Branch:** `bug/order-item-duplication`  
**Issue:** [#XX](https://github.com/equalizer999/board-game-jam/issues/XX)  
**Severity:** High  
**Category:** Concurrency / Race Condition

**Description:**  
Rapidly clicking "Add to Cart" for the same menu item creates duplicate entries instead of incrementing quantity, leading to incorrect order totals.

**Reproduction Steps:**
1. Click "Add to Cart" for "Meeple Mocha" once
2. Quickly click "Add to Cart" 4 more times (within 1 second)
3. **Bug:** Cart shows 5 separate line items instead of 1 item with quantity 5
4. Total is calculated incorrectly

**Hints:**
<details>
<summary>Click to reveal hint 1</summary>
This is a race condition - multiple requests hit the server before the first completes.
</details>

<details>
<summary>Click to reveal hint 2</summary>
Check if the "add item to order" logic checks for existing order items before creating new ones.
</details>

<details>
<summary>Click to reveal hint 3</summary>
Need to implement optimistic concurrency or check for existing OrderItem with same MenuItemId before inserting.
</details>

**Suggested Copilot Prompts:**
```
Generate integration test simulating concurrent add-to-cart requests.
Send 5 parallel requests to add same menu item to order.
Verify result has only 1 OrderItem with Quantity = 5.
Use Task.WhenAll for concurrent execution.
```

**Expected Test Structure:**
```csharp
[Fact]
public async Task AddOrderItem_ConcurrentRequests_ShouldConsolidateQuantity()
{
    // Arrange
    var orderId = await CreateTestOrder();
    var menuItemId = await GetTestMenuItem();
    var request = new AddOrderItemRequest { MenuItemId = menuItemId, Quantity = 1 };
    
    // Act - Send 5 concurrent requests
    var tasks = Enumerable.Range(0, 5)
        .Select(_ => _client.PostAsJsonAsync($"/api/v1/orders/{orderId}/items", request));
    await Task.WhenAll(tasks);
    
    // Assert
    var order = await _client.GetFromJsonAsync<OrderDto>($"/api/v1/orders/{orderId}");
    var menuItemOrders = order.Items.Where(i => i.MenuItemId == menuItemId);
    menuItemOrders.Should().HaveCount(1);
    menuItemOrders.First().Quantity.Should().Be(5);
}
```

---

### 🐛 Bug #6: Case Sensitive Email Bug
**Branch:** `bug/case-sensitive-email`  
**Issue:** [#XX](https://github.com/equalizer999/board-game-jam/issues/XX)  
**Severity:** Medium  
**Category:** Data Integrity

**Description:**  
Customers can create multiple accounts with the same email address using different casing (e.g., john@example.com, John@Example.com, JOHN@EXAMPLE.COM), leading to duplicate accounts.

**Reproduction Steps:**
1. Create customer with email "john@example.com"
2. Create another customer with email "John@Example.com"
3. **Bug:** Both accounts are created successfully
4. Loyalty points and order history are split across accounts

**Hints:**
<details>
<summary>Click to reveal hint 1</summary>
Email uniqueness constraint isn't case-insensitive.
</details>

<details>
<summary>Click to reveal hint 2</summary>
Check the database index on Customer.Email - is it case-sensitive or case-insensitive?
</details>

<details>
<summary>Click to reveal hint 3</summary>
EF Core needs explicit configuration for case-insensitive uniqueness. Check OnModelCreating in DbContext.
</details>

**Suggested Copilot Prompts:**
```
Generate integration test for customer registration with duplicate email in different case.
First registration: john@example.com
Second registration: JOHN@EXAMPLE.COM
Should return 409 Conflict on second attempt.
```

**Expected Test Structure:**
```csharp
[Fact]
public async Task CreateCustomer_WithDuplicateEmailDifferentCase_ShouldReturn409()
{
    // Arrange
    var email = "john@example.com";
    var request1 = new CreateCustomerRequest { Email = email.ToLower() };
    var request2 = new CreateCustomerRequest { Email = email.ToUpper() };
    
    // Act
    var response1 = await _client.PostAsJsonAsync("/api/v1/customers", request1);
    var response2 = await _client.PostAsJsonAsync("/api/v1/customers", request2);
    
    // Assert
    response1.StatusCode.Should().Be(HttpStatusCode.Created);
    response2.StatusCode.Should().Be(HttpStatusCode.Conflict);
}
```

---

### 🐛 Bug #7: Event Registration Race Condition
**Branch:** `bug/event-registration-race`  
**Issue:** [#XX](https://github.com/equalizer999/board-game-jam/issues/XX)  
**Severity:** High  
**Category:** Concurrency

**Description:**  
When an event has 1 spot remaining and 5 users try to register simultaneously, sometimes 2-3 users successfully register, exceeding the MaxParticipants limit.

**Reproduction Steps:**
1. Create event with MaxParticipants = 10
2. Register 9 customers
3. Have 5 users click "Register" simultaneously
4. **Bug:** Event shows 11-12 participants instead of 10

**Hints:**
<details>
<summary>Click to reveal hint 1</summary>
Classic race condition - capacity check and registration aren't atomic.
</details>

<details>
<summary>Click to reveal hint 2</summary>
Need to use database transaction with appropriate isolation level.
</details>

<details>
<summary>Click to reveal hint 3</summary>
Use IsolationLevel.Serializable or add unique constraint on (EventId, CustomerId) + check capacity within transaction.
</details>

**Suggested Copilot Prompts:**
```
Generate integration test simulating race condition for last event spot.
Create event with MaxParticipants = 10 and 9 existing registrations.
Send 5 concurrent registration requests using Task.WhenAll.
Verify only 1 succeeds (201 Created), others fail (409 Conflict).
Final participant count should be exactly 10.
```

**Expected Test Structure:**
```csharp
[Fact]
public async Task RegisterForEvent_ConcurrentRequestsForLastSpot_ShouldAllowOnlyOne()
{
    // Arrange
    var eventId = await CreateEventWithCapacity(10);
    await RegisterCustomers(eventId, count: 9);
    
    var customers = Enumerable.Range(0, 5).Select(i => CreateCustomer($"user{i}@test.com"));
    
    // Act
    var registrationTasks = customers.Select(customerId =>
        _client.PostAsJsonAsync($"/api/v1/events/{eventId}/register", 
            new { CustomerId = customerId }));
    var responses = await Task.WhenAll(registrationTasks);
    
    // Assert
    responses.Count(r => r.StatusCode == HttpStatusCode.Created).Should().Be(1);
    responses.Count(r => r.StatusCode == HttpStatusCode.Conflict).Should().Be(4);
    
    var eventDetails = await _client.GetFromJsonAsync<EventDto>($"/api/v1/events/{eventId}");
    eventDetails.CurrentParticipants.Should().Be(10);
}
```

---

### 🐛 Bug #8: Loyalty Points Reversal Bug
**Branch:** `bug/loyalty-points-reversal`  
**Issue:** [#XX](https://github.com/equalizer999/board-game-jam/issues/XX)  
**Severity:** Medium  
**Category:** Business Logic

**Description:**  
When an order is cancelled after being paid, the loyalty points earned from that order are not deducted from the customer's account, allowing customers to accumulate fraudulent points.

**Reproduction Steps:**
1. Customer places order for $100 (earns 100 loyalty points)
2. Order status changes to "Paid" - points are added to account
3. Customer cancels the order
4. **Bug:** Customer keeps the 100 points
5. Repeat to accumulate unlimited points

**Hints:**
<details>
<summary>Click to reveal hint 1</summary>
Check the order cancellation logic - does it reverse loyalty point transactions?
</details>

<details>
<summary>Click to reveal hint 2</summary>
Need to create a negative LoyaltyPointsHistory entry when order is cancelled.
</details>

<details>
<summary>Click to reveal hint 3</summary>
The fix should be in the order status update endpoint when transitioning to "Cancelled" status.
</details>

**Suggested Copilot Prompts:**
```
Generate integration test for order cancellation with loyalty points reversal.
Steps:
1. Create paid order that earned 100 points
2. Verify customer has 100 points
3. Cancel the order
4. Verify customer points reduced by 100
5. Check LoyaltyPointsHistory has deduction entry
```

**Expected Test Structure:**
```csharp
[Fact]
public async Task CancelOrder_AfterPointsEarned_ShouldReversePoints()
{
    // Arrange
    var customerId = await CreateTestCustomer();
    var orderId = await CreatePaidOrder(customerId, total: 100m);
    
    var customerBefore = await _client.GetFromJsonAsync<CustomerDto>(
        $"/api/v1/customers/{customerId}");
    customerBefore.LoyaltyPoints.Should().Be(100);
    
    // Act
    await _client.PatchAsync($"/api/v1/orders/{orderId}/cancel", null);
    
    // Assert
    var customerAfter = await _client.GetFromJsonAsync<CustomerDto>(
        $"/api/v1/customers/{customerId}");
    customerAfter.LoyaltyPoints.Should().Be(0);
    
    var history = await _client.GetFromJsonAsync<List<LoyaltyTransactionDto>>(
        $"/api/v1/customers/{customerId}/loyalty-history");
    history.Should().Contain(t => t.Points == -100 && t.Description.Contains("cancelled"));
}
```

---

## General Tips for Bug Hunting

### 1. Read the Issue First
- Understand the expected behavior
- Note the severity and category
- Review reproduction steps

### 2. Write the Test First (TDD Approach)
- Create a failing test that demonstrates the bug
- This becomes your regression test
- Ensures the bug is actually fixed

### 3. Use Copilot Effectively
- Provide context about the bug in your prompts
- Reference existing test patterns
- Ask for edge cases related to the bug

### 4. Debug with Tests
- Run tests in isolation
- Use debugger breakpoints in test methods
- Check actual vs. expected values

### 5. Fix Minimally
- Make the smallest change that fixes the bug
- Don't refactor unrelated code
- Ensure no other tests break

### 6. Verify the Fix
- Run all related tests
- Test manually if possible
- Check for similar bugs elsewhere

---

## Copilot Prompts for Each Phase

### Phase 1: Understanding
```
Explain how [component/feature] works based on the code in [file].
What are the edge cases for [functionality]?
```

### Phase 2: Test Creation
```
Generate integration test for [bug scenario].
Include setup, execution, and assertion.
Use [existing test pattern] as reference.
```

### Phase 3: Debugging
```
What could cause [unexpected behavior] in this code?
Identify the validation/calculation logic for [feature].
```

### Phase 4: Fixing
```
Implement fix for [bug] in [file].
Add validation rule: [business rule].
Update [method] to handle [edge case].
```

### Phase 5: Verification
```
Generate additional test cases for [fixed functionality].
Test scenarios: [list edge cases].
Ensure [business rule] is enforced.
```

---

## Success Criteria

For each bug, you should:
- ✅ Write a failing test that reproduces the bug
- ✅ Understand the root cause
- ✅ Implement a fix that makes the test pass
- ✅ Verify all existing tests still pass
- ✅ Document the fix in commit message
- ✅ Optionally: Add additional tests for similar edge cases

---

## Workshop Flow

1. **Introduction (5 min)** - Overview of bugs and testing approach
2. **Demo Bug #1 (10 min)** - Instructor demonstrates full cycle
3. **Hands-On Bugs #2-4 (30 min)** - Participants work on bugs with hints
4. **Discussion (10 min)** - Share solutions and learnings
5. **Advanced Bugs #5-8 (Optional)** - For experienced participants

---

## Additional Resources

- [xUnit Documentation](https://xunit.net/)
- [FluentAssertions Documentation](https://fluentassertions.com/)
- [EF Core Testing Guide](https://learn.microsoft.com/en-us/ef/core/testing/)
- [GitHub Copilot for Testing](https://docs.github.com/en/copilot)

---

**Note:** These bugs are intentional for educational purposes. In a real project, always write tests first and follow TDD practices to prevent bugs from reaching production!
