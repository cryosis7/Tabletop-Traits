using BoardGameRankings.Application.Interfaces;
using BoardGameRankings.Application.Services;
using BoardGameRankings.Domain.Entities;
using BoardGameRankings.Domain.Interfaces;

namespace BoardGameRankings.Application.Tests;

public class MechanismAnalysisServiceTests
{
    private readonly InMemoryBoardGameRepository _gameRepo = new();
    private readonly InMemoryUserRatingRepository _ratingRepo = new();
    private readonly MechanismAnalysisService _service;

    public MechanismAnalysisServiceTests()
    {
        _service = new MechanismAnalysisService(_gameRepo, _ratingRepo);
    }

    [Fact]
    public async Task GetMechanismScoresAsync_ArithmeticMean_ComputesSumOverCount()
    {
        SetupSingleMechanism("Dice Rolling", [6m, 8m, 10m]);

        var scores = await _service.GetMechanismScoresAsync("user");

        var score = Assert.Single(scores);
        Assert.Equal(8.00m, score.ArithmeticMean);
    }

    [Fact]
    public async Task GetMechanismScoresAsync_Median_OddCount_ReturnsMiddleValue()
    {
        SetupSingleMechanism("Dice Rolling", [3m, 7m, 9m]);

        var scores = await _service.GetMechanismScoresAsync("user");

        var score = Assert.Single(scores);
        Assert.Equal(7.00m, score.Median);
    }

    [Fact]
    public async Task GetMechanismScoresAsync_Median_EvenCount_ReturnsAverageOfMiddleTwo()
    {
        SetupSingleMechanism("Dice Rolling", [3m, 5m, 7m, 9m]);

        var scores = await _service.GetMechanismScoresAsync("user");

        var score = Assert.Single(scores);
        Assert.Equal(6.00m, score.Median);
    }

    [Fact]
    public async Task GetMechanismScoresAsync_TrimmedMean_WithFewGames_FallsBackToArithmetic()
    {
        SetupSingleMechanism("Dice Rolling", [1m, 5m, 10m]);

        var scores = await _service.GetMechanismScoresAsync("user");

        var score = Assert.Single(scores);
        // n=3 < 5, so trimmed mean equals arithmetic mean
        Assert.Equal(score.ArithmeticMean, score.TrimmedMean);
    }

    [Fact]
    public async Task GetMechanismScoresAsync_TrimmedMean_TrimsTopAndBottom10Percent()
    {
        // 10 ratings: trim 1 from each end
        SetupSingleMechanism("Worker Placement", [1m, 4m, 5m, 6m, 6m, 7m, 7m, 8m, 9m, 10m]);

        var scores = await _service.GetMechanismScoresAsync("user");

        var score = Assert.Single(scores);
        // After trimming: [4, 5, 6, 6, 7, 7, 8, 9] -> sum=52, n=8 -> 6.50
        Assert.Equal(6.50m, score.TrimmedMean);
    }

    [Fact]
    public async Task GetMechanismScoresAsync_BayesianAverage_ShrinksSingleGameTowardGlobalMean()
    {
        // Two mechanisms: one with 1 game (rating 10), one with 3 games
        var mech1 = new Mechanism(1, "Dice Rolling");
        var mech2 = new Mechanism(2, "Hand Management");

        _gameRepo.Games =
        [
            new BoardGame(100, "Game A", null, null, [mech1]),
            new BoardGame(101, "Game B", null, null, [mech2]),
            new BoardGame(102, "Game C", null, null, [mech2]),
            new BoardGame(103, "Game D", null, null, [mech2]),
        ];
        _ratingRepo.Ratings =
        [
            new UserRating(100, "user", 10m),
            new UserRating(101, "user", 6m),
            new UserRating(102, "user", 7m),
            new UserRating(103, "user", 8m),
        ];

        var scores = await _service.GetMechanismScoresAsync("user");

        var diceScore = scores.First(s => s.MechanismName == "Dice Rolling");
        // With only 1 game, Bayesian should pull toward global mean (mean of all 4 ratings = 7.75)
        Assert.True(diceScore.BayesianAverage < 10m, "Bayesian should shrink toward global mean");
        Assert.True(diceScore.BayesianAverage > diceScore.ArithmeticMean * 0.5m, "Should not shrink excessively");
    }

    [Fact]
    public async Task GetMechanismScoresAsync_ConfidenceAdjusted_PenalizesSmallSamples()
    {
        var mech1 = new Mechanism(1, "Dice Rolling");
        var mech2 = new Mechanism(2, "Hand Management");

        _gameRepo.Games =
        [
            new BoardGame(100, "Game A", null, null, [mech1]),
            new BoardGame(101, "Game B", null, null, [mech2]),
            new BoardGame(102, "Game C", null, null, [mech2]),
            new BoardGame(103, "Game D", null, null, [mech2]),
            new BoardGame(104, "Game E", null, null, [mech2]),
            new BoardGame(105, "Game F", null, null, [mech2]),
        ];
        _ratingRepo.Ratings =
        [
            new UserRating(100, "user", 8m),
            new UserRating(101, "user", 8m),
            new UserRating(102, "user", 8m),
            new UserRating(103, "user", 8m),
            new UserRating(104, "user", 8m),
            new UserRating(105, "user", 8m),
        ];

        var scores = await _service.GetMechanismScoresAsync("user");

        var dice = scores.First(s => s.MechanismName == "Dice Rolling");
        var hand = scores.First(s => s.MechanismName == "Hand Management");

        // Same arithmetic mean, but smaller sample should get lower confidence score
        Assert.True(dice.ConfidenceAdjusted <= hand.ConfidenceAdjusted,
            "Single-game mechanism should have equal or lower confidence-adjusted score");
    }

