# E2E Testing Workflow Documentation

## Overview

The E2E (End-to-End) testing workflow runs Playwright tests across multiple browsers (Chromium, Firefox, and WebKit) to ensure cross-browser compatibility of the Board Game Café application.

## Workflow File

`.github/workflows/e2e.yml`

## Triggers

The workflow runs automatically on:
- **Push** to `main` or `develop` branches
- **Pull requests** targeting `main` or `develop` branches
- **Manual dispatch** via GitHub Actions UI

## Jobs

### E2E Tests

Runs Playwright E2E tests in parallel across three browser engines:
- **Chromium** (Chrome, Edge, Brave)
- **Firefox**
- **WebKit** (Safari)

**Timeout:** 30 minutes per browser

**Strategy:** `fail-fast: false` - All browser tests continue even if one fails

## Workflow Steps

### 1. Setup
- Checkout repository code
- Install .NET 9 SDK
- Install Node.js LTS
- Cache npm packages for faster builds
- Cache Playwright browsers to avoid re-downloading

### 2. Build
- Restore backend dependencies (`dotnet restore`)
- Restore frontend dependencies (`npm ci`)
- Build backend in Release configuration
- Build frontend production bundle

### 3. Service Startup
- Start backend API on `http://localhost:5000` in background
- Start frontend dev server on `http://localhost:5173` in background
- Wait for backend health check endpoint (`/health`) with 30 retries (5 seconds apart)
- Wait for frontend to respond with 30 retries (5 seconds apart)

### 4. Test Execution
- Install Playwright browser for the current matrix job
- Run Playwright E2E tests with:
  - Project filter: `--project=${{ matrix.project }}`
  - Reporters: `html,github` for CI-friendly output
  - Environment: `CI=true` for optimal settings

### 5. Artifact Collection

#### On Test Failure:
- **Screenshots** (`test-results/**/*.png`) - 7 day retention
- **Videos** (`test-results/**/*.webm`) - 7 day retention
- **Backend logs** (`backend.log`) - 7 day retention
- **Frontend logs** (`frontend.log`) - 7 day retention

#### Always:
- **Playwright HTML Report** (`playwright-report/`) - 7 day retention

### 6. Cleanup
- Stop backend service (kill process by PID)
- Stop frontend service (kill process by PID)

### 7. Job Summary
Creates a GitHub Actions summary with:
- Browser name
- Test status
- Links to artifacts (on failure)

## Playwright Configuration

The workflow integrates with `client/playwright.config.ts`:

```typescript
reporter: process.env.CI ? [['html'], ['github']] : 'html'
```

In CI:
- Uses both HTML and GitHub reporters
- Runs with 1 worker (no parallelization)
- Enables 2 retries for flaky tests
- Runs in headless mode
- Takes screenshots on failure
- Records videos on retry

## Caching Strategy

### NPM Packages
- **Path:** `~/.npm`
- **Key:** Based on `client/package-lock.json` hash
- **Speed improvement:** ~20-30 seconds per run

### Playwright Browsers
- **Path:** `~/.cache/ms-playwright`
- **Key:** `playwright-{OS}-{package-lock hash}`
- **Speed improvement:** ~60-90 seconds per run (avoids re-downloading browsers)

## Viewing Test Results

### GitHub Actions UI

1. Navigate to **Actions** tab
2. Click on the workflow run
3. View job summary for quick status
4. Click on individual browser jobs for detailed logs

### Artifacts

On test failure, download artifacts:

1. Go to workflow run
2. Scroll to **Artifacts** section
3. Download:
   - `screenshots-{browser}-{run-number}.zip`
   - `videos-{browser}-{run-number}.zip`
   - `playwright-report-{browser}-{run-number}.zip`
   - `backend-logs-{browser}-{run-number}.zip` (if needed)
   - `frontend-logs-{browser}-{run-number}.zip` (if needed)

### HTML Report

Extract and open `playwright-report-{browser}-{run-number}.zip`:

