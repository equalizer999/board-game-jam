namespace BoardGameCafe.Api.Features.Orders;

/// <summary>
/// Represents an order with all details
/// </summary>
public record OrderDto
{
    /// <summary>
    /// Unique identifier for the order
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Customer ID who placed the order
    /// </summary>
    public Guid CustomerId { get; init; }

    /// <summary>
    /// Optional reservation ID if order is linked to a reservation
    /// </summary>
    public Guid? ReservationId { get; init; }

    /// <summary>
    /// Date and time when the order was created
    /// </summary>
    public DateTime OrderDate { get; init; }

    /// <summary>
    /// Current status of the order
    /// </summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// Subtotal before discounts and tax
    /// </summary>
    public decimal Subtotal { get; init; }

    /// <summary>
    /// Total discount amount applied
    /// </summary>
    public decimal DiscountAmount { get; init; }

    /// <summary>
    /// Tax amount
    /// </summary>
    public decimal TaxAmount { get; init; }

    /// <summary>
    /// Final total amount (Subtotal - Discount + Tax)
    /// </summary>
    public decimal TotalAmount { get; init; }

    /// <summary>
    /// Payment method used
    /// </summary>
    public string PaymentMethod { get; init; } = string.Empty;

    /// <summary>
    /// Customer's full name
    /// </summary>
    public string CustomerName { get; init; } = string.Empty;

    /// <summary>
    /// List of items in the order
    /// </summary>
    public List<OrderItemDto> Items { get; init; } = new();
}

/// <summary>
/// Represents a single item in an order
/// </summary>
public record OrderItemDto
{
    /// <summary>
    /// Unique identifier for the order item
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Menu item ID
    /// </summary>
    public Guid MenuItemId { get; init; }

    /// <summary>
    /// Menu item name
    /// </summary>
    public string MenuItemName { get; init; } = string.Empty;

    /// <summary>
    /// Quantity ordered
    /// </summary>
    public int Quantity { get; init; }

    /// <summary>
    /// Unit price at the time of order (price snapshot)
    /// </summary>
    public decimal UnitPrice { get; init; }

    /// <summary>
    /// Total price for this item (Quantity * UnitPrice)
    /// </summary>
    public decimal ItemTotal { get; init; }

    /// <summary>
    /// Optional special instructions for this item
    /// </summary>
    public string? SpecialInstructions { get; init; }
}
