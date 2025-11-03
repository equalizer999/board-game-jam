# Exercise 4: Bug Hunting & Regression Testing with GitHub Copilot

## Overview
This exercise demonstrates how to use GitHub Copilot to find intentional bugs, write regression tests, and verify fixes.

**Duration:** 10 minutes  
**Focus:** Debugging, regression test creation, bug reproduction

---

## Learning Objectives
- Use Copilot to understand buggy code
- Write tests that reproduce bugs
- Create regression tests to prevent re-introduction
- Debug with Copilot assistance

---

## Intentional Bugs to Hunt

This repository has **8 intentional bugs** in separate branches for practice:

| Branch | Bug | Impact |
|--------|-----|--------|
| `bug/midnight-reservation` | Timezone conversion issue | Reservations fail at midnight |
| `bug/double-discount` | Discount stacking | Negative order totals |
| `bug/vanishing-game` | Cache invalidation | Game disappears after checkout |
| `bug/table-time-traveler` | Date validation bypass | Can book past dates |
| `bug/order-item-duplication` | Race condition | Duplicate items on rapid clicks |
| `bug/case-sensitive-email` | Email comparison | Duplicate accounts created |
| `bug/event-registration-race` | Concurrency issue | Overbooking events |
| `bug/loyalty-points-reversal` | Points not refunded | Cancelled orders keep points |

---

## Exercise Steps

### Step 1: Choose a Bug to Hunt

**Pick one:** `bug/midnight-reservation`

```bash
# Switch to bug branch
git checkout bug/midnight-reservation

# Start the API
cd src/BoardGameCafe.Api
dotnet run
```

---

### Step 2: Reproduce the Bug with a Test

**Copilot Prompt:**
```csharp
// Generate failing test that reproduces midnight reservation bug
// Test: CreateReservation_AtMidnight_ShouldSucceed
// This test SHOULD FAIL with current code (proving bug exists)
```

---

### Step 3: Locate the Bug with Copilot

**Copilot Prompt:**
```csharp
// @copilot Where is the bug in ReservationValidator.cs?
// Focus on date/time validation logic
// Look for timezone conversion issues
```

---

### Step 4: Fix the Bug

**Copilot Prompt:**
```csharp
// @copilot Fix the timezone bug in IsValidReservationTime
// Ensure consistent timezone usage (prefer UTC)
```

---

### Step 5: Verify Fix with Test

```bash
dotnet test --filter "CreateReservation_AtMidnight_ShouldSucceed"
# Expected: PASS (bug is fixed)
```

---

### Step 6: Add Regression Test Suite

**Copilot Prompt:**
```csharp
// Generate regression test suite for datetime edge cases
// Tests: midnight, before midnight, after midnight, DST boundary
// Use [Theory] with [InlineData]
```

---

## Success Criteria

You've completed this exercise when:
1. ✅ Found and fixed at least 2 bugs
2. ✅ Created reproduction tests for each bug
3. ✅ All regression tests pass
4. ✅ Documented bugs in code comments
5. ✅ Can use Copilot to debug efficiently

---

## Wrap Up

You've completed all 4 workshop exercises! 🎉

**Next Steps:**
- Review the [Copilot Agent Assignment Guide](../docs/copilot-agent-assignment-guide.md)
- Try assigning GitHub issues to Copilot
- Build your own features with Copilot assistance
