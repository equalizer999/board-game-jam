import React from 'react';
import { Link } from 'react-router-dom';
import { useReservations } from '../api/reservations';

export default function ReservationsPage() {
  // TODO: Replace with actual authenticated user ID
  const customerId = 'customer-1';
  const { data: reservations, isLoading, error } = useReservations(customerId);

  // Format date for display
  const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', {
      weekday: 'short',
      year: 'numeric',
      month: 'short',
      day: 'numeric',
    });
  };

  // Format time for display
  const formatTime = (time: string) => {
    const [hours, minutes] = time.split(':').map(Number);
    const period = hours >= 12 ? 'PM' : 'AM';
    const displayHours = hours > 12 ? hours - 12 : hours === 0 ? 12 : hours;
    return `${displayHours}:${minutes.toString().padStart(2, '0')} ${period}`;
  };

  // Filter upcoming reservations (future dates only)
  const upcomingReservations = reservations?.filter((reservation) => {
    const reservationDateTime = new Date(`${reservation.reservationDate}T${reservation.startTime}`);
    return reservationDateTime >= new Date();
  }) || [];

  return (
    <div
      style={{
        minHeight: 'calc(100vh - 200px)',
        backgroundColor: '#f3f4f6',
        padding: '40px 20px',
      }}
    >
      <div style={{ maxWidth: '1200px', margin: '0 auto' }}>
        {/* Page Header */}
        <div
          style={{
            display: 'flex',
            justifyContent: 'space-between',
            alignItems: 'center',
            marginBottom: '32px',
          }}
        >
          <div>
            <h1
              style={{
                fontSize: '36px',
                fontWeight: 'bold',
                margin: '0 0 8px 0',
                color: '#1f2937',
              }}
            >
              🎲 My Reservations
            </h1>
            <p style={{ fontSize: '16px', color: '#6b7280', margin: 0 }}>
              Manage your table reservations
            </p>
          </div>
          <Link
            to="/reservations/new"
            style={{
              padding: '12px 24px',
              backgroundColor: '#10b981',
              color: '#ffffff',
              border: 'none',
              borderRadius: '6px',
              fontSize: '14px',
              fontWeight: '500',
              textDecoration: 'none',
              display: 'inline-block',
            }}
          >
            + New Reservation
          </Link>
        </div>

        {/* Loading State */}
        {isLoading && (
          <div
            style={{
              textAlign: 'center',
              padding: '60px 20px',
              backgroundColor: '#ffffff',
              borderRadius: '12px',
            }}
          >
            <div style={{ fontSize: '48px', marginBottom: '16px' }}>⏳</div>
            <p style={{ fontSize: '18px', color: '#6b7280' }}>Loading reservations...</p>
          </div>
        )}

        {/* Error State */}
        {error && (
          <div
            style={{
              backgroundColor: '#fee2e2',
              border: '2px solid #ef4444',
              borderRadius: '12px',
              padding: '20px',
              textAlign: 'center',
            }}
          >
            <p style={{ fontSize: '18px', color: '#991b1b', margin: 0 }}>
              ⚠️ Error loading reservations. Please try again later.
            </p>
          </div>
        )}

        {/* No Reservations */}
        {!isLoading && !error && upcomingReservations.length === 0 && (
          <div
            style={{
              textAlign: 'center',
              padding: '60px 20px',
              backgroundColor: '#ffffff',
              borderRadius: '12px',
              border: '2px dashed #d1d5db',
            }}
          >
            <div style={{ fontSize: '64px', marginBottom: '16px' }}>📅</div>
            <h3
              style={{
                fontSize: '24px',
                fontWeight: '600',
                margin: '0 0 8px 0',
                color: '#1f2937',
              }}
            >
              No Upcoming Reservations
            </h3>
            <p style={{ fontSize: '16px', color: '#6b7280', marginBottom: '24px' }}>
              You don't have any upcoming reservations. Book a table to get started!
            </p>
            <Link
              to="/reservations/new"
              style={{
                padding: '12px 24px',
                backgroundColor: '#10b981',
                color: '#ffffff',
                border: 'none',
                borderRadius: '6px',
                fontSize: '14px',
                fontWeight: '500',
                textDecoration: 'none',
                display: 'inline-block',
              }}
            >
              Book Your First Table
            </Link>
          </div>
        )}

        {/* Reservations List */}
        {!isLoading && !error && upcomingReservations.length > 0 && (
          <div style={{ display: 'grid', gap: '16px' }}>
            {upcomingReservations.map((reservation) => (
              <div
                key={reservation.id}
                style={{
                  backgroundColor: '#ffffff',
                  border: '1px solid #e5e7eb',
                  borderRadius: '12px',
                  padding: '24px',
                  display: 'grid',
                  gridTemplateColumns: '1fr auto',
                  gap: '24px',
                  alignItems: 'center',
                }}
              >
                <div>
                  <div
                    style={{
                      fontSize: '20px',
                      fontWeight: '600',
                      color: '#1f2937',
                      marginBottom: '8px',
                    }}
                  >
                    📍 Table {reservation.tableNumber}
                  </div>
                  <div style={{ fontSize: '14px', color: '#6b7280', marginBottom: '4px' }}>
                    📅 {formatDate(reservation.reservationDate)}
                  </div>
                  <div style={{ fontSize: '14px', color: '#6b7280', marginBottom: '4px' }}>
                    🕐 {formatTime(reservation.startTime)} - {formatTime(reservation.endTime)}
                  </div>
                  <div style={{ fontSize: '14px', color: '#6b7280', marginBottom: '4px' }}>
                    👥 Party of {reservation.partySize}
                  </div>
                  {reservation.specialRequests && (
                    <div style={{ fontSize: '14px', color: '#6b7280', marginTop: '8px' }}>
                      💬 {reservation.specialRequests}
                    </div>
                  )}
                </div>
                <div
                  style={{
                    padding: '8px 16px',
                    backgroundColor: '#dcfce7',
                    color: '#166534',
                    borderRadius: '6px',
                    fontSize: '12px',
                    fontWeight: '600',
                    textTransform: 'uppercase',
                  }}
                >
                  {reservation.status}
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}
