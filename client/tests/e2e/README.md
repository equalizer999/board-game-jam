# E2E Testing with Playwright

This directory contains end-to-end tests for the Board Game Café application using Playwright.

## 📁 Structure

```
tests/e2e/
├── fixtures/       # Test data and helper functions
├── pages/          # Page Object Models (POM)
└── specs/          # Test specifications
```

## 🎯 Page Object Models

### GameCatalogPage
Handles interactions with the game catalog:
- Browse available games
- Filter by category, complexity, player count
- Search games
- View game details
- Navigate game detail modals

### ReservationPage
Manages table reservation workflow:
- Select date and time
- Choose party size and duration
- View available tables
- Fill customer information
- Submit reservations
- Verify booking confirmation

### OrderPage
Handles menu ordering:
- Browse menu by category
- Add/remove items from cart
- Update quantities
- Apply dietary filters (vegetarian, vegan, gluten-free)
- Calculate pricing with tax
- Complete checkout

## 🧪 Running Tests

```bash
# All tests on all browsers
npm run test:e2e

# Interactive UI mode
npm run test:e2e:ui

# Headed mode (see browser)
npm run test:e2e:headed

# Debug mode
npm run test:e2e:debug

# Specific browser
npm run test:e2e:chromium
npm run test:e2e:firefox
npm run test:e2e:webkit
```

## 📝 Writing Tests

### Best Practices

1. **Use data-testid attributes** for selectors:
   ```typescript
   page.getByTestId('game-card')
   ```

2. **Leverage Page Object Models**:
   ```typescript
   const gameCatalogPage = new GameCatalogPage(page);
   await gameCatalogPage.goto();
   await gameCatalogPage.filterByCategory('Strategy');
   ```

3. **Use Playwright's auto-waiting**:
   - No need for manual waits in most cases
   - Playwright waits for elements to be actionable

4. **Organize tests with describe blocks**:
   ```typescript
   test.describe('Game Browsing', () => {
     test('should load games', async ({ page }) => {
       // test code
     });
   });
   ```

5. **Use fixtures for test data**:
   ```typescript
   import { sampleGames, seedGames } from '../fixtures/testData';
   ```

## 🔧 Configuration

Tests are configured via `playwright.config.ts`:
- **Base URL**: http://localhost:5173
- **Workers**: 1 in CI, 4 locally
- **Retries**: 2 in CI, 0 locally
- **Screenshots**: On failure
- **Videos**: On retry
- **Browsers**: Chromium, Firefox, WebKit

## 🐛 Debugging

1. **Use UI mode** for visual debugging:
   ```bash
   npm run test:e2e:ui
   ```

2. **Use debug mode** for step-by-step execution:
   ```bash
   npm run test:e2e:debug
   ```

3. **Check test artifacts**:
   - Screenshots: `test-results/`
   - Videos: `test-results/`
   - Traces: `test-results/`

4. **View HTML report**:
   ```bash
   npx playwright show-report
   ```

## 📦 Test Data

The `fixtures/testData.ts` file provides:
- Sample games, tables, menu items, and customers
- Helper functions for API seeding
- Utilities for date/time manipulation

Example usage:
```typescript
import { sampleGames, seedGames, getFutureDate } from '../fixtures/testData';

// In your test
await seedGames(page);
const reservationDate = getFutureDate(3); // 3 days from now
```

## 🚀 CI/CD Integration

Tests run automatically in CI with:
- Headless mode enabled
- Single worker for stability
- 2 retries for flaky tests
- Automatic dev server startup
- Screenshot and video capture on failure

## 📚 Resources

- [Playwright Documentation](https://playwright.dev)
- [Best Practices](https://playwright.dev/docs/best-practices)
- [Page Object Models](https://playwright.dev/docs/pom)
- [Debugging Tests](https://playwright.dev/docs/debug)
