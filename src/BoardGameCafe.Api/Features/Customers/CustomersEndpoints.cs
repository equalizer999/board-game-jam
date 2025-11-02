using BoardGameCafe.Api.Data;
using BoardGameCafe.Domain;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BoardGameCafe.Api.Features.Customers;

public static class CustomersEndpoints
{
    // Tier thresholds and discount rates
    private const int BronzeThreshold = 0;
    private const int SilverThreshold = 500;
    private const int GoldThreshold = 2000;
    
    private const decimal BronzeDiscount = 0.05m;
    private const decimal SilverDiscount = 0.10m;
    private const decimal GoldDiscount = 0.15m;

    public static IEndpointRouteBuilder MapCustomersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/customers")
            .WithTags("Customers");

        /// <summary>
        /// Get current customer profile
        /// </summary>
        group.MapGet("/me", async Task<Results<Ok<CustomerDto>, NotFound>> (
            AppDbContext db,
            Guid customerId) =>
        {
            var customer = await db.Customers
                .Where(c => c.Id == customerId)
                .Select(c => new CustomerDto
                {
                    Id = c.Id,
                    Email = c.Email,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Phone = c.Phone,
                    MembershipTier = c.MembershipTier.ToString(),
                    LoyaltyPoints = c.LoyaltyPoints,
                    JoinedDate = c.JoinedDate,
                    TotalVisits = c.TotalVisits
                })
                .FirstOrDefaultAsync();

            return customer is null
                ? TypedResults.NotFound()
                : TypedResults.Ok(customer);
        })
        .WithName("GetCustomerProfile")
        .WithSummary("Get current customer profile")
        .WithDescription("Retrieves the profile information for the authenticated customer")
        .Produces<CustomerDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        /// <summary>
        /// Update customer profile
        /// </summary>
        group.MapPut("/me", async Task<Results<Ok<CustomerDto>, NotFound, BadRequest<ProblemDetails>>> (
            AppDbContext db,
            Guid customerId,
            [FromBody] UpdateCustomerRequest request) =>
        {
            var customer = await db.Customers.FindAsync(customerId);
            if (customer is null)
            {
                return TypedResults.NotFound();
            }

            customer.FirstName = request.FirstName;
            customer.LastName = request.LastName;
            customer.Phone = request.Phone;

            await db.SaveChangesAsync();

            var dto = new CustomerDto
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

            return TypedResults.Ok(dto);
        })
        .WithName("UpdateCustomerProfile")
        .WithSummary("Update customer profile")
        .WithDescription("Updates the profile information for the authenticated customer")
        .Produces<CustomerDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        /// <summary>
        /// Get loyalty points balance and tier information
        /// </summary>
        group.MapGet("/me/loyalty-points", async Task<Results<Ok<LoyaltyPointsDto>, NotFound>> (
            AppDbContext db,
            Guid customerId) =>
        {
            var customer = await db.Customers.FindAsync(customerId);
            if (customer is null)
            {
                return TypedResults.NotFound();
            }

            var tierInfo = GetTierInfo(customer.LoyaltyPoints);
            var dto = new LoyaltyPointsDto
            {
                CurrentPoints = customer.LoyaltyPoints,
                CurrentTier = tierInfo.Tier.ToString(),
                CurrentDiscount = tierInfo.Discount,
                NextTier = tierInfo.NextTier?.ToString(),
                PointsToNextTier = tierInfo.PointsToNextTier,
                PointsThreshold = tierInfo.Threshold
            };

            return TypedResults.Ok(dto);
        })
        .WithName("GetLoyaltyPoints")
        .WithSummary("Get loyalty points balance and tier")
        .WithDescription("Retrieves current loyalty points balance, tier information, and progress to next tier")
        .Produces<LoyaltyPointsDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        /// <summary>
        /// Get loyalty points transaction history
        /// </summary>
        group.MapGet("/me/loyalty-history", async Task<Results<Ok<List<LoyaltyTransactionDto>>, NotFound>> (
            AppDbContext db,
            Guid customerId) =>
        {
            var customer = await db.Customers.FindAsync(customerId);
            if (customer is null)
            {
                return TypedResults.NotFound();
            }

            var history = await db.LoyaltyPointsHistory
                .Where(h => h.CustomerId == customerId)
                .OrderByDescending(h => h.TransactionDate)
                .Select(h => new LoyaltyTransactionDto
                {
                    Id = h.Id,
                    PointsChange = h.PointsChange,
                    TransactionType = h.TransactionType.ToString(),
                    Description = h.Description,
                    TransactionDate = h.TransactionDate,
                    OrderId = h.OrderId
                })
                .ToListAsync();

            return TypedResults.Ok(history);
        })
        .WithName("GetLoyaltyHistory")
        .WithSummary("Get loyalty points transaction history")
        .WithDescription("Retrieves the history of all loyalty points transactions (earned, redeemed, adjustments)")
        .Produces<List<LoyaltyTransactionDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        /// <summary>
        /// Add a game to favorites
        /// </summary>
        group.MapPost("/me/favorites", async Task<Results<NoContent, NotFound, Conflict<ProblemDetails>>> (
            AppDbContext db,
            Guid customerId,
            Guid gameId) =>
        {
            var customer = await db.Customers
                .Include(c => c.FavoriteGames)
                .FirstOrDefaultAsync(c => c.Id == customerId);

            if (customer is null)
            {
                return TypedResults.NotFound();
            }

            var game = await db.Games.FindAsync(gameId);
            if (game is null)
            {
                return TypedResults.NotFound();
            }

            if (customer.FavoriteGames.Any(g => g.Id == gameId))
            {
                return TypedResults.Conflict(new ProblemDetails
                {
                    Title = "Already favorited",
                    Detail = "This game is already in your favorites"
                });
            }

            customer.FavoriteGames.Add(game);
            await db.SaveChangesAsync();

            return TypedResults.NoContent();
        })
        .WithName("AddFavoriteGame")
        .WithSummary("Add game to favorites")
        .WithDescription("Adds a game to the customer's favorite games list")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status409Conflict);

        /// <summary>
        /// Remove a game from favorites
        /// </summary>
        group.MapDelete("/me/favorites/{gameId:guid}", async Task<Results<NoContent, NotFound>> (
            AppDbContext db,
            Guid customerId,
            Guid gameId) =>
        {
            var customer = await db.Customers
                .Include(c => c.FavoriteGames)
                .FirstOrDefaultAsync(c => c.Id == customerId);

            if (customer is null)
            {
                return TypedResults.NotFound();
            }

            var game = customer.FavoriteGames.FirstOrDefault(g => g.Id == gameId);
            if (game is null)
            {
                return TypedResults.NotFound();
            }

            customer.FavoriteGames.Remove(game);
            await db.SaveChangesAsync();

            return TypedResults.NoContent();
        })
        .WithName("RemoveFavoriteGame")
        .WithSummary("Remove game from favorites")
        .WithDescription("Removes a game from the customer's favorite games list")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);

        /// <summary>
        /// Get customer visit statistics
        /// </summary>
        group.MapGet("/me/visit-stats", async Task<Results<Ok<VisitStatsDto>, NotFound>> (
            AppDbContext db,
            Guid customerId) =>
        {
            var customer = await db.Customers.FindAsync(customerId);
            if (customer is null)
            {
                return TypedResults.NotFound();
            }

            // Get games played count (unique game sessions)
            var gamesPlayed = await db.GameSessions
                .Include(gs => gs.Reservation)
                .Where(gs => gs.Reservation != null && gs.Reservation.CustomerId == customerId)
                .Select(gs => gs.GameId)
                .Distinct()
                .CountAsync();

            // Get total spending from completed orders
            var totalSpending = await db.Orders
                .Where(o => o.CustomerId == customerId && 
                           (o.Status == OrderStatus.Completed || o.Status == OrderStatus.Delivered))
                .SumAsync(o => o.TotalAmount);

            // Get last visit from most recent reservation
            var lastVisit = await db.Reservations
                .Where(r => r.CustomerId == customerId)
                .OrderByDescending(r => r.ReservationDate)
                .Select(r => (DateTime?)r.ReservationDate)
                .FirstOrDefaultAsync();

            var stats = new VisitStatsDto
            {
                TotalVisits = customer.TotalVisits,
                GamesPlayed = gamesPlayed,
                TotalSpending = totalSpending,
                LastVisit = lastVisit
            };

            return TypedResults.Ok(stats);
        })
        .WithName("GetVisitStats")
        .WithSummary("Get customer visit statistics")
        .WithDescription("Retrieves statistics including total visits, games played, and total spending")
        .Produces<VisitStatsDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        return app;
    }

    /// <summary>
    /// Helper method to calculate tier information based on points
    /// </summary>
    private static (MembershipTier Tier, decimal Discount, MembershipTier? NextTier, int? PointsToNextTier, int Threshold) GetTierInfo(int points)
    {
        if (points >= GoldThreshold)
        {
            return (MembershipTier.Gold, GoldDiscount, null, null, GoldThreshold);
        }
        else if (points >= SilverThreshold)
        {
            return (MembershipTier.Silver, SilverDiscount, MembershipTier.Gold, GoldThreshold - points, SilverThreshold);
        }
        else if (points > BronzeThreshold)
        {
            return (MembershipTier.Bronze, BronzeDiscount, MembershipTier.Silver, SilverThreshold - points, BronzeThreshold);
        }
        else
        {
            return (MembershipTier.None, 0m, MembershipTier.Bronze, 1 - points, BronzeThreshold);
        }
    }

    /// <summary>
    /// Helper method to update customer tier based on loyalty points
    /// </summary>
    public static void UpdateCustomerTier(Customer customer)
    {
        var tierInfo = GetTierInfo(customer.LoyaltyPoints);
        customer.MembershipTier = tierInfo.Tier;
    }
}
