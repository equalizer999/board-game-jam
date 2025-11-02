namespace BoardGameCafe.Api.Features.Orders;

/// <summary>
/// Request to update item quantities in an order
/// </summary>
public record UpdateOrderRequest
{
    /// <summary>
    /// List of item updates (item ID and new quantity)
    /// </summary>
    public List<OrderItemUpdate> ItemUpdates { get; init; } = new();
}

/// <summary>
/// Represents an update to a single order item
/// </summary>
public record OrderItemUpdate
{
    /// <summary>
    /// Order item ID to update
    /// </summary>
    public Guid OrderItemId { get; init; }
    
    /// <summary>
    /// New quantity (0 to remove the item)
    /// </summary>
    public int Quantity { get; init; }
}
