using BoardGameRankings.Application.DTOs;

namespace BoardGameRankings.Application.Interfaces;

public enum ScoringMode
{
    Arithmetic,
    Bayesian,
    Median,
    Trimmed,
    Confidence,
    PositiveRate
}

public interface IMechanismAnalysisService
{
    Task<IReadOnlyList<MechanismScoreDto>> GetMechanismScoresAsync(string username, ScoringMode sortBy = ScoringMode.Arithmetic);
}
