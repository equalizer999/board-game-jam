namespace BoardGameCafe.Domain;

public class GameSession
{
    public Guid Id { get; set; }
    public Guid GameId { get; set; }
    public Game Game { get; set; } = null!;
    public Guid ReservationId { get; set; }
    public Reservation Reservation { get; set; } = null!;
    public DateTime CheckedOutAt { get; set; }
    public DateTime DueBackAt { get; set; }
    public DateTime? ReturnedAt { get; set; }
    public GameCondition Condition { get; set; }
    public decimal LateFeeApplied { get; set; }
}

public enum GameCondition
{
    Excellent,
    Good,
    Fair,
    Damaged
}
