namespace BoardGameCafe.Api.Features.Customers;

/// <summary>
/// Loyalty points transaction record
/// </summary>
public class LoyaltyTransactionDto
{
    public Guid Id { get; set; }
    public int PointsChange { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
    public Guid? OrderId { get; set; }
}
