using BoardGameCafe.Domain;

namespace BoardGameCafe.Api.Features.Games;

/// <summary>
/// Repository interface for Game entity operations
/// </summary>
public interface IGameRepository
{
    /// <summary>
    /// Get all games from the database
    /// </summary>
    /// <returns>List of all games</returns>
    Task<List<Game>> GetAllAsync();

    /// <summary>
    /// Get a game by ID
    /// </summary>
    /// <param name="id">Game ID</param>
    /// <returns>Game if found, null otherwise</returns>
    Task<Game?> GetByIdAsync(Guid id);

    /// <summary>
    /// Add a new game
    /// </summary>
    /// <param name="game">Game to add</param>
    /// <returns>The added game</returns>
    Task<Game> AddAsync(Game game);

    /// <summary>
    /// Update an existing game
    /// </summary>
    /// <param name="game">Game to update</param>
    Task UpdateAsync(Game game);

    /// <summary>
    /// Delete a game
    /// </summary>
    /// <param name="id">Game ID to delete</param>
    Task DeleteAsync(Guid id);
}
