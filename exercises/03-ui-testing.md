# Exercise 3: UI Testing with Playwright and GitHub Copilot

**Duration:** 15-20 minutes  
**Difficulty:** Intermediate  
**Focus:** Using Copilot to generate end-to-end UI tests with Playwright across multiple browsers

---

## Learning Objectives

By the end of this exercise, you will:
- Write Playwright tests using Page Object Model pattern
- Use Copilot to generate UI interaction code
- Test across multiple browsers (Chrome, Firefox, Safari)
- Capture screenshots and videos on test failure
- Handle async UI interactions and waiting strategies

---

## Prerequisites

- ✅ Frontend app running (`npm run dev` in `client/`)
- ✅ Playwright installed (`npx playwright install`)
- ✅ Issue #19 (Playwright Setup) and Issue #20 (E2E Tests) completed
- ✅ Test data seeded in database

---

## Scenario

You need to write end-to-end tests for the Board Game Café customer workflows:
1. **Browse game catalog** - Filter by category, player count, view details
2. **Make a reservation** - Select table, choose date/time, confirm booking
3. **Order food and drinks** - Add items to cart, apply discount, checkout

---

## Part 1: Setup Page Object Models

### TODO 1.1: Create Game Catalog Page Object

**Your task**: Use Copilot to generate a Page Object Model for the game catalog.

**Copilot Prompt**:
```typescript
// Create Playwright Page Object Model for Game Catalog page
// URL: http://localhost:5173/games
// Elements:
//   - Game cards (data-testid="game-card")
//   - Category filter dropdown (data-testid="category-filter")
//   - Search input (data-testid="game-search")
//   - Game detail modal (data-testid="game-modal")
// Methods:
//   - goto()
//   - searchGame(name: string)
//   - filterByCategory(category: string)
//   - clickGameCard(title: string)
//   - getGameCards()
```

**Expected Output**:
```typescript
// client/tests/e2e/pages/GameCatalogPage.ts
import { Page, Locator, expect } from '@playwright/test';

export class GameCatalogPage {
  readonly page: Page;
  readonly gameCards: Locator;
  readonly categoryFilter: Locator;
  readonly searchInput: Locator;
  readonly gameModal: Locator;

  constructor(page: Page) {
    this.page = page;
    this.gameCards = page.locator('[data-testid="game-card"]');
    this.categoryFilter = page.locator('[data-testid="category-filter"]');
    this.searchInput = page.locator('[data-testid="game-search"]');
    this.gameModal = page.locator('[data-testid="game-modal"]');
  }

  async goto() {
    await this.page.goto('/games');
    await expect(this.gameCards.first()).toBeVisible();
  }

  async searchGame(name: string) {
    await this.searchInput.fill(name);
    await this.page.keyboard.press('Enter');
  }

  async filterByCategory(category: string) {
    await this.categoryFilter.selectOption(category);
  }

  async clickGameCard(title: string) {
    await this.page.locator(`[data-testid="game-card"]:has-text("${title}")`).click();
    await expect(this.gameModal).toBeVisible();
  }

  async getGameCards() {
    return await this.gameCards.all();
  }

  async getGameTitles() {
    const cards = await this.gameCards.all();
    return Promise.all(
      cards.map(card => card.locator('[data-testid="game-title"]').textContent())
    );
  }
}
```

**Verification**: File created with all methods

---

## Part 2: Write Game Browsing Tests

### TODO 2.1: Test Game Catalog Loads

**Your task**: Write a test to verify the game catalog displays games.

**Copilot Prompt**:
```typescript
// Playwright test: Game catalog page displays all games
// Arrange: Navigate to /games
// Assert: At least 5 game cards visible
// Assert: Each card has title, image, player count
```

