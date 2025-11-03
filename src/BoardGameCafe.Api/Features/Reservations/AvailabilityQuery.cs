namespace BoardGameCafe.Api.Features.Reservations;

/// <summary>
/// Query parameters for checking table availability
/// </summary>
public record AvailabilityQuery
{
    /// <summary>
    /// Date to check availability for
    /// </summary>
    public DateTime Date { get; init; }

    /// <summary>
    /// Start time for the desired reservation
    /// </summary>
    public TimeSpan StartTime { get; init; }

    /// <summary>
    /// End time for the desired reservation
    /// </summary>
    public TimeSpan EndTime { get; init; }

    /// <summary>
    /// Number of people in the party
    /// </summary>
    public int PartySize { get; init; }
}

/// <summary>
/// Represents an available table with pricing information
/// </summary>
public record AvailableTableDto
{
    /// <summary>
    /// Table ID
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Table number for display
    /// </summary>
    public string TableNumber { get; init; } = string.Empty;

    /// <summary>
    /// Maximum seating capacity
    /// </summary>
    public int SeatingCapacity { get; init; }

    /// <summary>
    /// Whether this is a window seat
    /// </summary>
    public bool IsWindowSeat { get; init; }

    /// <summary>
    /// Whether this table is accessible
    /// </summary>
    public bool IsAccessible { get; init; }

    /// <summary>
    /// Hourly rental rate
    /// </summary>
    public decimal HourlyRate { get; init; }

    /// <summary>
    /// Total price for the requested time slot
    /// </summary>
    public decimal TotalPrice { get; init; }
}
