namespace BoardGameCafe.Api.Features.Orders;

/// <summary>
/// Request to add an item to an order
/// </summary>
public record AddOrderItemRequest
{
    /// <summary>
    /// Menu item ID to add
    /// </summary>
    public Guid MenuItemId { get; init; }
    
    /// <summary>
    /// Quantity to add
    /// </summary>
    public int Quantity { get; init; }
    
    /// <summary>
    /// Optional special instructions for this item
    /// </summary>
    public string? SpecialInstructions { get; init; }
}
