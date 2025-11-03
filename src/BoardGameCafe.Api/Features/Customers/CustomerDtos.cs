namespace BoardGameCafe.Api.Features.Customers;

/// <summary>
/// Represents customer profile information
/// </summary>
public record CustomerDto
{
    /// <summary>
    /// Unique identifier for the customer
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Customer's email address
    /// </summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// Customer's first name
    /// </summary>
    public string FirstName { get; init; } = string.Empty;

    /// <summary>
    /// Customer's last name
    /// </summary>
    public string LastName { get; init; } = string.Empty;

    /// <summary>
    /// Customer's phone number
    /// </summary>
    public string? Phone { get; init; }

    /// <summary>
    /// Current membership tier
    /// </summary>
    public string MembershipTier { get; init; } = string.Empty;

    /// <summary>
    /// Current loyalty points balance
    /// </summary>
    public int LoyaltyPoints { get; init; }

    /// <summary>
    /// Date when customer joined
    /// </summary>
    public DateTime JoinedDate { get; init; }

    /// <summary>
    /// Total number of visits
    /// </summary>
    public int TotalVisits { get; init; }
}

/// <summary>
/// Request to update customer profile
/// </summary>
public record UpdateCustomerRequest
{
    /// <summary>
    /// Customer's first name
    /// </summary>
    public string? FirstName { get; init; }

    /// <summary>
    /// Customer's last name
    /// </summary>
    public string? LastName { get; init; }

    /// <summary>
    /// Customer's phone number
    /// </summary>
    public string? Phone { get; init; }
}

/// <summary>
/// Represents loyalty points balance and tier information
/// </summary>
public record LoyaltyPointsDto
{
    /// <summary>
    /// Current loyalty points balance
    /// </summary>
    public int CurrentBalance { get; init; }

    /// <summary>
    /// Current membership tier
    /// </summary>
    public string CurrentTier { get; init; } = string.Empty;

    /// <summary>
    /// Discount percentage for current tier
    /// </summary>
    public decimal DiscountPercentage { get; init; }

    /// <summary>
    /// Next tier name (null if already at Gold)
    /// </summary>
    public string? NextTier { get; init; }

    /// <summary>
    /// Points needed to reach next tier (null if already at Gold)
    /// </summary>
    public int? PointsToNextTier { get; init; }

    /// <summary>
    /// Points threshold for current tier
    /// </summary>
    public int CurrentTierThreshold { get; init; }

    /// <summary>
    /// Points threshold for next tier (null if already at Gold)
    /// </summary>
    public int? NextTierThreshold { get; init; }
}

/// <summary>
/// Represents a single loyalty points transaction
/// </summary>
public record LoyaltyTransactionDto
{
    /// <summary>
    /// Unique identifier for the transaction
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Points change (positive for earned, negative for redeemed)
    /// </summary>
    public int PointsChange { get; init; }

    /// <summary>
    /// Type of transaction (Earned, Redeemed, Expired, Adjusted)
    /// </summary>
    public string TransactionType { get; init; } = string.Empty;

    /// <summary>
    /// Date and time of the transaction
    /// </summary>
    public DateTime TransactionDate { get; init; }

    /// <summary>
    /// Description of the transaction
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Associated order ID (if applicable)
    /// </summary>
    public Guid? OrderId { get; init; }
}

/// <summary>
/// Represents customer visit statistics
/// </summary>
public record VisitStatsDto
{
    /// <summary>
    /// Total number of visits
    /// </summary>
    public int TotalVisits { get; init; }

    /// <summary>
    /// Total number of games played
    /// </summary>
    public int GamesPlayed { get; init; }

    /// <summary>
    /// Total amount spent
    /// </summary>
    public decimal TotalSpent { get; init; }

    /// <summary>
    /// Total orders placed
    /// </summary>
    public int TotalOrders { get; init; }

    /// <summary>
    /// Average order value
    /// </summary>
    public decimal AverageOrderValue { get; init; }
}
