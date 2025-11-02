import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { apiClient } from './client';
import type { Game, GameFilterRequest, CreateGameRequest, UpdateGameRequest } from '../types/game';

/**
 * Fetch all games with optional filters
 */
export const useGames = (filters?: GameFilterRequest) => {
  return useQuery({
    queryKey: ['games', filters],
    queryFn: async () => {
      const { data } = await apiClient.get<Game[]>('/games', { params: filters });
      return data;
    },
  });
};

/**
 * Fetch a single game by ID
 */
export const useGame = (id: string) => {
  return useQuery({
    queryKey: ['game', id],
    queryFn: async () => {
      const { data } = await apiClient.get<Game>(`/games/${id}`);
      return data;
    },
    enabled: !!id, // Only fetch if id is provided
  });
};

/**
 * Create a new game
 */
export const useCreateGame = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (game: CreateGameRequest) => {
      const { data } = await apiClient.post<Game>('/games', game);
      return data;
    },
    onSuccess: () => {
      // Invalidate and refetch games list
      queryClient.invalidateQueries({ queryKey: ['games'] });
    },
  });
};

/**
 * Update an existing game
 */
export const useUpdateGame = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({ id, game }: { id: string; game: UpdateGameRequest }) => {
      const { data } = await apiClient.put<Game>(`/games/${id}`, game);
      return data;
    },
    onSuccess: (_, variables) => {
      // Invalidate the specific game and the games list
      queryClient.invalidateQueries({ queryKey: ['game', variables.id] });
      queryClient.invalidateQueries({ queryKey: ['games'] });
    },
  });
};

/**
 * Delete a game
 */
export const useDeleteGame = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (id: string) => {
      await apiClient.delete(`/games/${id}`);
    },
    onSuccess: () => {
      // Invalidate games list
      queryClient.invalidateQueries({ queryKey: ['games'] });
    },
  });
};