    [Fact]
    public async Task GetMechanismScoresAsync_ConfidenceAdjusted_NeverGoesNegative()
    {
        SetupSingleMechanism("Dice Rolling", [1m]);

        var scores = await _service.GetMechanismScoresAsync("user");

        var score = Assert.Single(scores);
        Assert.True(score.ConfidenceAdjusted >= 0m);
    }

    [Fact]
    public async Task GetMechanismScoresAsync_PositiveRate_CountsGames7OrAbove()
    {
        SetupSingleMechanism("Dice Rolling", [5m, 6m, 7m, 8m, 9m]);

        var scores = await _service.GetMechanismScoresAsync("user");

        var score = Assert.Single(scores);
        // 3 out of 5 are >= 7 -> 60%
        Assert.Equal(60.00m, score.PositiveRate);
    }

    [Fact]
    public async Task GetMechanismScoresAsync_PositiveRate_ZeroWhenAllBelow7()
    {
        SetupSingleMechanism("Dice Rolling", [3m, 4m, 5m, 6m]);

        var scores = await _service.GetMechanismScoresAsync("user");

        var score = Assert.Single(scores);
        Assert.Equal(0.00m, score.PositiveRate);
    }

    [Fact]
    public async Task GetMechanismScoresAsync_SortsBySelectedMode()
    {
        var mech1 = new Mechanism(1, "Dice Rolling");
        var mech2 = new Mechanism(2, "Hand Management");

        _gameRepo.Games =
        [
            new BoardGame(100, "Game A", null, null, [mech1]),
            new BoardGame(101, "Game B", null, null, [mech2]),
            new BoardGame(102, "Game C", null, null, [mech2]),
        ];
        _ratingRepo.Ratings =
        [
            new UserRating(100, "user", 10m),
            new UserRating(101, "user", 6m),
            new UserRating(102, "user", 7m),
        ];

        var byArithmetic = await _service.GetMechanismScoresAsync("user", ScoringMode.Arithmetic);
        Assert.Equal("Dice Rolling", byArithmetic[0].MechanismName);

        var byPositiveRate = await _service.GetMechanismScoresAsync("user", ScoringMode.PositiveRate);
        // Dice Rolling: 1/1 = 100%, Hand Management: 1/2 = 50%
        Assert.Equal("Dice Rolling", byPositiveRate[0].MechanismName);
    }

    [Fact]
    public async Task GetMechanismScoresAsync_EmptyCollection_ReturnsEmptyList()
    {
        _gameRepo.Games = [];
        _ratingRepo.Ratings = [];

        var scores = await _service.GetMechanismScoresAsync("user");

        Assert.Empty(scores);
    }

    [Fact]
    public async Task GetMechanismScoresAsync_GameCount_ReflectsNumberOfRatedGamesWithMechanism()
    {
        SetupSingleMechanism("Dice Rolling", [5m, 7m, 9m]);

        var scores = await _service.GetMechanismScoresAsync("user");

        var score = Assert.Single(scores);
        Assert.Equal(3, score.GameCount);
    }

    private void SetupSingleMechanism(string mechanismName, decimal[] ratings)
    {
        var mechanism = new Mechanism(1, mechanismName);
        _gameRepo.Games = ratings.Select((r, i) =>
            new BoardGame(100 + i, $"Game {i}", null, null, [mechanism])).ToList();
        _ratingRepo.Ratings = ratings.Select((r, i) =>
            new UserRating(100 + i, "user", r)).ToList();
    }

    private class InMemoryBoardGameRepository : IBoardGameRepository
    {
        public List<BoardGame> Games { get; set; } = [];

        public Task<IReadOnlyList<BoardGame>> GetByIdsAsync(IEnumerable<int> gameIds, CancellationToken cancellationToken = default)
        {
            var ids = gameIds.ToHashSet();
            IReadOnlyList<BoardGame> result = Games.Where(g => ids.Contains(g.Id)).ToList();
            return Task.FromResult(result);
        }
    }

    private class InMemoryUserRatingRepository : IUserRatingRepository
    {
        public List<UserRating> Ratings { get; set; } = [];

        public Task<IReadOnlyList<UserRating>> GetAllAsync(string username, CancellationToken cancellationToken = default)
        {
            IReadOnlyList<UserRating> result = Ratings.Where(r => r.Username == username).ToList();
            return Task.FromResult(result);
        }

        public void Invalidate(string username) { }
    }
}
