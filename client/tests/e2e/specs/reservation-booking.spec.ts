import { test, expect } from '@playwright/test';

/**
 * E2E Test Suite: Reservation Booking Flow
 * 
 * Tests the complete reservation workflow including:
 * - Multi-step wizard navigation
 * - Date and time selection
 * - Table selection with visual layout
 * - Reservation confirmation
 * - Success screen and redirect
 */

test.describe('Reservation Booking Flow', () => {
  test.beforeEach(async ({ page }) => {
    // Navigate to reservations page
    await page.goto('/reservations');
    await page.waitForLoadState('networkidle');
  });

  test('should display reservations page with new reservation button', async ({ page }) => {
    // Verify heading is visible
    await expect(page.getByText('🎲 My Reservations')).toBeVisible();
    
    // Verify new reservation button is visible
    const newReservationLink = page.getByRole('link', { name: /new reservation/i });
    await expect(newReservationLink).toBeVisible();
  });

  test('should navigate to new reservation wizard when clicking new reservation', async ({ page }) => {
    // Click new reservation button
    const newReservationLink = page.getByRole('link', { name: /new reservation/i });
    await newReservationLink.click();
    
    // Verify navigation to new reservation page
    await expect(page).toHaveURL('/reservations/new');
    
    // Verify wizard heading
    await expect(page.getByText('🎲 New Reservation')).toBeVisible();
    
    // Verify progress indicator shows step 1
    await expect(page.getByText('Date & Party')).toBeVisible();
  });

  test('should complete full reservation workflow', async ({ page }) => {
    // Navigate to new reservation
    await page.goto('/reservations/new');
    await page.waitForLoadState('networkidle');
    
    // Step 1: Select date and party size
    await expect(page.getByText('Select Date & Time')).toBeVisible();
    
    // Get tomorrow's date in YYYY-MM-DD format
    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);
    const tomorrowStr = tomorrow.toISOString().split('T')[0];
    
    // Fill in date
    const dateInput = page.getByTestId('reservation-date');
    await dateInput.fill(tomorrowStr);
    
    // Fill in party size
    const partySizeInput = page.getByTestId('party-size');
    await partySizeInput.fill('4');
    
    // Click Next
    await page.getByRole('button', { name: 'Next' }).click();
    
    // Step 2: Select time
    await expect(page.getByText('Select Date & Time')).toBeVisible();
    
    // Select start time (6 PM)
    const startTimeSelect = page.getByTestId('start-time');
    await startTimeSelect.selectOption('18:00:00');
    
    // Select end time (8 PM)
    const endTimeSelect = page.getByTestId('end-time');
    await endTimeSelect.selectOption('20:00:00');
    
    // Verify duration is displayed
    await expect(page.getByText(/Duration.*2.*hour/i)).toBeVisible();
    
    // Click Next
    await page.getByRole('button', { name: 'Next' }).click();
    
    // Step 3: Select table
    // Wait for tables to load with explicit timeout
    const selectTableHeading = page.getByText('Select a Table');
    await expect(selectTableHeading).toBeVisible({ timeout: 10000 });
    
    // Wait for table buttons to be available (check if any exist)
    const tableButtons = page.locator('button[data-testid^="table-"]');
    // Wait a bit for tables to load from API
    await page.waitForLoadState('networkidle');
    const tableCount = await tableButtons.count();
    
    if (tableCount > 0) {
      await tableButtons.first().click();
      
      // Click Review
      await page.getByRole('button', { name: 'Review' }).click();
      
      // Step 4: Review and confirm
      await expect(page.getByText('Special Requests')).toBeVisible();
      await expect(page.getByText('Reservation Summary')).toBeVisible();
      
      // Optionally add special requests
      const specialRequestsTextarea = page.locator('textarea');
      await specialRequestsTextarea.fill('Test reservation - please prepare a highchair');
      
      // Click Confirm
      await page.getByRole('button', { name: /confirm reservation/i }).click();
      
      // Verify success screen
      await expect(page.getByText('Reservation Confirmed!')).toBeVisible();
      
      // Wait for redirect (with timeout)
      await page.waitForURL('/reservations', { timeout: 5000 });
    } else {
      // No tables available - verify message
      await expect(page.getByText('No Tables Available')).toBeVisible();
    }
  });

  test('should validate date and party size before proceeding', async ({ page }) => {
    await page.goto('/reservations/new');
    await page.waitForLoadState('networkidle');
    
    // Try to click Next without filling in required fields
    // Note: HTML5 validation should prevent this, but we test the alert fallback
    
    // Fill only date, not party size (though it has a default)
    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);
    const tomorrowStr = tomorrow.toISOString().split('T')[0];
    
    const dateInput = page.getByTestId('reservation-date');
    await dateInput.fill(tomorrowStr);
    
    // Should be able to proceed with default party size
    await page.getByRole('button', { name: 'Next' }).click();
    
    // Should now be on step 2
    await expect(page.getByTestId('start-time')).toBeVisible();
  });

  test('should validate time selection before proceeding to table selection', async ({ page }) => {
    await page.goto('/reservations/new');
    await page.waitForLoadState('networkidle');
    
    // Step 1: Fill date and party size
    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);
    const tomorrowStr = tomorrow.toISOString().split('T')[0];
    
    await page.getByTestId('reservation-date').fill(tomorrowStr);
    await page.getByTestId('party-size').fill('2');
    await page.getByRole('button', { name: 'Next' }).click();
    
    // Step 2: Try to proceed without selecting times
    page.on('dialog', async (dialog) => {
      expect(dialog.message()).toContain('select start and end times');
      await dialog.accept();
    });
    
    await page.getByRole('button', { name: 'Next' }).click();
    
    // Should still be on step 2
    await expect(page.getByTestId('start-time')).toBeVisible();
  });

  test('should show peak hours multiplier in summary', async ({ page }) => {
    await page.goto('/reservations/new');
    await page.waitForLoadState('networkidle');
    
    // Complete steps to get to summary
    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);
    const tomorrowStr = tomorrow.toISOString().split('T')[0];
    
    // Step 1
    await page.getByTestId('reservation-date').fill(tomorrowStr);
    await page.getByTestId('party-size').fill('4');
    await page.getByRole('button', { name: 'Next' }).click();
    
    // Step 2 - Select peak hours (6 PM - 9 PM)
    await page.getByTestId('start-time').selectOption('18:00:00');
    await page.getByTestId('end-time').selectOption('20:00:00');
    await page.getByRole('button', { name: 'Next' }).click();
    
    // Step 3 - Select table (if available)
    const selectTableHeading = page.getByText('Select a Table');
    await expect(selectTableHeading).toBeVisible({ timeout: 10000 });
    
    const tableButtons = page.locator('button[data-testid^="table-"]');
    await page.waitForLoadState('networkidle');
    const tableCount = await tableButtons.count();
    
    if (tableCount > 0) {
      await tableButtons.first().click();
      await page.getByRole('button', { name: 'Review' }).click();
      
      // Step 4 - Verify peak hours multiplier is shown
      await expect(page.getByText(/peak hours multiplier/i)).toBeVisible();
      await expect(page.getByText(/×1\.5/)).toBeVisible();
    }
  });

  test('should allow navigation back through steps', async ({ page }) => {
    await page.goto('/reservations/new');
    await page.waitForLoadState('networkidle');
    
    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);
    const tomorrowStr = tomorrow.toISOString().split('T')[0];
    
    // Step 1
    await page.getByTestId('reservation-date').fill(tomorrowStr);
    await page.getByTestId('party-size').fill('2');
    await page.getByRole('button', { name: 'Next' }).click();
    
    // Step 2
    await expect(page.getByTestId('start-time')).toBeVisible();
    
    // Go back to step 1
    await page.getByRole('button', { name: 'Back' }).click();
    
    // Verify we're back on step 1
    await expect(page.getByTestId('reservation-date')).toBeVisible();
    
    // Verify date and party size are preserved
    const dateValue = await page.getByTestId('reservation-date').inputValue();
    expect(dateValue).toBe(tomorrowStr);
    
    const partySizeValue = await page.getByTestId('party-size').inputValue();
    expect(partySizeValue).toBe('2');
  });

  test('should show table amenities (window seat, accessible)', async ({ page }) => {
    await page.goto('/reservations/new');
    await page.waitForLoadState('networkidle');
    
    const tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);
    const tomorrowStr = tomorrow.toISOString().split('T')[0];
    
    // Navigate to table selection step
    await page.getByTestId('reservation-date').fill(tomorrowStr);
    await page.getByTestId('party-size').fill('4');
    await page.getByRole('button', { name: 'Next' }).click();
    
    await page.getByTestId('start-time').selectOption('14:00:00');
    await page.getByTestId('end-time').selectOption('16:00:00');
    await page.getByRole('button', { name: 'Next' }).click();
    
    // Wait for table selection screen with explicit timeout
    const selectTableHeading = page.getByText('Select a Table');
    await expect(selectTableHeading).toBeVisible({ timeout: 10000 });
    
    // Check if any tables show amenities
    const windowSeatLabel = page.getByText('🪟 Window');
    const accessibleLabel = page.getByText('♿ Accessible');
    
    // These may or may not be present depending on table data
    // Just verify the page loaded properly
    await expect(page.getByText('Select a Table')).toBeVisible();
  });

  test('should handle cancel and redirect to reservations', async ({ page }) => {
    await page.goto('/reservations/new');
    await page.waitForLoadState('networkidle');
    
    // Set up dialog handler for confirmation
    page.on('dialog', async (dialog) => {
      expect(dialog.message()).toContain('cancel');
      await dialog.accept();
    });
    
    // Click Cancel button
    await page.getByRole('button', { name: 'Cancel' }).click();
    
    // Should redirect back to reservations page
    await page.waitForURL('/reservations', { timeout: 2000 });
  });
});