**Expected Test**:
```typescript
// client/tests/e2e/specs/game-browsing.spec.ts
import { test, expect } from '@playwright/test';
import { GameCatalogPage } from '../pages/GameCatalogPage';

test.describe('Game Catalog', () => {
  test('displays game cards on load', async ({ page }) => {
    // Arrange
    const gameCatalog = new GameCatalogPage(page);
    
    // Act
    await gameCatalog.goto();
    
    // Assert
    const cards = await gameCatalog.getGameCards();
    expect(cards.length).toBeGreaterThanOrEqual(5);
    
    // Verify first card has required elements
    const firstCard = cards[0];
    await expect(firstCard.locator('[data-testid="game-title"]')).toBeVisible();
    await expect(firstCard.locator('[data-testid="game-image"]')).toBeVisible();
    await expect(firstCard.locator('[data-testid="player-count"]')).toBeVisible();
  });
});
```

**Run Test**:
```bash
cd client
npx playwright test game-browsing.spec.ts
```

**Expected**: ✅ Test passes in chromium, firefox, webkit

### TODO 2.2: Test Category Filter

**Your task**: Test filtering games by category.

**Copilot Prompt**:
```typescript
// Test: Filter games by "Strategy" category
// Act: Select "Strategy" from category dropdown
// Assert: All visible games have category "Strategy"
// Assert: Count matches expected (seed data has 2 strategy games)
```

**Expected**:
```typescript
test('filters games by category', async ({ page }) => {
  // Arrange
  const gameCatalog = new GameCatalogPage(page);
  await gameCatalog.goto();
  
  // Act
  await gameCatalog.filterByCategory('Strategy');
  
  // Assert
  const titles = await gameCatalog.getGameTitles();
  expect(titles).toContain('Catan');
  expect(titles).toContain('Wingspan');
  
  // Verify category badge on each card
  const cards = await gameCatalog.getGameCards();
  for (const card of cards) {
    const category = await card.locator('[data-testid="game-category"]').textContent();
    expect(category).toBe('Strategy');
  }
});
```

### TODO 2.3: Test Search Functionality

**Your task**: Use Copilot to test the game search feature.

**Copilot Prompt**:
```typescript
// Test: Search for game by name "Catan"
// Act: Type "Catan" in search input, press Enter
// Assert: Only matching games visible
// Assert: "Catan" card is visible, others filtered out
```

**Practice**: Generate tests for:
- Partial search: "Cat" matches "Catan"
- Case-insensitive: "catan" matches "Catan"
- No results: "NonExistentGame" shows empty state

---

## Part 3: Test Game Detail Modal

### TODO 3.1: Test Opening Game Details

**Your task**: Test clicking a game card opens the detail modal.

**Copilot Prompt**:
```typescript
// Test: Click game card opens detail modal
// Act: Click "Catan" game card
// Assert: Modal visible with game details
// Assert: Modal shows title, description, player count, rental price
// Assert: "Reserve" button present
```

**Expected**:
```typescript
test('opens game detail modal on card click', async ({ page }) => {
  // Arrange
  const gameCatalog = new GameCatalogPage(page);
  await gameCatalog.goto();
  
  // Act
  await gameCatalog.clickGameCard('Catan');
  
  // Assert
  await expect(page.locator('[data-testid="game-modal"]')).toBeVisible();
  await expect(page.locator('[data-testid="modal-title"]')).toHaveText('Catan');
  await expect(page.locator('[data-testid="modal-description"]')).toBeVisible();
  await expect(page.locator('[data-testid="modal-player-count"]')).toContainText('3-4');
  await expect(page.locator('[data-testid="modal-rental-price"]')).toContainText('$5');
  await expect(page.locator('[data-testid="reserve-button"]')).toBeEnabled();
});
```

### TODO 3.2: Test Closing Modal

**Your task**: Test modal can be closed.

**Copilot Prompt**:
```typescript
// Test: Close modal with X button or ESC key
// Scenarios:
//   1. Click close button → modal hidden
//   2. Press Escape key → modal hidden
//   3. Click outside modal (backdrop) → modal hidden
```

