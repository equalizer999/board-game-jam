using BoardGameCafe.Api.Data;
using BoardGameCafe.Domain;
using Microsoft.EntityFrameworkCore;

namespace BoardGameCafe.Api.Features.Games;

/// <summary>
/// Repository implementation for Game entity operations
/// </summary>
public class GameRepository : IGameRepository
{
    private readonly BoardGameCafeDbContext _context;

    public GameRepository(BoardGameCafeDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<List<Game>> GetAllAsync()
    {
        return await _context.Games.ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<Game?> GetByIdAsync(Guid id)
    {
        return await _context.Games.FindAsync(id);
    }

    /// <inheritdoc/>
    public async Task<Game> AddAsync(Game game)
    {
        _context.Games.Add(game);
        await _context.SaveChangesAsync();
        return game;
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(Game game)
    {
        _context.Games.Update(game);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(Guid id)
    {
        var game = await GetByIdAsync(id);
        if (game != null)
        {
            _context.Games.Remove(game);
            await _context.SaveChangesAsync();
        }
    }
}
