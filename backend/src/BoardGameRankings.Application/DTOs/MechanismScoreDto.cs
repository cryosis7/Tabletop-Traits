namespace BoardGameRankings.Application.DTOs;

public record MechanismScoreDto(
    int MechanismId,
    string MechanismName,
    decimal AverageRating,
    decimal TotalRating,
    int GameCount
);
