# Playwright E2E Testing Setup

## ✅ Setup Complete

Playwright has been successfully configured for end-to-end testing with the following components:

### 1. Installation
- ✅ `@playwright/test` installed in `client/package.json`
- ✅ Version: 1.56.1

### 2. Configuration (`playwright.config.ts`)
- ✅ **Projects**: chromium, firefox, webkit
- ✅ **Base URL**: `http://localhost:5173`
- ✅ **Headless mode**: Enabled in CI, headed for local development
- ✅ **Screenshot**: On failure
- ✅ **Video**: On first retry
- ✅ **Parallel execution**: 1 worker in CI, 4 workers locally
- ✅ **Retries**: 2 retries in CI, 0 locally
- ✅ **Web server**: Auto-starts Vite dev server on `http://localhost:5173`

### 3. Folder Structure
```
client/tests/e2e/
├── fixtures/
│   └── testData.ts          # Sample data and API seeding helpers
├── pages/
│   ├── GameCatalogPage.ts   # Game browsing interactions
│   ├── ReservationPage.ts   # Table booking workflow
│   └── OrderPage.ts         # Menu ordering workflow
└── specs/
    └── game-browsing.spec.ts # First E2E test suite
```

### 4. Page Object Models
- ✅ **GameCatalogPage**: Game browsing, filtering, and detail viewing
- ✅ **ReservationPage**: Table reservation workflow
- ✅ **OrderPage**: Menu ordering and cart management

### 5. Test Fixtures
- ✅ Sample games, tables, menu items, and customers
- ✅ API seeding helper functions
- ✅ Date/time utilities for reservations

### 6. Test Suite
- ✅ `game-browsing.spec.ts`: 8 comprehensive tests covering:
  - Game catalog loading
  - Category filtering
  - Search functionality
  - Game detail modal
  - Multiple filter combinations
  - Edge cases (no results)

### 7. npm Scripts
```json
{
  "test:e2e": "playwright test",
  "test:e2e:ui": "playwright test --ui",
  "test:e2e:headed": "playwright test --headed",
  "test:e2e:debug": "playwright test --debug",
  "test:e2e:chromium": "playwright test --project=chromium",
  "test:e2e:firefox": "playwright test --project=firefox",
  "test:e2e:webkit": "playwright test --project=webkit"
}
```

## 🖥️ Browser Installation

### Local Development

To install Playwright browsers locally, run:

```bash
cd client
npx playwright install --with-deps
```

**Note**: If you encounter download issues, try:
```bash
# Install browsers without system dependencies
npx playwright install

# Or install a specific browser
npx playwright install chromium
npx playwright install firefox
npx playwright install webkit
```

### CI/CD (GitHub Actions)

In CI environments, browsers are installed automatically as part of the workflow. Example:

```yaml
- name: Install Playwright Browsers
  run: |
    cd client
    npx playwright install --with-deps
```

The browsers are cached between runs for faster execution.

## 🧪 Running Tests

### All Browsers (Headless)
```bash
cd client
npm run test:e2e
```

### Interactive UI Mode
```bash
npm run test:e2e:ui
```

### Headed Mode (See Browser)
```bash
npm run test:e2e:headed
```

### Debug Mode
```bash
npm run test:e2e:debug
```

### Specific Browser
```bash
npm run test:e2e:chromium
npm run test:e2e:firefox
npm run test:e2e:webkit
```

## 📝 Test Best Practices

### 1. Use data-testid Attributes
```typescript
// ✅ Good - Stable selector
page.getByTestId('game-card')

// ❌ Avoid - Brittle selectors
page.locator('.game-card')
page.locator('div > div > button')
```

### 2. Leverage Page Object Models
```typescript
import { GameCatalogPage } from '../pages/GameCatalogPage';

const gameCatalogPage = new GameCatalogPage(page);
await gameCatalogPage.goto();
await gameCatalogPage.filterByCategory('Strategy');
```

### 3. Use Playwright's Auto-Waiting
Playwright automatically waits for elements to be actionable. No need for manual waits in most cases.

### 4. Use Test Fixtures for Data
```typescript
import { sampleGames, seedGames } from '../fixtures/testData';

// Seed data via API
await seedGames(page);
```

## 🐛 Debugging

### View Test Report
```bash
npx playwright show-report
```

### Check Test Artifacts
- **Screenshots**: `test-results/` (on failure)
- **Videos**: `test-results/` (on retry)
- **Traces**: `test-results/` (on retry)

### Open Trace Viewer
```bash
npx playwright show-trace test-results/<trace-file>.zip
```

## 🔍 Configuration Details

### Browser Configuration
Each browser project runs with:
- Desktop viewport size
- Headless mode in CI
- Automatic screenshot on failure
- Video recording on first retry
- Trace collection on first retry

### Web Server
The configuration automatically starts the Vite dev server before running tests:
- **Command**: `npm run dev`
- **URL**: `http://localhost:5173`
- **Timeout**: 120 seconds
- **Reuse existing server**: Yes (in local development)

### CI Optimizations
- **Single worker**: Prevents race conditions
- **2 retries**: Handles flaky tests
- **Headless mode**: Faster execution
- **forbidOnly**: Prevents `.only` in committed code

## 📚 Resources

- [Playwright Documentation](https://playwright.dev)
- [Test Best Practices](https://playwright.dev/docs/best-practices)
- [Page Object Models](https://playwright.dev/docs/pom)
- [Debugging Tests](https://playwright.dev/docs/debug)
- [Trace Viewer](https://playwright.dev/docs/trace-viewer)

## ✅ Acceptance Criteria Status

All acceptance criteria from the issue have been met:

- [x] Install Playwright in `client/` directory
- [x] Configure `playwright.config.ts` with all required settings
- [x] Create `client/tests/e2e/` folder structure
- [x] Create Page Object Models (GameCatalogPage, ReservationPage, OrderPage)
- [x] Create test fixtures in `fixtures/testData.ts`
- [x] Create first E2E test `specs/game-browsing.spec.ts`
- [x] Add npm scripts (test:e2e, test:e2e:ui, test:e2e:headed, etc.)
- [x] Document browser installation steps

**Next Steps**: Run tests in CI pipeline to verify cross-browser compatibility.