---

## Part 4: Test Reservation Flow

### TODO 4.1: Create Reservation Page Object

**Your task**: Use Copilot to generate Page Object for reservations.

**Copilot Prompt**:
```typescript
// Create Playwright Page Object for Reservation page
// URL: http://localhost:5173/reservations/new
// Elements:
//   - Date picker (data-testid="reservation-date")
//   - Time select (data-testid="reservation-time")
//   - Party size input (data-testid="party-size")
//   - Table grid (data-testid="table-grid")
//   - Confirm button (data-testid="confirm-reservation")
// Methods:
//   - goto()
//   - selectDate(date: Date)
//   - selectTime(time: string)
//   - setPartySize(size: number)
//   - selectTable(tableNumber: number)
//   - confirmReservation()
```

**Expected**: Similar structure to GameCatalogPage

### TODO 4.2: Test Complete Reservation Flow

**Your task**: Write end-to-end test for making a reservation.

**Copilot Prompt**:
```typescript
// E2E test: Complete reservation workflow
// Steps:
//   1. Navigate to /reservations/new
//   2. Select date (tomorrow)
//   3. Select time (18:00)
//   4. Set party size (4)
//   5. Click available table
//   6. Click confirm
//   7. Assert: Success message shown
//   8. Assert: Redirected to /reservations
//   9. Assert: New reservation in list
```

**Expected**:
```typescript
test('completes full reservation flow', async ({ page }) => {
  // Arrange
  const reservation = new ReservationPage(page);
  await reservation.goto();
  
  // Act
  const tomorrow = new Date();
  tomorrow.setDate(tomorrow.getDate() + 1);
  
  await reservation.selectDate(tomorrow);
  await reservation.selectTime('18:00');
  await reservation.setPartySize(4);
  
  // Wait for tables to load
  await page.waitForSelector('[data-testid="table-card"]:not(.unavailable)');
  
  await reservation.selectTable(1);
  await reservation.confirmReservation();
  
  // Assert
  await expect(page.locator('[data-testid="success-message"]')).toContainText('Reservation confirmed');
  await expect(page).toHaveURL(/\/reservations$/);
  
  // Verify reservation appears in list
  const reservationList = await page.locator('[data-testid="reservation-item"]').all();
  expect(reservationList.length).toBeGreaterThan(0);
});
```

---

## Part 5: Test Error Scenarios

### TODO 5.1: Test Validation Errors

**Your task**: Test form validation.

**Copilot Prompt**:
```typescript
// Test: Reservation form validation
// Scenarios:
//   - Submit without date → error "Date required"
//   - Select past date → error "Cannot book past dates"
//   - Party size 0 → error "Party size must be at least 1"
//   - Party size > 8 → error "Maximum party size is 8"
```

**Expected Pattern**:
```typescript
test('shows error for past date selection', async ({ page }) => {
  const reservation = new ReservationPage(page);
  await reservation.goto();
  
  // Act: Select yesterday
  const yesterday = new Date();
  yesterday.setDate(yesterday.getDate() - 1);
  await reservation.selectDate(yesterday);
  
  await reservation.selectTime('18:00');
  await page.locator('[data-testid="confirm-reservation"]').click();
  
  // Assert
  await expect(page.locator('[data-testid="error-message"]'))
    .toContainText('Cannot book past dates');
});
```

### TODO 5.2: Test No Available Tables

**Your task**: Test behavior when no tables available.

**Copilot Prompt**:
```typescript
// Test: No tables available for selected time
// Setup: All tables reserved for selected date/time
// Assert: Empty state message shown
// Assert: "Try different time" suggestion displayed
```

---

## Part 6: Cross-Browser Testing

### TODO 6.1: Configure Browser Matrix

**Your task**: Ensure tests run on all browsers.

