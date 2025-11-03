# Workshop Exercises: GitHub Copilot Testing Demonstrations

## Overview

This folder contains **hands-on exercises** for the 60-minute Board Game Café workshop, demonstrating GitHub Copilot's testing capabilities across different domains.

Each exercise includes:
- 📝 Learning objectives
- 🎯 Step-by-step prompts for Copilot
- ✅ Verification checklists
- 💡 Tips and best practices

---

## Exercise Structure

| Exercise | Duration | Focus Area | Key Skills |
|----------|----------|------------|------------|
| [01-unit-testing.md](./01-unit-testing.md) | 10 min | Backend business logic | xUnit, FluentAssertions, test data builders |
| [02-api-testing.md](./02-api-testing.md) | 8 min | REST API integration tests | Swagger contracts, HTTP testing, WebApplicationFactory |
| [03-ui-testing.md](./03-ui-testing.md) | 7 min | E2E browser automation | Playwright, Page Object Model, cross-browser testing |
| [04-bug-hunting.md](./04-bug-hunting.md) | 10 min | Debugging & regression tests | Bug reproduction, regression suites, edge case testing |

**Total Duration:** ~35 minutes of core exercises + 25 minutes for Q&A and exploration

---

## Prerequisites

Before starting the exercises, ensure:

1. **Backend is running:**
   ```bash
   cd src/BoardGameCafe.Api
   dotnet restore
   dotnet run
   ```

2. **Frontend is running:**
   ```bash
   cd client
   npm install
   npm run dev
   ```

3. **Playwright is installed (for Exercise 3):**
   ```bash
   cd client
   npx playwright install
   ```

4. **Swagger is accessible:**
   - Navigate to: https://localhost:5001/swagger
   - Verify API endpoints are documented

---

## Quick Start

### For Workshop Instructors

**Live Demo Flow (60 minutes):**

| Time | Activity | Exercise |
|------|----------|----------|
| 0-5 min | Welcome & Setup | Verify environments running |
| 5-15 min | Unit Testing Demo | Exercise 1: Steps 1-5 |
| 15-23 min | API Testing Demo | Exercise 2: Steps 1-5 |
| 23-30 min | UI Testing Demo | Exercise 3: Steps 1-4 |
| 30-40 min | Bug Hunting Demo | Exercise 4: Fix 2 bugs |
| 40-50 min | CI/CD Integration | Show GitHub Actions running tests |
| 50-60 min | Q&A & Wrap-up | Open discussion |

**Instructor Tips:**
- Start each exercise with a quick overview (1 min)
- Demonstrate 2-3 key prompts live
- Let Copilot generate code on screen
- Highlight when Copilot suggestions are helpful vs. need refinement
- Keep exercises moving - don't get stuck on one test

### For Self-Paced Learning

**Recommended Path:**
1. Complete exercises in order (1 → 2 → 3 → 4)
2. Spend full time on each exercise
3. Try all Copilot prompts
4. Run all tests to verify your work
5. Experiment with your own variations

---

## Exercise Summaries

### Exercise 1: Unit Testing (10 min)
**File:** [01-unit-testing.md](./01-unit-testing.md)

Generate comprehensive unit tests for `OrderCalculationService`:
- Tax calculation tests (8% food, 10% alcohol)
- Member discount tests (Bronze/Silver/Gold tiers)
- Loyalty points redemption tests
- Edge case handling (zeros, nulls, boundaries)

**Key Takeaway:** Use Copilot to quickly generate test suites with `[Theory]` and `[InlineData]` for parameterized testing.

---

### Exercise 2: API Testing (8 min)
**File:** [02-api-testing.md](./02-api-testing.md)

Create integration tests for Games REST API:
- Test CRUD operations (Create, Read, Update, Delete)
- Validate Swagger contracts
- Test query filters (category, player count, availability)
- Test error scenarios (400, 404, 409 status codes)

**Key Takeaway:** Use Swagger as the source of truth for API contracts and let Copilot generate matching integration tests.

---

### Exercise 3: UI Testing (7 min)
**File:** [03-ui-testing.md](./03-ui-testing.md)

Build E2E tests with Playwright:
- Create Page Object Models for game catalog, reservations
- Test user workflows (browse games, make reservation)
- Run tests across 3 browsers (Chrome, Firefox, Safari)
- Add stable test selectors with data-testid

**Key Takeaway:** Use Copilot to generate Page Object Models that encapsulate page interactions for reusable, maintainable tests.

---

### Exercise 4: Bug Hunting (10 min)
**File:** [04-bug-hunting.md](./04-bug-hunting.md)

Find and fix intentional bugs:
- Reproduce bugs with failing tests
- Use Copilot to locate bug in code
- Apply fixes
- Create regression test suites

**Bugs available:**
- Midnight reservation timezone issue
- Double discount negative total
- Game cache invalidation
- (5 more bugs in separate branches)

**Key Takeaway:** Write failing tests first to prove bugs exist, then use Copilot to suggest fixes, then verify with passing tests.

---

## Running the Tests

All exercises include verification steps. Run these commands to ensure everything works:

```bash
# Unit tests (Exercise 1)
cd tests/BoardGameCafe.Tests.Unit
dotnet test

# Integration tests (Exercise 2)
cd tests/BoardGameCafe.Tests.Integration
dotnet test

# E2E tests (Exercise 3)
cd client
npx playwright test

# Regression tests (Exercise 4)
dotnet test --filter "BugRegressionTests"
```

---

## Resources

- **Copilot Prompts Guide:** [../docs/copilot-prompts-guide.md](../docs/copilot-prompts-guide.md)
- **Copilot Agent Assignment Guide:** [../docs/copilot-agent-assignment-guide.md](../docs/copilot-agent-assignment-guide.md)
- **API Testing Guide:** [../docs/api-testing-guide.md](../docs/api-testing-guide.md)
- **Bug Hunting Guide:** [../docs/bug-hunting-guide.md](../docs/bug-hunting-guide.md)

---

## Feedback

After completing the exercises, we'd love to hear:
- Which exercise was most valuable?
- Which Copilot prompts worked best?
- What would you add or change?
- How will you apply this in your work?

**Share your experience:** Open a GitHub Discussion in this repository!

---

## Next Steps

After completing all exercises:
1. ✅ Review the [Copilot Agent Assignment Guide](../docs/copilot-agent-assignment-guide.md)
2. ✅ Try assigning GitHub issues to Copilot (Issues #5, #10, #27)
3. ✅ Explore the [Roadmap](../docs/ROADMAP.md) for the full implementation plan
4. ✅ Build your own features with Copilot assistance

Happy testing! 🚀
