namespace BoardGameCafe.Api.Features.Reservations;

/// <summary>
/// Request to create a new reservation
/// </summary>
public record CreateReservationRequest
{
    /// <summary>
    /// Customer ID making the reservation
    /// </summary>
    public Guid CustomerId { get; init; }

    /// <summary>
    /// Table ID to reserve
    /// </summary>
    public Guid TableId { get; init; }

    /// <summary>
    /// Date of the reservation (must be a future date)
    /// </summary>
    public DateTime ReservationDate { get; init; }

    /// <summary>
    /// Start time for the reservation
    /// </summary>
    public TimeSpan StartTime { get; init; }

    /// <summary>
    /// End time for the reservation
    /// </summary>
    public TimeSpan EndTime { get; init; }

    /// <summary>
    /// Number of people in the party (must be less than or equal to table capacity)
    /// </summary>
    public int PartySize { get; init; }

    /// <summary>
    /// Optional special requests or notes
    /// </summary>
    public string? SpecialRequests { get; init; }
}
