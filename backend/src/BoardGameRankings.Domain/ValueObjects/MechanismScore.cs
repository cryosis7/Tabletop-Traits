namespace BoardGameRankings.Domain.ValueObjects;

public class MechanismScore(
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
    public int MechanismId { get; } = mechanismId;
    public string MechanismName { get; } = mechanismName;
    public decimal ArithmeticMean { get; } = arithmeticMean;
    public decimal BayesianAverage { get; } = bayesianAverage;
    public decimal Median { get; } = median;
    public decimal TrimmedMean { get; } = trimmedMean;
    public decimal ConfidenceAdjusted { get; } = confidenceAdjusted;
    public decimal PositiveRate { get; } = positiveRate;
    public int GameCount { get; } = gameCount;
}
