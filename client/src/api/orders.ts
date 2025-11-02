import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { apiClient } from './client';
import type {
  Order,
  MenuItem,
  CreateOrderRequest,
  UpdateOrderStatusRequest,
} from '../types/order';

/**
 * Fetch customer's orders
 */
export const useOrders = (customerId?: string) => {
  return useQuery({
    queryKey: ['orders', customerId],
    queryFn: async () => {
      const { data } = await apiClient.get<Order[]>('/orders', {
        params: { customerId },
      });
      return data;
    },
    enabled: !!customerId,
  });
};

/**
 * Fetch a single order by ID
 */
export const useOrder = (id: string) => {
  return useQuery({
    queryKey: ['order', id],
    queryFn: async () => {
      const { data } = await apiClient.get<Order>(`/orders/${id}`);
      return data;
    },
    enabled: !!id,
  });
};

/**
 * Fetch menu items
 */
export const useMenuItems = () => {
  return useQuery({
    queryKey: ['menu'],
    queryFn: async () => {
      const { data } = await apiClient.get<MenuItem[]>('/menu');
      return data;
    },
  });
};

/**
 * Create a new order
 */
export const useCreateOrder = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (order: CreateOrderRequest) => {
      const { data } = await apiClient.post<Order>('/orders', order);
      return data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['orders'] });
    },
  });
};

/**
 * Update order status
 */
export const useUpdateOrderStatus = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({ id, status }: { id: string; status: UpdateOrderStatusRequest }) => {
      const { data } = await apiClient.put<Order>(`/orders/${id}/status`, status);
      return data;
    },
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({ queryKey: ['order', variables.id] });
      queryClient.invalidateQueries({ queryKey: ['orders'] });
    },
  });
};
