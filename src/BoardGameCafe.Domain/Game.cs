namespace BoardGameCafe.Domain;

public class Game
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;
    public int MinPlayers { get; set; }
    public int MaxPlayers { get; set; }
    public int PlayTimeMinutes { get; set; }
    public int AgeRating { get; set; }
    public decimal Complexity { get; set; }
    public GameCategory Category { get; set; }
    public int CopiesOwned { get; set; }
    public int CopiesInUse { get; set; }
    public decimal DailyRentalFee { get; set; }
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
