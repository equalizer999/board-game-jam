using BoardGameCafe.Domain;

namespace BoardGameCafe.Api.Features.Events;

public record EventDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public DateTime EventDate { get; init; }
    public int DurationMinutes { get; init; }
    public int MaxParticipants { get; init; }
    public int CurrentParticipants { get; init; }
    public decimal TicketPrice { get; init; }
    public EventType EventType { get; init; }
    public bool RequiresRegistration { get; init; }
    public string? ImageUrl { get; init; }
}

public record CreateEventRequest
{
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public DateTime EventDate { get; init; }
    public int DurationMinutes { get; init; }
    public int MaxParticipants { get; init; }
    public decimal TicketPrice { get; init; }
    public EventType EventType { get; init; }
    public bool RequiresRegistration { get; init; }
    public string? ImageUrl { get; init; }
}

public record RegisterForEventRequest
{
    public Guid CustomerId { get; init; }
}

public record EventRegistrationDto
{
    public Guid Id { get; init; }
    public Guid EventId { get; init; }
    public Guid CustomerId { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public string CustomerEmail { get; init; } = string.Empty;
    public DateTime RegisteredAt { get; init; }
    public RegistrationStatus Status { get; init; }
    public PaymentStatus PaymentStatus { get; init; }
}
