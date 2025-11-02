/**
 * Game DTO matching backend BoardGameCafe.Api.Features.Games.GameDto
 */
export interface Game {
  id: string;
  title: string;
  publisher: string;
  minPlayers: number;
  maxPlayers: number;
  playTimeMinutes: number;
  ageRating: number;
  complexity: number;
  category: string;
  copiesOwned: number;
  copiesInUse: number;
  dailyRentalFee: number;
  description?: string;
  imageUrl?: string;
  isAvailable: boolean;
}

/**
 * Request for creating a new game
 */
export interface CreateGameRequest {
  title: string;
  publisher: string;
  minPlayers: number;
  maxPlayers: number;
  playTimeMinutes: number;
  ageRating: number;
  complexity: number;
  category: string;
  copiesOwned: number;
  dailyRentalFee: number;
  description?: string;
  imageUrl?: string;
}

/**
 * Request for updating an existing game
 */
export interface UpdateGameRequest {
  title?: string;
  publisher?: string;
  minPlayers?: number;
  maxPlayers?: number;
  playTimeMinutes?: number;
  ageRating?: number;
  complexity?: number;
  category?: string;
  copiesOwned?: number;
  copiesInUse?: number;
  dailyRentalFee?: number;
  description?: string;
  imageUrl?: string;
}

/**
 * Filter parameters for game queries
 */
export interface GameFilterRequest {
  category?: string;
  minPlayers?: number;
  maxPlayers?: number;
  availableOnly?: boolean;
  page?: number;
  pageSize?: number;
}
