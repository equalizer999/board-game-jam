# Exercise 3: UI Testing with Playwright and GitHub Copilot

## Overview
This exercise demonstrates how to use GitHub Copilot to generate end-to-end UI tests with Playwright for cross-browser testing.

**Duration:** 7 minutes  
**Focus:** E2E testing, Page Object Model, visual testing across browsers

---

## Learning Objectives
- Generate Playwright tests with Copilot
- Use Page Object Model pattern
- Test user workflows (browse, reserve, order)
- Run tests across Chrome, Firefox, Safari

---

## Prerequisites

```bash
# Ensure Playwright is installed
cd client
npm install
npx playwright install

# Start the backend API
cd ../src/BoardGameCafe.Api
dotnet run

# In new terminal, start frontend
cd client
npm run dev
```

---

## Exercise Steps

### Step 1: Create Page Object for Game Catalog

**Copilot Prompt:**
```typescript
// Create Page Object Model for Game Catalog page
// Path: client/tests/e2e/pages/GameCatalogPage.ts
// Methods: goto(), getGameCards(), filterByCategory(), clickGame()
// Use data-testid selectors for stability
```

---

### Step 2: Generate Test for Game Browsing

**Copilot Prompt:**
```typescript
// Create E2E test for game browsing workflow
// Path: client/tests/e2e/specs/game-browsing.spec.ts
// Test: navigate to catalog, verify games load, apply filter
// Use Page Object Model pattern
```

---

### Step 3: Test Reservation Workflow

**Copilot Prompt:**
```typescript
// Generate E2E test for complete reservation workflow
// Steps: select date/time, choose party size, select table, confirm
// Assert confirmation message appears
```

---

### Step 4: Cross-Browser Testing

```bash
# Run tests in all browsers
npx playwright test

# Run in specific browser
npx playwright test --project=chromium
npx playwright test --project=firefox
npx playwright test --project=webkit
```

---

## Success Criteria

You've completed this exercise when:
1. ✅ All user workflows have E2E tests
2. ✅ Tests pass in all 3 browsers
3. ✅ Page Object Models are reusable
4. ✅ Tests use stable selectors (data-testid)

---

## Next Steps

Continue to [Exercise 4: Test Data Builder](./04-test-data-builder.md) to learn how to create fluent test data builders.
