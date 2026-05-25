namespace BoardGameRankings.Application.DTOs;

public record SyncStatusDto(
    string Status,
    int GamesProcessed,
    int TotalGames,
    DateTime? LastSyncTime
);
