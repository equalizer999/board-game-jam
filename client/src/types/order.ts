/**
 * Order DTO matching backend BoardGameCafe.Api.Features.Orders.OrderDto
 */
export interface Order {
  id: string;
  customerId: string;
  reservationId?: string;
  orderDate: string; // ISO datetime string
  status: string;
  subtotal: number;
  discountAmount: number;
  taxAmount: number;
  totalAmount: number;
  paymentMethod: string;
  customerName: string;
  items: OrderItem[];
}

/**
 * Order item DTO
 */
export interface OrderItem {
  id: string;
  menuItemId: string;
  menuItemName: string;
  quantity: number;
  unitPrice: number;
  itemTotal: number;
  specialInstructions?: string;
}

/**
 * Request for creating a new order
 */
export interface CreateOrderRequest {
  customerId: string;
  reservationId?: string;
  items: CreateOrderItemRequest[];
  paymentMethod: string;
}

/**
 * Request for creating an order item
 */
export interface CreateOrderItemRequest {
  menuItemId: string;
  quantity: number;
  specialInstructions?: string;
}

/**
 * Request for updating order status
 */
export interface UpdateOrderStatusRequest {
  status: string;
}

/**
 * Menu Item DTO
 */
export interface MenuItem {
  id: string;
  name: string;
  description?: string;
  category: string;
  price: number;
  isAvailable: boolean;
  preparationTimeMinutes: number;
  allergenInfo?: string;
  isVegetarian: boolean;
  isVegan: boolean;
  isGlutenFree: boolean;
}
