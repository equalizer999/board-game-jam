import { Page, Locator, expect } from '@playwright/test';

/**
 * Page Object Model for Menu/Order page
 * Handles food and beverage ordering workflow
 */
export class OrderPage {
  readonly page: Page;
  readonly menuHeading: Locator;
  readonly categoryTabs: Locator;
  readonly menuItems: Locator;
  readonly cartButton: Locator;
  readonly cartItemCount: Locator;
  readonly cartPanel: Locator;
  readonly cartItems: Locator;
  readonly cartSubtotal: Locator;
  readonly cartTax: Locator;
  readonly cartTotal: Locator;
  readonly checkoutButton: Locator;
  readonly emptyCartMessage: Locator;
  readonly orderSuccessMessage: Locator;
  readonly filterVegetarian: Locator;
  readonly filterVegan: Locator;
  readonly filterGlutenFree: Locator;

  constructor(page: Page) {
    this.page = page;
    
    // Menu and ordering elements using data-testid
    this.menuHeading = page.getByTestId('menu-heading');
    this.categoryTabs = page.getByTestId('category-tab');
    this.menuItems = page.getByTestId('menu-item');
    this.cartButton = page.getByTestId('cart-button');
    this.cartItemCount = page.getByTestId('cart-item-count');
    this.cartPanel = page.getByTestId('cart-panel');
    this.cartItems = page.getByTestId('cart-item');
    this.cartSubtotal = page.getByTestId('cart-subtotal');
    this.cartTax = page.getByTestId('cart-tax');
    this.cartTotal = page.getByTestId('cart-total');
    this.checkoutButton = page.getByTestId('checkout-button');
    this.emptyCartMessage = page.getByTestId('empty-cart-message');
    this.orderSuccessMessage = page.getByTestId('order-success-message');
    this.filterVegetarian = page.getByTestId('filter-vegetarian');
    this.filterVegan = page.getByTestId('filter-vegan');
    this.filterGlutenFree = page.getByTestId('filter-gluten-free');
  }

  /**
   * Navigate to the menu/order page
   */
  async goto() {
    await this.page.goto('/menu');
    await this.page.waitForLoadState('networkidle');
  }

  /**
   * Wait for menu items to load
   */
  async waitForMenuItemsLoad() {
    await this.menuItems.first().waitFor({ state: 'visible', timeout: 10000 });
  }

  /**
   * Select menu category
   */
  async selectCategory(category: string) {
    const categoryTab = this.page.getByTestId(`category-tab-${category.toLowerCase()}`);
    await categoryTab.click();
    await this.page.waitForTimeout(500);
  }

  /**
   * Get count of visible menu items
   */
  async getMenuItemCount(): Promise<number> {
    return await this.menuItems.count();
  }

  /**
   * Add menu item to cart by name
   */
  async addItemToCart(itemName: string, quantity: number = 1) {
    const menuItem = this.page.getByTestId(`menu-item-${itemName.toLowerCase().replace(/\s+/g, '-')}`);
    
    // Click add button multiple times for quantity
    for (let i = 0; i < quantity; i++) {
      const addButton = menuItem.getByTestId('add-to-cart-button');
      await addButton.click();
      await this.page.waitForTimeout(200);
    }
  }

  /**
   * Remove item from cart by name
   */
  async removeItemFromCart(itemName: string) {
    const cartItem = this.page.getByTestId(`cart-item-${itemName.toLowerCase().replace(/\s+/g, '-')}`);
    const removeButton = cartItem.getByTestId('remove-item-button');
    await removeButton.click();
  }

  /**
   * Update item quantity in cart
   */
  async updateCartItemQuantity(itemName: string, quantity: number) {
    const cartItem = this.page.getByTestId(`cart-item-${itemName.toLowerCase().replace(/\s+/g, '-')}`);
    const quantityInput = cartItem.getByTestId('quantity-input');
    await quantityInput.fill(quantity.toString());
  }

  /**
   * Open cart panel
   */
  async openCart() {
    await this.cartButton.click();
    await expect(this.cartPanel).toBeVisible();
  }

  /**
   * Close cart panel
   */
  async closeCart() {
    const closeButton = this.page.getByTestId('close-cart-button');
    await closeButton.click();
    await expect(this.cartPanel).not.toBeVisible();
  }

