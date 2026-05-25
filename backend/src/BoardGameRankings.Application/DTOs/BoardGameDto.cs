namespace BoardGameRankings.Application.DTOs;

public record BoardGameDto(
    int Id,
    string Name,
    int? YearPublished,
    string? ThumbnailUrl,
    decimal UserRating,
    IReadOnlyList<string> Mechanisms
);
