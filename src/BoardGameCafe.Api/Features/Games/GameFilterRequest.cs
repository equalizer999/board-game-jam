using System.ComponentModel.DataAnnotations;

namespace BoardGameCafe.Api.Features.Games;

/// <summary>
/// Request parameters for filtering and paginating the game catalog
/// </summary>
public record GameFilterRequest
{
    /// <summary>
    /// Filter by game category (0=Strategy, 1=Party, 2=Family, 3=Cooperative, 4=Abstract)
    /// </summary>
    /// <example>0</example>
    public int? Category { get; init; }

    /// <summary>
    /// Filter by minimum player count (game must support at least this many players)
    /// </summary>
    /// <example>4</example>
    [Range(1, 20)]
    public int? MinPlayerCount { get; init; }

    /// <summary>
    /// Filter by maximum player count (game must support no more than this many players)
    /// </summary>
    /// <example>6</example>
    [Range(1, 20)]
    public int? MaxPlayerCount { get; init; }

    /// <summary>
    /// Filter by exact player count (game must support this exact number of players)
    /// </summary>
    /// <example>4</example>
    [Range(1, 20)]
    public int? PlayerCount { get; init; }

    /// <summary>
    /// Filter to only show available games (has copies not in use)
    /// </summary>
    /// <example>true</example>
    public bool? AvailableOnly { get; init; }

    /// <summary>
    /// Page number for pagination (1-based, default: 1 if not provided)
    /// </summary>
    /// <example>1</example>
    [Range(1, int.MaxValue)]
    public int? Page { get; init; }

    /// <summary>
    /// Number of results per page (default: 10 if not provided, max: 100)
    /// </summary>
    /// <example>10</example>
    [Range(1, 100)]
    public int? PageSize { get; init; }
}
