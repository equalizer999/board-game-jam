namespace BoardGameCafe.Domain;

/// <summary>
/// Tracks loyalty points transactions (earned and redeemed)
/// </summary>
public class LoyaltyPointsHistory
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public int PointsChange { get; set; } // Positive for earned, negative for redeemed
    public Guid? OrderId { get; set; }
    public Order? Order { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
    public LoyaltyTransactionType TransactionType { get; set; }
}

public enum LoyaltyTransactionType
{
    Earned,
    Redeemed,
    Adjustment
}
