using System.ComponentModel.DataAnnotations;

namespace BoardGameCafe.Api.Features.Games;

/// <summary>
/// Request to create a new game in the catalog
/// </summary>
public record CreateGameRequest
{
    /// <summary>
    /// Name of the game
    /// </summary>
    /// <example>Settlers of Catan</example>
    [Required]
    [MaxLength(200)]
    public string Title { get; init; } = string.Empty;
    
    /// <summary>
    /// Publisher of the game
    /// </summary>
    /// <example>Catan Studio</example>
    [Required]
    [MaxLength(100)]
    public string Publisher { get; init; } = string.Empty;
    
    /// <summary>
    /// Minimum number of players (1-20)
    /// </summary>
    /// <example>3</example>
    [Range(1, 20)]
    public int MinPlayers { get; init; }
    
    /// <summary>
    /// Maximum number of players (1-20)
    /// </summary>
    /// <example>4</example>
    [Range(1, 20)]
    public int MaxPlayers { get; init; }
    
    /// <summary>
    /// Estimated play time in minutes (5-600)
    /// </summary>
    /// <example>90</example>
    [Range(5, 600)]
    public int PlayTimeMinutes { get; init; }
    
    /// <summary>
    /// Minimum age rating (3-18)
    /// </summary>
    /// <example>10</example>
    [Range(3, 18)]
    public int AgeRating { get; init; }
    
    /// <summary>
    /// Complexity rating from 1.0 (simple) to 5.0 (very complex)
    /// </summary>
    /// <example>2.5</example>
    [Range(1.0, 5.0)]
    public decimal Complexity { get; init; }
    
    /// <summary>
    /// Game category (0=Strategy, 1=Party, 2=Family, 3=Cooperative, 4=Abstract)
    /// </summary>
    /// <example>0</example>
    public int Category { get; init; }
    
    /// <summary>
    /// Total number of copies owned (minimum 1)
    /// </summary>
    /// <example>3</example>
    [Range(1, int.MaxValue)]
    public int CopiesOwned { get; init; }
    
    /// <summary>
    /// Daily rental fee
    /// </summary>
    /// <example>5.00</example>
    [Range(0, double.MaxValue)]
    public decimal DailyRentalFee { get; init; }
    
    /// <summary>
    /// Description of the game
    /// </summary>
    /// <example>A classic resource management and trading game</example>
    [MaxLength(1000)]
    public string? Description { get; init; }
    
    /// <summary>
    /// URL to the game's image
    /// </summary>
    /// <example>https://example.com/games/catan.jpg</example>
    public string? ImageUrl { get; init; }
}
