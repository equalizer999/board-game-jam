using BoardGameCafe.Domain;

namespace BoardGameCafe.Tests.Unit.Builders;

/// <summary>
/// Test data builder for Customer entities using fluent API
/// </summary>
public class CustomerBuilder
{
    private Guid _id = Guid.NewGuid();
    private string _email = "test@example.com";
    private string _firstName = "John";
    private string _lastName = "Doe";
    private string? _phone = null;
    private MembershipTier _membershipTier = MembershipTier.None;
    private int _loyaltyPoints = 0;
    private DateTime _joinedDate = DateTime.UtcNow.AddMonths(-6);
    private int _totalVisits = 0;
    private List<Game> _favoriteGames = new();

    public CustomerBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public CustomerBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public CustomerBuilder WithName(string firstName, string lastName)
    {
        _firstName = firstName;
        _lastName = lastName;
        return this;
    }

    public CustomerBuilder WithPhone(string phone)
    {
        _phone = phone;
        return this;
    }

    public CustomerBuilder WithMembershipTier(MembershipTier tier)
    {
        _membershipTier = tier;
        return this;
    }

    public CustomerBuilder WithLoyaltyPoints(int points)
    {
        _loyaltyPoints = points;
        return this;
    }

    public CustomerBuilder WithJoinedDate(DateTime joinedDate)
    {
        _joinedDate = joinedDate;
        return this;
    }

    public CustomerBuilder WithTotalVisits(int visits)
    {
        _totalVisits = visits;
        return this;
    }

    public CustomerBuilder WithFavoriteGames(params Game[] games)
    {
        _favoriteGames = games.ToList();
        return this;
    }

    public CustomerBuilder AsBronzeMember()
    {
        _membershipTier = MembershipTier.Bronze;
        return this;
    }

    public CustomerBuilder AsSilverMember()
    {
        _membershipTier = MembershipTier.Silver;
        return this;
    }

    public CustomerBuilder AsGoldMember()
    {
        _membershipTier = MembershipTier.Gold;
        return this;
    }

    public Customer Build()
    {
        return new Customer
        {
            Id = _id,
            Email = _email,
            FirstName = _firstName,
            LastName = _lastName,
            Phone = _phone,
            MembershipTier = _membershipTier,
            LoyaltyPoints = _loyaltyPoints,
            JoinedDate = _joinedDate,
            TotalVisits = _totalVisits,
            FavoriteGames = _favoriteGames
        };
    }

    public static implicit operator Customer(CustomerBuilder builder) => builder.Build();
}
