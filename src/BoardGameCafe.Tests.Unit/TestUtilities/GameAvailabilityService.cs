using BoardGameCafe.Domain;

namespace BoardGameCafe.Tests.Unit.TestUtilities;

/// <summary>
/// Service for managing game availability and calculating late fees
/// </summary>
public class GameAvailabilityService
{
    private const decimal HourlyLateFeeRate = 2.00m;
    private const int GracePeriodMinutes = 15;

    /// <summary>
    /// Gets the number of available copies for a game
    /// </summary>
    /// <param name="game">The game to check</param>
    /// <returns>Number of available copies (owned - in use)</returns>
    public int GetAvailableCopies(Game game)
    {
        if (game == null)
        {
            throw new ArgumentNullException(nameof(game));
        }

        return Math.Max(0, game.CopiesOwned - game.CopiesInUse);
    }

    /// <summary>
    /// Calculates the late fee for a game session based on how long it's overdue
    /// </summary>
    /// <param name="dueBackAt">When the game was due back</param>
    /// <param name="returnedAt">When the game was actually returned (null if not yet returned, uses current time)</param>
    /// <returns>Late fee amount (with grace period applied)</returns>
    public decimal CalculateLateFee(DateTime dueBackAt, DateTime? returnedAt = null)
    {
        var actualReturnTime = returnedAt ?? DateTime.UtcNow;
        
        if (actualReturnTime <= dueBackAt)
        {
            return 0; // Not late
        }

        var overdueDuration = actualReturnTime - dueBackAt;
        
        // Apply grace period
        if (overdueDuration.TotalMinutes <= GracePeriodMinutes)
        {
            return 0;
        }

        // Calculate hours overdue (after grace period)
        var overdueMinutesAfterGrace = overdueDuration.TotalMinutes - GracePeriodMinutes;
        var overdueHours = Math.Ceiling(overdueMinutesAfterGrace / 60.0);

        return (decimal)overdueHours * HourlyLateFeeRate;
    }

    /// <summary>
    /// Checks if a game session is overdue
    /// </summary>
    /// <param name="dueBackAt">When the game was due back</param>
    /// <param name="currentTime">Current time (defaults to UtcNow if not provided)</param>
    /// <returns>True if the game is overdue (past grace period), false otherwise</returns>
    public bool IsOverdue(DateTime dueBackAt, DateTime? currentTime = null)
    {
        var checkTime = currentTime ?? DateTime.UtcNow;
        
        if (checkTime <= dueBackAt)
        {
            return false;
        }

        var overdueDuration = checkTime - dueBackAt;
        
        // Only consider overdue after grace period
        return overdueDuration.TotalMinutes > GracePeriodMinutes;
    }
}
