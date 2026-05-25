namespace BoardGameRankings.Application.DTOs;

/// <summary>
/// Represents a rated board game in a user's synced collection.
/// </summary>
public record BoardGameDto
{
    public BoardGameDto(
        int id,
        string name,
        int? yearPublished,
        string? thumbnailUrl,
        decimal userRating,
        IReadOnlyList<string> mechanisms)
    {
        Id = id;
        Name = name;
        YearPublished = yearPublished;
        ThumbnailUrl = thumbnailUrl;
        UserRating = userRating;
        Mechanisms = mechanisms;
    }

    /// <summary>
    /// BoardGameGeek identifier for the game.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Display name of the board game.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// Original publication year reported by BoardGameGeek.
    /// </summary>
    public int? YearPublished { get; init; }

    /// <summary>
    /// Thumbnail image URL for the game, when available.
    /// </summary>
    public string? ThumbnailUrl { get; init; }

    /// <summary>
    /// Rating the user assigned to the game on BoardGameGeek.
    /// </summary>
    public decimal UserRating { get; init; }

    /// <summary>
    /// Mechanism names associated with the game.
    /// </summary>
    public IReadOnlyList<string> Mechanisms { get; init; }
}
