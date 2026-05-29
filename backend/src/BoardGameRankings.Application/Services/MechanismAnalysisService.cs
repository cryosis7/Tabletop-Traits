using BoardGameRankings.Application.DTOs;
using BoardGameRankings.Application.Interfaces;
using BoardGameRankings.Domain.Interfaces;

namespace BoardGameRankings.Application.Services;

public class MechanismAnalysisService(
    IBoardGameRepository boardGameRepository,
    IUserRatingRepository userRatingRepository)
    : IMechanismAnalysisService
{
    private const decimal TrimFraction = 0.10m;
    private const decimal ConfidenceZ = 1.96m;
    private const decimal PositiveThreshold = 7m;

    public async Task<IReadOnlyList<MechanismScoreDto>> GetMechanismScoresAsync(string username, ScoringMode sortBy = ScoringMode.Arithmetic)
    {
        var ratings = await userRatingRepository.GetAllAsync(username);
        var ratingLookup = ratings.ToDictionary(r => r.GameId, r => r.Rating);

        var games = await boardGameRepository.GetByIdsAsync(ratingLookup.Keys);

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
                    mechanismRatings[key] = [];

                mechanismRatings[key].Add(rating);
            }
        }

        // Compute global stats for Bayesian calculation
        var allRatings = ratingLookup.Values;
        var globalMean = allRatings.Any() ? allRatings.Average() : 5m;
        var meanGameCount = mechanismRatings.Count > 0
            ? (decimal)mechanismRatings.Values.Average(v => v.Count)
            : 1m;

        // Calculate all scores for each mechanism
        var scores = mechanismRatings.Select(kvp =>
        {
            var values = kvp.Value;
            var n = values.Count;

            return new MechanismScoreDto(
                kvp.Key.Id,
                kvp.Key.Name,
                Math.Round(ComputeArithmeticMean(values, n), 2),
                Math.Round(ComputeBayesianAverage(values, n, globalMean, meanGameCount), 2),
                Math.Round(ComputeMedian(values, n), 2),
                Math.Round(ComputeTrimmedMean(values, n), 2),
                Math.Round(ComputeConfidenceAdjusted(values, n), 2),
                Math.Round(ComputePositiveRate(values, n), 2),
                n
            );
        });

        // Sort by the selected scoring method
        var sorted = sortBy switch
        {
            ScoringMode.Bayesian => scores.OrderByDescending(s => s.BayesianAverage),
            ScoringMode.Median => scores.OrderByDescending(s => s.Median),
            ScoringMode.Trimmed => scores.OrderByDescending(s => s.TrimmedMean),
            ScoringMode.Confidence => scores.OrderByDescending(s => s.ConfidenceAdjusted),
            ScoringMode.PositiveRate => scores.OrderByDescending(s => s.PositiveRate),
            _ => scores.OrderByDescending(s => s.ArithmeticMean)
        };

        return sorted.ToList();
    }

    private static decimal ComputeArithmeticMean(List<decimal> values, int n)
    {
        return n > 0 ? values.Sum() / n : 0m;
    }

    private static decimal ComputeBayesianAverage(List<decimal> values, int n, decimal globalMean, decimal c)
    {
        if (n == 0) return 0m;
        return (c * globalMean + values.Sum()) / (c + n);
    }

    private static decimal ComputeMedian(List<decimal> values, int n)
    {
        if (n == 0) return 0m;
        var sorted = values.OrderBy(v => v).ToList();
        if (n % 2 == 1)
            return sorted[n / 2];
        return (sorted[n / 2 - 1] + sorted[n / 2]) / 2m;
    }

    private static decimal ComputeTrimmedMean(List<decimal> values, int n)
    {
        if (n < 5)
            return ComputeArithmeticMean(values, n);

        var trimCount = (int)Math.Floor(n * TrimFraction);
        var sorted = values.OrderBy(v => v).ToList();
        var trimmed = sorted.Skip(trimCount).Take(n - 2 * trimCount).ToList();
        return trimmed.Count > 0 ? trimmed.Sum() / trimmed.Count : 0m;
    }

    private static decimal ComputeConfidenceAdjusted(List<decimal> values, int n)
    {
        if (n == 0) return 0m;
        var mean = values.Sum() / n;
        if (n == 1) return Math.Max(0m, mean - ConfidenceZ);

        var variance = values.Sum(v => (v - mean) * (v - mean)) / (n - 1);
        var stddev = (decimal)Math.Sqrt((double)variance);
        var result = mean - ConfidenceZ * (stddev / (decimal)Math.Sqrt(n));
        return Math.Max(0m, result);
    }

    private static decimal ComputePositiveRate(List<decimal> values, int n)
    {
        if (n == 0) return 0m;
        var positiveCount = values.Count(v => v >= PositiveThreshold);
        return (decimal)positiveCount / n * 100m;
    }
}
