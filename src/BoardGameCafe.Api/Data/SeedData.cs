using BoardGameCafe.Domain;
using Microsoft.EntityFrameworkCore;

namespace BoardGameCafe.Api.Data;

public static class SeedData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using var context = new BoardGameCafeDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<BoardGameCafeDbContext>>());

        // Check if already seeded - make it more robust
        if (context.Games.Any() || context.Tables.Any() || context.Customers.Any())
        {
            return; // Database has been seeded
        }

        SeedGames(context);
        SeedTables(context);
        SeedCustomers(context);
        SeedMenuItems(context);
        SeedEvents(context);
        SeedReservations(context);
        SeedSampleOrders(context);

        context.SaveChanges();
    }

    private static void SeedGames(BoardGameCafeDbContext context)
    {
        var games = new List<Game>
        {
            // Strategy Games
            new Game { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Title = "Catan", Publisher = "Catan Studio", MinPlayers = 3, MaxPlayers = 4, PlayTimeMinutes = 90, AgeRating = 10, Complexity = 2.3m, Category = GameCategory.Strategy, CopiesOwned = 3, CopiesInUse = 0, DailyRentalFee = 5.00m, Description = "Collect resources to build roads and settlements on the island of Catan" },
            new Game { Id = Guid.Parse("11111111-1111-1111-1111-111111111112"), Title = "Ticket to Ride", Publisher = "Days of Wonder", MinPlayers = 2, MaxPlayers = 5, PlayTimeMinutes = 60, AgeRating = 8, Complexity = 1.9m, Category = GameCategory.Strategy, CopiesOwned = 2, CopiesInUse = 0, DailyRentalFee = 5.00m, Description = "Build train routes across North America to complete destination tickets" },
            new Game { Id = Guid.Parse("11111111-1111-1111-1111-111111111113"), Title = "Wingspan", Publisher = "Stonemaier Games", MinPlayers = 1, MaxPlayers = 5, PlayTimeMinutes = 70, AgeRating = 10, Complexity = 2.4m, Category = GameCategory.Strategy, CopiesOwned = 2, CopiesInUse = 0, DailyRentalFee = 6.00m, Description = "Attract birds to your wildlife preserve in this engine-building game" },
            new Game { Id = Guid.Parse("11111111-1111-1111-1111-111111111114"), Title = "Terraforming Mars", Publisher = "FryxGames", MinPlayers = 1, MaxPlayers = 5, PlayTimeMinutes = 120, AgeRating = 12, Complexity = 3.2m, Category = GameCategory.Strategy, CopiesOwned = 2, CopiesInUse = 0, DailyRentalFee = 7.00m, Description = "Compete to terraform Mars and make it habitable" },
            new Game { Id = Guid.Parse("11111111-1111-1111-1111-111111111115"), Title = "Brass Birmingham", Publisher = "Roxley Games", MinPlayers = 2, MaxPlayers = 4, PlayTimeMinutes = 120, AgeRating = 14, Complexity = 3.9m, Category = GameCategory.Strategy, CopiesOwned = 1, CopiesInUse = 0, DailyRentalFee = 7.00m, Description = "Build industries and networks in the Industrial Revolution" },
            new Game { Id = Guid.Parse("11111111-1111-1111-1111-111111111116"), Title = "Scythe", Publisher = "Stonemaier Games", MinPlayers = 1, MaxPlayers = 5, PlayTimeMinutes = 115, AgeRating = 14, Complexity = 3.4m, Category = GameCategory.Strategy, CopiesOwned = 2, CopiesInUse = 0, DailyRentalFee = 7.00m, Description = "Control factions in an alternate-history 1920s Europa" },
            new Game { Id = Guid.Parse("11111111-1111-1111-1111-111111111117"), Title = "7 Wonders", Publisher = "Repos Production", MinPlayers = 2, MaxPlayers = 7, PlayTimeMinutes = 30, AgeRating = 10, Complexity = 2.3m, Category = GameCategory.Strategy, CopiesOwned = 2, CopiesInUse = 0, DailyRentalFee = 5.00m, Description = "Build an ancient civilization through card drafting" },
            new Game { Id = Guid.Parse("11111111-1111-1111-1111-111111111118"), Title = "Agricola", Publisher = "Lookout Games", MinPlayers = 1, MaxPlayers = 4, PlayTimeMinutes = 90, AgeRating = 12, Complexity = 3.6m, Category = GameCategory.Strategy, CopiesOwned = 1, CopiesInUse = 0, DailyRentalFee = 6.00m, Description = "Develop your farm in 17th century Europe" },
            new Game { Id = Guid.Parse("11111111-1111-1111-1111-111111111119"), Title = "Puerto Rico", Publisher = "Ravensburger", MinPlayers = 2, MaxPlayers = 5, PlayTimeMinutes = 120, AgeRating = 12, Complexity = 3.3m, Category = GameCategory.Strategy, CopiesOwned = 1, CopiesInUse = 0, DailyRentalFee = 6.00m, Description = "Build plantations and ship goods in colonial Puerto Rico" },
            new Game { Id = Guid.Parse("11111111-1111-1111-1111-111111111120"), Title = "Power Grid", Publisher = "Rio Grande Games", MinPlayers = 2, MaxPlayers = 6, PlayTimeMinutes = 120, AgeRating = 12, Complexity = 3.3m, Category = GameCategory.Strategy, CopiesOwned = 1, CopiesInUse = 0, DailyRentalFee = 6.00m, Description = "Bid on power plants and supply electricity to cities" },
            new Game { Id = Guid.Parse("11111111-1111-1111-1111-111111111121"), Title = "Viticulture", Publisher = "Stonemaier Games", MinPlayers = 1, MaxPlayers = 6, PlayTimeMinutes = 90, AgeRating = 13, Complexity = 2.9m, Category = GameCategory.Strategy, CopiesOwned = 2, CopiesInUse = 0, DailyRentalFee = 6.00m, Description = "Create the most successful winery in Tuscany" },
            new Game { Id = Guid.Parse("11111111-1111-1111-1111-111111111122"), Title = "Everdell", Publisher = "Starling Games", MinPlayers = 1, MaxPlayers = 4, PlayTimeMinutes = 80, AgeRating = 13, Complexity = 2.8m, Category = GameCategory.Strategy, CopiesOwned = 2, CopiesInUse = 0, DailyRentalFee = 6.00m, Description = "Build a city of critters in a charming woodland" },
            new Game { Id = Guid.Parse("11111111-1111-1111-1111-111111111123"), Title = "Concordia", Publisher = "Rio Grande Games", MinPlayers = 2, MaxPlayers = 5, PlayTimeMinutes = 100, AgeRating = 13, Complexity = 3.0m, Category = GameCategory.Strategy, CopiesOwned = 1, CopiesInUse = 0, DailyRentalFee = 6.00m, Description = "Trade and build in the Roman Empire" },
            new Game { Id = Guid.Parse("11111111-1111-1111-1111-111111111124"), Title = "Great Western Trail", Publisher = "Stronghold Games", MinPlayers = 2, MaxPlayers = 4, PlayTimeMinutes = 120, AgeRating = 12, Complexity = 3.7m, Category = GameCategory.Strategy, CopiesOwned = 1, CopiesInUse = 0, DailyRentalFee = 7.00m, Description = "Herd cattle from Texas to Kansas City" },
            new Game { Id = Guid.Parse("11111111-1111-1111-1111-111111111125"), Title = "Ark Nova", Publisher = "Feuerland Spiele", MinPlayers = 1, MaxPlayers = 4, PlayTimeMinutes = 150, AgeRating = 14, Complexity = 3.7m, Category = GameCategory.Strategy, CopiesOwned = 1, CopiesInUse = 0, DailyRentalFee = 7.00m, Description = "Build and manage a modern zoo" },

            // Party Games
            new Game { Id = Guid.Parse("22222222-2222-2222-2222-222222222221"), Title = "Codenames", Publisher = "Czech Games Edition", MinPlayers = 2, MaxPlayers = 8, PlayTimeMinutes = 15, AgeRating = 10, Complexity = 1.3m, Category = GameCategory.Party, CopiesOwned = 3, CopiesInUse = 0, DailyRentalFee = 4.00m, Description = "Give one-word clues to find your team's secret agents" },
            new Game { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Title = "Wavelength", Publisher = "CMYK", MinPlayers = 2, MaxPlayers = 12, PlayTimeMinutes = 30, AgeRating = 14, Complexity = 1.1m, Category = GameCategory.Party, CopiesOwned = 2, CopiesInUse = 0, DailyRentalFee = 4.00m, Description = "A party game about finding your wavelength with friends" },
            new Game { Id = Guid.Parse("22222222-2222-2222-2222-222222222223"), Title = "Telestrations", Publisher = "USAopoly", MinPlayers = 4, MaxPlayers = 8, PlayTimeMinutes = 30, AgeRating = 12, Complexity = 1.1m, Category = GameCategory.Party, CopiesOwned = 2, CopiesInUse = 0, DailyRentalFee = 4.00m, Description = "The telephone game meets Pictionary" },
            new Game { Id = Guid.Parse("22222222-2222-2222-2222-222222222224"), Title = "Dixit", Publisher = "Libellud", MinPlayers = 3, MaxPlayers = 6, PlayTimeMinutes = 30, AgeRating = 8, Complexity = 1.2m, Category = GameCategory.Party, CopiesOwned = 2, CopiesInUse = 0, DailyRentalFee = 4.00m, Description = "Use creative clues to describe beautiful illustrated cards" },
            new Game { Id = Guid.Parse("22222222-2222-2222-2222-222222222225"), Title = "Decrypto", Publisher = "Le Scorpion Masqué", MinPlayers = 3, MaxPlayers = 8, PlayTimeMinutes = 30, AgeRating = 12, Complexity = 1.9m, Category = GameCategory.Party, CopiesOwned = 2, CopiesInUse = 0, DailyRentalFee = 4.00m, Description = "Give clues to your team while intercepting the other team's codes" },
            new Game { Id = Guid.Parse("22222222-2222-2222-2222-222222222226"), Title = "Just One", Publisher = "Repos Production", MinPlayers = 3, MaxPlayers = 7, PlayTimeMinutes = 20, AgeRating = 8, Complexity = 1.0m, Category = GameCategory.Party, CopiesOwned = 2, CopiesInUse = 0, DailyRentalFee = 3.00m, Description = "Cooperative word guessing game" },
            new Game { Id = Guid.Parse("22222222-2222-2222-2222-222222222227"), Title = "Sushi Go Party!", Publisher = "Gamewright", MinPlayers = 2, MaxPlayers = 8, PlayTimeMinutes = 20, AgeRating = 8, Complexity = 1.2m, Category = GameCategory.Party, CopiesOwned = 2, CopiesInUse = 0, DailyRentalFee = 3.00m, Description = "Draft sushi dishes to score the most points" },
            new Game { Id = Guid.Parse("22222222-2222-2222-2222-222222222228"), Title = "Concept", Publisher = "Repos Production", MinPlayers = 4, MaxPlayers = 12, PlayTimeMinutes = 40, AgeRating = 10, Complexity = 1.4m, Category = GameCategory.Party, CopiesOwned = 1, CopiesInUse = 0, DailyRentalFee = 4.00m, Description = "Use universal icons to get others to guess words and phrases" },
            new Game { Id = Guid.Parse("22222222-2222-2222-2222-222222222229"), Title = "Wits & Wagers", Publisher = "North Star Games", MinPlayers = 3, MaxPlayers = 7, PlayTimeMinutes = 25, AgeRating = 8, Complexity = 1.2m, Category = GameCategory.Party, CopiesOwned = 1, CopiesInUse = 0, DailyRentalFee = 3.00m, Description = "Bet on answers to trivia questions" },
            new Game { Id = Guid.Parse("22222222-2222-2222-2222-222222222230"), Title = "The Mind", Publisher = "Pandasaurus Games", MinPlayers = 2, MaxPlayers = 4, PlayTimeMinutes = 20, AgeRating = 8, Complexity = 1.1m, Category = GameCategory.Party, CopiesOwned = 2, CopiesInUse = 0, DailyRentalFee = 3.00m, Description = "Play cards in ascending order without communicating" },

            // Cooperative Games
            new Game { Id = Guid.Parse("33333333-3333-3333-3333-333333333331"), Title = "Pandemic", Publisher = "Z-Man Games", MinPlayers = 2, MaxPlayers = 4, PlayTimeMinutes = 45, AgeRating = 8, Complexity = 2.4m, Category = GameCategory.Cooperative, CopiesOwned = 2, CopiesInUse = 0, DailyRentalFee = 5.00m, Description = "Work together to stop global disease outbreaks" },
            new Game { Id = Guid.Parse("33333333-3333-3333-3333-333333333332"), Title = "Spirit Island", Publisher = "Greater Than Games", MinPlayers = 1, MaxPlayers = 4, PlayTimeMinutes = 120, AgeRating = 13, Complexity = 4.0m, Category = GameCategory.Cooperative, CopiesOwned = 1, CopiesInUse = 0, DailyRentalFee = 7.00m, Description = "Defend your island from colonizing invaders" },
            new Game { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Title = "The Crew: Mission Deep Sea", Publisher = "KOSMOS", MinPlayers = 2, MaxPlayers = 5, PlayTimeMinutes = 20, AgeRating = 10, Complexity = 2.0m, Category = GameCategory.Cooperative, CopiesOwned = 2, CopiesInUse = 0, DailyRentalFee = 4.00m, Description = "Cooperative trick-taking game with missions" },
            new Game { Id = Guid.Parse("33333333-3333-3333-3333-333333333334"), Title = "Forbidden Island", Publisher = "Gamewright", MinPlayers = 2, MaxPlayers = 4, PlayTimeMinutes = 30, AgeRating = 10, Complexity = 1.7m, Category = GameCategory.Cooperative, CopiesOwned = 2, CopiesInUse = 0, DailyRentalFee = 4.00m, Description = "Collect treasures and escape before the island sinks" },
            new Game { Id = Guid.Parse("33333333-3333-3333-3333-333333333335"), Title = "Gloomhaven", Publisher = "Cephalofair Games", MinPlayers = 1, MaxPlayers = 4, PlayTimeMinutes = 120, AgeRating = 14, Complexity = 3.9m, Category = GameCategory.Cooperative, CopiesOwned = 1, CopiesInUse = 0, DailyRentalFee = 8.00m, Description = "Campaign-based tactical combat in a fantasy world" },
            new Game { Id = Guid.Parse("33333333-3333-3333-3333-333333333336"), Title = "Hanabi", Publisher = "Cocktail Games", MinPlayers = 2, MaxPlayers = 5, PlayTimeMinutes = 25, AgeRating = 8, Complexity = 1.8m, Category = GameCategory.Cooperative, CopiesOwned = 2, CopiesInUse = 0, DailyRentalFee = 3.00m, Description = "Create a fireworks display with limited communication" },
            new Game { Id = Guid.Parse("33333333-3333-3333-3333-333333333337"), Title = "Forbidden Desert", Publisher = "Gamewright", MinPlayers = 2, MaxPlayers = 5, PlayTimeMinutes = 45, AgeRating = 10, Complexity = 2.0m, Category = GameCategory.Cooperative, CopiesOwned = 1, CopiesInUse = 0, DailyRentalFee = 4.00m, Description = "Survive and escape from an ancient desert city" },
            new Game { Id = Guid.Parse("33333333-3333-3333-3333-333333333338"), Title = "Horrified", Publisher = "Ravensburger", MinPlayers = 1, MaxPlayers = 5, PlayTimeMinutes = 60, AgeRating = 10, Complexity = 2.1m, Category = GameCategory.Cooperative, CopiesOwned = 1, CopiesInUse = 0, DailyRentalFee = 5.00m, Description = "Work together to defeat classic movie monsters" },
            new Game { Id = Guid.Parse("33333333-3333-3333-3333-333333333339"), Title = "Arkham Horror: The Card Game", Publisher = "Fantasy Flight Games", MinPlayers = 1, MaxPlayers = 2, PlayTimeMinutes = 90, AgeRating = 14, Complexity = 3.4m, Category = GameCategory.Cooperative, CopiesOwned = 1, CopiesInUse = 0, DailyRentalFee = 6.00m, Description = "Investigate Lovecraftian mysteries" },

            // Family Games
            new Game { Id = Guid.Parse("44444444-4444-4444-4444-444444444441"), Title = "Azul", Publisher = "Plan B Games", MinPlayers = 2, MaxPlayers = 4, PlayTimeMinutes = 40, AgeRating = 8, Complexity = 1.8m, Category = GameCategory.Family, CopiesOwned = 2, CopiesInUse = 0, DailyRentalFee = 5.00m, Description = "Draft tiles to decorate a Portuguese palace" },
            new Game { Id = Guid.Parse("44444444-4444-4444-4444-444444444442"), Title = "Splendor", Publisher = "Space Cowboys", MinPlayers = 2, MaxPlayers = 4, PlayTimeMinutes = 30, AgeRating = 10, Complexity = 1.8m, Category = GameCategory.Family, CopiesOwned = 2, CopiesInUse = 0, DailyRentalFee = 5.00m, Description = "Collect gems to purchase cards and attract nobles" },
            new Game { Id = Guid.Parse("44444444-4444-4444-4444-444444444443"), Title = "Kingdomino", Publisher = "Blue Orange Games", MinPlayers = 2, MaxPlayers = 4, PlayTimeMinutes = 15, AgeRating = 8, Complexity = 1.2m, Category = GameCategory.Family, CopiesOwned = 2, CopiesInUse = 0, DailyRentalFee = 4.00m, Description = "Build your kingdom by connecting domino tiles" },
            new Game { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), Title = "Sushi Go!", Publisher = "Gamewright", MinPlayers = 2, MaxPlayers = 5, PlayTimeMinutes = 15, AgeRating = 8, Complexity = 1.2m, Category = GameCategory.Family, CopiesOwned = 2, CopiesInUse = 0, DailyRentalFee = 3.00m, Description = "Fast-paced card drafting sushi game" },
            new Game { Id = Guid.Parse("44444444-4444-4444-4444-444444444445"), Title = "Carcassonne", Publisher = "Z-Man Games", MinPlayers = 2, MaxPlayers = 5, PlayTimeMinutes = 45, AgeRating = 7, Complexity = 1.9m, Category = GameCategory.Family, CopiesOwned = 2, CopiesInUse = 0, DailyRentalFee = 5.00m, Description = "Build a medieval landscape tile by tile" },
            new Game { Id = Guid.Parse("44444444-4444-4444-4444-444444444446"), Title = "Ticket to Ride: Europe", Publisher = "Days of Wonder", MinPlayers = 2, MaxPlayers = 5, PlayTimeMinutes = 60, AgeRating = 8, Complexity = 1.9m, Category = GameCategory.Family, CopiesOwned = 1, CopiesInUse = 0, DailyRentalFee = 5.00m, Description = "Build train routes across Europe" },
            new Game { Id = Guid.Parse("44444444-4444-4444-4444-444444444447"), Title = "Patchwork", Publisher = "Lookout Games", MinPlayers = 2, MaxPlayers = 2, PlayTimeMinutes = 30, AgeRating = 8, Complexity = 1.6m, Category = GameCategory.Family, CopiesOwned = 2, CopiesInUse = 0, DailyRentalFee = 4.00m, Description = "Create a beautiful quilt in this two-player game" },
            new Game { Id = Guid.Parse("44444444-4444-4444-4444-444444444448"), Title = "Quacks of Quedlinburg", Publisher = "Schmidt Spiele", MinPlayers = 2, MaxPlayers = 4, PlayTimeMinutes = 45, AgeRating = 10, Complexity = 1.9m, Category = GameCategory.Family, CopiesOwned = 2, CopiesInUse = 0, DailyRentalFee = 5.00m, Description = "Push your luck brewing potions" },
            new Game { Id = Guid.Parse("44444444-4444-4444-4444-444444444449"), Title = "Jaipur", Publisher = "GameWorks", MinPlayers = 2, MaxPlayers = 2, PlayTimeMinutes = 30, AgeRating = 10, Complexity = 1.5m, Category = GameCategory.Family, CopiesOwned = 2, CopiesInUse = 0, DailyRentalFee = 4.00m, Description = "Trade goods in an Indian marketplace" },
            new Game { Id = Guid.Parse("44444444-4444-4444-4444-444444444450"), Title = "Cascadia", Publisher = "Flatout Games", MinPlayers = 1, MaxPlayers = 4, PlayTimeMinutes = 45, AgeRating = 10, Complexity = 1.8m, Category = GameCategory.Family, CopiesOwned = 2, CopiesInUse = 0, DailyRentalFee = 5.00m, Description = "Create habitats for Pacific Northwest wildlife" },
            new Game { Id = Guid.Parse("44444444-4444-4444-4444-444444444451"), Title = "Century: Spice Road", Publisher = "Plan B Games", MinPlayers = 2, MaxPlayers = 5, PlayTimeMinutes = 45, AgeRating = 8, Complexity = 1.8m, Category = GameCategory.Family, CopiesOwned = 1, CopiesInUse = 0, DailyRentalFee = 4.00m, Description = "Trade spices along the Silk Road" },
            new Game { Id = Guid.Parse("44444444-4444-4444-4444-444444444452"), Title = "Reef", Publisher = "Next Move Games", MinPlayers = 2, MaxPlayers = 4, PlayTimeMinutes = 40, AgeRating = 8, Complexity = 1.7m, Category = GameCategory.Family, CopiesOwned = 1, CopiesInUse = 0, DailyRentalFee = 4.00m, Description = "Build a colorful coral reef" },

            // Abstract Games
            new Game { Id = Guid.Parse("55555555-5555-5555-5555-555555555551"), Title = "Chess", Publisher = "Public Domain", MinPlayers = 2, MaxPlayers = 2, PlayTimeMinutes = 60, AgeRating = 6, Complexity = 3.7m, Category = GameCategory.Abstract, CopiesOwned = 5, CopiesInUse = 0, DailyRentalFee = 2.00m, Description = "Classic strategic war game" },
            new Game { Id = Guid.Parse("55555555-5555-5555-5555-555555555552"), Title = "Go", Publisher = "Public Domain", MinPlayers = 2, MaxPlayers = 2, PlayTimeMinutes = 90, AgeRating = 8, Complexity = 3.9m, Category = GameCategory.Abstract, CopiesOwned = 2, CopiesInUse = 0, DailyRentalFee = 2.00m, Description = "Ancient Chinese territory control game" },
            new Game { Id = Guid.Parse("55555555-5555-5555-5555-555555555553"), Title = "Hive", Publisher = "Gen42 Games", MinPlayers = 2, MaxPlayers = 2, PlayTimeMinutes = 20, AgeRating = 9, Complexity = 2.3m, Category = GameCategory.Abstract, CopiesOwned = 2, CopiesInUse = 0, DailyRentalFee = 4.00m, Description = "Surround the enemy queen bee" },
            new Game { Id = Guid.Parse("55555555-5555-5555-5555-555555555554"), Title = "Onitama", Publisher = "Arcane Wonders", MinPlayers = 2, MaxPlayers = 2, PlayTimeMinutes = 20, AgeRating = 8, Complexity = 1.6m, Category = GameCategory.Abstract, CopiesOwned = 2, CopiesInUse = 0, DailyRentalFee = 4.00m, Description = "Elegant martial arts-themed chess variant" },
            new Game { Id = Guid.Parse("55555555-5555-5555-5555-555555555555"), Title = "Azul: Summer Pavilion", Publisher = "Plan B Games", MinPlayers = 2, MaxPlayers = 4, PlayTimeMinutes = 45, AgeRating = 8, Complexity = 2.1m, Category = GameCategory.Abstract, CopiesOwned = 1, CopiesInUse = 0, DailyRentalFee = 5.00m, Description = "Draft tiles to complete patterns" },
            new Game { Id = Guid.Parse("55555555-5555-5555-5555-555555555556"), Title = "Santorini", Publisher = "Roxley Games", MinPlayers = 2, MaxPlayers = 4, PlayTimeMinutes = 20, AgeRating = 8, Complexity = 1.7m, Category = GameCategory.Abstract, CopiesOwned = 2, CopiesInUse = 0, DailyRentalFee = 4.00m, Description = "Build towers and reach the top" },
            new Game { Id = Guid.Parse("55555555-5555-5555-5555-555555555557"), Title = "Tak", Publisher = "Cheapass Games", MinPlayers = 2, MaxPlayers = 2, PlayTimeMinutes = 30, AgeRating = 10, Complexity = 2.8m, Category = GameCategory.Abstract, CopiesOwned = 1, CopiesInUse = 0, DailyRentalFee = 3.00m, Description = "Build a road across the board" },
            new Game { Id = Guid.Parse("55555555-5555-5555-5555-555555555558"), Title = "Patchwork Doodle", Publisher = "Lookout Games", MinPlayers = 1, MaxPlayers = 6, PlayTimeMinutes = 20, AgeRating = 8, Complexity = 1.3m, Category = GameCategory.Abstract, CopiesOwned = 1, CopiesInUse = 0, DailyRentalFee = 3.00m, Description = "Draw polyomino pieces on your grid" },
            new Game { Id = Guid.Parse("55555555-5555-5555-5555-555555555559"), Title = "Checkers", Publisher = "Public Domain", MinPlayers = 2, MaxPlayers = 2, PlayTimeMinutes = 30, AgeRating = 6, Complexity = 1.9m, Category = GameCategory.Abstract, CopiesOwned = 3, CopiesInUse = 0, DailyRentalFee = 2.00m, Description = "Classic jumping and capturing game" },
            new Game { Id = Guid.Parse("55555555-5555-5555-5555-555555555560"), Title = "Blokus", Publisher = "Mattel", MinPlayers = 2, MaxPlayers = 4, PlayTimeMinutes = 30, AgeRating = 7, Complexity = 1.8m, Category = GameCategory.Abstract, CopiesOwned = 2, CopiesInUse = 0, DailyRentalFee = 4.00m, Description = "Place all your pieces on the board" },
            
            // Additional Strategy Games
            new Game { Id = Guid.Parse("11111111-1111-1111-1111-111111111126"), Title = "Dominion", Publisher = "Rio Grande Games", MinPlayers = 2, MaxPlayers = 4, PlayTimeMinutes = 30, AgeRating = 13, Complexity = 2.3m, Category = GameCategory.Strategy, CopiesOwned = 2, CopiesInUse = 0, DailyRentalFee = 5.00m, Description = "Build your deck of cards to gain the most victory points" },
            new Game { Id = Guid.Parse("11111111-1111-1111-1111-111111111127"), Title = "Five Tribes", Publisher = "Days of Wonder", MinPlayers = 2, MaxPlayers = 4, PlayTimeMinutes = 60, AgeRating = 13, Complexity = 2.8m, Category = GameCategory.Strategy, CopiesOwned = 1, CopiesInUse = 0, DailyRentalFee = 6.00m, Description = "Strategic mancala game set in Arabian Nights" },
            
            // Additional Family Games
            new Game { Id = Guid.Parse("44444444-4444-4444-4444-444444444453"), Title = "Love Letter", Publisher = "Z-Man Games", MinPlayers = 2, MaxPlayers = 6, PlayTimeMinutes = 20, AgeRating = 10, Complexity = 1.2m, Category = GameCategory.Family, CopiesOwned = 2, CopiesInUse = 0, DailyRentalFee = 3.00m, Description = "Quick card game of deduction and risk" },
            new Game { Id = Guid.Parse("44444444-4444-4444-4444-444444444454"), Title = "Skull", Publisher = "Lui-même", MinPlayers = 3, MaxPlayers = 6, PlayTimeMinutes = 15, AgeRating = 10, Complexity = 1.2m, Category = GameCategory.Family, CopiesOwned = 2, CopiesInUse = 0, DailyRentalFee = 3.00m, Description = "Bluffing game with coasters" },
            
            // Additional Party Game
            new Game { Id = Guid.Parse("22222222-2222-2222-2222-222222222231"), Title = "One Night Werewolf", Publisher = "Bezier Games", MinPlayers = 3, MaxPlayers = 10, PlayTimeMinutes = 10, AgeRating = 8, Complexity = 1.4m, Category = GameCategory.Party, CopiesOwned = 2, CopiesInUse = 0, DailyRentalFee = 4.00m, Description = "Fast-paced social deduction game" }
        };

        context.Games.AddRange(games);
    }

    private static void SeedTables(BoardGameCafeDbContext context)
    {
        var tables = new List<Table>
        {
            new Table { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), TableNumber = "T-101", SeatingCapacity = 2, IsWindowSeat = false, IsAccessible = true, HourlyRate = 10.00m, Status = TableStatus.Available },
            new Table { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaab"), TableNumber = "T-102", SeatingCapacity = 2, IsWindowSeat = true, IsAccessible = true, HourlyRate = 12.00m, Status = TableStatus.Available },
            new Table { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaac"), TableNumber = "T-103", SeatingCapacity = 4, IsWindowSeat = false, IsAccessible = true, HourlyRate = 15.00m, Status = TableStatus.Available },
            new Table { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaad"), TableNumber = "T-104", SeatingCapacity = 4, IsWindowSeat = true, IsAccessible = false, HourlyRate = 18.00m, Status = TableStatus.Available },
            new Table { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaae"), TableNumber = "T-105", SeatingCapacity = 4, IsWindowSeat = false, IsAccessible = true, HourlyRate = 15.00m, Status = TableStatus.Available },
            new Table { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaf"), TableNumber = "T-106", SeatingCapacity = 6, IsWindowSeat = false, IsAccessible = true, HourlyRate = 20.00m, Status = TableStatus.Available },
            new Table { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaab0"), TableNumber = "T-107", SeatingCapacity = 6, IsWindowSeat = true, IsAccessible = true, HourlyRate = 22.00m, Status = TableStatus.Available },
            new Table { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaab1"), TableNumber = "T-108", SeatingCapacity = 6, IsWindowSeat = false, IsAccessible = false, HourlyRate = 20.00m, Status = TableStatus.Available },
            new Table { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaab2"), TableNumber = "T-109", SeatingCapacity = 8, IsWindowSeat = false, IsAccessible = true, HourlyRate = 25.00m, Status = TableStatus.Available },
            new Table { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaab3"), TableNumber = "T-110", SeatingCapacity = 8, IsWindowSeat = true, IsAccessible = true, HourlyRate = 25.00m, Status = TableStatus.Available },
            new Table { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaab4"), TableNumber = "T-201", SeatingCapacity = 2, IsWindowSeat = false, IsAccessible = true, HourlyRate = 10.00m, Status = TableStatus.Available },
            new Table { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaab5"), TableNumber = "T-202", SeatingCapacity = 4, IsWindowSeat = false, IsAccessible = true, HourlyRate = 15.00m, Status = TableStatus.Available },
            new Table { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaab6"), TableNumber = "T-203", SeatingCapacity = 4, IsWindowSeat = true, IsAccessible = true, HourlyRate = 18.00m, Status = TableStatus.Available },
            new Table { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaab7"), TableNumber = "T-204", SeatingCapacity = 6, IsWindowSeat = false, IsAccessible = true, HourlyRate = 20.00m, Status = TableStatus.Available },
            new Table { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaab8"), TableNumber = "T-205", SeatingCapacity = 6, IsWindowSeat = true, IsAccessible = false, HourlyRate = 22.00m, Status = TableStatus.Available },
            new Table { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaab9"), TableNumber = "T-VIP", SeatingCapacity = 8, IsWindowSeat = true, IsAccessible = true, HourlyRate = 30.00m, Status = TableStatus.Available }
        };

        context.Tables.AddRange(tables);
    }

    private static void SeedCustomers(BoardGameCafeDbContext context)
    {
        var customers = new List<Customer>
        {
            new Customer { Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"), Email = "john.doe@email.com", FirstName = "John", LastName = "Doe", Phone = "555-0101", MembershipTier = MembershipTier.Gold, LoyaltyPoints = 1250, JoinedDate = DateTime.UtcNow.AddYears(-2), TotalVisits = 87 },
            new Customer { Id = Guid.Parse("cccccccc-cccc-cccc-cccc-ccccccccccc2"), Email = "jane.smith@email.com", FirstName = "Jane", LastName = "Smith", Phone = "555-0102", MembershipTier = MembershipTier.Silver, LoyaltyPoints = 650, JoinedDate = DateTime.UtcNow.AddYears(-1).AddMonths(-3), TotalVisits = 42 },
            new Customer { Id = Guid.Parse("cccccccc-cccc-cccc-cccc-ccccccccccc3"), Email = "bob.johnson@email.com", FirstName = "Bob", LastName = "Johnson", Phone = "555-0103", MembershipTier = MembershipTier.Bronze, LoyaltyPoints = 280, JoinedDate = DateTime.UtcNow.AddMonths(-8), TotalVisits = 18 },
            new Customer { Id = Guid.Parse("cccccccc-cccc-cccc-cccc-ccccccccccc4"), Email = "alice.williams@email.com", FirstName = "Alice", LastName = "Williams", Phone = "555-0104", MembershipTier = MembershipTier.None, LoyaltyPoints = 45, JoinedDate = DateTime.UtcNow.AddMonths(-2), TotalVisits = 5 },
            new Customer { Id = Guid.Parse("cccccccc-cccc-cccc-cccc-ccccccccccc5"), Email = "charlie.brown@email.com", FirstName = "Charlie", LastName = "Brown", Phone = "555-0105", MembershipTier = MembershipTier.Silver, LoyaltyPoints = 720, JoinedDate = DateTime.UtcNow.AddYears(-1).AddMonths(-6), TotalVisits = 51 },
            new Customer { Id = Guid.Parse("cccccccc-cccc-cccc-cccc-ccccccccccc6"), Email = "emma.davis@email.com", FirstName = "Emma", LastName = "Davis", Phone = "555-0106", MembershipTier = MembershipTier.Gold, LoyaltyPoints = 1580, JoinedDate = DateTime.UtcNow.AddYears(-3), TotalVisits = 112 },
            new Customer { Id = Guid.Parse("cccccccc-cccc-cccc-cccc-ccccccccccc7"), Email = "david.miller@email.com", FirstName = "David", LastName = "Miller", Phone = "555-0107", MembershipTier = MembershipTier.Bronze, LoyaltyPoints = 310, JoinedDate = DateTime.UtcNow.AddMonths(-6), TotalVisits = 22 },
            new Customer { Id = Guid.Parse("cccccccc-cccc-cccc-cccc-ccccccccccc8"), Email = "sophia.wilson@email.com", FirstName = "Sophia", LastName = "Wilson", Phone = "555-0108", MembershipTier = MembershipTier.None, LoyaltyPoints = 95, JoinedDate = DateTime.UtcNow.AddMonths(-3), TotalVisits = 8 },
            new Customer { Id = Guid.Parse("cccccccc-cccc-cccc-cccc-ccccccccccc9"), Email = "michael.moore@email.com", FirstName = "Michael", LastName = "Moore", Phone = "555-0109", MembershipTier = MembershipTier.Silver, LoyaltyPoints = 580, JoinedDate = DateTime.UtcNow.AddYears(-1), TotalVisits = 38 },
            new Customer { Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccc10"), Email = "olivia.taylor@email.com", FirstName = "Olivia", LastName = "Taylor", Phone = "555-0110", MembershipTier = MembershipTier.Bronze, LoyaltyPoints = 245, JoinedDate = DateTime.UtcNow.AddMonths(-5), TotalVisits = 16 },
            new Customer { Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccc11"), Email = "james.anderson@email.com", FirstName = "James", LastName = "Anderson", Phone = "555-0111", MembershipTier = MembershipTier.Gold, LoyaltyPoints = 1420, JoinedDate = DateTime.UtcNow.AddYears(-2).AddMonths(-3), TotalVisits = 95 },
            new Customer { Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccc12"), Email = "isabella.thomas@email.com", FirstName = "Isabella", LastName = "Thomas", Phone = "555-0112", MembershipTier = MembershipTier.None, LoyaltyPoints = 0, JoinedDate = DateTime.UtcNow.AddDays(-15), TotalVisits = 1 }
        };

        context.Customers.AddRange(customers);
    }

    private static void SeedMenuItems(BoardGameCafeDbContext context)
    {
        var menuItems = new List<MenuItem>
        {
            // Coffee Items
            new MenuItem
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-ddddddddddd1"),
                Name = "Meeple Mocha",
                Description = "Rich espresso with steamed milk and chocolate, topped with whipped cream and a chocolate meeple",
                Category = MenuCategory.Coffee,
                Price = 5.50m,
                IsAvailable = true,
                PreparationTimeMinutes = 5,
                AllergenInfo = "Dairy",
                IsVegetarian = true,
                IsVegan = false,
                IsGlutenFree = true
            },
            new MenuItem
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-ddddddddddd2"),
                Name = "Catan Cappuccino",
                Description = "Classic cappuccino with perfectly steamed foam, served in our custom hexagonal cup",
                Category = MenuCategory.Coffee,
                Price = 4.75m,
                IsAvailable = true,
                PreparationTimeMinutes = 4,
                AllergenInfo = "Dairy",
                IsVegetarian = true,
                IsVegan = false,
                IsGlutenFree = true
            },
            new MenuItem
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-ddddddddddd3"),
                Name = "Pandemic Pour-Over",
                Description = "Single-origin coffee brewed to perfection with a pour-over method",
                Category = MenuCategory.Coffee,
                Price = 4.50m,
                IsAvailable = true,
                PreparationTimeMinutes = 6,
                AllergenInfo = null,
                IsVegetarian = true,
                IsVegan = true,
                IsGlutenFree = true
            },
            new MenuItem
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-ddddddddddd4"),
                Name = "Espresso Shot",
                Description = "Double shot of our house espresso blend",
                Category = MenuCategory.Coffee,
                Price = 3.00m,
                IsAvailable = true,
                PreparationTimeMinutes = 2,
                AllergenInfo = null,
                IsVegetarian = true,
                IsVegan = true,
                IsGlutenFree = true
            },
            new MenuItem
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-ddddddddddd5"),
                Name = "Azul Americano",
                Description = "Smooth espresso diluted with hot water",
                Category = MenuCategory.Coffee,
                Price = 3.50m,
                IsAvailable = true,
                PreparationTimeMinutes = 3,
                AllergenInfo = null,
                IsVegetarian = true,
                IsVegan = true,
                IsGlutenFree = true
            },
            new MenuItem
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-ddddddddddd6"),
                Name = "Root Beer Latte",
                Description = "Unique latte with house-made root beer syrup",
                Category = MenuCategory.Coffee,
                Price = 5.25m,
                IsAvailable = true,
                PreparationTimeMinutes = 5,
                AllergenInfo = "Dairy",
                IsVegetarian = true,
                IsVegan = false,
                IsGlutenFree = true
            },
            new MenuItem
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-ddddddddddd7"),
                Name = "Terraforming Macchiato",
                Description = "Espresso marked with a dollop of steamed milk foam",
                Category = MenuCategory.Coffee,
                Price = 4.25m,
                IsAvailable = true,
                PreparationTimeMinutes = 3,
                AllergenInfo = "Dairy",
                IsVegetarian = true,
                IsVegan = false,
                IsGlutenFree = true
            },

            // Tea Items
            new MenuItem
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddde1"),
                Name = "Chai of Cthulhu",
                Description = "Dark and mysterious chai blend with exotic spices",
                Category = MenuCategory.Tea,
                Price = 4.50m,
                IsAvailable = true,
                PreparationTimeMinutes = 5,
                AllergenInfo = "Dairy",
                IsVegetarian = true,
                IsVegan = false,
                IsGlutenFree = true
            },
            new MenuItem
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddde2"),
                Name = "Earl Grey's Anatomy",
                Description = "Premium Earl Grey tea with bergamot essence",
                Category = MenuCategory.Tea,
                Price = 3.75m,
                IsAvailable = true,
                PreparationTimeMinutes = 4,
                AllergenInfo = null,
                IsVegetarian = true,
                IsVegan = true,
                IsGlutenFree = true
            },
            new MenuItem
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddde3"),
                Name = "Ticket to Chai",
                Description = "Aromatic chai tea with warm spices and steamed milk",
                Category = MenuCategory.Tea,
                Price = 4.25m,
                IsAvailable = true,
                PreparationTimeMinutes = 5,
                AllergenInfo = "Dairy",
                IsVegetarian = true,
                IsVegan = false,
                IsGlutenFree = true
            },
            new MenuItem
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddde4"),
                Name = "Green Tea of Carcassonne",
                Description = "Delicate Japanese green tea",
                Category = MenuCategory.Tea,
                Price = 3.50m,
                IsAvailable = true,
                PreparationTimeMinutes = 4,
                AllergenInfo = null,
                IsVegetarian = true,
                IsVegan = true,
                IsGlutenFree = true
            },
            new MenuItem
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddde5"),
                Name = "Oolong the Dragon",
                Description = "Premium oolong tea with a smooth finish",
                Category = MenuCategory.Tea,
                Price = 4.00m,
                IsAvailable = true,
                PreparationTimeMinutes = 4,
                AllergenInfo = null,
                IsVegetarian = true,
                IsVegan = true,
                IsGlutenFree = true
            },
            new MenuItem
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddde6"),
                Name = "Peppermint Pathfinder",
                Description = "Refreshing peppermint herbal tea",
                Category = MenuCategory.Tea,
                Price = 3.25m,
                IsAvailable = true,
                PreparationTimeMinutes = 4,
                AllergenInfo = null,
                IsVegetarian = true,
                IsVegan = true,
                IsGlutenFree = true
            },

            // Snacks
            new MenuItem
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddd51"),
                Name = "Game Night Nachos",
                Description = "Crispy tortilla chips with melted cheese, jalapeños, sour cream, and salsa",
                Category = MenuCategory.Snacks,
                Price = 8.50m,
                IsAvailable = true,
                PreparationTimeMinutes = 10,
                AllergenInfo = "Dairy, Gluten",
                IsVegetarian = true,
                IsVegan = false,
                IsGlutenFree = false
            },
            new MenuItem
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddd52"),
                Name = "Dice Tower Fries",
                Description = "Crispy seasoned fries stacked high with dipping sauces",
                Category = MenuCategory.Snacks,
                Price = 6.00m,
                IsAvailable = true,
                PreparationTimeMinutes = 8,
                AllergenInfo = null,
                IsVegetarian = true,
                IsVegan = true,
                IsGlutenFree = true
            },
            new MenuItem
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddd53"),
                Name = "Meeple Mix",
                Description = "Assorted nuts, dried fruits, and chocolate pieces",
                Category = MenuCategory.Snacks,
                Price = 5.50m,
                IsAvailable = true,
                PreparationTimeMinutes = 1,
                AllergenInfo = "Nuts, Dairy",
                IsVegetarian = true,
                IsVegan = false,
                IsGlutenFree = true
            },
            new MenuItem
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddd54"),
                Name = "Catan Cheese Board",
                Description = "Selection of artisan cheeses with crackers and fruit",
                Category = MenuCategory.Snacks,
                Price = 12.00m,
                IsAvailable = true,
                PreparationTimeMinutes = 5,
                AllergenInfo = "Dairy, Gluten",
                IsVegetarian = true,
                IsVegan = false,
                IsGlutenFree = false
            },
            new MenuItem
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddd55"),
                Name = "Pretzel Tokens",
                Description = "Warm soft pretzels with cheese sauce",
                Category = MenuCategory.Snacks,
                Price = 7.00m,
                IsAvailable = true,
                PreparationTimeMinutes = 8,
                AllergenInfo = "Dairy, Gluten",
                IsVegetarian = true,
                IsVegan = false,
                IsGlutenFree = false
            },
            new MenuItem
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddd56"),
                Name = "Popcorn Pioneers",
                Description = "Gourmet popcorn with various seasonings",
                Category = MenuCategory.Snacks,
                Price = 4.50m,
                IsAvailable = true,
                PreparationTimeMinutes = 2,
                AllergenInfo = null,
                IsVegetarian = true,
                IsVegan = true,
                IsGlutenFree = true
            },

            // Meals
            new MenuItem
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddd31"),
                Name = "Pandemic Pizza",
                Description = "12-inch pizza with your choice of toppings, named after the game that brought the world together",
                Category = MenuCategory.Meals,
                Price = 14.99m,
                IsAvailable = true,
                PreparationTimeMinutes = 20,
                AllergenInfo = "Dairy, Gluten",
                IsVegetarian = true,
                IsVegan = false,
                IsGlutenFree = false
            },
            new MenuItem
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddd32"),
                Name = "Wingspan Wings",
                Description = "Crispy chicken wings tossed in your choice of sauce",
                Category = MenuCategory.Meals,
                Price = 12.50m,
                IsAvailable = true,
                PreparationTimeMinutes = 18,
                AllergenInfo = "Gluten, Soy",
                IsVegetarian = false,
                IsVegan = false,
                IsGlutenFree = false
            },
            new MenuItem
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddd33"),
                Name = "Settlers Sampler",
                Description = "Assorted appetizer platter perfect for sharing",
                Category = MenuCategory.Meals,
                Price = 16.00m,
                IsAvailable = true,
                PreparationTimeMinutes = 15,
                AllergenInfo = "Dairy, Gluten, Eggs",
                IsVegetarian = false,
                IsVegan = false,
                IsGlutenFree = false
            },
            new MenuItem
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddd34"),
                Name = "Splendor Sliders",
                Description = "Three mini burgers with premium toppings",
                Category = MenuCategory.Meals,
                Price = 13.50m,
                IsAvailable = true,
                PreparationTimeMinutes = 15,
                AllergenInfo = "Dairy, Gluten, Eggs",
                IsVegetarian = false,
                IsVegan = false,
                IsGlutenFree = false
            },
            new MenuItem
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddd35"),
                Name = "Settlers Sandwich",
                Description = "Grilled chicken sandwich with lettuce, tomato, and house sauce on artisan bread",
                Category = MenuCategory.Meals,
                Price = 11.00m,
                IsAvailable = true,
                PreparationTimeMinutes = 15,
                AllergenInfo = "Dairy, Gluten",
                IsVegetarian = false,
                IsVegan = false,
                IsGlutenFree = false
            },
            new MenuItem
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddd36"),
                Name = "Carcassonne Cobb Salad",
                Description = "Fresh mixed greens with chicken, bacon, eggs, avocado, and blue cheese",
                Category = MenuCategory.Meals,
                Price = 13.50m,
                IsAvailable = true,
                PreparationTimeMinutes = 12,
                AllergenInfo = "Dairy, Eggs",
                IsVegetarian = false,
                IsVegan = false,
                IsGlutenFree = true
            },
            new MenuItem
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddd37"),
                Name = "Vegan Victory Bowl",
                Description = "Quinoa bowl with roasted vegetables, chickpeas, and tahini dressing",
                Category = MenuCategory.Meals,
                Price = 12.00m,
                IsAvailable = true,
                PreparationTimeMinutes = 15,
                AllergenInfo = "Sesame",
                IsVegetarian = true,
                IsVegan = true,
                IsGlutenFree = true
            },
            new MenuItem
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddd38"),
                Name = "Strategist's Steak",
                Description = "8oz sirloin steak with mashed potatoes and vegetables",
                Category = MenuCategory.Meals,
                Price = 19.99m,
                IsAvailable = true,
                PreparationTimeMinutes = 25,
                AllergenInfo = "Dairy",
                IsVegetarian = false,
                IsVegan = false,
                IsGlutenFree = true
            },
            new MenuItem
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddd39"),
                Name = "Codenames Curry",
                Description = "Flavorful curry with rice and naan bread",
                Category = MenuCategory.Meals,
                Price = 14.50m,
                IsAvailable = true,
                PreparationTimeMinutes = 18,
                AllergenInfo = "Dairy, Gluten",
                IsVegetarian = true,
                IsVegan = false,
                IsGlutenFree = false
            },

            // Desserts
            new MenuItem
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddd21"),
                Name = "Chocolate Chip Cookie",
                Description = "Warm, fresh-baked chocolate chip cookie",
                Category = MenuCategory.Desserts,
                Price = 3.50m,
                IsAvailable = true,
                PreparationTimeMinutes = 2,
                AllergenInfo = "Dairy, Eggs, Gluten",
                IsVegetarian = true,
                IsVegan = false,
                IsGlutenFree = false
            },
            new MenuItem
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddd22"),
                Name = "Kingdomino Brownie",
                Description = "Rich chocolate brownie topped with vanilla ice cream",
                Category = MenuCategory.Desserts,
                Price = 6.50m,
                IsAvailable = true,
                PreparationTimeMinutes = 5,
                AllergenInfo = "Dairy, Eggs, Gluten",
                IsVegetarian = true,
                IsVegan = false,
                IsGlutenFree = false
            },
            new MenuItem
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddd23"),
                Name = "Game Over Cheesecake",
                Description = "Creamy New York style cheesecake with seasonal fruit topping",
                Category = MenuCategory.Desserts,
                Price = 7.00m,
                IsAvailable = true,
                PreparationTimeMinutes = 3,
                AllergenInfo = "Dairy, Eggs, Gluten",
                IsVegetarian = true,
                IsVegan = false,
                IsGlutenFree = false
            },
            new MenuItem
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddd24"),
                Name = "Dice Cream Sundae",
                Description = "Three scoops of ice cream with toppings and whipped cream",
                Category = MenuCategory.Desserts,
                Price = 6.00m,
                IsAvailable = true,
                PreparationTimeMinutes = 4,
                AllergenInfo = "Dairy, Nuts",
                IsVegetarian = true,
                IsVegan = false,
                IsGlutenFree = true
            },
            new MenuItem
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddd25"),
                Name = "Victory Point Tiramisu",
                Description = "Classic Italian dessert with espresso-soaked ladyfingers",
                Category = MenuCategory.Desserts,
                Price = 7.50m,
                IsAvailable = true,
                PreparationTimeMinutes = 3,
                AllergenInfo = "Dairy, Eggs, Gluten",
                IsVegetarian = true,
                IsVegan = false,
                IsGlutenFree = false
            },

            // Alcohol/Beverages
            new MenuItem
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddd11"),
                Name = "Mana Potion IPA",
                Description = "Local craft IPA with citrus and pine notes",
                Category = MenuCategory.Alcohol,
                Price = 7.50m,
                IsAvailable = true,
                PreparationTimeMinutes = 2,
                AllergenInfo = "Gluten",
                IsVegetarian = true,
                IsVegan = true,
                IsGlutenFree = false
            },
            new MenuItem
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddd12"),
                Name = "Critical Hit Cider",
                Description = "Crisp apple cider, refreshing and slightly sweet",
                Category = MenuCategory.Alcohol,
                Price = 6.50m,
                IsAvailable = true,
                PreparationTimeMinutes = 2,
                AllergenInfo = null,
                IsVegetarian = true,
                IsVegan = true,
                IsGlutenFree = true
            },
            new MenuItem
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddd13"),
                Name = "Red Wine - Catan Reserve",
                Description = "Medium-bodied red wine with berry notes",
                Category = MenuCategory.Alcohol,
                Price = 9.00m,
                IsAvailable = true,
                PreparationTimeMinutes = 2,
                AllergenInfo = "Sulfites",
                IsVegetarian = true,
                IsVegan = false,
                IsGlutenFree = true
            },
            new MenuItem
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddd14"),
                Name = "White Wine - Wingspan Chardonnay",
                Description = "Crisp white wine with hints of apple and pear",
                Category = MenuCategory.Alcohol,
                Price = 9.00m,
                IsAvailable = true,
                PreparationTimeMinutes = 2,
                AllergenInfo = "Sulfites",
                IsVegetarian = true,
                IsVegan = false,
                IsGlutenFree = true
            },
            new MenuItem
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddd15"),
                Name = "Splendor Stout",
                Description = "Rich and creamy chocolate stout",
                Category = MenuCategory.Alcohol,
                Price = 7.00m,
                IsAvailable = true,
                PreparationTimeMinutes = 2,
                AllergenInfo = "Gluten",
                IsVegetarian = true,
                IsVegan = true,
                IsGlutenFree = false
            },
            new MenuItem
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddd16"),
                Name = "Lemonade of Legends",
                Description = "Fresh-squeezed lemonade, perfect for non-drinkers",
                Category = MenuCategory.Alcohol,
                Price = 3.50m,
                IsAvailable = true,
                PreparationTimeMinutes = 3,
                AllergenInfo = null,
                IsVegetarian = true,
                IsVegan = true,
                IsGlutenFree = true
            },
            
            // Additional Menu Items
            new MenuItem
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddd17"),
                Name = "Quest Coffee Cake",
                Description = "Moist coffee cake with cinnamon streusel topping",
                Category = MenuCategory.Desserts,
                Price = 5.50m,
                IsAvailable = true,
                PreparationTimeMinutes = 3,
                AllergenInfo = "Dairy, Eggs, Gluten",
                IsVegetarian = true,
                IsVegan = false,
                IsGlutenFree = false
            },
            new MenuItem
            {
                Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddd18"),
                Name = "Roll for Initiative Energy Drink",
                Description = "Refreshing energy drink to keep you gaming all night",
                Category = MenuCategory.Alcohol,
                Price = 4.50m,
                IsAvailable = true,
                PreparationTimeMinutes = 1,
                AllergenInfo = null,
                IsVegetarian = true,
                IsVegan = true,
                IsGlutenFree = true
            }
        };

        context.MenuItems.AddRange(menuItems);
    }

    private static void SeedEvents(BoardGameCafeDbContext context)
    {
        var events = new List<Event>
        {
            new Event
            {
                Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                Title = "Catan Championship Tournament",
                Description = "Compete for the title of Catan Champion! Open to all skill levels. Prizes for top 3 finishers.",
                EventDate = DateTime.UtcNow.AddDays(14).Date.AddHours(18),
                DurationMinutes = 180,
                MaxParticipants = 16,
                TicketPrice = 15.00m,
                EventType = EventType.Tournament,
                RequiresRegistration = true
            },
            new Event
            {
                Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeee2"),
                Title = "Friday Night Magic: The Gathering",
                Description = "Weekly MTG tournament. Bring your own deck or borrow one of ours!",
                EventDate = DateTime.UtcNow.AddDays(5).Date.AddHours(19),
                DurationMinutes = 240,
                MaxParticipants = 20,
                TicketPrice = 10.00m,
                EventType = EventType.Tournament,
                RequiresRegistration = true
            },
            new Event
            {
                Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeee3"),
                Title = "Learn to Play: Wingspan Workshop",
                Description = "New to Wingspan? Join our expert instructor for a comprehensive tutorial and practice game.",
                EventDate = DateTime.UtcNow.AddDays(7).Date.AddHours(14),
                DurationMinutes = 120,
                MaxParticipants = 8,
                TicketPrice = 5.00m,
                EventType = EventType.Workshop,
                RequiresRegistration = true
            },
            new Event
            {
                Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeee4"),
                Title = "Board Game Social Night",
                Description = "Casual gaming night! Meet new people and try new games. No registration required, just drop in!",
                EventDate = DateTime.UtcNow.AddDays(3).Date.AddHours(18),
                DurationMinutes = 300,
                MaxParticipants = 40,
                TicketPrice = 0.00m,
                EventType = EventType.GameNight,
                RequiresRegistration = false
            },
            new Event
            {
                Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeee5"),
                Title = "Pandemic Legacy Campaign Launch",
                Description = "Start a new Pandemic Legacy campaign with a dedicated group. Commitment to monthly sessions required.",
                EventDate = DateTime.UtcNow.AddDays(21).Date.AddHours(15),
                DurationMinutes = 150,
                MaxParticipants = 4,
                TicketPrice = 20.00m,
                EventType = EventType.GameNight,
                RequiresRegistration = true
            },
            new Event
            {
                Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeee6"),
                Title = "New Release: Ark Nova Demo Day",
                Description = "Be among the first to try our newest addition! Staff will teach and answer questions.",
                EventDate = DateTime.UtcNow.AddDays(10).Date.AddHours(13),
                DurationMinutes = 180,
                MaxParticipants = 12,
                TicketPrice = 0.00m,
                EventType = EventType.Release,
                RequiresRegistration = true
            },
            new Event
            {
                Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeee7"),
                Title = "Kids Game Day",
                Description = "Family-friendly gaming event with age-appropriate games for children 6-12.",
                EventDate = DateTime.UtcNow.AddDays(8).Date.AddHours(11),
                DurationMinutes = 180,
                MaxParticipants = 24,
                TicketPrice = 8.00m,
                EventType = EventType.GameNight,
                RequiresRegistration = true
            }
        };

        context.Events.AddRange(events);
    }

    private static void SeedReservations(BoardGameCafeDbContext context)
    {
        // Note: Cannot query unsaved entities, so we create reservations with direct GUID references
        // These GUIDs correspond to the customers and tables we created above
        var reservations = new List<Reservation>
        {
            // Past reservation - Completed (Customer: john.doe, Table: T-101)
            new Reservation
            {
                Id = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                CustomerId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"), // john.doe
                TableId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), // T-101
                ReservationDate = DateTime.UtcNow.AddDays(-5).Date,
                StartTime = new TimeSpan(14, 0, 0),
                EndTime = new TimeSpan(17, 0, 0),
                PartySize = 2,
                Status = ReservationStatus.Completed,
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                SpecialRequests = "Window seat preferred"
            },
            // Past reservation - No Show (Customer: jane.smith, Table: T-102)
            new Reservation
            {
                Id = Guid.Parse("ffffffff-ffff-ffff-ffff-fffffffffffe"),
                CustomerId = Guid.Parse("cccccccc-cccc-cccc-cccc-ccccccccccc2"), // jane.smith
                TableId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaab"), // T-102
                ReservationDate = DateTime.UtcNow.AddDays(-2).Date,
                StartTime = new TimeSpan(18, 0, 0),
                EndTime = new TimeSpan(21, 0, 0),
                PartySize = 4,
                Status = ReservationStatus.NoShow,
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                SpecialRequests = null
            },
            // Current/Today - Confirmed (Customer: bob.johnson, Table: T-103)
            new Reservation
            {
                Id = Guid.Parse("ffffffff-ffff-ffff-ffff-fffffffffffd"),
                CustomerId = Guid.Parse("cccccccc-cccc-cccc-cccc-ccccccccccc3"), // bob.johnson
                TableId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaac"), // T-103
                ReservationDate = DateTime.UtcNow.Date,
                StartTime = new TimeSpan(15, 0, 0),
                EndTime = new TimeSpan(18, 0, 0),
                PartySize = 4,
                Status = ReservationStatus.Confirmed,
                CreatedAt = DateTime.UtcNow.AddDays(-3),
                SpecialRequests = "Celebrating a birthday!"
            },
            // Future reservation - Confirmed (Customer: alice.williams, Table: T-104)
            new Reservation
            {
                Id = Guid.Parse("ffffffff-ffff-ffff-ffff-fffffffffffc"),
                CustomerId = Guid.Parse("cccccccc-cccc-cccc-cccc-ccccccccccc4"), // alice.williams
                TableId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaad"), // T-104
                ReservationDate = DateTime.UtcNow.AddDays(3).Date,
                StartTime = new TimeSpan(17, 0, 0),
                EndTime = new TimeSpan(20, 0, 0),
                PartySize = 6,
                Status = ReservationStatus.Confirmed,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                SpecialRequests = "Group game night"
            },
            // Future reservation - Pending (Customer: charlie.brown, Table: T-105)
            new Reservation
            {
                Id = Guid.Parse("ffffffff-ffff-ffff-ffff-fffffffffffb"),
                CustomerId = Guid.Parse("cccccccc-cccc-cccc-cccc-ccccccccccc5"), // charlie.brown
                TableId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaae"), // T-105
                ReservationDate = DateTime.UtcNow.AddDays(7).Date,
                StartTime = new TimeSpan(19, 0, 0),
                EndTime = new TimeSpan(22, 0, 0),
                PartySize = 2,
                Status = ReservationStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                SpecialRequests = "First time visitor"
            },
            // Future reservation - Cancelled (Customer: john.doe, Table: T-101)
            new Reservation
            {
                Id = Guid.Parse("ffffffff-ffff-ffff-ffff-fffffffffffa"),
                CustomerId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"), // john.doe
                TableId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), // T-101
                ReservationDate = DateTime.UtcNow.AddDays(10).Date,
                StartTime = new TimeSpan(16, 0, 0),
                EndTime = new TimeSpan(19, 0, 0),
                PartySize = 2,
                Status = ReservationStatus.Cancelled,
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                SpecialRequests = "Change of plans"
            }
        };

        context.Reservations.AddRange(reservations);
    }

    private static void SeedSampleOrders(BoardGameCafeDbContext context)
    {
        // Only seed orders if we have customers
        if (!context.Customers.Any())
            return;

        var customer = context.Customers.First();

        // Sample Order 1: Coffee and snacks
        var order1 = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customer.Id,
            OrderDate = DateTime.UtcNow.AddDays(-5),
            Status = OrderStatus.Completed,
            PaymentMethod = PaymentMethod.Card,
            Items = new List<OrderItem>()
        };

        var meepleMocha = context.MenuItems.FirstOrDefault(m => m.Name == "Meeple Mocha");
        var nachos = context.MenuItems.FirstOrDefault(m => m.Name == "Game Night Nachos");

        if (meepleMocha != null)
        {
            order1.Items.Add(new OrderItem
            {
                Id = Guid.NewGuid(),
                MenuItemId = meepleMocha.Id,
                Quantity = 2,
                UnitPrice = meepleMocha.Price,
                SpecialInstructions = "Extra chocolate please"
            });
        }

        if (nachos != null)
        {
            order1.Items.Add(new OrderItem
            {
                Id = Guid.NewGuid(),
                MenuItemId = nachos.Id,
                Quantity = 1,
                UnitPrice = nachos.Price,
                SpecialInstructions = null
            });
        }

        context.Orders.Add(order1);

        // Sample Order 2: Full meal with drinks
        var order2 = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customer.Id,
            OrderDate = DateTime.UtcNow.AddDays(-2),
            Status = OrderStatus.Completed,
            PaymentMethod = PaymentMethod.Card,
            Items = new List<OrderItem>()
        };

        var pizza = context.MenuItems.FirstOrDefault(m => m.Name == "Pandemic Pizza");
        var wings = context.MenuItems.FirstOrDefault(m => m.Name == "Wingspan Wings");
        var beer = context.MenuItems.FirstOrDefault(m => m.Name == "Board Game Brew IPA");

        if (pizza != null)
        {
            order2.Items.Add(new OrderItem
            {
                Id = Guid.NewGuid(),
                MenuItemId = pizza.Id,
                Quantity = 1,
                UnitPrice = pizza.Price
            });
        }

        if (wings != null)
        {
            order2.Items.Add(new OrderItem
            {
                Id = Guid.NewGuid(),
                MenuItemId = wings.Id,
                Quantity = 1,
                UnitPrice = wings.Price,
                SpecialInstructions = "Buffalo sauce"
            });
        }

        if (beer != null)
        {
            order2.Items.Add(new OrderItem
            {
                Id = Guid.NewGuid(),
                MenuItemId = beer.Id,
                Quantity = 2,
                UnitPrice = beer.Price
            });
        }

        context.Orders.Add(order2);
    }
}
