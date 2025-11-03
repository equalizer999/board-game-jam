namespace BoardGameCafe.Api.Features.Orders;

/// <summary>
/// Detailed summary of an order with pricing breakdown
/// </summary>
public record OrderSummaryDto
{
    /// <summary>
    /// Order ID
    /// </summary>
    public Guid OrderId { get; init; }

    /// <summary>
    /// Customer ID
    /// </summary>
    public Guid CustomerId { get; init; }

    /// <summary>
    /// Customer's full name
    /// </summary>
    public string CustomerName { get; init; } = string.Empty;

    /// <summary>
    /// Customer's membership tier
    /// </summary>
    public string MembershipTier { get; init; } = string.Empty;

    /// <summary>
    /// Current loyalty points balance
    /// </summary>
    public int LoyaltyPointsBalance { get; init; }

    /// <summary>
    /// Subtotal before any discounts or tax
    /// </summary>
    public decimal Subtotal { get; init; }

    /// <summary>
    /// Member discount amount (based on membership tier)
    /// </summary>
    public decimal MemberDiscountAmount { get; init; }

    /// <summary>
    /// Member discount percentage applied
    /// </summary>
    public decimal MemberDiscountPercentage { get; init; }

    /// <summary>
    /// Loyalty points redeemed for this order
    /// </summary>
    public int LoyaltyPointsRedeemed { get; init; }

    /// <summary>
    /// Discount amount from loyalty points redemption (100 points = $1)
    /// </summary>
    public decimal LoyaltyPointsDiscountAmount { get; init; }

    /// <summary>
    /// Total discount amount (member discount + loyalty points discount)
    /// </summary>
    public decimal TotalDiscountAmount { get; init; }

    /// <summary>
    /// Tax amount
    /// </summary>
    public decimal TaxAmount { get; init; }

    /// <summary>
    /// Final total amount (Subtotal - Total Discount + Tax)
    /// </summary>
    public decimal TotalAmount { get; init; }

    /// <summary>
    /// Loyalty points earned from this order (1 point per $1 spent)
    /// </summary>
    public int LoyaltyPointsEarned { get; init; }

    /// <summary>
    /// Order status
    /// </summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// Payment method
    /// </summary>
    public string PaymentMethod { get; init; } = string.Empty;

    /// <summary>
    /// List of items in the order
    /// </summary>
    public List<OrderItemDto> Items { get; init; } = new();
}
