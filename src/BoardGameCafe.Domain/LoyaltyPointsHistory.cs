namespace BoardGameCafe.Domain;

public class LoyaltyPointsHistory
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public Guid? OrderId { get; set; }
    public Order? Order { get; set; }
    public int PointsChange { get; set; }
    public LoyaltyTransactionType TransactionType { get; set; }
    public DateTime TransactionDate { get; set; }
    public string? Description { get; set; }
}

public enum LoyaltyTransactionType
{
    Earned,
    Redeemed,
    Expired,
    Adjusted
}
