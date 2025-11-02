using BoardGameCafe.Api.Data;
using BoardGameCafe.Domain;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BoardGameCafe.Api.Features.Customers;

public static class CustomersEndpoints
{
    public static IEndpointRouteBuilder MapCustomersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/customers")
            .WithTags("Customers");

        // GET /api/v1/customers/me
        group.MapGet("/me", GetCustomerProfile)
            .WithName("GetCustomerProfile")
            .WithSummary("Get current customer profile")
            .Produces<CustomerDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        // PUT /api/v1/customers/me
        group.MapPut("/me", UpdateCustomerProfile)
            .WithName("UpdateCustomerProfile")
            .WithSummary("Update customer profile")
            .Produces<CustomerDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        // GET /api/v1/customers/me/loyalty-points
        group.MapGet("/me/loyalty-points", GetLoyaltyPoints)
            .WithName("GetLoyaltyPoints")
            .WithSummary("Get loyalty points balance and tier information")
            .Produces<LoyaltyPointsDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        // GET /api/v1/customers/me/loyalty-history
        group.MapGet("/me/loyalty-history", GetLoyaltyHistory)
            .WithName("GetLoyaltyHistory")
            .WithSummary("Get loyalty points transaction history")
            .Produces<List<LoyaltyTransactionDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        // POST /api/v1/customers/me/favorites
        group.MapPost("/me/favorites", AddFavoriteGame)
            .WithName("AddFavoriteGame")
            .WithSummary("Add a game to customer's favorites")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        // DELETE /api/v1/customers/me/favorites/{gameId}
        group.MapDelete("/me/favorites/{gameId:guid}", RemoveFavoriteGame)
            .WithName("RemoveFavoriteGame")
            .WithSummary("Remove a game from customer's favorites")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        // GET /api/v1/customers/me/visit-stats
        group.MapGet("/me/visit-stats", GetVisitStats)
            .WithName("GetVisitStats")
            .WithSummary("Get customer visit statistics")
            .Produces<VisitStatsDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }

    /// <summary>
    /// Get customer profile
    /// </summary>
    private static async Task<Results<Ok<CustomerDto>, NotFound>> GetCustomerProfile(
        AppDbContext db,
        Guid customerId,
        CancellationToken ct)
    {
        var customer = await db.Customers
            .FirstOrDefaultAsync(c => c.Id == customerId, ct);

        if (customer == null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(MapToCustomerDto(customer));
    }

    /// <summary>
    /// Update customer profile
    /// </summary>
    private static async Task<Results<Ok<CustomerDto>, NotFound>> UpdateCustomerProfile(
        AppDbContext db,
        Guid customerId,
        UpdateCustomerRequest request,
        CancellationToken ct)
    {
        var customer = await db.Customers
            .FirstOrDefaultAsync(c => c.Id == customerId, ct);

        if (customer == null)
        {
            return TypedResults.NotFound();
        }

        // Update only provided fields
        if (!string.IsNullOrWhiteSpace(request.FirstName))
        {
            customer.FirstName = request.FirstName;
        }
        
        if (!string.IsNullOrWhiteSpace(request.LastName))
        {
            customer.LastName = request.LastName;
        }
        
        if (request.Phone != null)
        {
            customer.Phone = request.Phone;
        }

        await db.SaveChangesAsync(ct);

        return TypedResults.Ok(MapToCustomerDto(customer));
    }

    /// <summary>
    /// Get loyalty points balance and tier information
    /// </summary>
    private static async Task<Results<Ok<LoyaltyPointsDto>, NotFound>> GetLoyaltyPoints(
        AppDbContext db,
        Guid customerId,
        CancellationToken ct)
    {
        var customer = await db.Customers
            .FirstOrDefaultAsync(c => c.Id == customerId, ct);

        if (customer == null)
        {
            return TypedResults.NotFound();
        }

        // Check and update tier if needed
        var previousTier = customer.MembershipTier;
        UpdateCustomerTier(customer);
        
        if (previousTier != customer.MembershipTier)
        {
            await db.SaveChangesAsync(ct);
        }

        var loyaltyInfo = CalculateLoyaltyInfo(customer);
        return TypedResults.Ok(loyaltyInfo);
    }

    /// <summary>
    /// Get loyalty points transaction history
    /// </summary>
    private static async Task<Results<Ok<List<LoyaltyTransactionDto>>, NotFound>> GetLoyaltyHistory(
        AppDbContext db,
        Guid customerId,
        CancellationToken ct)
    {
        var customerExists = await db.Customers.AnyAsync(c => c.Id == customerId, ct);
        if (!customerExists)
        {
            return TypedResults.NotFound();
        }

        var transactions = await db.LoyaltyPointsHistory
            .Where(h => h.CustomerId == customerId)
            .OrderByDescending(h => h.TransactionDate)
            .ToListAsync(ct);

        var transactionDtos = transactions.Select(MapToLoyaltyTransactionDto).ToList();
        return TypedResults.Ok(transactionDtos);
    }

    /// <summary>
    /// Add a game to customer's favorites (placeholder - would need a FavoriteGames table)
    /// </summary>
    private static async Task<Results<NoContent, NotFound, BadRequest<ProblemDetails>>> AddFavoriteGame(
        AppDbContext db,
        Guid customerId,
        Guid gameId,
        CancellationToken ct)
    {
        var customer = await db.Customers.FindAsync([customerId], ct);
        if (customer == null)
        {
            return TypedResults.NotFound();
        }

        var game = await db.Games.FindAsync([gameId], ct);
        if (game == null)
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Invalid game",
                Detail = "Game not found"
            });
        }

        // Note: This is a placeholder. A full implementation would need a FavoriteGames table
        // For now, just return success
        return TypedResults.NoContent();
    }

    /// <summary>
    /// Remove a game from customer's favorites (placeholder)
    /// </summary>
    private static async Task<Results<NoContent, NotFound>> RemoveFavoriteGame(
        AppDbContext db,
        Guid customerId,
        Guid gameId,
        CancellationToken ct)
    {
        var customerExists = await db.Customers.AnyAsync(c => c.Id == customerId, ct);
        if (!customerExists)
        {
            return TypedResults.NotFound();
        }

        // Note: This is a placeholder. A full implementation would need a FavoriteGames table
        return TypedResults.NoContent();
    }

    /// <summary>
    /// Get customer visit statistics
    /// </summary>
    private static async Task<Results<Ok<VisitStatsDto>, NotFound>> GetVisitStats(
        AppDbContext db,
        Guid customerId,
        CancellationToken ct)
    {
        var customer = await db.Customers
            .FirstOrDefaultAsync(c => c.Id == customerId, ct);

        if (customer == null)
        {
            return TypedResults.NotFound();
        }

        // Get total spent and order count
        var orderStats = await db.Orders
            .Where(o => o.CustomerId == customerId && o.Status == OrderStatus.Completed)
            .GroupBy(o => o.CustomerId)
            .Select(g => new
            {
                TotalOrders = g.Count(),
                TotalSpent = g.Sum(o => o.TotalAmount)
            })
            .FirstOrDefaultAsync(ct);

        // Get games played count
        var gamesPlayed = await db.GameSessions
            .Where(gs => gs.Reservation!.CustomerId == customerId)
            .CountAsync(ct);

        var totalOrders = orderStats?.TotalOrders ?? 0;
        var totalSpent = orderStats?.TotalSpent ?? 0;
        
        var stats = new VisitStatsDto
        {
            TotalVisits = customer.TotalVisits,
            GamesPlayed = gamesPlayed,
            TotalSpent = totalSpent,
            TotalOrders = totalOrders,
            AverageOrderValue = totalOrders > 0 ? totalSpent / totalOrders : 0
        };

        return TypedResults.Ok(stats);
    }

    /// <summary>
    /// Update customer membership tier based on loyalty points
    /// None: 0 points
    /// Bronze: 1-499 points (5% discount)
    /// Silver: 500-1999 points (10% discount)
    /// Gold: 2000+ points (15% discount)
    /// </summary>
    private static void UpdateCustomerTier(Customer customer)
    {
        var newTier = customer.LoyaltyPoints switch
        {
            >= 2000 => MembershipTier.Gold,
            >= 500 => MembershipTier.Silver,
            >= 1 => MembershipTier.Bronze,
            _ => MembershipTier.None
        };

        customer.MembershipTier = newTier;
    }

    /// <summary>
    /// Calculate loyalty points information including tier progress
    /// </summary>
    private static LoyaltyPointsDto CalculateLoyaltyInfo(Customer customer)
    {
        var currentPoints = customer.LoyaltyPoints;
        var currentTier = customer.MembershipTier;
        
        decimal discountPercentage = currentTier switch
        {
            MembershipTier.Bronze => 5m,
            MembershipTier.Silver => 10m,
            MembershipTier.Gold => 15m,
            _ => 0m
        };

        int currentTierThreshold = currentTier switch
        {
            MembershipTier.Gold => 2000,
            MembershipTier.Silver => 500,
            MembershipTier.Bronze => 1,
            _ => 0
        };

        string? nextTier = currentTier switch
        {
            MembershipTier.None => "Bronze",
            MembershipTier.Bronze => "Silver",
            MembershipTier.Silver => "Gold",
            _ => null
        };

        int? nextTierThreshold = currentTier switch
        {
            MembershipTier.None => 1,
            MembershipTier.Bronze => 500,
            MembershipTier.Silver => 2000,
            _ => null
        };

        int? pointsToNextTier = nextTierThreshold.HasValue 
            ? Math.Max(0, nextTierThreshold.Value - currentPoints)
            : null;

        return new LoyaltyPointsDto
        {
            CurrentBalance = currentPoints,
            CurrentTier = currentTier.ToString(),
            DiscountPercentage = discountPercentage,
            NextTier = nextTier,
            PointsToNextTier = pointsToNextTier,
            CurrentTierThreshold = currentTierThreshold,
            NextTierThreshold = nextTierThreshold
        };
    }

    private static CustomerDto MapToCustomerDto(Customer customer)
    {
        return new CustomerDto
        {
            Id = customer.Id,
            Email = customer.Email,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Phone = customer.Phone,
            MembershipTier = customer.MembershipTier.ToString(),
            LoyaltyPoints = customer.LoyaltyPoints,
            JoinedDate = customer.JoinedDate,
            TotalVisits = customer.TotalVisits
        };
    }

    private static LoyaltyTransactionDto MapToLoyaltyTransactionDto(LoyaltyPointsHistory transaction)
    {
        return new LoyaltyTransactionDto
        {
            Id = transaction.Id,
            PointsChange = transaction.PointsChange,
            TransactionType = transaction.TransactionType.ToString(),
            TransactionDate = transaction.TransactionDate,
            Description = transaction.Description,
            OrderId = transaction.OrderId
        };
    }
}
