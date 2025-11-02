namespace BoardGameCafe.Api.Features.Reservations;

/// <summary>
/// Represents a reservation with all details
/// </summary>
public record ReservationDto
{
    /// <summary>
    /// Unique identifier for the reservation
    /// </summary>
    public Guid Id { get; init; }
    
    /// <summary>
    /// Customer ID who made the reservation
    /// </summary>
    public Guid CustomerId { get; init; }
    
    /// <summary>
    /// Table ID for the reservation
    /// </summary>
    public Guid TableId { get; init; }
    
    /// <summary>
    /// Date of the reservation (date only, no time component)
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
    /// Number of people in the party
    /// </summary>
    public int PartySize { get; init; }
    
    /// <summary>
    /// Current status of the reservation
    /// </summary>
    public string Status { get; init; } = string.Empty;
    
    /// <summary>
    /// When the reservation was created
    /// </summary>
    public DateTime CreatedAt { get; init; }
    
    /// <summary>
    /// Optional special requests or notes
    /// </summary>
    public string? SpecialRequests { get; init; }
    
    /// <summary>
    /// Table number for display purposes
    /// </summary>
    public string TableNumber { get; init; } = string.Empty;
    
    /// <summary>
    /// Customer's full name
    /// </summary>
    public string CustomerName { get; init; } = string.Empty;
}
