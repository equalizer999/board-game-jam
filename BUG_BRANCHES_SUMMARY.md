# Bug Demonstration Branches - Summary

This document provides a summary of all bug demonstration branches created for the workshop.

## Overview

Eight intentional bug branches have been created, each demonstrating a common real-world bug that participants will practice finding and fixing using GitHub Copilot.

## Branch List

### 1. bug/midnight-reservation
**Commit:** f9f7985  
**Category:** Date/Time Handling  
**Severity:** High

**Bug Description:**  
Timezone bug where combining date and time without proper DateTimeKind specification causes midnight reservations to display on wrong day.

**Root Cause:**  
- In `ReservationsEndpoints.ToDto()`, using `DateTime.Add()` without specifying Kind creates unspecified DateTime
- This leads to timezone conversion issues, especially at midnight boundaries

**File Modified:**  
`src/BoardGameCafe.Api/Features/Reservations/ReservationsEndpoints.cs`

**How to Reproduce:**
1. Create a reservation for midnight (00:00)
2. Check the ReservationDto returned by API
3. Bug: ReservationDate shows wrong day after timezone conversion

---

### 2. bug/double-discount
**Commit:** 9ea94e0  
**Category:** Business Logic  
**Severity:** Critical

**Bug Description:**  
Missing negative total prevention allows orders with Gold membership (15% discount) + loyalty points to result in negative totals.

**Root Cause:**  
- Removed negative total validation in `OrderCalculationService.CalculateOrderTotals()`
- Loyalty discount calculated from original subtotal instead of post-member-discount amount

**File Modified:**  
`src/BoardGameCafe.Api/Features/Orders/OrderCalculationService.cs`

**How to Reproduce:**
1. Create order for $50 with Gold member (15% = $7.50 off)
2. Apply 1000 loyalty points ($10 off)
3. Bug: Total becomes negative instead of $32.50

---

### 3. bug/vanishing-game
**Commit:** a9d26c7  
**Category:** Caching / State Management  
**Severity:** Medium

**Bug Description:**  
Game disappears from available games list when ANY copy is checked out, instead of when ALL copies are in use.

**Root Cause:**  
- `Game.IsAvailable` property uses wrong logic: `CopiesInUse <= 0`
- Should be: `CopiesInUse < CopiesOwned`

**File Modified:**  
`src/BoardGameCafe.Domain/Game.cs`

**How to Reproduce:**
1. Game has 3 copies owned, 0 in use (available)
2. Check out 1 copy (now 1 in use)
3. Bug: Game vanishes from available games list
4. Expected: Game should still show as available with 2 copies remaining

---

### 4. bug/table-time-traveler
**Commit:** 6a1f1d6  
**Category:** Validation  
**Severity:** Medium

**Bug Description:**  
Missing server-side validation allows booking tables for past dates via direct API calls.

**Root Cause:**  
- Removed past date validation in `ValidateReservationRequest()`
- Validation only exists on client-side

**File Modified:**  
`src/BoardGameCafe.Api/Features/Reservations/ReservationsEndpoints.cs`

**How to Reproduce:**
1. Send POST to `/api/v1/reservations` with past ReservationDate
2. Bug: Request succeeds with 201 Created
3. Expected: Should return 400 Bad Request

---

### 5. bug/order-item-duplication
**Commit:** b2b7c50  
**Category:** Concurrency / Race Condition  
**Severity:** High

**Bug Description:**  
Rapid clicking "Add to Cart" creates duplicate line items instead of consolidating quantity.

**Root Cause:**  
- Removed check for existing order items in `AddOrderItem()`
- No debounce or idempotency check
- Multiple concurrent requests all pass validation before any SaveChanges

**File Modified:**  
`src/BoardGameCafe.Api/Features/Orders/OrdersEndpoints.cs`

**How to Reproduce:**
1. Rapidly click "Add to Cart" 5 times for same item
2. Bug: Cart shows 5 separate line items with quantity 1 each
3. Expected: 1 line item with quantity 5

---

### 6. bug/case-sensitive-email
**Commit:** 2d86716  
**Category:** Data Integrity  
**Severity:** Medium

**Bug Description:**  
Email unique constraint is case-sensitive, allowing duplicate accounts with different casing (john@example.com, John@Example.com).

**Root Cause:**  
- SQLite unique index is case-sensitive by default
- No email normalization to lowercase before storage
- Missing NOCASE collation on email index

**File Modified:**  
`src/BoardGameCafe.Api/Data/BoardGameCafeDbContext.cs`

**How to Reproduce:**
1. Create customer with email "john@example.com"
2. Create another customer with "John@Example.com"
3. Bug: Both succeed, creating two accounts
4. Expected: Second should fail with 409 Conflict

---

### 7. bug/event-registration-race
**Commit:** 83d76e2  
**Category:** Concurrency  
**Severity:** High

**Bug Description:**  
Classic check-then-act race condition allows event overbooking when multiple users register simultaneously for last spot.

**Root Cause:**  
- Removed transaction with Serializable isolation level
- Capacity check and registration aren't atomic
- Multiple requests can all see available capacity before any SaveChanges

**File Modified:**  
`src/BoardGameCafe.Api/Features/Events/EventsEndpoints.cs`

**How to Reproduce:**
1. Event has MaxParticipants = 10, CurrentParticipants = 9
2. 5 users click "Register" simultaneously
3. Bug: 2-3 succeed, event now has 11-12 participants
4. Expected: Only 1 succeeds, others get 409 Conflict

---

### 8. bug/loyalty-points-reversal
**Commit:** 6d067e4  
**Category:** Business Logic / Compensating Transaction  
**Severity:** Medium

**Bug Description:**  
Cancelled orders don't reverse loyalty points earned, allowing customers to accumulate fraudulent points.

**Root Cause:**  
- Added `CancelOrder()` endpoint without point reversal logic
- No negative LoyaltyPointsHistory entry created
- Customer keeps points from cancelled orders

**File Modified:**  
`src/BoardGameCafe.Api/Features/Orders/OrdersEndpoints.cs`

**How to Reproduce:**
1. Place $100 order (earns 100 points)
2. Order status changes to Completed (points awarded)
3. Cancel the order
4. Bug: Customer keeps 100 points
5. Expected: Points should be deducted

---

## Branch Base

All bug branches are created from commit `74c0915` (Merge pull request #64).

## Git Commands to Access Branches

```bash
# List all bug branches
git branch -r | grep bug/

# Checkout a specific bug branch
git checkout bug/midnight-reservation

# View commit details
git show --stat

# Compare bug branch with base
git diff 74c0915..bug/midnight-reservation
```

## Next Steps

1. **Create GitHub Issues:** Each bug needs a corresponding GitHub Issue with:
   - Clear reproduction steps
   - Expected vs Actual behavior
   - Link to buggy code location
   - Suggested test to prevent regression
   - Label: `bug-demo`

2. **Update bug-hunting-guide.md:** Replace placeholder issue numbers (XX) with actual issue numbers.

3. **Workshop Setup:** Each participant should:
   - Fork the repository
   - Checkout a bug branch
   - Write failing test
   - Fix the bug
   - Verify all tests pass

## Testing Each Bug

Each bug can be tested by:
1. Checking out the bug branch
2. Running the application
3. Following reproduction steps in the issue
4. Writing a test that demonstrates the bug
5. Fixing the bug
6. Verifying the test passes

## Notes

- All bugs are intentional and designed for educational purposes
- Each bug represents a common real-world mistake
- Bugs demonstrate different categories: validation, concurrency, business logic, data integrity
- Solutions should be minimal and focused on the specific bug
