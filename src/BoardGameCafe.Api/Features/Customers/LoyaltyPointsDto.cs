namespace BoardGameCafe.Api.Features.Customers;

/// <summary>
/// Loyalty points balance and tier information
/// </summary>
public class LoyaltyPointsDto
{
    public int CurrentPoints { get; set; }
    public string CurrentTier { get; set; } = string.Empty;
    public decimal CurrentDiscount { get; set; }
    public string? NextTier { get; set; }
    public int? PointsToNextTier { get; set; }
    public int PointsThreshold { get; set; }
}
