using BoardGameCafe.Api.Data;
using BoardGameCafe.Domain;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BoardGameCafe.Api.Features.Events;

public static class EventsEndpoints
{
    public static IEndpointRouteBuilder MapEventsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/events")
            .WithTags("Events");

        // GET /api/v1/events
        group.MapGet("/", GetUpcomingEvents)
            .WithName("GetUpcomingEvents")
            .WithSummary("List upcoming events")
            .Produces<List<EventDto>>(StatusCodes.Status200OK);

        // GET /api/v1/events/{id}
        group.MapGet("/{id:guid}", GetEvent)
            .WithName("GetEvent")
            .WithSummary("Get event details with participant count")
            .Produces<EventDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        // POST /api/v1/events
        group.MapPost("/", CreateEvent)
            .WithName("CreateEvent")
            .WithSummary("Create a new event (admin)")
            .Produces<EventDto>(StatusCodes.Status201Created)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        // POST /api/v1/events/{id}/register
        group.MapPost("/{id:guid}/register", RegisterForEvent)
            .WithName("RegisterForEvent")
            .WithSummary("Register customer for an event")
            .Produces<EventRegistrationDto>(StatusCodes.Status201Created)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict);

        // DELETE /api/v1/events/{id}/register
        group.MapDelete("/{id:guid}/register", CancelRegistration)
            .WithName("CancelRegistration")
            .WithSummary("Cancel registration for an event")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        // GET /api/v1/events/{id}/participants
        group.MapGet("/{id:guid}/participants", GetParticipants)
            .WithName("GetParticipants")
            .WithSummary("List event registrations (staff/admin)")
            .Produces<List<EventRegistrationDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }

    /// <summary>
    /// Get list of upcoming events
    /// </summary>
    private static async Task<Ok<List<EventDto>>> GetUpcomingEvents(
        BoardGameCafeDbContext db,
        CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var events = await db.Events
            .Include(e => e.Registrations)
            .Where(e => e.EventDate >= now)
            .OrderBy(e => e.EventDate)
            .ToListAsync(ct);

        var eventDtos = events.Select(e => new EventDto
        {
            Id = e.Id,
            Title = e.Title,
            Description = e.Description,
            EventDate = e.EventDate,
            DurationMinutes = e.DurationMinutes,
            MaxParticipants = e.MaxParticipants,
            CurrentParticipants = e.CurrentParticipants,
            TicketPrice = e.TicketPrice,
            EventType = e.EventType,
            RequiresRegistration = e.RequiresRegistration,
            ImageUrl = e.ImageUrl
        }).ToList();

        return TypedResults.Ok(eventDtos);
    }

    /// <summary>
    /// Get event details with participant count
    /// </summary>
    private static async Task<Results<Ok<EventDto>, NotFound>> GetEvent(
        Guid id,
        BoardGameCafeDbContext db,
        CancellationToken ct)
    {
        var eventEntity = await db.Events
            .Include(e => e.Registrations)
            .FirstOrDefaultAsync(e => e.Id == id, ct);

        if (eventEntity == null)
            return TypedResults.NotFound();

        var eventDto = new EventDto
        {
            Id = eventEntity.Id,
            Title = eventEntity.Title,
            Description = eventEntity.Description,
            EventDate = eventEntity.EventDate,
            DurationMinutes = eventEntity.DurationMinutes,
            MaxParticipants = eventEntity.MaxParticipants,
            CurrentParticipants = eventEntity.CurrentParticipants,
            TicketPrice = eventEntity.TicketPrice,
            EventType = eventEntity.EventType,
            RequiresRegistration = eventEntity.RequiresRegistration,
            ImageUrl = eventEntity.ImageUrl
        };

        return TypedResults.Ok(eventDto);
    }

    /// <summary>
    /// Create a new event (admin)
    /// </summary>
    private static async Task<Results<Created<EventDto>, BadRequest<ProblemDetails>>> CreateEvent(
        CreateEventRequest request,
        BoardGameCafeDbContext db,
        CancellationToken ct)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Invalid event data",
                Detail = "Title is required"
            });
        }

        if (request.EventDate <= DateTime.UtcNow)
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Invalid event data",
                Detail = "Event date must be in the future"
            });
        }

        if (request.MaxParticipants <= 0)
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Invalid event data",
                Detail = "MaxParticipants must be greater than 0"
            });
        }

        var eventEntity = new Event
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            EventDate = request.EventDate,
            DurationMinutes = request.DurationMinutes,
            MaxParticipants = request.MaxParticipants,
            TicketPrice = request.TicketPrice,
            EventType = request.EventType,
            RequiresRegistration = request.RequiresRegistration,
            ImageUrl = request.ImageUrl
        };

        db.Events.Add(eventEntity);
        await db.SaveChangesAsync(ct);

        var eventDto = new EventDto
        {
            Id = eventEntity.Id,
            Title = eventEntity.Title,
            Description = eventEntity.Description,
            EventDate = eventEntity.EventDate,
            DurationMinutes = eventEntity.DurationMinutes,
            MaxParticipants = eventEntity.MaxParticipants,
            CurrentParticipants = eventEntity.CurrentParticipants,
            TicketPrice = eventEntity.TicketPrice,
            EventType = eventEntity.EventType,
            RequiresRegistration = eventEntity.RequiresRegistration,
            ImageUrl = eventEntity.ImageUrl
        };

        return TypedResults.Created($"/api/v1/events/{eventDto.Id}", eventDto);
    }

    /// <summary>
    /// Register customer for an event with concurrency handling
    /// </summary>
    private static async Task<Results<Created<EventRegistrationDto>, NotFound, BadRequest<ProblemDetails>, Conflict<ProblemDetails>>> RegisterForEvent(
        Guid id,
        RegisterForEventRequest request,
        BoardGameCafeDbContext db,
        CancellationToken ct)
    {
        // Validate customer exists
        var customer = await db.Customers.FindAsync(new object[] { request.CustomerId }, ct);
        if (customer == null)
        {
            return TypedResults.NotFound();
        }

        // Use a transaction with serializable isolation to handle concurrency
        using var transaction = await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable, ct);
        try
        {
            // Load event with registrations
            var eventEntity = await db.Events
                .Include(e => e.Registrations)
                .FirstOrDefaultAsync(e => e.Id == id, ct);

            if (eventEntity == null)
            {
                return TypedResults.NotFound();
            }

            // Check if customer is already registered (excluding cancelled)
            var isAlreadyRegistered = eventEntity.Registrations
                .Any(r => r.CustomerId == request.CustomerId && r.Status != RegistrationStatus.Cancelled);

            if (isAlreadyRegistered)
            {
                return TypedResults.Conflict(new ProblemDetails
                {
                    Title = "Already registered",
                    Detail = "Customer is already registered for this event"
                });
            }

            // Check capacity (only count non-cancelled registrations)
            if (eventEntity.CurrentParticipants >= eventEntity.MaxParticipants)
            {
                return TypedResults.Conflict(new ProblemDetails
                {
                    Title = "Event full",
                    Detail = "This event has reached maximum capacity"
                });
            }

            var registration = new EventRegistration
            {
                Id = Guid.NewGuid(),
                EventId = id,
                CustomerId = request.CustomerId,
                RegisteredAt = DateTime.UtcNow,
                Status = RegistrationStatus.Registered,
                PaymentStatus = eventEntity.TicketPrice > 0 ? PaymentStatus.Pending : PaymentStatus.Paid
            };

            db.EventRegistrations.Add(registration);
            await db.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            var registrationDto = new EventRegistrationDto
            {
                Id = registration.Id,
                EventId = registration.EventId,
                CustomerId = registration.CustomerId,
                CustomerName = $"{customer.FirstName} {customer.LastName}",
                CustomerEmail = customer.Email,
                RegisteredAt = registration.RegisteredAt,
                Status = registration.Status,
                PaymentStatus = registration.PaymentStatus
            };

            return TypedResults.Created($"/api/v1/events/{id}/participants", registrationDto);
        }
        catch (DbUpdateException ex)
        {
            // Handle unique constraint violation (race condition)
            // Check if it's the EventId+CustomerId unique constraint
            var isUniqueConstraintViolation = ex.InnerException?.Message.Contains("UNIQUE") == true ||
                                               ex.InnerException?.Message.Contains("IX_EventRegistrations_EventId_CustomerId") == true;

            if (isUniqueConstraintViolation)
            {
                return TypedResults.Conflict(new ProblemDetails
                {
                    Title = "Already registered",
                    Detail = "Customer is already registered for this event"
                });
            }

            // For other DB exceptions, rollback and rethrow
            await transaction.RollbackAsync(ct);
            throw;
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    /// <summary>
    /// Cancel registration for an event
    /// </summary>
    private static async Task<Results<NoContent, NotFound>> CancelRegistration(
        Guid id,
        [FromQuery] Guid customerId,
        BoardGameCafeDbContext db,
        CancellationToken ct)
    {
        var registration = await db.EventRegistrations
            .FirstOrDefaultAsync(r => r.EventId == id && r.CustomerId == customerId, ct);

        if (registration == null)
        {
            return TypedResults.NotFound();
        }

        registration.Status = RegistrationStatus.Cancelled;
        await db.SaveChangesAsync(ct);

        return TypedResults.NoContent();
    }

    /// <summary>
    /// List event registrations (staff/admin)
    /// </summary>
    private static async Task<Results<Ok<List<EventRegistrationDto>>, NotFound>> GetParticipants(
        Guid id,
        BoardGameCafeDbContext db,
        CancellationToken ct)
    {
        var eventExists = await db.Events.AnyAsync(e => e.Id == id, ct);
        if (!eventExists)
        {
            return TypedResults.NotFound();
        }

        var registrations = await db.EventRegistrations
            .Include(r => r.Customer)
            .Where(r => r.EventId == id)
            .OrderBy(r => r.RegisteredAt)
            .ToListAsync(ct);

        var registrationDtos = registrations.Select(r => new EventRegistrationDto
        {
            Id = r.Id,
            EventId = r.EventId,
            CustomerId = r.CustomerId,
            CustomerName = $"{r.Customer.FirstName} {r.Customer.LastName}",
            CustomerEmail = r.Customer.Email,
            RegisteredAt = r.RegisteredAt,
            Status = r.Status,
            PaymentStatus = r.PaymentStatus
        }).ToList();

        return TypedResults.Ok(registrationDtos);
    }
}
