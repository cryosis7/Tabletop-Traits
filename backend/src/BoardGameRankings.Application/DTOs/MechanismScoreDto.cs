namespace BoardGameRankings.Application.DTOs;

/// <summary>
/// Represents the calculated scores of a mechanism across a user's rated games.
/// All scoring methods are computed in one pass and returned together.
/// </summary>
public record MechanismScoreDto
{
    public MechanismScoreDto(
        int mechanismId,
        string mechanismName,
        decimal arithmeticMean,
        decimal bayesianAverage,
        decimal median,
        decimal trimmedMean,
        decimal confidenceAdjusted,
        decimal positiveRate,
        int gameCount)
    {
        MechanismId = mechanismId;
        MechanismName = mechanismName;
        ArithmeticMean = arithmeticMean;
        BayesianAverage = bayesianAverage;
        Median = median;
        TrimmedMean = trimmedMean;
        ConfidenceAdjusted = confidenceAdjusted;
        PositiveRate = positiveRate;
        GameCount = gameCount;
    }

    public int MechanismId { get; init; }
    public string MechanismName { get; init; }

    /// <summary>
    /// Simple average: sum(ratings) / n.
    /// </summary>
    public decimal ArithmeticMean { get; init; }

    /// <summary>
    /// Bayesian average: (C * globalMean + sum) / (C + n), where C = mean game count across mechanisms.
    /// Shrinks small-sample mechanisms toward the user's overall mean.
    /// </summary>
    public decimal BayesianAverage { get; init; }

    /// <summary>
    /// Middle value of sorted ratings (average of two middle values for even n).
    /// </summary>
    public decimal Median { get; init; }

    /// <summary>
    /// Mean after removing the top and bottom 10% of ratings.
    /// Falls back to arithmetic mean when n is less than 5.
    /// </summary>
    public decimal TrimmedMean { get; init; }

    /// <summary>
    /// Lower bound of 95% confidence interval: mean - 1.96 * (stddev / sqrt(n)).
    /// Penalizes mechanisms with few data points.
    /// </summary>
    public decimal ConfidenceAdjusted { get; init; }

    /// <summary>
    /// Percentage of rated games scoring 7 or higher (BGG: "Good game, usually willing to play").
    /// </summary>
    public decimal PositiveRate { get; init; }

    /// <summary>
    /// Number of rated games that include this mechanism.
    /// </summary>
    public int GameCount { get; init; }
}
