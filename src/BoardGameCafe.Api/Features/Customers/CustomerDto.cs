namespace BoardGameCafe.Api.Features.Customers;

/// <summary>
/// Customer profile information
/// </summary>
public class CustomerDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string MembershipTier { get; set; } = string.Empty;
    public int LoyaltyPoints { get; set; }
    public DateTime JoinedDate { get; set; }
    public int TotalVisits { get; set; }
}
