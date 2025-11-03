# E2E Test Prompt Templates (Playwright)

Use these templates when generating end-to-end tests with Playwright.

## Page Object Model Template

```
Create Playwright Page Object Model for {PageName}:
- File: client/tests/e2e/pages/{PageName}Page.ts
- URL: {page url}
- Selectors using data-testid: {list elements}
- Methods for user actions: {list actions}
- Properties for page elements: {list elements}
```

**Example:**
```
Create Playwright Page Object Model for ReservationPage:
- File: client/tests/e2e/pages/ReservationPage.ts
- URL: /reservations/new
- Selectors using data-testid: date-picker, party-size-select, find-tables-btn, table-card, customer-name-input, customer-email-input, confirm-btn, success-message
- Methods: selectDate(date), selectPartySize(size), clickFindTables(), selectTable(index), fillCustomerInfo(name, email), confirmReservation()
- Properties: datePicker, partySizeSelect, availableTables, successMessage
```

## User Journey Test

```
Create Playwright E2E test for {journey description}:
- Test file: client/tests/e2e/specs/{feature}.spec.ts
- Steps:
  1. {step 1}
  2. {step 2}
  3. {step 3}
- Assertions at each step
- Use Page Object: {PageName}Page
- Test on: chromium, firefox, webkit
```

**Example:**
```
Create Playwright E2E test for complete reservation booking flow:
- Test file: client/tests/e2e/specs/reservation-booking.spec.ts
- Steps:
  1. Navigate to /reservations/new
  2. Select tomorrow's date from date picker
  3. Select party size of 4
  4. Click "Find Available Tables"
  5. Wait for available tables to load
  6. Select first available table
  7. Fill customer name "John Doe" and email "john@example.com"
  8. Click "Confirm Reservation" button
  9. Wait for success message
- Assertions: date picker visible, tables load (count > 0), success message contains confirmation number
- Use Page Object: ReservationPage
- Test on: chromium, firefox, webkit
```

## Form Interaction Test

```
Create Playwright test for form submission on {page}:
- Navigate to: {url}
- Fill fields: {list fields with test data}
- Submit form
- Verify: {expected outcome}
- Check: error handling for invalid inputs
```

**Example:**
```
Create Playwright test for game checkout form:
- Navigate to: /games/{id}/checkout
- Fill fields:
  - Reservation ID: {valid reservation ID from fixture}
  - Customer name: "Jane Smith"
  - Notes: "First time playing Catan"
- Submit form by clicking "Confirm Checkout"
- Verify: success message appears, game availability decrements by 1, redirects to /my-games
- Check error handling:
  - Invalid reservation ID → error message "Reservation not found"
  - Empty customer name → error "Name is required"
```

## Navigation Test

```
Create Playwright test for navigation flow:
- Start at: {starting page}
- Navigate to: {page 2} by {action}
- Navigate to: {page 3} by {action}
- Verify: URL changes, breadcrumbs update, page content loads
```

**Example:**
```
Create Playwright test for game browsing to checkout flow:
- Start at: /games (game catalog)
- Navigate to: /games/catan-id (game detail) by clicking "Catan" card
- Navigate to: /games/catan-id/checkout by clicking "Checkout Game" button
- Verify: 
  - URL is /games/catan-id/checkout
  - Breadcrumbs show "Games > Catan > Checkout"
  - Checkout form is visible with pre-filled game title
```

## Search and Filter Test

```
Create Playwright test for search and filtering on {page}:
- Navigate to: {url}
- Test search: enter "{query}", verify results
- Test filter: select "{filter}", verify results update
- Test combined: search + filter, verify results match both
- Test no results: search for "{nonexistent}", verify empty state
```

**Example:**
```
Create Playwright test for game catalog filtering:
- Navigate to: /games
- Test search: enter "Catan", verify only games with "Catan" in title appear
- Test category filter: select "Strategy", verify all displayed games have category badge "Strategy"
- Test player count filter: select "2-4 players", verify games shown have MinPlayers ≤ 2 and MaxPlayers ≥ 4
- Test combined: search "ticket" + category "Family", verify results match both criteria
- Test no results: search for "XYZ123", verify "No games found" message and suggestion to clear filters
```

## CRUD Workflow Test

