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
            BoardGameCafeDbContext db,
            Guid customerId) =>
        {
            var customer = await db.Customers.FindAsync(customerId);

            if (customer is null)
            {
                return TypedResults.NotFound();
            }

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
        .WithName("GetCustomerProfile")
        .WithSummary("Get current customer profile")
        .WithDescription("Retrieves the profile information for the authenticated customer")
        .Produces<CustomerDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        /// <summary>
        /// Update customer profile
        /// </summary>
        group.MapPut("/me", async Task<Results<Ok<CustomerDto>, NotFound, BadRequest<ProblemDetails>>> (
            BoardGameCafeDbContext db,
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
            BoardGameCafeDbContext db,
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
                CurrentBalance = customer.LoyaltyPoints,
                CurrentTier = tierInfo.Tier.ToString(),
                DiscountPercentage = tierInfo.Discount,
                NextTier = tierInfo.NextTier?.ToString(),
                PointsToNextTier = tierInfo.PointsToNextTier,
                CurrentTierThreshold = tierInfo.Threshold,
                NextTierThreshold = tierInfo.NextTierThreshold
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
            BoardGameCafeDbContext db,
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
                .ToListAsync();

            var dtos = history.Select(h => new LoyaltyTransactionDto
            {
                Id = h.Id,
                PointsChange = h.PointsChange,
                TransactionType = h.TransactionType.ToString(),
                Description = h.Description,
                TransactionDate = h.TransactionDate,
                OrderId = h.OrderId
            }).ToList();

            return TypedResults.Ok(dtos);
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
            BoardGameCafeDbContext db,
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
            BoardGameCafeDbContext db,
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
            BoardGameCafeDbContext db,
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
            var orderStats = await db.Orders
                .Where(o => o.CustomerId == customerId &&
                           (o.Status == OrderStatus.Completed || o.Status == OrderStatus.Delivered))
                .GroupBy(o => o.CustomerId)
                .Select(g => new
                {
                    TotalSpent = g.Sum(o => o.TotalAmount),
                    TotalOrders = g.Count()
                })
                .FirstOrDefaultAsync();

            var totalSpent = orderStats?.TotalSpent ?? 0;
            var totalOrders = orderStats?.TotalOrders ?? 0;
            var averageOrderValue = totalOrders > 0 ? totalSpent / totalOrders : 0;

            var stats = new VisitStatsDto
            {
                TotalVisits = customer.TotalVisits,
                GamesPlayed = gamesPlayed,
                TotalSpent = totalSpent,
                TotalOrders = totalOrders,
                AverageOrderValue = averageOrderValue
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
    private static (MembershipTier Tier, decimal Discount, MembershipTier? NextTier, int? PointsToNextTier, int Threshold, int? NextTierThreshold) GetTierInfo(int points)
    {
        if (points >= GoldThreshold)
        {
            return (MembershipTier.Gold, GoldDiscount, null, null, GoldThreshold, null);
        }
        else if (points >= SilverThreshold)
        {
            return (MembershipTier.Silver, SilverDiscount, MembershipTier.Gold, GoldThreshold - points, SilverThreshold, GoldThreshold);
        }
        else if (points > BronzeThreshold)
        {
            return (MembershipTier.Bronze, BronzeDiscount, MembershipTier.Silver, SilverThreshold - points, BronzeThreshold, SilverThreshold);
        }
        else
        {
            return (MembershipTier.None, 0m, MembershipTier.Bronze, 1 - points, BronzeThreshold, 1);
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
