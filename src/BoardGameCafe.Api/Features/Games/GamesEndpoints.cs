using BoardGameCafe.Api.Data;
using BoardGameCafe.Domain;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BoardGameCafe.Api.Features.Games;

public static class GamesEndpoints
{
    public static IEndpointRouteBuilder MapGamesEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/games")
            .WithTags("Games");

        /// <summary>
        /// Retrieves a paginated and filtered list of board games from the catalog
        /// </summary>
        /// <param name="db">Database context</param>
        /// <param name="filter">Filter and pagination parameters</param>
        /// <returns>List of games matching the filter criteria</returns>
        /// <response code="200">Returns the list of games</response>
        /// <response code="400">If the filter parameters are invalid</response>
        group.MapGet("/", async Task<Results<Ok<List<GameDto>>, BadRequest<ProblemDetails>>> (
            AppDbContext db,
            [AsParameters] GameFilterRequest filter) =>
        {
            var query = db.Games.AsQueryable();

            // Apply category filter
            if (filter.Category.HasValue)
            {
                if (!Enum.IsDefined(typeof(GameCategory), filter.Category.Value))
                {
                    return TypedResults.BadRequest(new ProblemDetails
                    {
                        Title = "Invalid category",
                        Detail = "Category must be a valid GameCategory value (0-4)"
                    });
                }
                query = query.Where(g => (int)g.Category == filter.Category.Value);
            }

            // Apply player count filters
            if (filter.PlayerCount.HasValue)
            {
                // Exact player count: game must support this number
                query = query.Where(g => g.MinPlayers <= filter.PlayerCount.Value 
                    && g.MaxPlayers >= filter.PlayerCount.Value);
            }
            else
            {
                // Min/Max player count filters
                if (filter.MinPlayerCount.HasValue)
                {
                    query = query.Where(g => g.MaxPlayers >= filter.MinPlayerCount.Value);
                }
                if (filter.MaxPlayerCount.HasValue)
                {
                    query = query.Where(g => g.MinPlayers <= filter.MaxPlayerCount.Value);
                }
            }

            // Apply availability filter
            if (filter.AvailableOnly == true)
            {
                query = query.Where(g => g.CopiesOwned > g.CopiesInUse);
            }

            // Apply pagination
            var page = filter.Page ?? 1;
            var pageSize = filter.PageSize ?? 10;
            
            var totalCount = await query.CountAsync();
            var games = await query
                .OrderBy(g => g.Title)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(g => new GameDto
                {
                    Id = g.Id,
                    Title = g.Title,
                    Publisher = g.Publisher,
                    MinPlayers = g.MinPlayers,
                    MaxPlayers = g.MaxPlayers,
                    PlayTimeMinutes = g.PlayTimeMinutes,
                    AgeRating = g.AgeRating,
                    Complexity = g.Complexity,
                    Category = g.Category.ToString(),
                    CopiesOwned = g.CopiesOwned,
                    CopiesInUse = g.CopiesInUse,
                    DailyRentalFee = g.DailyRentalFee,
                    Description = g.Description,
                    ImageUrl = g.ImageUrl,
                    IsAvailable = g.CopiesOwned > g.CopiesInUse
                })
                .ToListAsync();

            return TypedResults.Ok(games);
        })
        .WithName("GetGames")
        .WithSummary("List and filter games from the catalog")
        .WithDescription("Retrieves a paginated list of games with optional filters for category, player count, and availability")
        .Produces<List<GameDto>>(StatusCodes.Status200OK)
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);

        /// <summary>
        /// Retrieves a specific board game by its unique identifier
        /// </summary>
        /// <param name="db">Database context</param>
        /// <param name="id">The unique identifier of the game</param>
        /// <returns>The game details if found</returns>
        /// <response code="200">Returns the game details</response>
        /// <response code="404">If the game is not found</response>
        group.MapGet("/{id:guid}", async Task<Results<Ok<GameDto>, NotFound>> (
            AppDbContext db,
            Guid id) =>
        {
            var game = await db.Games
                .Where(g => g.Id == id)
                .Select(g => new GameDto
                {
                    Id = g.Id,
                    Title = g.Title,
                    Publisher = g.Publisher,
                    MinPlayers = g.MinPlayers,
                    MaxPlayers = g.MaxPlayers,
                    PlayTimeMinutes = g.PlayTimeMinutes,
                    AgeRating = g.AgeRating,
                    Complexity = g.Complexity,
                    Category = g.Category.ToString(),
                    CopiesOwned = g.CopiesOwned,
                    CopiesInUse = g.CopiesInUse,
                    DailyRentalFee = g.DailyRentalFee,
                    Description = g.Description,
                    ImageUrl = g.ImageUrl,
                    IsAvailable = g.CopiesOwned > g.CopiesInUse
                })
                .FirstOrDefaultAsync();

            return game is null 
                ? TypedResults.NotFound() 
                : TypedResults.Ok(game);
        })
        .WithName("GetGameById")
        .WithSummary("Get a single game by ID")
        .WithDescription("Retrieves detailed information about a specific game from the catalog")
        .Produces<GameDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        /// <summary>
        /// Creates a new board game in the catalog (Admin Only - Placeholder)
        /// </summary>
        /// <param name="db">Database context</param>
        /// <param name="request">The game details to create</param>
        /// <returns>The created game details</returns>
        /// <response code="201">Returns the newly created game</response>
        /// <response code="400">If the request data is invalid</response>
        /// <response code="409">If a game with the same title already exists</response>
        group.MapPost("/", async Task<Results<Created<GameDto>, BadRequest<ProblemDetails>, Conflict<ProblemDetails>>> (
            AppDbContext db,
            [FromBody] CreateGameRequest request) =>
        {
            // Validate MinPlayers <= MaxPlayers
            if (request.MinPlayers > request.MaxPlayers)
            {
                return TypedResults.BadRequest(new ProblemDetails
                {
                    Title = "Invalid player count",
                    Detail = "MinPlayers must be less than or equal to MaxPlayers"
                });
            }

            // Validate category is valid enum value
            if (!Enum.IsDefined(typeof(GameCategory), request.Category))
            {
                return TypedResults.BadRequest(new ProblemDetails
                {
                    Title = "Invalid category",
                    Detail = "Category must be a valid GameCategory value (0-4)"
                });
            }

            // Check for duplicate title (case-insensitive)
            var existingGame = await db.Games
                .Where(g => g.Title.ToLower() == request.Title.ToLower())
                .FirstOrDefaultAsync();

            if (existingGame is not null)
            {
                return TypedResults.Conflict(new ProblemDetails
                {
                    Title = "Duplicate game",
                    Detail = $"A game with the title '{request.Title}' already exists"
                });
            }

            var game = new Game
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Publisher = request.Publisher,
                MinPlayers = request.MinPlayers,
                MaxPlayers = request.MaxPlayers,
                PlayTimeMinutes = request.PlayTimeMinutes,
                AgeRating = request.AgeRating,
                Complexity = request.Complexity,
                Category = (GameCategory)request.Category,
                CopiesOwned = request.CopiesOwned,
                CopiesInUse = 0, // New games start with 0 copies in use
                DailyRentalFee = request.DailyRentalFee,
                Description = request.Description,
                ImageUrl = request.ImageUrl
            };

            db.Games.Add(game);
            await db.SaveChangesAsync();

            var dto = new GameDto
            {
                Id = game.Id,
                Title = game.Title,
                Publisher = game.Publisher,
                MinPlayers = game.MinPlayers,
                MaxPlayers = game.MaxPlayers,
                PlayTimeMinutes = game.PlayTimeMinutes,
                AgeRating = game.AgeRating,
                Complexity = game.Complexity,
                Category = game.Category.ToString(),
                CopiesOwned = game.CopiesOwned,
                CopiesInUse = game.CopiesInUse,
                DailyRentalFee = game.DailyRentalFee,
                Description = game.Description,
                ImageUrl = game.ImageUrl,
                IsAvailable = game.IsAvailable
            };

            return TypedResults.Created($"/api/v1/games/{game.Id}", dto);
        })
        .WithName("CreateGame")
        .WithSummary("Create a new game (Admin only - placeholder)")
        .WithDescription("Adds a new board game to the catalog. Currently no authentication required (admin placeholder)")
        .Produces<GameDto>(StatusCodes.Status201Created)
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .Produces<ProblemDetails>(StatusCodes.Status409Conflict);

        /// <summary>
        /// Updates an existing board game in the catalog (Admin Only - Placeholder)
        /// </summary>
        /// <param name="db">Database context</param>
        /// <param name="id">The unique identifier of the game to update</param>
        /// <param name="request">The updated game details</param>
        /// <returns>The updated game details</returns>
        /// <response code="200">Returns the updated game</response>
        /// <response code="400">If the request data is invalid</response>
        /// <response code="404">If the game is not found</response>
        /// <response code="409">If the update would violate business rules</response>
        group.MapPut("/{id:guid}", async Task<Results<Ok<GameDto>, NotFound, BadRequest<ProblemDetails>, Conflict<ProblemDetails>>> (
            AppDbContext db,
            Guid id,
            [FromBody] UpdateGameRequest request) =>
        {
            // Validate MinPlayers <= MaxPlayers
            if (request.MinPlayers > request.MaxPlayers)
            {
                return TypedResults.BadRequest(new ProblemDetails
                {
                    Title = "Invalid player count",
                    Detail = "MinPlayers must be less than or equal to MaxPlayers"
                });
            }

            // Validate category is valid enum value
            if (!Enum.IsDefined(typeof(GameCategory), request.Category))
            {
                return TypedResults.BadRequest(new ProblemDetails
                {
                    Title = "Invalid category",
                    Detail = "Category must be a valid GameCategory value (0-4)"
                });
            }

            // Validate CopiesInUse <= CopiesOwned
            if (request.CopiesInUse > request.CopiesOwned)
            {
                return TypedResults.BadRequest(new ProblemDetails
                {
                    Title = "Invalid copies count",
                    Detail = "CopiesInUse cannot exceed CopiesOwned"
                });
            }

            var game = await db.Games.FindAsync(id);
            if (game is null)
            {
                return TypedResults.NotFound();
            }

            // Check for duplicate title (excluding current game, case-insensitive)
            var duplicateGame = await db.Games
                .Where(g => g.Id != id && g.Title.ToLower() == request.Title.ToLower())
                .FirstOrDefaultAsync();

            if (duplicateGame is not null)
            {
                return TypedResults.Conflict(new ProblemDetails
                {
                    Title = "Duplicate game",
                    Detail = $"Another game with the title '{request.Title}' already exists"
                });
            }

            // Update game properties
            game.Title = request.Title;
            game.Publisher = request.Publisher;
            game.MinPlayers = request.MinPlayers;
            game.MaxPlayers = request.MaxPlayers;
            game.PlayTimeMinutes = request.PlayTimeMinutes;
            game.AgeRating = request.AgeRating;
            game.Complexity = request.Complexity;
            game.Category = (GameCategory)request.Category;
            game.CopiesOwned = request.CopiesOwned;
            game.CopiesInUse = request.CopiesInUse;
            game.DailyRentalFee = request.DailyRentalFee;
            game.Description = request.Description;
            game.ImageUrl = request.ImageUrl;

            await db.SaveChangesAsync();

            var dto = new GameDto
            {
                Id = game.Id,
                Title = game.Title,
                Publisher = game.Publisher,
                MinPlayers = game.MinPlayers,
                MaxPlayers = game.MaxPlayers,
                PlayTimeMinutes = game.PlayTimeMinutes,
                AgeRating = game.AgeRating,
                Complexity = game.Complexity,
                Category = game.Category.ToString(),
                CopiesOwned = game.CopiesOwned,
                CopiesInUse = game.CopiesInUse,
                DailyRentalFee = game.DailyRentalFee,
                Description = game.Description,
                ImageUrl = game.ImageUrl,
                IsAvailable = game.IsAvailable
            };

            return TypedResults.Ok(dto);
        })
        .WithName("UpdateGame")
        .WithSummary("Update an existing game (Admin only - placeholder)")
        .WithDescription("Updates the details of an existing board game in the catalog. Currently no authentication required (admin placeholder)")
        .Produces<GameDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
        .Produces<ProblemDetails>(StatusCodes.Status409Conflict);

        /// <summary>
        /// Soft deletes a board game from the catalog (Admin Only - Placeholder)
        /// </summary>
        /// <param name="db">Database context</param>
        /// <param name="id">The unique identifier of the game to delete</param>
        /// <returns>No content on success</returns>
        /// <response code="204">Game successfully deleted</response>
        /// <response code="404">If the game is not found</response>
        /// <response code="409">If the game is currently in use and cannot be deleted</response>
        group.MapDelete("/{id:guid}", async Task<Results<NoContent, NotFound, Conflict<ProblemDetails>>> (
            AppDbContext db,
            Guid id) =>
        {
            var game = await db.Games.FindAsync(id);
            if (game is null)
            {
                return TypedResults.NotFound();
            }

            // Business rule: Cannot delete a game that has copies currently in use
            if (game.CopiesInUse > 0)
            {
                return TypedResults.Conflict(new ProblemDetails
                {
                    Title = "Game in use",
                    Detail = $"Cannot delete game '{game.Title}' because {game.CopiesInUse} {(game.CopiesInUse == 1 ? "copy is" : "copies are")} currently in use"
                });
            }

            db.Games.Remove(game);
            await db.SaveChangesAsync();

            return TypedResults.NoContent();
        })
        .WithName("DeleteGame")
        .WithSummary("Delete a game (Admin only - placeholder)")
        .WithDescription("Removes a board game from the catalog. Games with copies in use cannot be deleted. Currently no authentication required (admin placeholder)")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .Produces<ProblemDetails>(StatusCodes.Status409Conflict);

        return app;
    }
}
