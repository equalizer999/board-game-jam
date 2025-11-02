namespace BoardGameCafe.Api.Features.Reservations;

/// <summary>
/// Request to update an existing reservation
/// </summary>
public record UpdateReservationRequest
{
    /// <summary>
    /// New table ID (optional)
    /// </summary>
    public Guid? TableId { get; init; }
    
    /// <summary>
    /// New reservation date (optional, must be future date if provided)
    /// </summary>
    public DateTime? ReservationDate { get; init; }
    
    /// <summary>
    /// New start time (optional)
    /// </summary>
    public TimeSpan? StartTime { get; init; }
    
    /// <summary>
    /// New end time (optional)
    /// </summary>
    public TimeSpan? EndTime { get; init; }
    
    /// <summary>
    /// New party size (optional, must be less than or equal to table capacity if provided)
    /// </summary>
    public int? PartySize { get; init; }
    
    /// <summary>
    /// New special requests (optional)
    /// </summary>
    public string? SpecialRequests { get; init; }
}
