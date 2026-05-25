namespace BoardGameRankings.Domain.ValueObjects;

public class MechanismScore
{
    public int MechanismId { get; }
    public string MechanismName { get; }
    public decimal AverageRating { get; }
    public decimal TotalRating { get; }
    public int GameCount { get; }

    public MechanismScore(int mechanismId, string mechanismName, decimal averageRating, decimal totalRating, int gameCount)
    {
        MechanismId = mechanismId;
        MechanismName = mechanismName;
        AverageRating = averageRating;
        TotalRating = totalRating;
        GameCount = gameCount;
    }
}
