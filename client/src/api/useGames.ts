import { useQuery } from '@tanstack/react-query';

// API base URL - in production this would come from environment variables
const API_BASE_URL = 'http://localhost:5001';

export interface GameDto {
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

export interface GameFilters {
  category?: number;
  playerCount?: number;
  minPlayerCount?: number;
  maxPlayerCount?: number;
  minComplexity?: number;
  maxComplexity?: number;
  availableOnly?: boolean;
  search?: string;
  page?: number;
  pageSize?: number;
}

/**
 * Fetch games with optional filters
 */
async function fetchGames(filters?: GameFilters): Promise<GameDto[]> {
  const params = new URLSearchParams();
  
  if (filters?.category !== undefined) {
    params.append('category', filters.category.toString());
  }
  if (filters?.playerCount !== undefined) {
    params.append('playerCount', filters.playerCount.toString());
  }
  if (filters?.minPlayerCount !== undefined) {
    params.append('minPlayerCount', filters.minPlayerCount.toString());
  }
  if (filters?.maxPlayerCount !== undefined) {
    params.append('maxPlayerCount', filters.maxPlayerCount.toString());
  }
  if (filters?.availableOnly !== undefined) {
    params.append('availableOnly', filters.availableOnly.toString());
  }
  if (filters?.page !== undefined) {
    params.append('page', filters.page.toString());
  }
  if (filters?.pageSize !== undefined) {
    params.append('pageSize', filters.pageSize.toString());
  }

  const url = `${API_BASE_URL}/api/v1/games?${params.toString()}`;
  const response = await fetch(url);
  
  if (!response.ok) {
    throw new Error('Failed to fetch games');
  }
  
  let games = await response.json();
  
  // Client-side filtering for search and complexity range
  // (API doesn't support these filters yet)
  if (filters?.search) {
    const searchLower = filters.search.toLowerCase();
    games = games.filter((game: GameDto) => 
      game.title.toLowerCase().includes(searchLower) ||
      game.description?.toLowerCase().includes(searchLower) ||
      game.publisher.toLowerCase().includes(searchLower)
    );
  }
  
  if (filters?.minComplexity !== undefined) {
    games = games.filter((game: GameDto) => game.complexity >= filters.minComplexity!);
  }
  
  if (filters?.maxComplexity !== undefined) {
    games = games.filter((game: GameDto) => game.complexity <= filters.maxComplexity!);
  }
  
  return games;
}

/**
 * Fetch a single game by ID
 */
async function fetchGame(id: string): Promise<GameDto> {
  const response = await fetch(`${API_BASE_URL}/api/v1/games/${id}`);
  
  if (!response.ok) {
    throw new Error('Failed to fetch game');
  }
  
  return response.json();
}

/**
 * React Query hook for fetching games with filters
 */
export function useGames(filters?: GameFilters) {
  return useQuery({
    queryKey: ['games', filters],
    queryFn: () => fetchGames(filters),
    staleTime: 1000 * 60 * 5, // 5 minutes
  });
}

/**
 * React Query hook for fetching a single game
 */
export function useGame(id: string) {
  return useQuery({
    queryKey: ['game', id],
    queryFn: () => fetchGame(id),
    enabled: !!id,
    staleTime: 1000 * 60 * 5, // 5 minutes
  });
}
