namespace BoardGameCafe.Domain;

public class Customer
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public MembershipTier MembershipTier { get; set; } = MembershipTier.None;
    public int LoyaltyPoints { get; set; } = 0;
    public DateTime JoinedDate { get; set; }
    public int TotalVisits { get; set; } = 0;
    public List<Game> FavoriteGames { get; set; } = new();
}

public enum MembershipTier
{
    None,
    Bronze,
    Silver,
    Gold
}