```
Create Playwright test for full CRUD workflow on {resource}:
- Create: {describe creation}
- Read: verify created item appears in list
- Update: {describe update}
- Verify: updated values appear
- Delete: {describe deletion}
- Verify: item no longer in list
```

**Example:**
```
Create Playwright test for full reservation CRUD workflow:
- Create: navigate to /reservations/new, fill form with tomorrow 7pm for 4 people, submit
- Read: navigate to /my-reservations, verify new reservation appears with correct date/time
- Update: click "Edit" on reservation, change party size to 6, save
- Verify: reservation list shows party size 6, confirmation email sent
- Delete: click "Cancel Reservation", confirm cancellation in modal
- Verify: reservation no longer appears in active list, appears in "Past/Cancelled" tab
```

## Cross-Browser Test

```
Create Playwright test that runs on multiple browsers:
- Test: {describe test}
- Configure: chromium, firefox, webkit in playwright.config.ts
- Verify: consistent behavior across all browsers
- Screenshot on failure: for debugging browser-specific issues
```

**Example:**
```
Create Playwright test for reservation booking on all browsers:
- Test: complete reservation booking flow from date selection to confirmation
- Configure: projects for chromium, firefox, webkit in playwright.config.ts
- Verify: 
  - Date picker works consistently (different implementations in browsers)
  - Table selection highlights correctly
  - Success message displays with same styling
  - API calls complete successfully
- Screenshot on failure: capture state at failure point to identify browser-specific issues
```

## Mobile Responsive Test

```
Create Playwright test for mobile responsiveness:
- Set viewport: {mobile size}
- Navigate to: {page}
- Verify: {mobile-specific layout}
- Test interactions: {mobile gestures}
- Compare with desktop: {differences}
```

**Example:**
```
Create Playwright test for game catalog mobile view:
- Set viewport: 375x667 (iPhone SE size)
- Navigate to: /games
- Verify:
  - Game cards stack vertically (1 column, not grid)
  - Filter menu is collapsed, opens from hamburger icon
  - Search bar is full-width
  - Navigation is bottom tab bar instead of top nav
- Test interactions: swipe to scroll, tap to open filters, tap outside to close
- Compare with desktop: desktop shows 3-column grid, top navigation, sidebar filters
```

## Authentication Flow Test

```
Create Playwright test for authentication:
- Start: logged out state
- Navigate to: {protected page}
- Verify: redirected to login
- Fill login form: {credentials}
- Submit
- Verify: redirected back to original page, user menu shows name
- Test logout: click logout, verify redirected to home
```

**Example:**
```
Create Playwright test for reservation authentication flow:
- Start: clear cookies (logged out)
- Navigate to: /my-reservations
- Verify: redirected to /login with returnUrl=/my-reservations
- Fill login form: email "test@example.com", password "Test123!"
- Click "Log In" button
- Verify: 
  - Redirected to /my-reservations
  - User menu shows "Test User"
  - Reservations load successfully
- Test logout: click user menu → "Log Out", verify redirected to /home, cookie cleared
```

## Error Handling Test

```
Create Playwright test for error scenarios:
- Scenario 1: {error condition} → verify error message: {message}
- Scenario 2: {error condition} → verify error message: {message}
- Test: error recovery (close error, retry action)
- Verify: user can continue after error
```

**Example:**
```
Create Playwright test for reservation booking errors:
- Scenario 1: Network error during submission → verify error toast "Unable to connect. Please try again."
- Scenario 2: Table becomes unavailable during booking → verify error "This table was just booked. Please select another."
- Scenario 3: Past date selected → verify inline error "Reservation date must be in the future"
- Test error recovery: close error toast, select new table, resubmit
- Verify: user can successfully book different table after error
```

## API Mocking Test

```
Create Playwright test with API mocking:
- Mock endpoint: {method} {url}
- Return: {mock response}
- Verify: UI renders mocked data correctly
- Test: error response (500, 404)
- Verify: UI shows appropriate error
```

**Example:**
```
Create Playwright test with mocked games API:
- Mock endpoint: GET /api/v1/games
- Return: array of 3 games (Catan, Pandemic, Azul)
- Verify: game catalog shows exactly 3 cards with correct titles and images
- Test error response: return 500 Internal Server Error
- Verify: UI shows "Unable to load games" error with retry button
- Click retry: remock successful response, verify games load
```

