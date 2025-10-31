namespace BoardGameCafe.Domain;

public class Event
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public int DurationMinutes { get; set; }
    public int MaxParticipants { get; set; }
    public int CurrentParticipants { get; set; }
    public decimal TicketPrice { get; set; }
    public EventType EventType { get; set; }
    public bool RequiresRegistration { get; set; }
    public List<EventRegistration> Registrations { get; set; } = new();
}

public enum EventType
{
    Tournament,
    GameNight,
    Workshop,
    Release
}
