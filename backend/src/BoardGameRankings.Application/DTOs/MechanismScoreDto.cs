namespace BoardGameRankings.Application.DTOs;

/// <summary>
/// Represents the calculated score of a mechanism across a user's rated games.
/// </summary>
public record MechanismScoreDto
{
    public MechanismScoreDto(
        int mechanismId,
        string mechanismName,
        decimal averageRating,
        decimal totalRating,
        int gameCount)
    {
        MechanismId = mechanismId;
        MechanismName = mechanismName;
        AverageRating = averageRating;
        TotalRating = totalRating;
        GameCount = gameCount;
    }

    /// <summary>
    /// BoardGameGeek identifier for the mechanism.
    /// </summary>
    public int MechanismId { get; init; }

    /// <summary>
    /// Human-readable mechanism name.
    /// </summary>
    public string MechanismName { get; init; }

    /// <summary>
    /// Average user rating across games that include the mechanism.
    /// </summary>
    public decimal AverageRating { get; init; }

    /// <summary>
    /// Sum of user ratings across games that include the mechanism.
    /// </summary>
    public decimal TotalRating { get; init; }

    /// <summary>
    /// Number of rated games that include the mechanism.
    /// </summary>
    public int GameCount { get; init; }
}
