import { test, expect } from '@playwright/test';
import { GameCatalogPage } from '../pages/GameCatalogPage';
import { sampleGames } from '../fixtures/testData';

/**
 * E2E Test Suite: Game Browsing Workflow
 * 
 * Tests the game catalog browsing functionality including:
 * - Loading the game catalog
 * - Filtering games by category
 * - Viewing game details
 */

test.describe('Game Browsing', () => {
  let gameCatalogPage: GameCatalogPage;

  test.beforeEach(async ({ page }) => {
    gameCatalogPage = new GameCatalogPage(page);
    
    // Navigate to game catalog
    await gameCatalogPage.goto();
  });

  test('should navigate to game catalog and verify games load', async ({ page }) => {
    // Verify page heading is visible
    await expect(gameCatalogPage.gameCatalogHeading).toBeVisible();
    
    // Wait for games to load
    await gameCatalogPage.waitForGamesToLoad();
    
    // Verify games are displayed
    await gameCatalogPage.verifyGamesDisplayed();
    
    // Verify at least one game card has proper structure
    const gameCount = await gameCatalogPage.getGameCount();
    expect(gameCount).toBeGreaterThan(0);
    
    // Verify first game card has expected information
    await gameCatalogPage.verifyGameCardInfo(0);
  });

  test('should apply category filter and verify filtered results', async ({ page }) => {
    // Wait for games to load
    await gameCatalogPage.waitForGamesToLoad();
    
    // Get initial game count
    const initialCount = await gameCatalogPage.getGameCount();
    expect(initialCount).toBeGreaterThan(0);
    
    // Apply Strategy category filter
    await gameCatalogPage.filterByCategory('Strategy');
    
    // Verify filtered results
    const filteredCount = await gameCatalogPage.getGameCount();
    expect(filteredCount).toBeGreaterThan(0);
    
    // Verify all displayed games have Strategy category
    await gameCatalogPage.verifyFilteredResultsContainCategory('Strategy');
  });

  test('should click game card and verify detail modal opens', async ({ page }) => {
    // Wait for games to load
    await gameCatalogPage.waitForGamesToLoad();
    
    // Get the first game's title to verify later
    const firstGameTitle = await gameCatalogPage.getGameTitle(0);
    expect(firstGameTitle).toBeTruthy();
    
    // Click on the first game card
    await gameCatalogPage.clickGameCard(firstGameTitle);
    
    // Verify detail modal opens
    await gameCatalogPage.verifyDetailModalOpen();
    
    // Verify modal shows correct game details
    await gameCatalogPage.verifyModalGameDetails(firstGameTitle);
  });

  test('should filter by multiple criteria', async ({ page }) => {
    // Wait for games to load
    await gameCatalogPage.waitForGamesToLoad();
    
    // Apply category filter
    await gameCatalogPage.filterByCategory('Strategy');
    
    // Apply complexity filter
    await gameCatalogPage.filterByComplexity('Medium');
    
    // Verify games are still displayed with both filters
    const count = await gameCatalogPage.getGameCount();
    expect(count).toBeGreaterThan(0);
  });

  test('should search for games by title', async ({ page }) => {
    // Wait for games to load
    await gameCatalogPage.waitForGamesToLoad();
    
    // Search for a specific game (using sample data)
    const searchTerm = 'Catan';
    await gameCatalogPage.searchGames(searchTerm);
    
    // Verify results contain the search term
    const count = await gameCatalogPage.getGameCount();
    
    if (count > 0) {
      const gameTitle = await gameCatalogPage.getGameTitle(0);
      expect(gameTitle.toLowerCase()).toContain(searchTerm.toLowerCase());
    }
  });

  test('should display correct game information in cards', async ({ page }) => {
    // Wait for games to load
    await gameCatalogPage.waitForGamesToLoad();
    
    // Verify each visible game card has required elements
    const gameCount = await gameCatalogPage.getGameCount();
    const cardsToCheck = Math.min(gameCount, 3); // Check first 3 cards
    
    for (let i = 0; i < cardsToCheck; i++) {
      await gameCatalogPage.verifyGameCardInfo(i);
    }
  });

  test('should handle no results gracefully', async ({ page }) => {
    // Wait for games to load
    await gameCatalogPage.waitForGamesToLoad();
    
    // Search for non-existent game
    await gameCatalogPage.searchGames('NonExistentGameXYZ123');
    
    // Verify no results message or empty state
    const count = await gameCatalogPage.getGameCount();
    
    if (count === 0) {
      // Either no games displayed or a "no results" message
      await expect(page.getByText(/no.*games.*found|no.*results/i).first()).toBeVisible();
    }
  });

  test('should close detail modal when close button is clicked', async ({ page }) => {
    // Wait for games to load
    await gameCatalogPage.waitForGamesToLoad();
    
    // Get the first game and click it
    const firstGameTitle = await gameCatalogPage.getGameTitle(0);
    await gameCatalogPage.clickGameCard(firstGameTitle);
    
    // Verify modal is open
    await gameCatalogPage.verifyDetailModalOpen();
    
    // Close the modal
    await gameCatalogPage.closeDetailModal();
    
    // Verify modal is closed
    const modal = page.getByTestId('game-detail-modal');
    await expect(modal).not.toBeVisible();
  });
});
