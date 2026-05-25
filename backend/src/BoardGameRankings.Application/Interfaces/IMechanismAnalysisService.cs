using BoardGameRankings.Application.DTOs;

namespace BoardGameRankings.Application.Interfaces;

public enum ScoringMode
{
    Average,
    Cumulative
}

public interface IMechanismAnalysisService
{
    Task<IReadOnlyList<MechanismScoreDto>> GetMechanismScoresAsync(string username, ScoringMode mode = ScoringMode.Average);
}
