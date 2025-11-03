import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { apiClient } from './client';
import type {
  Reservation,
  CreateReservationRequest,
  UpdateReservationRequest,
  AvailabilityQuery,
  AvailableTable,
} from '../types/reservation';

/**
 * Fetch customer's reservations
 */
export const useReservations = (customerId?: string) => {
  return useQuery({
    queryKey: ['reservations', customerId],
    queryFn: async () => {
      const { data } = await apiClient.get<Reservation[]>('/reservations', {
        params: { customerId },
      });
      return data;
    },
    enabled: !!customerId,
  });
};

/**
 * Fetch a single reservation by ID
 */
export const useReservation = (id: string) => {
  return useQuery({
    queryKey: ['reservation', id],
    queryFn: async () => {
      const { data } = await apiClient.get<Reservation>(`/reservations/${id}`);
      return data;
    },
    enabled: !!id,
  });
};

/**
 * Check table availability
 */
export const useTableAvailability = (query: AvailabilityQuery) => {
  return useQuery({
    queryKey: ['availability', query],
    queryFn: async () => {
      const { data } = await apiClient.get<AvailableTable[]>('/reservations/availability', {
        params: query,
      });
      return data;
    },
    enabled: !!(query.reservationDate && query.startTime && query.endTime),
  });
};

/**
 * Create a new reservation
 */
export const useCreateReservation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (reservation: CreateReservationRequest) => {
      const { data } = await apiClient.post<Reservation>('/reservations', reservation);
      return data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['reservations'] });
      queryClient.invalidateQueries({ queryKey: ['availability'] });
    },
  });
};

/**
 * Update an existing reservation
 */
export const useUpdateReservation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({
      id,
      reservation,
    }: {
      id: string;
      reservation: UpdateReservationRequest;
    }) => {
      const { data } = await apiClient.put<Reservation>(`/reservations/${id}`, reservation);
      return data;
    },
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ['reservation', variables.id] });
      queryClient.invalidateQueries({ queryKey: ['reservations'] });
      queryClient.invalidateQueries({ queryKey: ['availability'] });
    },
  });
};

/**
 * Cancel a reservation
 */
export const useCancelReservation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (id: string) => {
      await apiClient.delete(`/reservations/${id}`);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['reservations'] });
      queryClient.invalidateQueries({ queryKey: ['availability'] });
    },
  });
};

/**
 * Check in to a reservation
 */
export const useCheckInReservation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (id: string) => {
      const { data } = await apiClient.post<Reservation>(`/reservations/${id}/check-in`);
      return data;
    },
    onSuccess: (_, id) => {
      queryClient.invalidateQueries({ queryKey: ['reservation', id] });
      queryClient.invalidateQueries({ queryKey: ['reservations'] });
    },
  });
};
