using System.ComponentModel.DataAnnotations;

namespace BoardGameCafe.Domain;

public class Game
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Publisher { get; set; } = string.Empty;

    [Range(1, 20)]
    public int MinPlayers { get; set; }

    [Range(1, 20)]
    public int MaxPlayers { get; set; }

    [Range(5, 600)]
    public int PlayTimeMinutes { get; set; }

    [Range(3, 18)]
    public int AgeRating { get; set; }

    [Range(1.0, 5.0)]
    public decimal Complexity { get; set; }

    public GameCategory Category { get; set; }

    [Range(1, int.MaxValue)]
    public int CopiesOwned { get; set; }

    [Range(0, int.MaxValue)]
    public int CopiesInUse { get; set; }

    [Range(0, double.MaxValue)]
    public decimal DailyRentalFee { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public bool IsAvailable => CopiesOwned > CopiesInUse;
}

public enum GameCategory
{
    Strategy,
    Party,
    Family,
    Cooperative,
    Abstract
}
