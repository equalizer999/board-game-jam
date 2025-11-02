namespace BoardGameCafe.Api.Features.Games;

/// <summary>
/// Represents a board game with all details
/// </summary>
public record GameDto
{
    /// <summary>
    /// Unique identifier for the game
    /// </summary>
    public Guid Id { get; init; }
    
    /// <summary>
    /// Name of the game
    /// </summary>
    public string Title { get; init; } = string.Empty;
    
    /// <summary>
    /// Publisher of the game
    /// </summary>
    public string Publisher { get; init; } = string.Empty;
    
    /// <summary>
    /// Minimum number of players
    /// </summary>
    public int MinPlayers { get; init; }
    
    /// <summary>
    /// Maximum number of players
    /// </summary>
    public int MaxPlayers { get; init; }
    
    /// <summary>
    /// Estimated play time in minutes
    /// </summary>
    public int PlayTimeMinutes { get; init; }
    
    /// <summary>
    /// Minimum age rating (3-18)
    /// </summary>
    public int AgeRating { get; init; }
    
    /// <summary>
    /// Complexity rating from 1.0 (simple) to 5.0 (very complex)
    /// </summary>
    public decimal Complexity { get; init; }
    
    /// <summary>
    /// Game category (Strategy, Party, Family, Cooperative, Abstract)
    /// </summary>
    public string Category { get; init; } = string.Empty;
    
    /// <summary>
    /// Total number of copies owned by the café
    /// </summary>
    public int CopiesOwned { get; init; }
    
    /// <summary>
    /// Number of copies currently checked out
    /// </summary>
    public int CopiesInUse { get; init; }
    
    /// <summary>
    /// Daily rental fee for the game
    /// </summary>
    public decimal DailyRentalFee { get; init; }
    
    /// <summary>
    /// Description of the game
    /// </summary>
    public string? Description { get; init; }
    
    /// <summary>
    /// URL to the game's image
    /// </summary>
    public string? ImageUrl { get; init; }
    
    /// <summary>
    /// Whether the game is available for rental (has copies not in use)
    /// </summary>
    public bool IsAvailable { get; init; }
}