## Performance Test

```
Create Playwright performance test for {page}:
- Navigate to: {url}
- Measure: page load time
- Verify: loaded in under {threshold} ms
- Check: no console errors
- Verify: all images loaded
```

**Example:**
```
Create Playwright performance test for game catalog:
- Navigate to: /games with 50 games in database
- Measure: time from navigation to all game cards visible
- Verify: page fully loaded in under 2000ms
- Check: browser console has no errors or warnings
- Verify: all 50 game thumbnail images loaded (no broken images)
- Check: smooth scrolling (no layout shifts)
```

## Accessibility Test

```
Create Playwright accessibility test for {page}:
- Use @axe-core/playwright
- Scan page for violations
- Verify: no critical WCAG issues
- Test keyboard navigation: {describe navigation}
- Test screen reader labels: {elements to check}
```

**Example:**
```
Create Playwright accessibility test for reservation form:
- Use @axe-core/playwright
- Scan /reservations/new page
- Verify: no critical WCAG AA violations (color contrast, labels, heading hierarchy)
- Test keyboard navigation:
  - Tab through all form fields in logical order
  - Arrow keys navigate date picker
  - Enter key submits form
  - Escape key closes modals
- Test screen reader labels:
  - Date picker has aria-label "Select reservation date"
  - Party size has label "Number of guests"
  - Each table card has descriptive name "Table 5, window seat, capacity 4"
```

---

## Page Object Model Patterns

### Basic Page Class
```typescript
import { Page, Locator } from '@playwright/test';

export class GameCatalogPage {
  readonly page: Page;
  readonly gameCards: Locator;
  readonly searchInput: Locator;
  readonly filterCategory: Locator;

  constructor(page: Page) {
    this.page = page;
    this.gameCards = page.getByTestId('game-card');
    this.searchInput = page.getByTestId('search-input');
    this.filterCategory = page.getByTestId('filter-category');
  }

  async goto() {
    await this.page.goto('/games');
  }

  async searchGames(query: string) {
    await this.searchInput.fill(query);
    await this.searchInput.press('Enter');
  }

  async selectCategory(category: string) {
    await this.filterCategory.selectOption(category);
  }
}
```

### Page with Complex Interactions
```typescript
export class ReservationPage {
  readonly page: Page;
  
  constructor(page: Page) {
    this.page = page;
  }

  async selectDateFromCalendar(date: Date) {
    await this.page.getByTestId('date-picker').click();
    await this.page.getByRole('button', { 
      name: date.toLocaleDateString() 
    }).click();
  }

  async waitForAvailableTables() {
    await this.page.getByTestId('table-card').first().waitFor();
  }

  async selectTable(index: number) {
    await this.page.getByTestId('table-card').nth(index).click();
  }
}
```

---

## Test Fixtures Template

```typescript
// client/tests/e2e/fixtures/games.ts
export const gameFixtures = [
  {
    id: '1',
    title: 'Catan',
    category: 'Strategy',
    minPlayers: 3,
    maxPlayers: 4,
    copiesOwned: 5,
    copiesInUse: 2
  },
  // ... more games
];

// Seed via API
export async function seedGames(page: Page) {
  await page.request.post('/api/v1/games', {
    data: gameFixtures
  });
}
```

---

## Usage Tips

1. **Use data-testid** for stable selectors that won't break with CSS changes
2. **Wait for network idle** before asserting to avoid flaky tests
3. **Take screenshots** on failure for debugging
4. **Test real user flows** not just happy paths
5. **Run in parallel** to speed up test suite

## Common Assertions

```typescript
// Visibility
await expect(page.getByTestId('element')).toBeVisible();
await expect(page.getByTestId('element')).toBeHidden();

// Text content
await expect(page.getByTestId('title')).toHaveText('Expected Text');
await expect(page.getByTestId('message')).toContainText('partial');

// Count
await expect(page.getByTestId('game-card')).toHaveCount(10);

// URL
await expect(page).toHaveURL(/.*reservations/);

// Attributes
await expect(page.getByTestId('link')).toHaveAttribute('href', '/games');

// State
await expect(page.getByRole('button')).toBeEnabled();
await expect(page.getByRole('checkbox')).toBeChecked();
```
