import { Page } from '@playwright/test';

/**
 * Test data fixtures for E2E tests
 * Provides sample data and helper functions for seeding via backend endpoints
 */

// Sample game data
export const sampleGames = [
  {
    id: 1,
    title: 'Catan',
    publisher: 'Catan Studio',
    minPlayers: 3,
    maxPlayers: 4,
    playTimeMinutes: 90,
    ageRating: 10,
    complexity: 'Medium',
    category: 'Strategy',
    copiesOwned: 3,
    copiesInUse: 0,
    dailyRentalFee: 5.00,
    description: 'Collect resources and build settlements on the island of Catan',
    imageUrl: '/images/games/catan.jpg'
  },
  {
    id: 2,
    title: 'Ticket to Ride',
    publisher: 'Days of Wonder',
    minPlayers: 2,
    maxPlayers: 5,
    playTimeMinutes: 60,
    ageRating: 8,
    complexity: 'Easy',
    category: 'Strategy',
    copiesOwned: 2,
    copiesInUse: 1,
    dailyRentalFee: 4.50,
    description: 'Build train routes across North America',
    imageUrl: '/images/games/ticket-to-ride.jpg'
  },
  {
    id: 3,
    title: 'Pandemic',
    publisher: 'Z-Man Games',
    minPlayers: 2,
    maxPlayers: 4,
    playTimeMinutes: 45,
    ageRating: 8,
    complexity: 'Medium',
    category: 'Cooperative',
    copiesOwned: 2,
    copiesInUse: 0,
    dailyRentalFee: 5.00,
    description: 'Work together to save humanity from global disease outbreaks',
    imageUrl: '/images/games/pandemic.jpg'
  },
  {
    id: 4,
    title: 'Codenames',
    publisher: 'Czech Games Edition',
    minPlayers: 4,
    maxPlayers: 8,
    playTimeMinutes: 15,
    ageRating: 14,
    complexity: 'Easy',
    category: 'Party',
    copiesOwned: 2,
    copiesInUse: 0,
    dailyRentalFee: 3.00,
    description: 'Team word-guessing game with spies and secret agents',
    imageUrl: '/images/games/codenames.jpg'
  },
  {
    id: 5,
    title: 'Azul',
    publisher: 'Plan B Games',
    minPlayers: 2,
    maxPlayers: 4,
    playTimeMinutes: 45,
    ageRating: 8,
    complexity: 'Easy',
    category: 'Abstract',
    copiesOwned: 1,
    copiesInUse: 0,
    dailyRentalFee: 4.00,
    description: 'Draft colorful tiles to decorate the palace walls',
    imageUrl: '/images/games/azul.jpg'
  }
];

// Sample table data
export const sampleTables = [
  {
    id: 1,
    tableNumber: 1,
    seatingCapacity: 4,
    isWindowSeat: true,
    isAccessible: true,
    hourlyRate: 10.00,
    status: 'Available'
  },
  {
    id: 2,
    tableNumber: 2,
    seatingCapacity: 6,
    isWindowSeat: false,
    isAccessible: true,
    hourlyRate: 15.00,
    status: 'Available'
  },
  {
    id: 3,
    tableNumber: 3,
    seatingCapacity: 2,
    isWindowSeat: true,
    isAccessible: false,
    hourlyRate: 8.00,
    status: 'Available'
  },
  {
    id: 4,
    tableNumber: 4,
    seatingCapacity: 8,
    isWindowSeat: false,
    isAccessible: true,
    hourlyRate: 20.00,
    status: 'Available'
  }
];

