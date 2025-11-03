namespace BoardGameCafe.Domain;

public class Reservation
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid TableId { get; set; }
    public DateTime ReservationDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int PartySize { get; set; }
    public ReservationStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? SpecialRequests { get; set; }

    // Navigation properties
    public Customer? Customer { get; set; }
    public Table? Table { get; set; }
}

public enum ReservationStatus
{
    Pending,
    Confirmed,
    CheckedIn,
    Completed,
    Cancelled,
    NoShow
}