```bash
unzip playwright-report-chromium-123.zip
cd playwright-report
npx playwright show-report .
```

## Local Testing

To replicate CI behavior locally:

```bash
# Navigate to client directory
cd client

# Install dependencies
npm ci

# Install Playwright browsers
npx playwright install --with-deps

# Build frontend
npm run build

# In separate terminal: Start backend
cd ../src/BoardGameCafe.Api
dotnet run --urls "http://localhost:5000"

# In separate terminal: Start frontend
cd client
npm run dev

# Run E2E tests
CI=true npm run test:e2e
```

## Troubleshooting

### Tests Failing Locally but Passing in CI (or vice versa)

**Possible causes:**
- Different browser versions
- Different screen resolutions
- Timing differences

**Solutions:**
- Run with `CI=true` locally
- Check Playwright version matches
- Review screenshots/videos from failed runs

### Health Check Timeouts

**Error:** "Backend failed to start!"

**Possible causes:**
- Build errors in backend
- Port already in use (5000 or 5173)
- Database migration issues

**Solutions:**
- Check backend logs artifact
- Verify local builds succeed
- Check for port conflicts

### Flaky Tests

**Symptoms:** Tests pass sometimes, fail other times

**Solutions:**
- Playwright already retries 2 times in CI
- Add explicit waits for dynamic content
- Use `data-testid` selectors instead of CSS classes
- Check for race conditions in test setup

### Artifact Upload Failures

**Error:** "No files found for artifact upload"

**This is expected** when:
- Tests pass (no screenshots/videos needed)
- Error occurred before test execution
- Using `if-no-files-found: ignore`

## Performance Optimization

### Current Optimizations:
- ✅ NPM package caching
- ✅ Playwright browser caching
- ✅ Parallel browser testing
- ✅ Single worker per browser (stability)
- ✅ Install only required browser per job

### Future Improvements:
- [ ] Add .NET package caching
- [ ] Parallel test execution within browsers (after stability proven)
- [ ] Sharded test execution for large test suites
- [ ] Test result caching to skip passing tests

## Best Practices

### For Test Authors:

1. **Use stable selectors:** Prefer `data-testid` over CSS classes
2. **Avoid hardcoded waits:** Use Playwright's auto-waiting
3. **Keep tests independent:** No shared state between tests
4. **Use Page Object Models:** Reusable, maintainable test code
5. **Test one thing:** Each test should verify one behavior

### For Workflow Maintenance:

1. **Monitor artifact storage:** 7-day retention, cleanup old runs
2. **Update dependencies:** Keep actions versions current
3. **Review flaky tests:** Address root causes, not just retries
4. **Optimize caching:** Monitor cache hit rates
5. **Keep timeout reasonable:** 30 minutes should be sufficient

## Security Considerations

- ✅ No secrets required (public API endpoints)
- ✅ Artifacts are stored in GitHub (not exposed publicly)
- ✅ Service logs don't contain sensitive data
- ✅ Using official GitHub Actions only

## Related Documentation

- [Playwright Testing Guide](../client/PLAYWRIGHT_SETUP.md)
- [Playwright Configuration](../client/playwright.config.ts)
- [Backend API Documentation](../src/BoardGameCafe.Api/README.md)
- [GitHub Actions Documentation](https://docs.github.com/en/actions)

## Metrics

**Average run time per browser:** ~5-8 minutes
- Setup: ~2 minutes
- Build: ~1 minute
- Service startup: ~30 seconds
- Test execution: ~2-3 minutes
- Artifact upload: ~30 seconds

**Total workflow time:** ~5-8 minutes (all browsers run in parallel)

**Artifact storage per run:** ~50-100 MB (when tests fail)

## Support

For issues with this workflow:

1. Check [Troubleshooting](#troubleshooting) section
2. Review workflow logs in GitHub Actions
3. Download and examine artifacts
4. Open an issue with:
   - Workflow run URL
   - Browser/OS information
   - Steps to reproduce
   - Relevant logs/screenshots
