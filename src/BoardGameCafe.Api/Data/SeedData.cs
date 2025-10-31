using BoardGameCafe.Domain;
using Microsoft.EntityFrameworkCore;

namespace BoardGameCafe.Api.Data;

public static class SeedData
{
    public static void Initialize(AppDbContext context)
    {
        // Check if we already have games seeded
        if (context.Games.Any())
        {
            return; // Database has been seeded
        }

        var games = new[]
        {
            new Game
            {
                Title = "Catan",
                Publisher = "Catan Studio",
                MinPlayers = 3,
                MaxPlayers = 4,
                PlayTimeMinutes = 90,
                AgeRating = 10,
                Complexity = 2.3m,
                Category = GameCategory.Strategy,
                CopiesOwned = 3,
                CopiesInUse = 1,
                DailyRentalFee = 5.00m,
                Description = "Trade, build, and settle the island of Catan in this modern classic board game.",
                ImageUrl = "https://example.com/images/catan.jpg"
            },
            new Game
            {
                Title = "Ticket to Ride",
                Publisher = "Days of Wonder",
                MinPlayers = 2,
                MaxPlayers = 5,
                PlayTimeMinutes = 60,
                AgeRating = 8,
                Complexity = 1.9m,
                Category = GameCategory.Family,
                CopiesOwned = 2,
                CopiesInUse = 0,
                DailyRentalFee = 4.50m,
                Description = "Claim railway routes connecting cities throughout North America in this fast-paced game.",
                ImageUrl = "https://example.com/images/ticket-to-ride.jpg"
            },
            new Game
            {
                Title = "Pandemic",
                Publisher = "Z-Man Games",
                MinPlayers = 2,
                MaxPlayers = 4,
                PlayTimeMinutes = 45,
                AgeRating = 8,
                Complexity = 2.4m,
                Category = GameCategory.Cooperative,
                CopiesOwned = 2,
                CopiesInUse = 2,
                DailyRentalFee = 5.50m,
                Description = "Work together to cure diseases and save humanity in this cooperative strategy game.",
                ImageUrl = "https://example.com/images/pandemic.jpg"
            },
            new Game
            {
                Title = "Codenames",
                Publisher = "Czech Games",
                MinPlayers = 4,
                MaxPlayers = 8,
                PlayTimeMinutes = 15,
                AgeRating = 10,
                Complexity = 1.3m,
                Category = GameCategory.Party,
                CopiesOwned = 4,
                CopiesInUse = 1,
                DailyRentalFee = 3.00m,
                Description = "A social word game with a simple premise and challenging game play.",
                ImageUrl = "https://example.com/images/codenames.jpg"
            },
            new Game
            {
                Title = "Azul",
                Publisher = "Plan B Games",
                MinPlayers = 2,
                MaxPlayers = 4,
                PlayTimeMinutes = 40,
                AgeRating = 8,
                Complexity = 1.8m,
                Category = GameCategory.Abstract,
                CopiesOwned = 2,
                CopiesInUse = 0,
                DailyRentalFee = 4.00m,
                Description = "Draft colorful tiles and place them in your palace to score points in this beautiful abstract game.",
                ImageUrl = "https://example.com/images/azul.jpg"
            }
        };

        context.Games.AddRange(games);
        context.SaveChanges();
    }
}