**Verify `playwright.config.ts`**:
```typescript
projects: [
  { name: 'chromium', use: { ...devices['Desktop Chrome'] } },
  { name: 'firefox', use: { ...devices['Desktop Firefox'] } },
  { name: 'webkit', use: { ...devices['Desktop Safari'] } },
]
```

**Run tests on all browsers**:
```bash
npx playwright test --project=chromium
npx playwright test --project=firefox
npx playwright test --project=webkit
```

### TODO 6.2: Test Mobile Viewport

**Your task**: Add mobile browser tests.

**Copilot Prompt**:
```typescript
// Add mobile browser configuration to playwright.config.ts
// Projects: 'Mobile Chrome', 'Mobile Safari'
// Use iPhone 12 and Pixel 5 viewports
```

**Run mobile tests**:
```bash
npx playwright test --project="Mobile Chrome"
```

---

## Part 7: Visual Testing

### TODO 7.1: Screenshot Comparison

**Your task**: Use Copilot to add visual regression tests.

**Copilot Prompt**:
```typescript
// Test: Game catalog visual regression
// Take screenshot of game catalog page
// Compare with baseline using toHaveScreenshot()
```

**Expected**:
```typescript
test('game catalog visual regression', async ({ page }) => {
  const gameCatalog = new GameCatalogPage(page);
  await gameCatalog.goto();
  
  // Wait for images to load
  await page.waitForLoadState('networkidle');
  
  // Compare with baseline
  await expect(page).toHaveScreenshot('game-catalog.png', {
    fullPage: true,
    maxDiffPixels: 100
  });
});
```

**Generate baseline**:
```bash
npx playwright test --update-snapshots
```

---

## Reflection Questions

1. **Page Objects**: How did the Page Object Model improve test maintainability?

2. **Async Handling**: What waiting strategies did you use? (`waitForSelector`, `toBeVisible`, `networkidle`)

3. **Browser Differences**: Did you notice any behavior differences between Chrome, Firefox, and Safari?

4. **Flakiness**: Did any tests fail intermittently? How did you make them more stable?

5. **Copilot Assistance**: Did Copilot generate correct Playwright selectors and async/await patterns?

---

## Success Criteria

- [ ] Page Object Models created for all pages
- [ ] E2E test for complete reservation flow passes
- [ ] Tests pass on all 3 browsers (Chrome, Firefox, Safari)
- [ ] Screenshots captured on failure
- [ ] Tests use `data-testid` for stable selectors
- [ ] No hardcoded waits (`page.waitForTimeout`)

---

## Bonus Challenges

### Challenge 1: Test Order Flow
```typescript
// E2E test: Order food and drinks workflow
// Steps:
//   1. Browse menu
//   2. Add items to cart
//   3. Apply loyalty discount
//   4. Checkout
//   5. Verify order confirmation
```

### Challenge 2: Accessibility Testing
```typescript
// Use @axe-core/playwright for a11y testing
import { injectAxe, checkA11y } from 'axe-playwright';

test('game catalog is accessible', async ({ page }) => {
  await page.goto('/games');
  await injectAxe(page);
  await checkA11y(page);
});
```

### Challenge 3: Performance Testing
```typescript
// Test: Page load performance
// Assert: First Contentful Paint < 1.5s
// Assert: Largest Contentful Paint < 2.5s
```

### Challenge 4: Network Mocking
```typescript
// Mock API responses for offline testing
await page.route('**/api/v1/games', route => {
  route.fulfill({
    status: 200,
    body: JSON.stringify({ games: mockGames })
  });
});
```

---

## Next Steps

- Exercise 4: Bug Hunting and Regression Tests

---

**Instructor Notes**:
- Demo Playwright UI mode: `npx playwright test --ui`
- Show trace viewer: `npx playwright show-trace`
- Explain data-testid best practices
- Discuss flaky test prevention strategies
- Review waiting strategies (auto-wait, explicit waits)
- Show screenshot comparison workflow
