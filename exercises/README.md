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
| [01-unit-test-generation.md](./01-unit-test-generation.md) | 10 min | Backend business logic | xUnit, FluentAssertions, test data builders |
| [02-api-endpoint-creation.md](./02-api-endpoint-creation.md) | 12 min | REST API endpoint creation | DTOs, validation, integration tests |
| [03-playwright-test-writing.md](./03-playwright-test-writing.md) | 7 min | E2E browser automation | Playwright, Page Object Model, cross-browser testing |
| [04-test-data-builder.md](./04-test-data-builder.md) | 10 min | Test data patterns | Builder pattern, fluent APIs, reusable test data |
| [05-bug-fix-with-test.md](./05-bug-fix-with-test.md) | 10 min | Debugging & regression tests | Bug reproduction, regression suites, edge case testing |

**Total Duration:** ~49 minutes of core exercises + 11 minutes for Q&A and exploration

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
   - Navigate to: http://localhost:5000/swagger
   - Verify API endpoints are documented

---

## Quick Start

### For Workshop Instructors

**Live Demo Flow (60 minutes):**

| Time | Activity | Exercise |
|------|----------|----------|
| 0-5 min | Welcome & Setup | Verify environments running |
| 5-15 min | Unit Testing Demo | Exercise 1: Steps 1-5 |
| 15-27 min | API Endpoint Creation | Exercise 2: Steps 1-6 |
| 27-34 min | UI Testing Demo | Exercise 3: Steps 1-4 |
| 34-44 min | Test Data Builder | Exercise 4: Steps 1-5 |
| 44-54 min | Bug Hunting Demo | Exercise 5: Fix 2 bugs |
| 54-60 min | Q&A & Wrap-up | Open discussion |

**Instructor Tips:**
- Start each exercise with a quick overview (1 min)
- Demonstrate 2-3 key prompts live
- Let Copilot generate code on screen
- Highlight when Copilot suggestions are helpful vs. need refinement
- Keep exercises moving - don't get stuck on one test

### For Self-Paced Learning

**Recommended Path:**
1. Complete exercises in order (1 → 2 → 3 → 4 → 5)
2. Spend full time on each exercise
3. Try all Copilot prompts
4. Run all tests to verify your work
5. Experiment with your own variations

---

## Exercise Summaries

### Exercise 1: Unit Testing (10 min)
**File:** [01-unit-test-generation.md](./01-unit-test-generation.md)

Generate comprehensive unit tests for `OrderCalculationService`:
- Tax calculation tests (8% food, 10% alcohol)
- Member discount tests (Bronze/Silver/Gold tiers)
- Loyalty points redemption tests
- Edge case handling (zeros, nulls, boundaries)

**Key Takeaway:** Use Copilot to quickly generate test suites with `[Theory]` and `[InlineData]` for parameterized testing.

---

### Exercise 2: API Endpoint Creation (12 min)
**File:** [02-api-endpoint-creation.md](./02-api-endpoint-creation.md)

Create a new REST API endpoint for game recommendations:
- Design request/response DTOs
- Implement validation rules  
- Add filtering and scoring logic
- Create integration tests
- Test with Swagger UI

**Key Takeaway:** Use Copilot to scaffold complete API endpoints with DTOs, validation, and tests in minutes.

---

### Exercise 3: UI Testing (7 min)
**File:** [03-playwright-test-writing.md](./03-playwright-test-writing.md)

Build E2E tests with Playwright:
- Create Page Object Models for game catalog, reservations
- Test user workflows (browse games, make reservation)
- Run tests across 3 browsers (Chrome, Firefox, Safari)
- Add stable test selectors with data-testid

**Key Takeaway:** Use Copilot to generate Page Object Models that encapsulate page interactions for reusable, maintainable tests.

---

### Exercise 4: Test Data Builder (10 min)
**File:** [04-test-data-builder.md](./04-test-data-builder.md)

Create fluent test data builders:
- Study existing ReservationBuilder pattern
- Create OrderBuilder with fluent API
- Add convenience methods (AsCompletedOrder(), WithSmallTotal())
- Refactor existing tests to use builders
- Reduce test data duplication

**Key Takeaway:** Use Copilot to generate builder patterns that make test setup clean, expressive, and maintainable.

---

### Exercise 5: Bug Hunting (10 min)
**File:** [05-bug-fix-with-test.md](./05-bug-fix-with-test.md)

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
