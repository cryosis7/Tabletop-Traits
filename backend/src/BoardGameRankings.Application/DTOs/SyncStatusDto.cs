namespace BoardGameRankings.Application.DTOs;

/// <summary>
/// Represents the outcome of a collection synchronization request.
/// </summary>
public record SyncStatusDto
{
    public SyncStatusDto(
        string status,
        int gamesProcessed,
        int totalGames,
        DateTime? lastSyncTime)
    {
        Status = status;
        GamesProcessed = gamesProcessed;
        TotalGames = totalGames;
        LastSyncTime = lastSyncTime;
    }

    /// <summary>
    /// High-level state of the sync request.
    /// </summary>
    public string Status { get; init; }

    /// <summary>
    /// Number of detailed game records processed during the sync.
    /// </summary>
    public int GamesProcessed { get; init; }

    /// <summary>
    /// Number of rated collection entries returned by BoardGameGeek.
    /// </summary>
    public int TotalGames { get; init; }

    /// <summary>
    /// UTC timestamp of the completed sync, when available.
    /// </summary>
    public DateTime? LastSyncTime { get; init; }
}
