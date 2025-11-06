# GitHub Issues Template for Bug Demonstration

This document contains templates for creating GitHub Issues for each bug demonstration branch.

---

## Issue #1: Midnight Reservation Shows Wrong Date

**Labels:** `bug-demo`, `datetime`, `high-severity`

**Description:**

Reservations created at midnight (00:00) are being stored with incorrect date information, causing them to appear on the wrong day when displayed to users.

**Branch:** `bug/midnight-reservation`

**Steps to Reproduce:**
1. Navigate to reservation creation page
2. Select a date (e.g., December 1, 2025)
3. Set time to 12:00 AM (midnight)
4. Create the reservation
5. View the reservation details

**Expected Behavior:**
The reservation should display on December 1, 2025.

**Actual Behavior:**
The reservation may display on November 30, 2025 or December 2, 2025 depending on timezone.

**Root Cause:**
Timezone conversion issues in `ReservationsEndpoints.ToDto()` - DateTime.Add() without proper DateTimeKind specification.

**Affected Code:**
- File: `src/BoardGameCafe.Api/Features/Reservations/ReservationsEndpoints.cs`
- Method: `ToDto()`
- Line: ~390

**Suggested Test:**
```csharp
[Fact]
public async Task CreateReservation_AtMidnight_ShouldDisplayCorrectDate()
{
    // Test creating reservation at midnight UTC
    // Verify ReservationDate is correct in different timezones
}
```

---

## Issue #2: Order Total Goes Negative with Member Discount and Loyalty Points

**Labels:** `bug-demo`, `business-logic`, `critical`

**Description:**

When a Gold member (15% discount) applies loyalty points to an order, the total can become negative instead of staying at zero minimum.

**Branch:** `bug/double-discount`

**Steps to Reproduce:**
1. Create customer with Gold membership tier (2000+ loyalty points)
2. Customer places order for $50 worth of items
3. Apply 1000 loyalty points ($10 discount)
4. Calculate order total

**Expected Behavior:**
- Gold discount: $50 × 15% = $7.50 off → $42.50
- Loyalty discount: $10 off from remaining amount
- Final total: $32.50 (or minimum $0)

**Actual Behavior:**
Final total becomes negative (e.g., -$2.50).

**Root Cause:**
Missing validation in `OrderCalculationService.CalculateOrderTotals()` - loyalty discount calculated from original subtotal instead of post-member-discount amount.

**Affected Code:**
- File: `src/BoardGameCafe.Api/Features/Orders/OrderCalculationService.cs`
- Method: `CalculateOrderTotals()`
- Lines: 30-48

**Suggested Test:**
```csharp
[Fact]
public void CalculateTotal_WithMemberDiscountAndLoyaltyPoints_ShouldNotGoNegative()
{
    // Test Gold member with $50 order and 1000 points redemption
    // Verify total is non-negative
}
```

---

## Issue #3: Game Disappears from Available List After First Checkout

**Labels:** `bug-demo`, `availability`, `medium-severity`

**Description:**

When a game with multiple copies (e.g., 3 owned) has one copy checked out, it completely disappears from the "Available Games" list instead of showing remaining copies.

**Branch:** `bug/vanishing-game`

**Steps to Reproduce:**
1. View game catalog, find game with 3 copies (all available)
2. Check out 1 copy of the game
3. Refresh the "Available Games" list

**Expected Behavior:**
Game should still appear in available games with "2 copies available" indicator.

**Actual Behavior:**
Game completely disappears from available games list.

**Root Cause:**
Wrong logic in `Game.IsAvailable` computed property - checks `CopiesInUse <= 0` instead of `CopiesInUse < CopiesOwned`.

**Affected Code:**
- File: `src/BoardGameCafe.Domain/Game.cs`
- Property: `IsAvailable`
- Line: ~47

**Suggested Test:**
```csharp
[Fact]
public async Task GetAvailableGames_AfterPartialCheckout_ShouldStillShowGame()
{
    // Create game with 3 copies
    // Check out 1 copy
    // Verify game appears in available games list
}
```

---

## Issue #4: Can Book Table for Past Dates via API

**Labels:** `bug-demo`, `validation`, `medium-severity`

**Description:**

Server-side validation is missing for past dates in reservation creation, allowing users to book tables for dates that have already passed through direct API calls.

**Branch:** `bug/table-time-traveler`

**Steps to Reproduce:**
1. Use curl or Postman to send POST request to `/api/v1/reservations`
2. Set `ReservationDate` to a past date (e.g., yesterday)
3. Include all other required valid fields

**Expected Behavior:**
API should return 400 Bad Request with error message about past dates.

**Actual Behavior:**
API returns 201 Created and creates the reservation.

**Root Cause:**
Server-side past date validation removed from `ValidateReservationRequest()` - only client-side validation exists.

**Affected Code:**
- File: `src/BoardGameCafe.Api/Features/Reservations/ReservationsEndpoints.cs`
- Method: `ValidateReservationRequest()`
- Lines: ~407-421

**cURL Example:**
```bash
curl -X POST https://api.example.com/api/v1/reservations \
  -H "Content-Type: application/json" \
  -d '{
    "customerId": "...",
    "tableId": "...",
    "reservationDate": "2024-01-01",
    "startTime": "19:00:00",
    "endTime": "21:00:00",
    "partySize": 4
  }'
```

**Suggested Test:**
```csharp
[Fact]
public async Task CreateReservation_WithPastDate_ShouldReturn400()
{
    // Create reservation with past date
    // Verify 400 Bad Request response
}
```

---

## Issue #5: Rapid "Add to Cart" Creates Duplicate Items

