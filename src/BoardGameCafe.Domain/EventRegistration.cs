namespace BoardGameCafe.Domain;

public class EventRegistration
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public Event Event { get; set; } = null!;
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public DateTime RegisteredAt { get; set; }
    public RegistrationStatus Status { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
}

public enum RegistrationStatus
{
    Registered,
    Attended,
    Cancelled
}

public enum PaymentStatus
{
    Pending,
    Paid,
    Refunded
}
