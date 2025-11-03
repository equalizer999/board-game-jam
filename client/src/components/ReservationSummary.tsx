import React from 'react';
import { AvailableTable } from '../types/reservation';

interface ReservationSummaryProps {
  selectedTable: AvailableTable | null;
  reservationDate: string;
  startTime: string;
  endTime: string;
  partySize: number;
  specialRequests: string;
  onConfirm: () => void;
  onCancel: () => void;
  isSubmitting?: boolean;
}

/**
 * ReservationSummary component displays reservation details for confirmation
 */
export function ReservationSummary({
  selectedTable,
  reservationDate,
  startTime,
  endTime,
  partySize,
  specialRequests,
  onConfirm,
  onCancel,
  isSubmitting,
}: ReservationSummaryProps) {
  // Format date for display
  const formatDate = (dateString: string) => {
    if (!dateString) return '';
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
      weekday: 'long',
      year: 'numeric',
      month: 'long',
      day: 'numeric',
    });
  };

  // Format time for display
  const formatTime = (time: string) => {
    if (!time) return '';
    const [hours, minutes] = time.split(':').map(Number);
    const period = hours >= 12 ? 'PM' : 'AM';
    const displayHours = hours > 12 ? hours - 12 : hours === 0 ? 12 : hours;
    return `${displayHours}:${minutes.toString().padStart(2, '0')} ${period}`;
  };

  // Calculate duration in hours
  const calculateDuration = () => {
    if (!startTime || !endTime) return 0;
    const [startHours, startMinutes] = startTime.split(':').map(Number);
    const [endHours, endMinutes] = endTime.split(':').map(Number);
    const startTotalMinutes = startHours * 60 + startMinutes;
    const endTotalMinutes = endHours * 60 + endMinutes;
    const durationMinutes = endTotalMinutes - startTotalMinutes;
    return durationMinutes / 60;
  };

  const duration = calculateDuration();

  // Calculate total price
  const calculateTotalPrice = () => {
    if (!selectedTable || duration <= 0) return 0;
    const basePrice = selectedTable.hourlyRate * duration;

    // Peak hours multiplier (6 PM - 9 PM)
    const isPeakHours = () => {
      const [hours] = startTime.split(':').map(Number);
      return hours >= 18 && hours < 21;
    };

    const multiplier = isPeakHours() ? 1.5 : 1;
    return basePrice * multiplier;
  };

  const totalPrice = calculateTotalPrice();
  const isPeakHours = () => {
    if (!startTime) return false;
    const [hours] = startTime.split(':').map(Number);
    return hours >= 18 && hours < 21;
  };

  return (
    <div
      style={{
        backgroundColor: '#ffffff',
        border: '1px solid #e5e7eb',
        borderRadius: '12px',
        padding: '24px',
      }}
    >
      <h3 style={{ fontSize: '20px', fontWeight: '600', marginBottom: '20px', color: '#1f2937' }}>
        Reservation Summary
      </h3>

      <div
        style={{
          backgroundColor: '#f9fafb',
          borderRadius: '8px',
          padding: '20px',
          marginBottom: '24px',
        }}
      >
        {/* Table Information */}
        {selectedTable && (
          <div style={{ marginBottom: '16px' }}>
            <div
              style={{
                fontSize: '16px',
                fontWeight: '600',
                color: '#1f2937',
                marginBottom: '8px',
              }}
            >
              📍 Table {selectedTable.tableNumber}
            </div>
            <div style={{ fontSize: '14px', color: '#6b7280', marginBottom: '4px' }}>
              👥 Seats up to {selectedTable.seatingCapacity} people
            </div>
            <div style={{ fontSize: '14px', color: '#6b7280', marginBottom: '8px' }}>
              {selectedTable.isWindowSeat && '🪟 Window Seat • '}
              {selectedTable.isAccessible && '♿ Accessible'}
            </div>
          </div>
        )}

        {/* Date & Time */}
        <div style={{ marginBottom: '16px' }}>
          <div
            style={{
              fontSize: '16px',
              fontWeight: '600',
              color: '#1f2937',
              marginBottom: '8px',
            }}
          >
            📅 {formatDate(reservationDate)}
          </div>
          <div style={{ fontSize: '14px', color: '#6b7280' }}>
            🕐 {formatTime(startTime)} - {formatTime(endTime)}
          </div>
          <div style={{ fontSize: '14px', color: '#6b7280' }}>
            ⏱️ Duration: {duration.toFixed(1)} hour{duration !== 1 ? 's' : ''}
          </div>
        </div>

        {/* Party Size */}
        <div style={{ marginBottom: '16px' }}>
          <div style={{ fontSize: '14px', color: '#6b7280' }}>
            👥 Party size: {partySize} {partySize === 1 ? 'person' : 'people'}
          </div>
        </div>

        {/* Special Requests */}
        {specialRequests && (
          <div style={{ marginBottom: '16px' }}>
            <div
              style={{
                fontSize: '14px',
                fontWeight: '500',
                color: '#374151',
                marginBottom: '4px',
              }}
            >
              💬 Special Requests:
            </div>
            <div
              style={{
                fontSize: '14px',
                color: '#6b7280',
                backgroundColor: '#ffffff',
                padding: '12px',
                borderRadius: '6px',
                border: '1px solid #e5e7eb',
              }}
            >
              {specialRequests}
            </div>
          </div>
        )}

        {/* Pricing */}
        <div
          style={{
            borderTop: '2px solid #e5e7eb',
            paddingTop: '16px',
            marginTop: '16px',
          }}
        >
          <div
            style={{
              fontSize: '14px',
              color: '#6b7280',
              marginBottom: '4px',
              display: 'flex',
              justifyContent: 'space-between',
            }}
          >
            <span>
              Base rate ({duration.toFixed(1)} hr × ${selectedTable?.hourlyRate}/hr):
            </span>
            <span>${(selectedTable ? selectedTable.hourlyRate * duration : 0).toFixed(2)}</span>
          </div>

          {isPeakHours() && (
            <div
              style={{
                fontSize: '14px',
                color: '#dc2626',
                marginBottom: '4px',
                display: 'flex',
                justifyContent: 'space-between',
              }}
            >
              <span>Peak hours multiplier (6 PM - 9 PM):</span>
              <span>×1.5</span>
            </div>
          )}

          <div
            style={{
              fontSize: '18px',
              fontWeight: '700',
              color: '#1f2937',
              marginTop: '8px',
              display: 'flex',
              justifyContent: 'space-between',
            }}
          >
            <span>Total:</span>
            <span>${totalPrice.toFixed(2)}</span>
          </div>
        </div>
      </div>

      {/* Action Buttons */}
      <div style={{ display: 'flex', gap: '12px', justifyContent: 'flex-end' }}>
        <button
          onClick={onCancel}
          disabled={isSubmitting}
          style={{
            padding: '12px 24px',
            backgroundColor: '#ffffff',
            color: '#374151',
            border: '1px solid #d1d5db',
            borderRadius: '6px',
            fontSize: '14px',
            fontWeight: '500',
            cursor: isSubmitting ? 'not-allowed' : 'pointer',
            opacity: isSubmitting ? 0.5 : 1,
          }}
        >
          Cancel
        </button>
        <button
          onClick={onConfirm}
          disabled={isSubmitting}
          style={{
            padding: '12px 24px',
            backgroundColor: '#10b981',
            color: '#ffffff',
            border: 'none',
            borderRadius: '6px',
            fontSize: '14px',
            fontWeight: '500',
            cursor: isSubmitting ? 'not-allowed' : 'pointer',
            opacity: isSubmitting ? 0.5 : 1,
          }}
        >
          {isSubmitting ? 'Confirming...' : 'Confirm Reservation'}
        </button>
      </div>
    </div>
  );
}
