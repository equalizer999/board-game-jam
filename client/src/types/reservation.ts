/**
 * Reservation DTO matching backend BoardGameCafe.Api.Features.Reservations.ReservationDto
 */
export interface Reservation {
  id: string;
  customerId: string;
  tableId: string;
  reservationDate: string; // ISO date string
  startTime: string; // TimeSpan as string (e.g., "18:00:00")
  endTime: string; // TimeSpan as string (e.g., "20:00:00")
  partySize: number;
  status: string;
  createdAt: string; // ISO datetime string
  specialRequests?: string;
  tableNumber: string;
  customerName: string;
}

/**
 * Request for creating a new reservation
 */
export interface CreateReservationRequest {
  customerId: string;
  tableId: string;
  reservationDate: string; // ISO date string
  startTime: string; // TimeSpan as string
  endTime: string; // TimeSpan as string
  partySize: number;
  specialRequests?: string;
}

/**
 * Request for updating an existing reservation
 */
export interface UpdateReservationRequest {
  tableId?: string;
  reservationDate?: string;
  startTime?: string;
  endTime?: string;
  partySize?: number;
  specialRequests?: string;
}

/**
 * Query for checking table availability
 */
export interface AvailabilityQuery {
  reservationDate: string; // ISO date string
  startTime: string; // TimeSpan as string
  endTime: string; // TimeSpan as string
  partySize: number;
}

/**
 * Available table information
 */
export interface AvailableTable {
  id: string;
  tableNumber: string;
  seatingCapacity: number;
  isWindowSeat: boolean;
  isAccessible: boolean;
  hourlyRate: number;
}
