import React from 'react';
import { AvailableTable } from '../types/reservation';

interface TableSelectorProps {
  availableTables: AvailableTable[];
  selectedTableId: string;
  onSelectTable: (tableId: string) => void;
  partySize: number;
  isLoading?: boolean;
}

/**
 * TableSelector component displays a visual layout of café tables
 * Color-coded: green (available & matches party size), blue (available), gray (not suitable)
 */
export function TableSelector({
  availableTables,
  selectedTableId,
  onSelectTable,
  partySize,
  isLoading,
}: TableSelectorProps) {
  if (isLoading) {
    return (
      <div
        style={{
          backgroundColor: '#ffffff',
          border: '1px solid #e5e7eb',
          borderRadius: '12px',
          padding: '24px',
          textAlign: 'center',
        }}
      >
        <div style={{ fontSize: '48px', marginBottom: '16px' }}>⏳</div>
        <p style={{ fontSize: '16px', color: '#6b7280' }}>Loading available tables...</p>
      </div>
    );
  }

  if (!availableTables || availableTables.length === 0) {
    return (
      <div
        style={{
          backgroundColor: '#ffffff',
          border: '1px solid #e5e7eb',
          borderRadius: '12px',
          padding: '24px',
          textAlign: 'center',
        }}
      >
        <div style={{ fontSize: '48px', marginBottom: '16px' }}>🚫</div>
        <h3 style={{ fontSize: '20px', fontWeight: '600', marginBottom: '8px', color: '#1f2937' }}>
          No Tables Available
        </h3>
        <p style={{ fontSize: '14px', color: '#6b7280' }}>
          Please select a different date or time.
        </p>
      </div>
    );
  }

  // Helper function to determine table color
  const getTableColor = (table: AvailableTable) => {
    if (selectedTableId === table.id) {
      return '#3b82f6'; // Blue for selected
    }
    if (table.seatingCapacity >= partySize) {
      return '#10b981'; // Green for suitable and available
    }
    return '#9ca3af'; // Gray for not suitable
  };

  // Helper function to get border color
  const getBorderColor = (table: AvailableTable) => {
    if (selectedTableId === table.id) {
      return '#1d4ed8'; // Dark blue for selected
    }
    if (table.seatingCapacity >= partySize) {
      return '#059669'; // Dark green for suitable
    }
    return '#6b7280'; // Dark gray for not suitable
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
      <h3 style={{ fontSize: '20px', fontWeight: '600', marginBottom: '16px', color: '#1f2937' }}>
        Select a Table
      </h3>

      {/* Legend */}
      <div
        style={{
          display: 'flex',
          gap: '16px',
          marginBottom: '24px',
          flexWrap: 'wrap',
          fontSize: '12px',
          color: '#374151',
        }}
      >
        <div style={{ display: 'flex', alignItems: 'center', gap: '6px' }}>
          <div
            style={{
              width: '16px',
              height: '16px',
              backgroundColor: '#10b981',
              borderRadius: '4px',
            }}
          />
          <span>Suitable for party</span>
        </div>
        <div style={{ display: 'flex', alignItems: 'center', gap: '6px' }}>
          <div
            style={{
              width: '16px',
              height: '16px',
              backgroundColor: '#3b82f6',
              borderRadius: '4px',
            }}
          />
          <span>Selected</span>
        </div>
        <div style={{ display: 'flex', alignItems: 'center', gap: '6px' }}>
          <div
            style={{
              width: '16px',
              height: '16px',
              backgroundColor: '#9ca3af',
              borderRadius: '4px',
            }}
          />
          <span>Too small</span>
        </div>
      </div>

      {/* Table Grid */}
      <div
        style={{
          display: 'grid',
          gridTemplateColumns: 'repeat(auto-fill, minmax(140px, 1fr))',
          gap: '16px',
        }}
      >
        {availableTables.map((table) => {
          const isSuitable = table.seatingCapacity >= partySize;
          const isSelected = selectedTableId === table.id;

          return (
            <button
              key={table.id}
              data-testid={`table-${table.tableNumber}`}
              onClick={() => onSelectTable(table.id)}
              disabled={!isSuitable}
              style={{
                backgroundColor: getTableColor(table),
                border: `2px solid ${getBorderColor(table)}`,
                borderRadius: '8px',
                padding: '16px',
                cursor: isSuitable ? 'pointer' : 'not-allowed',
                opacity: isSuitable ? 1 : 0.5,
                transition: 'transform 0.2s, box-shadow 0.2s',
                color: '#ffffff',
                fontWeight: '500',
                textAlign: 'left',
              }}
              onMouseEnter={(e) => {
                if (isSuitable && !isSelected) {
                  e.currentTarget.style.transform = 'scale(1.05)';
                  e.currentTarget.style.boxShadow = '0 4px 12px rgba(0, 0, 0, 0.15)';
                }
              }}
              onMouseLeave={(e) => {
                if (isSuitable && !isSelected) {
                  e.currentTarget.style.transform = 'scale(1)';
                  e.currentTarget.style.boxShadow = 'none';
                }
              }}
            >
              {/* Table Number */}
              <div style={{ fontSize: '18px', fontWeight: 'bold', marginBottom: '8px' }}>
                Table {table.tableNumber}
              </div>

              {/* Capacity */}
              <div style={{ fontSize: '12px', marginBottom: '4px' }}>
                👥 Seats {table.seatingCapacity}
              </div>

              {/* Hourly Rate */}
              <div style={{ fontSize: '12px', marginBottom: '8px' }}>
                💰 ${table.hourlyRate}/hr
              </div>

              {/* Amenities */}
              <div style={{ fontSize: '11px', display: 'flex', flexWrap: 'wrap', gap: '4px' }}>
                {table.isWindowSeat && (
                  <span
                    style={{
                      backgroundColor: 'rgba(255, 255, 255, 0.2)',
                      padding: '2px 6px',
                      borderRadius: '4px',
                    }}
                  >
                    🪟 Window
                  </span>
                )}
                {table.isAccessible && (
                  <span
                    style={{
                      backgroundColor: 'rgba(255, 255, 255, 0.2)',
                      padding: '2px 6px',
                      borderRadius: '4px',
                    }}
                  >
                    ♿ Accessible
                  </span>
                )}
              </div>

              {/* Selected Indicator */}
              {isSelected && (
                <div
                  style={{
                    marginTop: '8px',
                    fontSize: '12px',
                    fontWeight: 'bold',
                    textAlign: 'center',
                  }}
                >
                  ✓ SELECTED
                </div>
              )}
            </button>
          );
        })}
      </div>
    </div>
  );
}
