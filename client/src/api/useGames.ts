import { useQuery } from '@tanstack/react-query';

// API base URL - in production this would come from environment variables
const API_BASE_URL = 'http://localhost:5001';

// Mock data for development when backend is unavailable
const MOCK_GAMES: GameDto[] = [
  {
    id: '1',
    title: 'Catan',
    publisher: 'Catan Studio',
    minPlayers: 3,
    maxPlayers: 4,
    playTimeMinutes: 90,
    ageRating: 10,
    complexity: 2.5,
    category: 'Strategy',
    copiesOwned: 3,
    copiesInUse: 0,
    dailyRentalFee: 5.0,
    description:
      'Collect resources and build settlements on the island of Catan. Trade with other players and expand your territory to become the dominant settler.',
    imageUrl: '',
    isAvailable: true,
  },
  {
    id: '2',
    title: 'Ticket to Ride',
    publisher: 'Days of Wonder',
    minPlayers: 2,
    maxPlayers: 5,
    playTimeMinutes: 60,
    ageRating: 8,
    complexity: 1.8,
    category: 'Strategy',
    copiesOwned: 2,
    copiesInUse: 1,
    dailyRentalFee: 4.5,
    description:
      'Build train routes across North America in this classic board game. Collect cards and claim routes to complete destination tickets.',
    imageUrl: '',
    isAvailable: true,
  },
  {
    id: '3',
    title: 'Pandemic',
    publisher: 'Z-Man Games',
    minPlayers: 2,
    maxPlayers: 4,
    playTimeMinutes: 45,
    ageRating: 8,
    complexity: 2.4,
    category: 'Cooperative',
    copiesOwned: 2,
    copiesInUse: 0,
    dailyRentalFee: 5.0,
    description:
      'Work together to save humanity from global disease outbreaks. Each player has a unique role with special abilities to help cure diseases.',
    imageUrl: '',
    isAvailable: true,
  },
  {
    id: '4',
    title: 'Codenames',
    publisher: 'Czech Games Edition',
    minPlayers: 4,
    maxPlayers: 8,
    playTimeMinutes: 15,
    ageRating: 14,
    complexity: 1.3,
    category: 'Party',
    copiesOwned: 2,
    copiesInUse: 0,
    dailyRentalFee: 3.0,
    description:
      'Team word-guessing game with spies and secret agents. Give one-word clues to help your team identify agents while avoiding the assassin.',
    imageUrl: '',
    isAvailable: true,
  },
  {
    id: '5',
    title: 'Azul',
    publisher: 'Plan B Games',
    minPlayers: 2,
    maxPlayers: 4,
    playTimeMinutes: 45,
    ageRating: 8,
    complexity: 1.8,
    category: 'Abstract',
    copiesOwned: 1,
    copiesInUse: 0,
    dailyRentalFee: 4.0,
    description:
      'Draft colorful tiles to decorate the palace walls. Create beautiful patterns while managing your resources efficiently.',
    imageUrl: '',
    isAvailable: true,
  },
  {
    id: '6',
    title: 'Wingspan',
    publisher: 'Stonemaier Games',
    minPlayers: 1,
    maxPlayers: 5,
    playTimeMinutes: 70,
    ageRating: 10,
    complexity: 2.4,
    category: 'Strategy',
    copiesOwned: 2,
    copiesInUse: 2,
    dailyRentalFee: 6.0,
    description:
      'Build a diverse collection of birds in your wildlife preserves. Each bird has unique abilities that combo with others.',
    imageUrl: '',
    isAvailable: false,
  },
  {
    id: '7',
    title: 'Exploding Kittens',
    publisher: 'Exploding Kittens',
    minPlayers: 2,
    maxPlayers: 5,
    playTimeMinutes: 15,
    ageRating: 7,
    complexity: 1.1,
    category: 'Party',
    copiesOwned: 3,
    copiesInUse: 1,
    dailyRentalFee: 2.5,
    description:
      'A strategic card game about kittens, explosions, and sometimes goats. Draw cards until someone explodes.',
    imageUrl: '',
    isAvailable: true,
  },
  {
    id: '8',
    title: '7 Wonders',
    publisher: 'Repos Production',
    minPlayers: 2,
    maxPlayers: 7,
    playTimeMinutes: 30,
    ageRating: 10,
    complexity: 2.3,
    category: 'Strategy',
    copiesOwned: 2,
    copiesInUse: 0,
    dailyRentalFee: 5.5,
    description:
      'Build your civilization through three ages. Draft cards to develop your city and achieve victory.',
    imageUrl: '',
    isAvailable: true,
  },
  {
    id: '9',
    title: 'Splendor',
    publisher: 'Space Cowboys',
    minPlayers: 2,
    maxPlayers: 4,
    playTimeMinutes: 30,
    ageRating: 10,
    complexity: 1.8,
    category: 'Strategy',
    copiesOwned: 2,
    copiesInUse: 0,
    dailyRentalFee: 4.5,
    description:
      'Collect gems and develop your gem trade routes. Build prestige and become the most renowned jeweler.',
    imageUrl: '',
    isAvailable: true,
  },
  {
    id: '10',
    title: 'Dixit',
    publisher: 'Libellud',
    minPlayers: 3,
    maxPlayers: 6,
    playTimeMinutes: 30,
    ageRating: 8,
    complexity: 1.2,
    category: 'Party',
    copiesOwned: 1,
    copiesInUse: 0,
    dailyRentalFee: 3.5,
    description:
      "Use beautiful, dreamlike illustrations to tell stories. Guess which card matches the storyteller's clue.",
    imageUrl: '',
    isAvailable: true,
  },
  {
    id: '11',
    title: 'Carcassonne',
    publisher: 'Z-Man Games',
    minPlayers: 2,
    maxPlayers: 5,
    playTimeMinutes: 45,
    ageRating: 7,
    complexity: 1.9,
    category: 'Strategy',
    copiesOwned: 2,
    copiesInUse: 0,
    dailyRentalFee: 4.0,
    description:
      'Build the medieval landscape of Carcassonne tile by tile. Place followers to score points on roads, cities, and fields.',
    imageUrl: '',
    isAvailable: true,
  },
  {
    id: '12',
    title: 'Forbidden Island',
    publisher: 'Gamewright',
    minPlayers: 2,
    maxPlayers: 4,
    playTimeMinutes: 30,
    ageRating: 10,
    complexity: 1.7,
    category: 'Cooperative',
    copiesOwned: 1,
    copiesInUse: 0,
    dailyRentalFee: 3.5,
    description:
      'Work together to collect treasures before the island sinks. Strategic cooperation is key to survival.',
    imageUrl: '',
    isAvailable: true,
  },
  {
    id: '13',
    title: 'King of Tokyo',
    publisher: 'IELLO',
    minPlayers: 2,
    maxPlayers: 6,
    playTimeMinutes: 30,
    ageRating: 8,
    complexity: 1.5,
    category: 'Family',
    copiesOwned: 2,
    copiesInUse: 0,
    dailyRentalFee: 4.0,
    description:
      'Become a giant monster and fight for control of Tokyo. Roll dice, gain energy, and evolve with powerful mutations.',
    imageUrl: '',
    isAvailable: true,
  },
  {
    id: '14',
    title: 'Sushi Go!',
    publisher: 'Gamewright',
    minPlayers: 2,
    maxPlayers: 5,
    playTimeMinutes: 15,
    ageRating: 8,
    complexity: 1.2,
    category: 'Family',
    copiesOwned: 2,
    copiesInUse: 1,
    dailyRentalFee: 2.5,
    description:
      'Pass sushi cards and collect the best combinations. Quick and fun card drafting for the whole family.',
    imageUrl: '',
    isAvailable: true,
  },
  {
    id: '15',
    title: 'Blokus',
    publisher: 'Mattel',
    minPlayers: 2,
    maxPlayers: 4,
    playTimeMinutes: 20,
    ageRating: 7,
    complexity: 1.6,
    category: 'Abstract',
    copiesOwned: 1,
    copiesInUse: 0,
    dailyRentalFee: 3.0,
    description:
      'Place your colored pieces on the board, each touching another of your pieces at the corners only.',
    imageUrl: '',
    isAvailable: true,
  },
];

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
  try {
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
      games = games.filter(
        (game: GameDto) =>
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
  } catch (error) {
    // Fallback to mock data if backend is unavailable
    console.warn('Backend unavailable, using mock data:', error);
    let games = [...MOCK_GAMES];

    // Apply filters to mock data
    if (filters?.category !== undefined) {
      const categoryNames = ['Strategy', 'Party', 'Family', 'Cooperative', 'Abstract'];
      const categoryName = categoryNames[filters.category];
      games = games.filter((g) => g.category === categoryName);
    }

    if (filters?.minPlayerCount !== undefined) {
      games = games.filter((g) => g.maxPlayers >= filters.minPlayerCount!);
    }

    if (filters?.maxPlayerCount !== undefined) {
      games = games.filter((g) => g.minPlayers <= filters.maxPlayerCount!);
    }

    if (filters?.minComplexity !== undefined) {
      games = games.filter((g) => g.complexity >= filters.minComplexity!);
    }

    if (filters?.maxComplexity !== undefined) {
      games = games.filter((g) => g.complexity <= filters.maxComplexity!);
    }

    if (filters?.search) {
      const searchLower = filters.search.toLowerCase();
      games = games.filter(
        (g) =>
          g.title.toLowerCase().includes(searchLower) ||
          g.description?.toLowerCase().includes(searchLower) ||
          g.publisher.toLowerCase().includes(searchLower)
      );
    }

    if (filters?.availableOnly) {
      games = games.filter((g) => g.isAvailable);
    }

    return games;
  }
}

/**
 * Fetch a single game by ID
 */
async function fetchGame(id: string): Promise<GameDto> {
  try {
    const response = await fetch(`${API_BASE_URL}/api/v1/games/${id}`);

    if (!response.ok) {
      throw new Error('Failed to fetch game');
    }

    return response.json();
  } catch (error) {
    // Fallback to mock data if backend is unavailable
    console.warn('Backend unavailable, using mock data:', error);
    const game = MOCK_GAMES.find((g) => g.id === id);
    if (!game) {
      throw new Error('Game not found');
    }
    return game;
  }
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
