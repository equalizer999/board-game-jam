import React from 'react';

interface DateTimePickerProps {
  selectedDate: string;
  startTime: string;
  endTime: string;
  partySize: number;
  onDateChange: (date: string) => void;
  onStartTimeChange: (time: string) => void;
  onEndTimeChange: (time: string) => void;
  onPartySizeChange: (size: number) => void;
}

/**
 * DateTimePicker component for selecting reservation date, time range, and party size
 */
export function DateTimePicker({
  selectedDate,
  startTime,
  endTime,
  partySize,
  onDateChange,
  onStartTimeChange,
  onEndTimeChange,
  onPartySizeChange,
}: DateTimePickerProps) {
  // Generate time slots from 10:00 AM to 11:00 PM in 30-minute increments
  const generateTimeSlots = () => {
    const slots: string[] = [];
    for (let hour = 10; hour <= 23; hour++) {
      for (let minute = 0; minute < 60; minute += 30) {
        const timeString = `${hour.toString().padStart(2, '0')}:${minute.toString().padStart(2, '0')}:00`;
        slots.push(timeString);
      }
    }
    return slots;
  };

  const timeSlots = generateTimeSlots();

  // Format time for display (HH:MM AM/PM)
  const formatTimeForDisplay = (time: string) => {
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

  // Get today's date in YYYY-MM-DD format
  const today = new Date().toISOString().split('T')[0];

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
        Select Date & Time
      </h3>

      <div style={{ display: 'grid', gap: '20px' }}>
        {/* Date Picker */}
        <div>
          <label
            htmlFor="reservation-date"
            style={{
              display: 'block',
              fontSize: '14px',
              fontWeight: '500',
              marginBottom: '8px',
              color: '#374151',
            }}
          >
            Date *
          </label>
          <input
            id="reservation-date"
            data-testid="reservation-date"
            type="date"
            value={selectedDate}
            onChange={(e) => onDateChange(e.target.value)}
            min={today}
            required
            style={{
              width: '100%',
              padding: '10px 12px',
              border: '1px solid #d1d5db',
              borderRadius: '6px',
              fontSize: '14px',
            }}
          />
        </div>

        {/* Party Size */}
        <div>
          <label
            htmlFor="party-size"
            style={{
              display: 'block',
              fontSize: '14px',
              fontWeight: '500',
              marginBottom: '8px',
              color: '#374151',
            }}
          >
            Party Size *
          </label>
          <input
            id="party-size"
            data-testid="party-size"
            type="number"
            value={partySize}
            onChange={(e) => onPartySizeChange(parseInt(e.target.value, 10) || 1)}
            min={1}
            max={20}
            required
            style={{
              width: '100%',
              padding: '10px 12px',
              border: '1px solid #d1d5db',
              borderRadius: '6px',
              fontSize: '14px',
            }}
          />
        </div>

        {/* Start Time */}
        <div>
          <label
            htmlFor="start-time"
            style={{
              display: 'block',
              fontSize: '14px',
              fontWeight: '500',
              marginBottom: '8px',
              color: '#374151',
            }}
          >
            Start Time *
          </label>
          <select
            id="start-time"
            data-testid="start-time"
            value={startTime}
            onChange={(e) => onStartTimeChange(e.target.value)}
            required
            style={{
              width: '100%',
              padding: '10px 12px',
              border: '1px solid #d1d5db',
              borderRadius: '6px',
              fontSize: '14px',
              backgroundColor: '#ffffff',
            }}
          >
            <option value="">Select time</option>
            {timeSlots.map((slot) => (
              <option key={slot} value={slot}>
                {formatTimeForDisplay(slot)}
              </option>
            ))}
          </select>
        </div>

        {/* End Time */}
        <div>
          <label
            htmlFor="end-time"
            style={{
              display: 'block',
              fontSize: '14px',
              fontWeight: '500',
              marginBottom: '8px',
              color: '#374151',
            }}
          >
            End Time *
          </label>
          <select
            id="end-time"
            data-testid="end-time"
            value={endTime}
            onChange={(e) => onEndTimeChange(e.target.value)}
            required
            style={{
              width: '100%',
              padding: '10px 12px',
              border: '1px solid #d1d5db',
              borderRadius: '6px',
              fontSize: '14px',
              backgroundColor: '#ffffff',
            }}
          >
            <option value="">Select time</option>
            {timeSlots.map((slot) => (
              <option key={slot} value={slot}>
                {formatTimeForDisplay(slot)}
              </option>
            ))}
          </select>
        </div>

        {/* Duration Display */}
        {duration > 0 && (
          <div
            style={{
              backgroundColor: '#f3f4f6',
              padding: '12px',
              borderRadius: '6px',
              fontSize: '14px',
              color: '#374151',
            }}
          >
            <strong>Duration:</strong> {duration.toFixed(1)} hour{duration !== 1 ? 's' : ''}
          </div>
        )}

        {/* Validation Message */}
        {startTime && endTime && duration <= 0 && (
          <div
            style={{
              backgroundColor: '#fee2e2',
              padding: '12px',
              borderRadius: '6px',
              fontSize: '14px',
              color: '#991b1b',
            }}
          >
            ⚠️ End time must be after start time
          </div>
        )}
      </div>
    </div>
  );
}
