using BoardGameCafe.Domain;

namespace BoardGameCafe.Api.Features.Reservations;

/// <summary>
/// Validator for reservation business rules
/// </summary>
public class ReservationValidator
{
    private const int MinPartySize = 1;
    private const int MaxPartySize = 20;
    private static readonly TimeSpan BusinessHoursStart = new TimeSpan(10, 0, 0); // 10 AM
    private static readonly TimeSpan BusinessHoursEnd = new TimeSpan(22, 0, 0);   // 10 PM

    /// <summary>
    /// Validates that party size is within acceptable limits
    /// </summary>
    /// <param name="partySize">Number of people in the party</param>
    /// <returns>True if valid, false otherwise</returns>
    public bool ValidatePartySize(int partySize)
    {
        return partySize >= MinPartySize && partySize <= MaxPartySize;
    }

    /// <summary>
    /// Validates that reservation date is in the future
    /// </summary>
    /// <param name="reservationDate">The reservation date</param>
    /// <param name="currentDate">Current date (defaults to Today if not provided)</param>
    /// <returns>True if reservation is for today or future date, false if in past</returns>
    public bool ValidateFutureDate(DateTime reservationDate, DateTime? currentDate = null)
    {
        var checkDate = (currentDate ?? DateTime.Today).Date;
        return reservationDate.Date >= checkDate;
    }

    /// <summary>
    /// Validates that reservation time is within business hours
    /// </summary>
    /// <param name="startTime">Reservation start time</param>
    /// <param name="endTime">Reservation end time</param>
    /// <returns>True if both times are within business hours, false otherwise</returns>
    public bool ValidateTimeRange(TimeSpan startTime, TimeSpan endTime)
    {
        if (startTime >= endTime)
        {
            return false; // Start time must be before end time
        }

        return startTime >= BusinessHoursStart && 
               endTime <= BusinessHoursEnd;
    }

    /// <summary>
    /// Validates that table has sufficient capacity for party size
    /// </summary>
    /// <param name="table">The table to check</param>
    /// <param name="partySize">Number of people in the party</param>
    /// <returns>True if table capacity is sufficient, false otherwise</returns>
    public bool ValidateTableCapacity(Table table, int partySize)
    {
        if (table == null)
        {
            throw new ArgumentNullException(nameof(table));
        }

        return table.SeatingCapacity >= partySize;
    }

    /// <summary>
    /// Validates all reservation rules
    /// </summary>
    /// <param name="reservation">The reservation to validate</param>
    /// <param name="table">The table for the reservation</param>
    /// <returns>Tuple containing validation result and error message if invalid</returns>
    public (bool IsValid, string? ErrorMessage) ValidateReservation(Reservation reservation, Table? table = null)
    {
        if (reservation == null)
        {
            throw new ArgumentNullException(nameof(reservation));
        }

        if (!ValidatePartySize(reservation.PartySize))
        {
            return (false, $"Party size must be between {MinPartySize} and {MaxPartySize}");
        }

        if (!ValidateFutureDate(reservation.ReservationDate))
        {
            return (false, "Reservation date must be today or in the future");
        }

        if (!ValidateTimeRange(reservation.StartTime, reservation.EndTime))
        {
            return (false, $"Reservation must be within business hours ({BusinessHoursStart:hh\\:mm} - {BusinessHoursEnd:hh\\:mm})");
        }

        if (table != null && !ValidateTableCapacity(table, reservation.PartySize))
        {
            return (false, $"Table capacity ({table.SeatingCapacity}) is insufficient for party size ({reservation.PartySize})");
        }

        return (true, null);
    }
}