// Sample menu items
export const sampleMenuItems = [
  {
    id: 1,
    name: 'Meeple Mocha',
    description: 'Rich chocolate mocha with whipped cream',
    category: 'Coffee',
    price: 5.50,
    isAvailable: true,
    preparationTimeMinutes: 5,
    allergenInfo: 'Contains dairy',
    isVegetarian: true,
    isVegan: false,
    isGlutenFree: true
  },
  {
    id: 2,
    name: 'Catan Cappuccino',
    description: 'Classic cappuccino with house-made foam art',
    category: 'Coffee',
    price: 4.75,
    isAvailable: true,
    preparationTimeMinutes: 5,
    allergenInfo: 'Contains dairy',
    isVegetarian: true,
    isVegan: false,
    isGlutenFree: true
  },
  {
    id: 3,
    name: 'Ticket to Chai',
    description: 'Spiced chai latte with warming spices',
    category: 'Tea',
    price: 4.50,
    isAvailable: true,
    preparationTimeMinutes: 5,
    allergenInfo: 'Contains dairy',
    isVegetarian: true,
    isVegan: false,
    isGlutenFree: true
  },
  {
    id: 4,
    name: 'Pandemic Pizza',
    description: 'Personal 8" pizza with choice of toppings',
    category: 'Meals',
    price: 12.00,
    isAvailable: true,
    preparationTimeMinutes: 15,
    allergenInfo: 'Contains gluten, dairy',
    isVegetarian: true,
    isVegan: false,
    isGlutenFree: false
  },
  {
    id: 5,
    name: 'Wingspan Wings',
    description: 'Crispy chicken wings with buffalo or BBQ sauce',
    category: 'Snacks',
    price: 8.50,
    isAvailable: true,
    preparationTimeMinutes: 12,
    allergenInfo: 'Contains gluten',
    isVegetarian: false,
    isVegan: false,
    isGlutenFree: false
  },
  {
    id: 6,
    name: 'Strategy Cookie',
    description: 'Large chocolate chip cookie',
    category: 'Desserts',
    price: 3.50,
    isAvailable: true,
    preparationTimeMinutes: 2,
    allergenInfo: 'Contains gluten, dairy, eggs',
    isVegetarian: true,
    isVegan: false,
    isGlutenFree: false
  }
];

// Sample customer data
export const sampleCustomers = [
  {
    name: 'John Doe',
    email: 'john.doe@example.com',
    phone: '555-0101',
    membershipTier: 'Bronze',
    loyaltyPoints: 150
  },
  {
    name: 'Jane Smith',
    email: 'jane.smith@example.com',
    phone: '555-0102',
    membershipTier: 'Silver',
    loyaltyPoints: 750
  },
  {
    name: 'Bob Johnson',
    email: 'bob.johnson@example.com',
    phone: '555-0103',
    membershipTier: 'Gold',
    loyaltyPoints: 2500
  }
];

/**
 * Helper functions for API seeding via backend endpoints
 */

/**
 * Seed games via backend API
 */
export async function seedGames(page: Page, baseURL: string = 'http://localhost:5001') {
  for (const game of sampleGames) {
    await page.request.post(`${baseURL}/api/v1/games`, {
      data: game,
      headers: {
        'Content-Type': 'application/json'
      }
    });
  }
}

/**
 * Seed menu items via backend API
 */
export async function seedMenuItems(page: Page, baseURL: string = 'http://localhost:5001') {
  for (const item of sampleMenuItems) {
    await page.request.post(`${baseURL}/api/v1/menu`, {
      data: item,
      headers: {
        'Content-Type': 'application/json'
      }
    });
  }
}

/**
 * Seed tables via backend API
 */
export async function seedTables(page: Page, baseURL: string = 'http://localhost:5001') {
  for (const table of sampleTables) {
    await page.request.post(`${baseURL}/api/v1/tables`, {
      data: table,
      headers: {
        'Content-Type': 'application/json'
      }
    });
  }
}

/**
 * Seed customers via backend API
 */
export async function seedCustomers(page: Page, baseURL: string = 'http://localhost:5001') {
  for (const customer of sampleCustomers) {
    await page.request.post(`${baseURL}/api/v1/customers`, {
      data: customer,
      headers: {
        'Content-Type': 'application/json'
      }
    });
  }
}

/**
 * Clean up test data
 */
export async function cleanupTestData(page: Page, baseURL: string = 'http://localhost:5001') {
  // Delete in reverse dependency order
  await page.request.delete(`${baseURL}/api/v1/test/cleanup/orders`);
  await page.request.delete(`${baseURL}/api/v1/test/cleanup/reservations`);
  await page.request.delete(`${baseURL}/api/v1/test/cleanup/customers`);
  await page.request.delete(`${baseURL}/api/v1/test/cleanup/tables`);
  await page.request.delete(`${baseURL}/api/v1/test/cleanup/menu`);
  await page.request.delete(`${baseURL}/api/v1/test/cleanup/games`);
}

/**
 * Get a future date for reservations (days from now)
 */
export function getFutureDate(daysFromNow: number = 1): string {
  const date = new Date();
  date.setDate(date.getDate() + daysFromNow);
  return date.toISOString().split('T')[0]; // Returns YYYY-MM-DD
}

/**
 * Get a future datetime for reservations
 */
export function getFutureDateTime(daysFromNow: number = 1, hour: number = 18): Date {
  const date = new Date();
  date.setDate(date.getDate() + daysFromNow);
  date.setHours(hour, 0, 0, 0);
  return date;
}
