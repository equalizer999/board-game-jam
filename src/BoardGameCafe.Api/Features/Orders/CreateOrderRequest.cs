namespace BoardGameCafe.Api.Features.Orders;

/// <summary>
/// Request to create a new draft order
/// </summary>
public record CreateOrderRequest
{
    /// <summary>
    /// Customer ID who is placing the order
    /// </summary>
    public Guid CustomerId { get; init; }
    
    /// <summary>
    /// Optional reservation ID to link the order to a reservation
    /// </summary>
    public Guid? ReservationId { get; init; }
}
