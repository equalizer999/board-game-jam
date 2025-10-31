using BoardGameCafe.Api.Data;
using BoardGameCafe.Domain;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BoardGameCafe.Api.Features.Reservations;

public static class ReservationsEndpoints
{
    private static readonly TimeSpan ReservationBuffer = TimeSpan.FromMinutes(15);

    public static IEndpointRouteBuilder MapReservationsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/reservations")
            .WithTags("Reservations");

        // GET /api/v1/reservations?customerId={id}
        group.MapGet("/", GetReservations)
            .WithName("GetReservations")
            .WithSummary("List reservations for a customer")
            .Produces<List<ReservationDto>>(StatusCodes.Status200OK);

        // GET /api/v1/reservations/{id}
        group.MapGet("/{id:guid}", GetReservation)
            .WithName("GetReservation")
            .WithSummary("Get a single reservation by ID")
            .Produces<ReservationDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        // POST /api/v1/reservations
        group.MapPost("/", CreateReservation)
            .WithName("CreateReservation")
            .WithSummary("Create a new reservation")
            .Produces<ReservationDto>(StatusCodes.Status201Created)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict);

        // PUT /api/v1/reservations/{id}
        group.MapPut("/{id:guid}", UpdateReservation)
            .WithName("UpdateReservation")
            .WithSummary("Update an existing reservation")
            .Produces<ReservationDto>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status409Conflict);

        // DELETE /api/v1/reservations/{id}
        group.MapDelete("/{id:guid}", CancelReservation)
            .WithName("CancelReservation")
            .WithSummary("Cancel a reservation")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        // POST /api/v1/reservations/{id}/check-in
        group.MapPost("/{id:guid}/check-in", CheckInReservation)
            .WithName("CheckInReservation")
            .WithSummary("Mark a reservation as checked in")
            .Produces<ReservationDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        // GET /api/v1/reservations/availability
        group.MapGet("/availability", GetAvailability)
            .WithName("GetAvailability")
            .WithSummary("Query available tables by date, time, and party size")
            .Produces<List<AvailableTableDto>>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        return app;
    }

    /// <summary>
    /// List all reservations for a specific customer
    /// </summary>
    private static async Task<Ok<List<ReservationDto>>> GetReservations(
        AppDbContext db,
        Guid customerId,
        CancellationToken ct)
    {
        var reservations = await db.Reservations
            .Include(r => r.Customer)
            .Include(r => r.Table)
            .Where(r => r.CustomerId == customerId)
            .OrderByDescending(r => r.ReservationDate)
            .ThenByDescending(r => r.StartTime)
            .Select(r => ToDto(r))
            .ToListAsync(ct);

        return TypedResults.Ok(reservations);
    }

    /// <summary>
    /// Get a single reservation by ID
    /// </summary>
    private static async Task<Results<Ok<ReservationDto>, NotFound>> GetReservation(
        AppDbContext db,
        Guid id,
        CancellationToken ct)
    {
        var reservation = await db.Reservations
            .Include(r => r.Customer)
            .Include(r => r.Table)
            .FirstOrDefaultAsync(r => r.Id == id, ct);

        if (reservation is null)
            return TypedResults.NotFound();

        return TypedResults.Ok(ToDto(reservation));
    }

    /// <summary>
    /// Create a new reservation with validation
    /// </summary>
    private static async Task<Results<Created<ReservationDto>, BadRequest<ProblemDetails>, Conflict<ProblemDetails>>> CreateReservation(
        AppDbContext db,
        CreateReservationRequest request,
        CancellationToken ct)
    {
        // Validate request
        var validationResult = await ValidateReservationRequest(db, request, ct);
        if (validationResult is not null)
            return TypedResults.BadRequest(validationResult);

        // Check for conflicts
        var conflictResult = await CheckReservationConflicts(
            db, 
            request.TableId, 
            request.ReservationDate, 
            request.StartTime, 
            request.EndTime,
            null,
            ct);
        
        if (conflictResult is not null)
            return TypedResults.Conflict(conflictResult);

        // Create reservation
        var reservation = new Reservation
        {
            Id = Guid.NewGuid(),
            CustomerId = request.CustomerId,
            TableId = request.TableId,
            ReservationDate = request.ReservationDate.Date,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            PartySize = request.PartySize,
            Status = ReservationStatus.Confirmed,
            CreatedAt = DateTime.UtcNow,
            SpecialRequests = request.SpecialRequests
        };

        db.Reservations.Add(reservation);
        await db.SaveChangesAsync(ct);

        // Reload with navigation properties
        var created = await db.Reservations
            .Include(r => r.Customer)
            .Include(r => r.Table)
            .FirstAsync(r => r.Id == reservation.Id, ct);

        return TypedResults.Created($"/api/v1/reservations/{created.Id}", ToDto(created));
    }

    /// <summary>
    /// Update an existing reservation
    /// </summary>
    private static async Task<Results<Ok<ReservationDto>, NotFound, BadRequest<ProblemDetails>, Conflict<ProblemDetails>>> UpdateReservation(
        AppDbContext db,
        Guid id,
        UpdateReservationRequest request,
        CancellationToken ct)
    {
        var reservation = await db.Reservations
            .Include(r => r.Customer)
            .Include(r => r.Table)
            .FirstOrDefaultAsync(r => r.Id == id, ct);

        if (reservation is null)
            return TypedResults.NotFound();

        // Update fields if provided
        var tableId = request.TableId ?? reservation.TableId;
        var date = request.ReservationDate?.Date ?? reservation.ReservationDate;
        var startTime = request.StartTime ?? reservation.StartTime;
        var endTime = request.EndTime ?? reservation.EndTime;
        var partySize = request.PartySize ?? reservation.PartySize;

        // Validate the update
        var validationResult = await ValidateReservationUpdate(
            db, 
            tableId, 
            date, 
            startTime, 
            endTime, 
            partySize,
            ct);
        
        if (validationResult is not null)
            return TypedResults.BadRequest(validationResult);

        // Check for conflicts (exclude current reservation)
        var conflictResult = await CheckReservationConflicts(
            db,
            tableId,
            date,
            startTime,
            endTime,
            id,
            ct);
        
        if (conflictResult is not null)
            return TypedResults.Conflict(conflictResult);

        // Apply updates
        reservation.TableId = tableId;
        reservation.ReservationDate = date;
        reservation.StartTime = startTime;
        reservation.EndTime = endTime;
        reservation.PartySize = partySize;
        if (request.SpecialRequests is not null)
            reservation.SpecialRequests = request.SpecialRequests;

        await db.SaveChangesAsync(ct);

        // Reload to get updated navigation properties
        var updated = await db.Reservations
            .Include(r => r.Customer)
            .Include(r => r.Table)
            .FirstAsync(r => r.Id == id, ct);

        return TypedResults.Ok(ToDto(updated));
    }

    /// <summary>
    /// Cancel a reservation
    /// </summary>
    private static async Task<Results<NoContent, NotFound>> CancelReservation(
        AppDbContext db,
        Guid id,
        CancellationToken ct)
    {
        var reservation = await db.Reservations.FindAsync([id], ct);

        if (reservation is null)
            return TypedResults.NotFound();

        reservation.Status = ReservationStatus.Cancelled;
        await db.SaveChangesAsync(ct);

        return TypedResults.NoContent();
    }

    /// <summary>
    /// Check in a reservation
    /// </summary>
    private static async Task<Results<Ok<ReservationDto>, NotFound, BadRequest<ProblemDetails>>> CheckInReservation(
        AppDbContext db,
        Guid id,
        CancellationToken ct)
    {
        var reservation = await db.Reservations
            .Include(r => r.Customer)
            .Include(r => r.Table)
            .FirstOrDefaultAsync(r => r.Id == id, ct);

        if (reservation is null)
            return TypedResults.NotFound();

        if (reservation.Status == ReservationStatus.Cancelled)
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Cannot check in cancelled reservation",
                Detail = "This reservation has been cancelled and cannot be checked in.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        if (reservation.Status == ReservationStatus.CheckedIn)
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Already checked in",
                Detail = "This reservation is already checked in.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        reservation.Status = ReservationStatus.CheckedIn;
        await db.SaveChangesAsync(ct);

        return TypedResults.Ok(ToDto(reservation));
    }

    /// <summary>
    /// Query available tables
    /// </summary>
    private static async Task<Results<Ok<List<AvailableTableDto>>, BadRequest<ProblemDetails>>> GetAvailability(
        AppDbContext db,
        DateTime date,
        TimeSpan startTime,
        TimeSpan endTime,
        int partySize,
        CancellationToken ct)
    {
        // Validate input
        if (date.Date < DateTime.UtcNow.Date)
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Invalid date",
                Detail = "Reservation date must be today or in the future.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        if (startTime >= endTime)
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Invalid time range",
                Detail = "Start time must be before end time.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        if (partySize < 1)
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Invalid party size",
                Detail = "Party size must be at least 1.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        // Get all tables with sufficient capacity
        var suitableTables = await db.Tables
            .Where(t => t.SeatingCapacity >= partySize && t.Status == TableStatus.Available)
            .ToListAsync(ct);

        // Get existing reservations for this date
        var dateOnly = date.Date;
        var existingReservations = await db.Reservations
            .Where(r => r.ReservationDate == dateOnly 
                && r.Status != ReservationStatus.Cancelled 
                && r.Status != ReservationStatus.NoShow)
            .ToListAsync(ct);

        // Filter out tables with conflicts
        var availableTables = new List<AvailableTableDto>();
        
        foreach (var table in suitableTables)
        {
            var hasConflict = existingReservations
                .Where(r => r.TableId == table.Id)
                .Any(r => HasTimeOverlap(startTime, endTime, r.StartTime, r.EndTime));

            if (!hasConflict)
            {
                var duration = endTime - startTime;
                var hours = (decimal)duration.TotalHours;
                
                availableTables.Add(new AvailableTableDto
                {
                    Id = table.Id,
                    TableNumber = table.TableNumber,
                    SeatingCapacity = table.SeatingCapacity,
                    IsWindowSeat = table.IsWindowSeat,
                    IsAccessible = table.IsAccessible,
                    HourlyRate = table.HourlyRate,
                    TotalPrice = Math.Round(table.HourlyRate * hours, 2)
                });
            }
        }

        return TypedResults.Ok(availableTables.OrderBy(t => t.TotalPrice).ToList());
    }

    // Helper methods

    private static ReservationDto ToDto(Reservation reservation)
    {
        return new ReservationDto
        {
            Id = reservation.Id,
            CustomerId = reservation.CustomerId,
            TableId = reservation.TableId,
            ReservationDate = reservation.ReservationDate,
            StartTime = reservation.StartTime,
            EndTime = reservation.EndTime,
            PartySize = reservation.PartySize,
            Status = reservation.Status.ToString(),
            CreatedAt = reservation.CreatedAt,
            SpecialRequests = reservation.SpecialRequests,
            TableNumber = reservation.Table?.TableNumber ?? string.Empty,
            CustomerName = reservation.Customer != null 
                ? $"{reservation.Customer.FirstName} {reservation.Customer.LastName}" 
                : string.Empty
        };
    }

    private static async Task<ProblemDetails?> ValidateReservationRequest(
        AppDbContext db,
        CreateReservationRequest request,
        CancellationToken ct)
    {
        // Check if date is in the future
        if (request.ReservationDate.Date < DateTime.UtcNow.Date)
        {
            return new ProblemDetails
            {
                Title = "Invalid reservation date",
                Detail = "Reservation date must be today or in the future.",
                Status = StatusCodes.Status400BadRequest
            };
        }

        // Check if start time is before end time
        if (request.StartTime >= request.EndTime)
        {
            return new ProblemDetails
            {
                Title = "Invalid time range",
                Detail = "Start time must be before end time.",
                Status = StatusCodes.Status400BadRequest
            };
        }

        // Check if customer exists
        var customerExists = await db.Customers.AnyAsync(c => c.Id == request.CustomerId, ct);
        if (!customerExists)
        {
            return new ProblemDetails
            {
                Title = "Customer not found",
                Detail = $"Customer with ID {request.CustomerId} does not exist.",
                Status = StatusCodes.Status400BadRequest
            };
        }

        // Check if table exists and has sufficient capacity
        var table = await db.Tables.FindAsync([request.TableId], ct);
        if (table is null)
        {
            return new ProblemDetails
            {
                Title = "Table not found",
                Detail = $"Table with ID {request.TableId} does not exist.",
                Status = StatusCodes.Status400BadRequest
            };
        }

        if (request.PartySize > table.SeatingCapacity)
        {
            return new ProblemDetails
            {
                Title = "Party size exceeds table capacity",
                Detail = $"Table {table.TableNumber} has a capacity of {table.SeatingCapacity}, but party size is {request.PartySize}.",
                Status = StatusCodes.Status400BadRequest
            };
        }

        if (request.PartySize < 1)
        {
            return new ProblemDetails
            {
                Title = "Invalid party size",
                Detail = "Party size must be at least 1.",
                Status = StatusCodes.Status400BadRequest
            };
        }

        return null;
    }

    private static async Task<ProblemDetails?> ValidateReservationUpdate(
        AppDbContext db,
        Guid tableId,
        DateTime date,
        TimeSpan startTime,
        TimeSpan endTime,
        int partySize,
        CancellationToken ct)
    {
        // Check if date is in the future
        if (date.Date < DateTime.UtcNow.Date)
        {
            return new ProblemDetails
            {
                Title = "Invalid reservation date",
                Detail = "Reservation date must be today or in the future.",
                Status = StatusCodes.Status400BadRequest
            };
        }

        // Check if start time is before end time
        if (startTime >= endTime)
        {
            return new ProblemDetails
            {
                Title = "Invalid time range",
                Detail = "Start time must be before end time.",
                Status = StatusCodes.Status400BadRequest
            };
        }

        // Check table capacity
        var table = await db.Tables.FindAsync([tableId], ct);
        if (table is null)
        {
            return new ProblemDetails
            {
                Title = "Table not found",
                Detail = $"Table with ID {tableId} does not exist.",
                Status = StatusCodes.Status400BadRequest
            };
        }

        if (partySize > table.SeatingCapacity)
        {
            return new ProblemDetails
            {
                Title = "Party size exceeds table capacity",
                Detail = $"Table {table.TableNumber} has a capacity of {table.SeatingCapacity}, but party size is {partySize}.",
                Status = StatusCodes.Status400BadRequest
            };
        }

        if (partySize < 1)
        {
            return new ProblemDetails
            {
                Title = "Invalid party size",
                Detail = "Party size must be at least 1.",
                Status = StatusCodes.Status400BadRequest
            };
        }

        return null;
    }

    private static async Task<ProblemDetails?> CheckReservationConflicts(
        AppDbContext db,
        Guid tableId,
        DateTime date,
        TimeSpan startTime,
        TimeSpan endTime,
        Guid? excludeReservationId,
        CancellationToken ct)
    {
        var dateOnly = date.Date;
        
        // Query for overlapping reservations with buffer
        var conflictingReservations = await db.Reservations
            .Where(r => r.TableId == tableId
                && r.ReservationDate == dateOnly
                && r.Status != ReservationStatus.Cancelled
                && r.Status != ReservationStatus.NoShow
                && (excludeReservationId == null || r.Id != excludeReservationId))
            .ToListAsync(ct);

        var hasConflict = conflictingReservations.Any(r => 
            HasTimeOverlap(startTime, endTime, r.StartTime, r.EndTime));

        if (hasConflict)
        {
            return new ProblemDetails
            {
                Title = "Reservation conflict",
                Detail = "The requested time slot conflicts with an existing reservation for this table (including 15-minute buffer).",
                Status = StatusCodes.Status409Conflict
            };
        }

        return null;
    }

    private static bool HasTimeOverlap(TimeSpan start1, TimeSpan end1, TimeSpan start2, TimeSpan end2)
    {
        // Add buffer to both reservations
        var bufferedStart1 = start1 > ReservationBuffer ? start1 - ReservationBuffer : TimeSpan.Zero;
        var bufferedEnd1 = end1 + ReservationBuffer;
        
        var bufferedStart2 = start2 > ReservationBuffer ? start2 - ReservationBuffer : TimeSpan.Zero;
        var bufferedEnd2 = end2 + ReservationBuffer;

        // Check for overlap
        return bufferedStart1 < bufferedEnd2 && bufferedStart2 < bufferedEnd1;
    }
}
