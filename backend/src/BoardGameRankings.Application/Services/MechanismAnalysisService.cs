using BoardGameRankings.Application.DTOs;
using BoardGameRankings.Application.Interfaces;
using BoardGameRankings.Domain.Interfaces;

namespace BoardGameRankings.Application.Services;

public class MechanismAnalysisService : IMechanismAnalysisService
{
    private readonly IBoardGameRepository _boardGameRepository;
    private readonly IUserRatingRepository _userRatingRepository;

    public MechanismAnalysisService(
        IBoardGameRepository boardGameRepository,
        IUserRatingRepository userRatingRepository)
    {
        _boardGameRepository = boardGameRepository;
        _userRatingRepository = userRatingRepository;
    }

    public async Task<IReadOnlyList<MechanismScoreDto>> GetMechanismScoresAsync(string username, ScoringMode mode = ScoringMode.Average)
    {
        var games = await _boardGameRepository.GetAllAsync(username);
        var ratings = await _userRatingRepository.GetAllAsync(username);

        var ratingLookup = ratings.ToDictionary(r => r.GameId, r => r.Rating);

        // Build mechanism -> list of ratings mapping
        var mechanismRatings = new Dictionary<(int Id, string Name), List<decimal>>();

        foreach (var game in games)
        {
            if (!ratingLookup.TryGetValue(game.Id, out var rating))
                continue;

            foreach (var mechanism in game.Mechanisms)
            {
                var key = (mechanism.Id, mechanism.Name);
                if (!mechanismRatings.ContainsKey(key))
                    mechanismRatings[key] = new List<decimal>();

                mechanismRatings[key].Add(rating);
            }
        }

        // Calculate scores based on mode
        var scores = mechanismRatings.Select(kvp =>
        {
            var totalRating = kvp.Value.Sum();
            var averageRating = kvp.Value.Count > 0 ? totalRating / kvp.Value.Count : 0m;
            var gameCount = kvp.Value.Count;

            return new MechanismScoreDto(
                kvp.Key.Id,
                kvp.Key.Name,
                Math.Round(averageRating, 2),
                Math.Round(totalRating, 2),
                gameCount
            );
        });

        // Sort by the relevant metric
        scores = mode switch
        {
            ScoringMode.Cumulative => scores.OrderByDescending(s => s.TotalRating),
            _ => scores.OrderByDescending(s => s.AverageRating)
        };

        return scores.ToList();
    }
}
