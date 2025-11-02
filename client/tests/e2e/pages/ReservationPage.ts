import { Page, Locator, expect } from '@playwright/test';

/**
 * Page Object Model for Reservation/Table Booking page
 * Handles table reservation workflow including date selection, time slot, party size, and booking
 */
export class ReservationPage {
  readonly page: Page;
  readonly reservationHeading: Locator;
  readonly datePickerInput: Locator;
  readonly partySizeSelect: Locator;
  readonly durationSelect: Locator;
  readonly availableTablesGrid: Locator;
  readonly tableCards: Locator;
  readonly customerNameInput: Locator;
  readonly customerEmailInput: Locator;
  readonly customerPhoneInput: Locator;
  readonly specialRequestsInput: Locator;
  readonly confirmReservationButton: Locator;
  readonly successMessage: Locator;
  readonly errorMessage: Locator;
  readonly availabilityCalendar: Locator;

  constructor(page: Page) {
    this.page = page;
    
    // Reservation form elements using data-testid
    this.reservationHeading = page.getByTestId('reservation-heading');
    this.datePickerInput = page.getByTestId('date-picker-input');
    this.partySizeSelect = page.getByTestId('party-size-select');
    this.durationSelect = page.getByTestId('duration-select');
    this.availableTablesGrid = page.getByTestId('available-tables-grid');
    this.tableCards = page.getByTestId('table-card');
    this.customerNameInput = page.getByTestId('customer-name-input');
    this.customerEmailInput = page.getByTestId('customer-email-input');
    this.customerPhoneInput = page.getByTestId('customer-phone-input');
    this.specialRequestsInput = page.getByTestId('special-requests-input');
    this.confirmReservationButton = page.getByTestId('confirm-reservation-button');
    this.successMessage = page.getByTestId('success-message');
    this.errorMessage = page.getByTestId('error-message');
    this.availabilityCalendar = page.getByTestId('availability-calendar');
  }

  /**
   * Navigate to the reservations page
   */
  async goto() {
    await this.page.goto('/reservations');
    await this.page.waitForLoadState('networkidle');
  }

  /**
   * Select a date for reservation
   */
  async selectDate(date: string) {
    await this.datePickerInput.click();
    await this.page.getByTestId(`calendar-date-${date}`).click();
  }

  /**
   * Select date using native date input (alternative)
   */
  async fillDate(dateValue: string) {
    await this.datePickerInput.fill(dateValue);
  }

  /**
   * Select party size
   */
  async selectPartySize(size: number) {
    await this.partySizeSelect.selectOption({ value: size.toString() });
  }

  /**
   * Select reservation duration
   */
  async selectDuration(hours: number) {
    await this.durationSelect.selectOption({ value: hours.toString() });
  }

  /**
   * Wait for available tables to load
   */
  async waitForAvailableTablesLoad() {
    await this.availableTablesGrid.waitFor({ state: 'visible', timeout: 10000 });
    await this.tableCards.first().waitFor({ state: 'visible', timeout: 5000 });
  }

  /**
   * Get count of available tables
   */
  async getAvailableTableCount(): Promise<number> {
    return await this.tableCards.count();
  }

  /**
   * Select a table by table number
   */
  async selectTable(tableNumber: number) {
    const tableCard = this.page.getByTestId(`table-card-${tableNumber}`);
    await tableCard.click();
    
    // Verify table is selected
    await expect(tableCard).toHaveClass(/selected|active/);
  }

  /**
   * Select first available table
   */
  async selectFirstAvailableTable() {
    const firstTable = this.tableCards.first();
    await firstTable.click();
  }

  /**
   * Fill customer information
   */
  async fillCustomerInfo(name: string, email: string, phone: string) {
    await this.customerNameInput.fill(name);
    await this.customerEmailInput.fill(email);
    await this.customerPhoneInput.fill(phone);
  }

  /**
   * Add special requests
   */
  async addSpecialRequests(requests: string) {
    await this.specialRequestsInput.fill(requests);
  }

  /**
   * Submit reservation form
   */
  async submitReservation() {
    await this.confirmReservationButton.click();
  }

  /**
   * Complete full reservation workflow
   */
  async completeReservation(
    date: string,
    partySize: number,
    duration: number,
    customerName: string,
    customerEmail: string,
    customerPhone: string,
    specialRequests?: string
  ) {
    // Step 1: Select date and party details
    await this.fillDate(date);
    await this.selectPartySize(partySize);
    await this.selectDuration(duration);
    
    // Step 2: Wait for and select table
    await this.waitForAvailableTablesLoad();
    await this.selectFirstAvailableTable();
    
    // Step 3: Fill customer info
    await this.fillCustomerInfo(customerName, customerEmail, customerPhone);
    
    // Step 4: Add special requests if provided
    if (specialRequests) {
      await this.addSpecialRequests(specialRequests);
    }
    
    // Step 5: Submit
    await this.submitReservation();
  }

  /**
   * Verify reservation success
   */
  async verifyReservationSuccess() {
    await expect(this.successMessage).toBeVisible();
    await expect(this.successMessage).toContainText(/reservation.*confirmed|success/i);
  }

  /**
   * Verify reservation error
   */
  async verifyReservationError(expectedErrorText?: string) {
    await expect(this.errorMessage).toBeVisible();
    
    if (expectedErrorText) {
      await expect(this.errorMessage).toContainText(expectedErrorText);
    }
  }

  /**
   * Verify table card displays correct information
   */
  async verifyTableCardInfo(tableNumber: number) {
    const tableCard = this.page.getByTestId(`table-card-${tableNumber}`);
    
    await expect(tableCard).toBeVisible();
    await expect(tableCard.getByTestId('table-number')).toContainText(tableNumber.toString());
    await expect(tableCard.getByTestId('table-capacity')).toBeVisible();
  }

  /**
   * Verify table availability for given criteria
   */
  async verifyTablesAvailable() {
    const count = await this.getAvailableTableCount();
    expect(count).toBeGreaterThan(0);
  }

  /**
   * Verify no tables available message
   */
  async verifyNoTablesAvailable() {
    const noTablesMessage = this.page.getByTestId('no-tables-message');
    await expect(noTablesMessage).toBeVisible();
  }

  /**
   * Select time slot from calendar view
   */
  async selectTimeSlot(hour: number) {
    const timeSlot = this.page.getByTestId(`time-slot-${hour}`);
    await timeSlot.click();
  }

  /**
   * Verify calendar displays availability
   */
  async verifyCalendarDisplayed() {
    await expect(this.availabilityCalendar).toBeVisible();
  }

  /**
   * Get reservation confirmation number
   */
  async getConfirmationNumber(): Promise<string> {
    const confirmationElement = this.page.getByTestId('confirmation-number');
    await confirmationElement.waitFor({ state: 'visible' });
    return await confirmationElement.textContent() || '';
  }
}
