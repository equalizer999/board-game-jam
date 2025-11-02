import { Page, Locator, expect } from '@playwright/test';

/**
 * Page Object Model for Game Catalog page
 * Handles game browsing, filtering, and selection interactions
 */
export class GameCatalogPage {
  readonly page: Page;
  readonly gameCatalogHeading: Locator;
  readonly gameCards: Locator;
  readonly categoryFilter: Locator;
  readonly playerCountFilter: Locator;
  readonly complexityFilter: Locator;
  readonly searchInput: Locator;
  readonly noResultsMessage: Locator;

  constructor(page: Page) {
    this.page = page;
    
    // Main page elements using data-testid for stable selectors
    this.gameCatalogHeading = page.getByTestId('game-catalog-heading');
    this.gameCards = page.getByTestId('game-card');
    this.categoryFilter = page.getByTestId('category-filter');
    this.playerCountFilter = page.getByTestId('player-count-filter');
    this.complexityFilter = page.getByTestId('complexity-filter');
    this.searchInput = page.getByTestId('game-search-input');
    this.noResultsMessage = page.getByTestId('no-results-message');
  }

  /**
   * Navigate to the game catalog page
   */
  async goto() {
    await this.page.goto('/games');
    await this.page.waitForLoadState('networkidle');
  }

  /**
   * Wait for games to load
   */
  async waitForGamesToLoad() {
    await this.gameCards.first().waitFor({ state: 'visible', timeout: 10000 });
  }

  /**
   * Get all visible game cards
   */
  async getGameCards() {
    return await this.gameCards.all();
  }

  /**
   * Get count of visible games
   */
  async getGameCount(): Promise<number> {
    return await this.gameCards.count();
  }

  /**
   * Filter games by category
   */
  async filterByCategory(category: string) {
    await this.categoryFilter.click();
    await this.page.getByTestId(`category-option-${category.toLowerCase()}`).click();
    // Wait for filter to apply
    await this.page.waitForTimeout(500);
  }

  /**
   * Filter games by player count
   */
  async filterByPlayerCount(minPlayers: number, maxPlayers: number) {
    await this.playerCountFilter.click();
    await this.page.getByTestId(`player-count-${minPlayers}-${maxPlayers}`).click();
    await this.page.waitForTimeout(500);
  }

  /**
   * Filter games by complexity
   */
  async filterByComplexity(complexity: string) {
    await this.complexityFilter.click();
    await this.page.getByTestId(`complexity-${complexity.toLowerCase()}`).click();
    await this.page.waitForTimeout(500);
  }

  /**
   * Search for games by title or description
   */
  async searchGames(searchTerm: string) {
    await this.searchInput.fill(searchTerm);
    await this.page.waitForTimeout(500);
  }

  /**
   * Click on a game card by title
   */
  async clickGameCard(gameTitle: string) {
    const gameCard = this.page.getByTestId(`game-card-${gameTitle.toLowerCase().replace(/\s+/g, '-')}`);
    await gameCard.click();
  }

  /**
   * Verify game detail modal is open
   */
  async verifyDetailModalOpen() {
    const modal = this.page.getByTestId('game-detail-modal');
    await expect(modal).toBeVisible();
  }

  /**
   * Verify modal shows correct game details
   */
  async verifyModalGameDetails(gameTitle: string) {
    const modalTitle = this.page.getByTestId('modal-game-title');
    await expect(modalTitle).toContainText(gameTitle);
  }

  /**
   * Close the game detail modal
   */
  async closeDetailModal() {
    const closeButton = this.page.getByTestId('close-modal-button');
    await closeButton.click();
  }

  /**
   * Verify filtered results match category
   */
  async verifyFilteredResultsContainCategory(category: string) {
    const cards = await this.getGameCards();
    
    for (const card of cards) {
      const categoryBadge = card.getByTestId('game-category');
      await expect(categoryBadge).toContainText(category, { ignoreCase: true });
    }
  }

  /**
   * Verify no results message is shown
   */
  async verifyNoResultsShown() {
    await expect(this.noResultsMessage).toBeVisible();
  }

  /**
   * Verify games are displayed
   */
  async verifyGamesDisplayed() {
    const count = await this.getGameCount();
    expect(count).toBeGreaterThan(0);
  }

  /**
   * Get game title from a card
   */
  async getGameTitle(cardIndex: number): Promise<string> {
    const card = this.gameCards.nth(cardIndex);
    const title = card.getByTestId('game-title');
    return await title.textContent() || '';
  }

  /**
   * Verify game card contains expected information
   */
  async verifyGameCardInfo(cardIndex: number) {
    const card = this.gameCards.nth(cardIndex);
    
    // Verify essential elements exist
    await expect(card.getByTestId('game-title')).toBeVisible();
    await expect(card.getByTestId('game-image')).toBeVisible();
    await expect(card.getByTestId('game-category')).toBeVisible();
    await expect(card.getByTestId('game-players')).toBeVisible();
  }
}
