import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { DateTimePicker } from '../components/DateTimePicker';
import { TableSelector } from '../components/TableSelector';
import { ReservationSummary } from '../components/ReservationSummary';
import {
  useTableAvailability,
  useCreateReservation,
} from '../api/reservations';
import { CreateReservationRequest } from '../types/reservation';

interface ReservationFormData {
  customerId: string;
  tableId: string;
  reservationDate: string;
  startTime: string;
  endTime: string;
  partySize: number;
  specialRequests: string;
}

/**
 * NewReservation page - Multi-step wizard for creating table reservations
 */
export default function NewReservation() {
  const navigate = useNavigate();
  const [currentStep, setCurrentStep] = useState(1);
  const [showSuccess, setShowSuccess] = useState(false);

  const {
    register,
    watch,
    setValue,
    handleSubmit,
  } = useForm<ReservationFormData>({
    defaultValues: {
      customerId: 'customer-1', // TODO: Replace with actual authenticated user ID
      tableId: '',
      reservationDate: '',
      startTime: '',
      endTime: '',
      partySize: 2,
      specialRequests: '',
    },
  });

  // Watch form values
  const reservationDate = watch('reservationDate');
  const startTime = watch('startTime');
  const endTime = watch('endTime');
  const partySize = watch('partySize');
  const selectedTableId = watch('tableId');
  const specialRequests = watch('specialRequests');

  // Fetch available tables when date/time changes
  const { data: availableTables, isLoading: isLoadingTables } = useTableAvailability({
    reservationDate,
    startTime,
    endTime,
    partySize,
  });

  // Create reservation mutation
  const createReservation = useCreateReservation();

  // Find selected table from available tables
  const selectedTable = availableTables?.find(
    (table) => table.id === selectedTableId
  ) || null;

  // Update tableId when moving to step 3 if not already selected
  useEffect(() => {
    if (currentStep === 3 && availableTables && availableTables.length > 0 && !selectedTableId) {
      // Auto-select the first suitable table
      const suitableTable = availableTables.find(t => t.seatingCapacity >= partySize);
      if (suitableTable) {
        setValue('tableId', suitableTable.id);
      }
    }
  }, [currentStep, availableTables, selectedTableId, partySize, setValue]);

  const handleNext = () => {
    // Validation for each step
    if (currentStep === 1) {
      if (!reservationDate || !partySize) {
        alert('Please select a date and party size');
        return;
      }
    } else if (currentStep === 2) {
      if (!startTime || !endTime) {
        alert('Please select start and end times');
        return;
      }
      // Validate end time is after start time
      const [startHours, startMinutes] = startTime.split(':').map(Number);
      const [endHours, endMinutes] = endTime.split(':').map(Number);
      if (startHours * 60 + startMinutes >= endHours * 60 + endMinutes) {
        alert('End time must be after start time');
        return;
      }
    } else if (currentStep === 3) {
      if (!selectedTableId) {
        alert('Please select a table');
        return;
      }
    }

    setCurrentStep((prev) => Math.min(prev + 1, 4));
  };

  const handleBack = () => {
    setCurrentStep((prev) => Math.max(prev - 1, 1));
  };

  const onSubmit = async (data: ReservationFormData) => {
    try {
      const reservationRequest: CreateReservationRequest = {
        customerId: data.customerId,
        tableId: data.tableId,
        reservationDate: data.reservationDate,
        startTime: data.startTime,
        endTime: data.endTime,
        partySize: data.partySize,
        specialRequests: data.specialRequests || undefined,
      };

      await createReservation.mutateAsync(reservationRequest);

      // Show success message
      setShowSuccess(true);

      // Redirect to reservations page after 2 seconds
      setTimeout(() => {
        navigate('/reservations');
      }, 2000);
    } catch (error) {
      console.error('Failed to create reservation:', error);
      alert('Failed to create reservation. Please try again.');
    }
  };

  const handleCancel = () => {
    if (confirm('Are you sure you want to cancel? All progress will be lost.')) {
      navigate('/reservations');
    }
  };

  // Success Screen
  if (showSuccess) {
    return (
      <div
        style={{
          minHeight: 'calc(100vh - 200px)',
          backgroundColor: '#f3f4f6',
          padding: '40px 20px',
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'center',
        }}
      >
        <div
          style={{
            maxWidth: '600px',
            backgroundColor: '#ffffff',
            borderRadius: '12px',
            padding: '48px',
            textAlign: 'center',
            boxShadow: '0 4px 12px rgba(0, 0, 0, 0.1)',
          }}
        >
          <div style={{ fontSize: '72px', marginBottom: '24px' }}>✅</div>
          <h2 style={{ fontSize: '28px', fontWeight: '700', marginBottom: '16px', color: '#1f2937' }}>
            Reservation Confirmed!
          </h2>
          <p style={{ fontSize: '16px', color: '#6b7280', marginBottom: '24px' }}>
            Your table has been successfully reserved. You will be redirected to your reservations page.
          </p>
          <div
            style={{
              fontSize: '14px',
              color: '#9ca3af',
              fontStyle: 'italic',
            }}
          >
            Redirecting...
          </div>
        </div>
      </div>
    );
  }

  return (
    <div
      style={{
        minHeight: 'calc(100vh - 200px)',
        backgroundColor: '#f3f4f6',
        padding: '40px 20px',
      }}
    >
      <div style={{ maxWidth: '900px', margin: '0 auto' }}>
        {/* Page Header */}
        <div style={{ marginBottom: '32px', textAlign: 'center' }}>
          <h1
            style={{
              fontSize: '36px',
              fontWeight: 'bold',
              margin: '0 0 12px 0',
              color: '#1f2937',
            }}
          >
            🎲 New Reservation
          </h1>
          <p style={{ fontSize: '16px', color: '#6b7280', margin: 0 }}>
            Book a table for your gaming session
          </p>
        </div>

        {/* Progress Indicator */}
        <div
          style={{
            backgroundColor: '#ffffff',
            borderRadius: '12px',
            padding: '24px',
            marginBottom: '24px',
            boxShadow: '0 1px 3px rgba(0, 0, 0, 0.1)',
          }}
        >
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
            {[1, 2, 3, 4].map((step) => (
              <div
                key={step}
                style={{
                  display: 'flex',
                  alignItems: 'center',
                  flex: 1,
                }}
              >
                <div
                  style={{
                    display: 'flex',
                    flexDirection: 'column',
                    alignItems: 'center',
                    flex: 1,
                  }}
                >
                  <div
                    style={{
                      width: '40px',
                      height: '40px',
                      borderRadius: '50%',
                      backgroundColor: step <= currentStep ? '#10b981' : '#e5e7eb',
                      color: step <= currentStep ? '#ffffff' : '#9ca3af',
                      display: 'flex',
                      alignItems: 'center',
                      justifyContent: 'center',
                      fontWeight: 'bold',
                      fontSize: '16px',
                      marginBottom: '8px',
                    }}
                  >
                    {step}
                  </div>
                  <div
                    style={{
                      fontSize: '12px',
                      color: step <= currentStep ? '#1f2937' : '#9ca3af',
                      fontWeight: step === currentStep ? '600' : '400',
                      textAlign: 'center',
                    }}
                  >
                    {step === 1 && 'Date & Party'}
                    {step === 2 && 'Time'}
                    {step === 3 && 'Select Table'}
                    {step === 4 && 'Confirm'}
                  </div>
                </div>
                {step < 4 && (
                  <div
                    style={{
                      height: '2px',
                      flex: 1,
                      backgroundColor: step < currentStep ? '#10b981' : '#e5e7eb',
                      margin: '0 8px',
                      marginBottom: '30px',
                    }}
                  />
                )}
              </div>
            ))}
          </div>
        </div>

        {/* Step Content */}
        <form onSubmit={handleSubmit(onSubmit)}>
          {/* Step 1: Date & Party Size */}
          {currentStep === 1 && (
            <div>
              <DateTimePicker
                selectedDate={reservationDate}
                startTime=""
                endTime=""
                partySize={partySize}
                onDateChange={(date) => setValue('reservationDate', date)}
                onStartTimeChange={() => {}}
                onEndTimeChange={() => {}}
                onPartySizeChange={(size) => setValue('partySize', size)}
              />
            </div>
          )}

          {/* Step 2: Time Range */}
          {currentStep === 2 && (
            <div>
              <DateTimePicker
                selectedDate={reservationDate}
                startTime={startTime}
                endTime={endTime}
                partySize={partySize}
                onDateChange={(date) => setValue('reservationDate', date)}
                onStartTimeChange={(time) => setValue('startTime', time)}
                onEndTimeChange={(time) => setValue('endTime', time)}
                onPartySizeChange={(size) => setValue('partySize', size)}
              />
            </div>
          )}

          {/* Step 3: Table Selection */}
          {currentStep === 3 && (
            <div>
              <TableSelector
                availableTables={availableTables || []}
                selectedTableId={selectedTableId}
                onSelectTable={(id) => setValue('tableId', id)}
                partySize={partySize}
                isLoading={isLoadingTables}
              />
            </div>
          )}

          {/* Step 4: Special Requests & Summary */}
          {currentStep === 4 && (
            <div style={{ display: 'grid', gap: '24px' }}>
              {/* Special Requests */}
              <div
                style={{
                  backgroundColor: '#ffffff',
                  border: '1px solid #e5e7eb',
                  borderRadius: '12px',
                  padding: '24px',
                }}
              >
                <h3
                  style={{
                    fontSize: '20px',
                    fontWeight: '600',
                    marginBottom: '16px',
                    color: '#1f2937',
                  }}
                >
                  Special Requests (Optional)
                </h3>
                <textarea
                  {...register('specialRequests')}
                  placeholder="Any special requirements or requests? (e.g., highchair needed, birthday celebration)"
                  rows={4}
                  style={{
                    width: '100%',
                    padding: '12px',
                    border: '1px solid #d1d5db',
                    borderRadius: '6px',
                    fontSize: '14px',
                    fontFamily: 'inherit',
                    resize: 'vertical',
                  }}
                />
              </div>

              {/* Summary */}
              <ReservationSummary
                selectedTable={selectedTable}
                reservationDate={reservationDate}
                startTime={startTime}
                endTime={endTime}
                partySize={partySize}
                specialRequests={specialRequests}
                onConfirm={handleSubmit(onSubmit)}
                onCancel={handleCancel}
                isSubmitting={createReservation.isPending}
              />
            </div>
          )}

          {/* Navigation Buttons */}
          {currentStep < 4 && (
            <div
              style={{
                display: 'flex',
                justifyContent: 'space-between',
                marginTop: '24px',
                gap: '12px',
              }}
            >
              <button
                type="button"
                onClick={currentStep === 1 ? handleCancel : handleBack}
                style={{
                  padding: '12px 24px',
                  backgroundColor: '#ffffff',
                  color: '#374151',
                  border: '1px solid #d1d5db',
                  borderRadius: '6px',
                  fontSize: '14px',
                  fontWeight: '500',
                  cursor: 'pointer',
                }}
              >
                {currentStep === 1 ? 'Cancel' : 'Back'}
              </button>
              <button
                type="button"
                onClick={handleNext}
                style={{
                  padding: '12px 24px',
                  backgroundColor: '#10b981',
                  color: '#ffffff',
                  border: 'none',
                  borderRadius: '6px',
                  fontSize: '14px',
                  fontWeight: '500',
                  cursor: 'pointer',
                }}
              >
                {currentStep === 3 ? 'Review' : 'Next'}
              </button>
            </div>
          )}
        </form>
      </div>
    </div>
  );
}