**Labels:** `bug-demo`, `concurrency`, `high-severity`

**Description:**

Clicking "Add to Cart" multiple times in quick succession creates duplicate line items instead of incrementing quantity.

**Branch:** `bug/order-item-duplication`

**Steps to Reproduce:**
1. Open order/cart page
2. Rapidly click "Add to Cart" for "Meeple Mocha" 5 times (within 1 second)
3. View cart contents

**Expected Behavior:**
Cart shows 1 line item: "Meeple Mocha - Quantity: 5"

**Actual Behavior:**
Cart shows 5 separate line items: "Meeple Mocha - Quantity: 1" (×5)

**Root Cause:**
Race condition in `AddOrderItem()` endpoint - removed check for existing items, no debounce or idempotency.

**Affected Code:**
- File: `src/BoardGameCafe.Api/Features/Orders/OrdersEndpoints.cs`
- Method: `AddOrderItem()`
- Lines: ~237-258

**Suggested Test:**
```csharp
[Fact]
public async Task AddOrderItem_ConcurrentRequests_ShouldConsolidateQuantity()
{
    // Send 5 concurrent add-to-cart requests
    // Verify 1 order item with quantity 5
}
```

---

## Issue #6: Duplicate Accounts Created with Different Email Casing

**Labels:** `bug-demo`, `data-integrity`, `medium-severity`

**Description:**

Users can create multiple accounts with the same email address using different casing (e.g., john@example.com, John@Example.com, JOHN@EXAMPLE.COM).

**Branch:** `bug/case-sensitive-email`

**Steps to Reproduce:**
1. Create customer account with email "john@example.com"
2. Create another customer account with email "John@Example.com"
3. Both accounts are created successfully

**Expected Behavior:**
Second registration should fail with 409 Conflict error.

**Actual Behavior:**
Both accounts created, loyalty points and order history split across accounts.

**Root Cause:**
SQLite unique index on Email column is case-sensitive by default, no email normalization to lowercase.

**Affected Code:**
- File: `src/BoardGameCafe.Api/Data/BoardGameCafeDbContext.cs`
- Configuration: Customer entity
- Lines: ~52-63

**Suggested Test:**
```csharp
[Fact]
public async Task CreateCustomer_WithDuplicateEmailDifferentCase_ShouldReturn409()
{
    // Register with john@example.com
    // Try to register with JOHN@EXAMPLE.COM
    // Verify 409 Conflict
}
```

---

## Issue #7: Event Can Be Overbooked Under Concurrent Registrations

**Labels:** `bug-demo`, `concurrency`, `high-severity`

**Description:**

When an event has 1 spot remaining and multiple users try to register simultaneously, 2-3 users can successfully register, exceeding the MaxParticipants limit.

**Branch:** `bug/event-registration-race`

**Steps to Reproduce:**
1. Create event with MaxParticipants = 10
2. Register 9 customers
3. Have 5 different users click "Register" at the same time (simulate with concurrent API calls)

**Expected Behavior:**
- Only 1 registration succeeds (201 Created)
- Other 4 fail with 409 Conflict
- Event ends with exactly 10 participants

**Actual Behavior:**
- 2-3 registrations succeed
- Event ends with 11-12 participants

**Root Cause:**
Classic check-then-act race condition - removed transaction with Serializable isolation level.

**Affected Code:**
- File: `src/BoardGameCafe.Api/Features/Events/EventsEndpoints.cs`
- Method: `RegisterForEvent()`
- Lines: ~199-290

**Suggested Test:**
```csharp
[Fact]
public async Task RegisterForEvent_ConcurrentRequestsForLastSpot_ShouldAllowOnlyOne()
{
    // Setup event with 9/10 spots filled
    // Send 5 concurrent registration requests
    // Verify only 1 succeeds
}
```

---

## Issue #8: Cancelled Order Doesn't Reverse Loyalty Points

**Labels:** `bug-demo`, `business-logic`, `medium-severity`

**Description:**

When an order is cancelled after payment (and loyalty points have been earned), the points are not deducted from the customer's account.

**Branch:** `bug/loyalty-points-reversal`

**Steps to Reproduce:**
1. Customer places order for $100
2. Order is paid (status: Completed) - customer earns 100 loyalty points
3. Cancel the order
4. Check customer loyalty points balance

**Expected Behavior:**
- Customer loyalty points reduced by 100
- LoyaltyPointsHistory shows negative transaction entry

**Actual Behavior:**
- Customer keeps the 100 loyalty points
- No compensating transaction created

**Root Cause:**
`CancelOrder()` endpoint missing logic to reverse points for paid/completed orders.

**Affected Code:**
- File: `src/BoardGameCafe.Api/Features/Orders/OrdersEndpoints.cs`
- Method: `CancelOrder()`
- Lines: ~564-599

**Suggested Test:**
```csharp
[Fact]
public async Task CancelOrder_AfterPointsEarned_ShouldReversePoints()
{
    // Create paid order that earned 100 points
    // Cancel order
    // Verify customer points reduced by 100
}
```

---

## Creating Issues on GitHub

To create these issues on GitHub:

1. Go to https://github.com/equalizer999/board-game-jam/issues/new
2. Copy the content for each issue
3. Add the specified labels
4. Create the issue
5. Note the issue number
6. Update `docs/bug-hunting-guide.md` with the issue number

Or use GitHub CLI:
```bash
gh issue create --title "Midnight Reservation Shows Wrong Date" \
  --body "$(cat issue-1-content.md)" \
  --label "bug-demo,datetime,high-severity"
```
