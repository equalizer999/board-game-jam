namespace BoardGameCafe.Api.Features.Customers;

/// <summary>
/// Customer visit statistics
/// </summary>
public class VisitStatsDto
{
    public int TotalVisits { get; set; }
    public int GamesPlayed { get; set; }
    public decimal TotalSpending { get; set; }
    public DateTime? LastVisit { get; set; }
}