  /**
   * Get cart item count
   */
  async getCartItemCount(): Promise<number> {
    const countText = await this.cartItemCount.textContent();
    return parseInt(countText || '0', 10);
  }

  /**
   * Get cart subtotal value
   */
  async getCartSubtotal(): Promise<number> {
    const subtotalText = await this.cartSubtotal.textContent();
    const match = subtotalText?.match(/[\d.]+/);
    return match ? parseFloat(match[0]) : 0;
  }

  /**
   * Get cart tax value
   */
  async getCartTax(): Promise<number> {
    const taxText = await this.cartTax.textContent();
    const match = taxText?.match(/[\d.]+/);
    return match ? parseFloat(match[0]) : 0;
  }

  /**
   * Get cart total value
   */
  async getCartTotal(): Promise<number> {
    const totalText = await this.cartTotal.textContent();
    const match = totalText?.match(/[\d.]+/);
    return match ? parseFloat(match[0]) : 0;
  }

  /**
   * Proceed to checkout
   */
  async proceedToCheckout() {
    await this.checkoutButton.click();
  }

  /**
   * Complete order with payment details
   */
  async completeOrder(paymentMethod: 'credit-card' | 'debit-card' | 'cash' = 'credit-card') {
    // Select payment method
    const paymentMethodSelect = this.page.getByTestId('payment-method-select');
    await paymentMethodSelect.selectOption({ value: paymentMethod });
    
    // Submit order
    const placeOrderButton = this.page.getByTestId('place-order-button');
    await placeOrderButton.click();
  }

  /**
   * Verify order success
   */
  async verifyOrderSuccess() {
    await expect(this.orderSuccessMessage).toBeVisible();
    await expect(this.orderSuccessMessage).toContainText(/order.*placed|success/i);
  }

  /**
   * Verify cart is empty
   */
  async verifyCartEmpty() {
    await expect(this.emptyCartMessage).toBeVisible();
  }

  /**
   * Apply vegetarian filter
   */
  async applyVegetarianFilter() {
    await this.filterVegetarian.click();
    await this.page.waitForTimeout(500);
  }

  /**
   * Apply vegan filter
   */
  async applyVeganFilter() {
    await this.filterVegan.click();
    await this.page.waitForTimeout(500);
  }

  /**
   * Apply gluten-free filter
   */
  async applyGlutenFreeFilter() {
    await this.filterGlutenFree.click();
    await this.page.waitForTimeout(500);
  }

  /**
   * Verify menu item displays correct information
   */
  async verifyMenuItemInfo(itemName: string) {
    const menuItem = this.page.getByTestId(`menu-item-${itemName.toLowerCase().replace(/\s+/g, '-')}`);
    
    await expect(menuItem).toBeVisible();
    await expect(menuItem.getByTestId('item-name')).toContainText(itemName);
    await expect(menuItem.getByTestId('item-price')).toBeVisible();
    await expect(menuItem.getByTestId('item-description')).toBeVisible();
  }

  /**
   * Verify pricing calculations are correct
   */
  async verifyPricingCalculations() {
    const subtotal = await this.getCartSubtotal();
    const tax = await this.getCartTax();
    const total = await this.getCartTotal();
    
    // Verify total = subtotal + tax (with small floating point tolerance)
    const calculatedTotal = subtotal + tax;
    expect(Math.abs(total - calculatedTotal)).toBeLessThan(0.01);
  }

  /**
   * Add special instructions to cart item
   */
  async addSpecialInstructions(itemName: string, instructions: string) {
    const cartItem = this.page.getByTestId(`cart-item-${itemName.toLowerCase().replace(/\s+/g, '-')}`);
    const instructionsInput = cartItem.getByTestId('special-instructions-input');
    await instructionsInput.fill(instructions);
  }

  /**
   * Verify item is in cart
   */
  async verifyItemInCart(itemName: string) {
    const cartItem = this.page.getByTestId(`cart-item-${itemName.toLowerCase().replace(/\s+/g, '-')}`);
    await expect(cartItem).toBeVisible();
  }

  /**
   * Get order number from success message
   */
  async getOrderNumber(): Promise<string> {
    const orderNumberElement = this.page.getByTestId('order-number');
    await orderNumberElement.waitFor({ state: 'visible' });
    return await orderNumberElement.textContent() || '';
  }
}
