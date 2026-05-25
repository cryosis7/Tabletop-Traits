using BoardGameRankings.Application.DTOs;
using BoardGameRankings.Application.Interfaces;
using BoardGameRankings.Domain.Entities;
using BoardGameRankings.Domain.Interfaces;

namespace BoardGameRankings.Application.Services;

public class SyncService : ISyncService
{
    private readonly IBggApiClient _bggApiClient;
    private readonly IBoardGameRepository _boardGameRepository;
    private readonly IUserRatingRepository _userRatingRepository;

    public SyncService(
        IBggApiClient bggApiClient,
        IBoardGameRepository boardGameRepository,
        IUserRatingRepository userRatingRepository)
    {
        _bggApiClient = bggApiClient;
        _boardGameRepository = boardGameRepository;
        _userRatingRepository = userRatingRepository;
    }

    public async Task<SyncStatusDto> SyncUserCollectionAsync(string username, CancellationToken cancellationToken = default)
    {
        // Step 1: Fetch user's rated collection from BGG
        var collectionItems = await _bggApiClient.GetUserRatedCollectionAsync(username, cancellationToken);

        if (collectionItems.Count == 0)
        {
            return new SyncStatusDto("Complete", 0, 0, DateTime.UtcNow);
        }

        // Step 2: Fetch game details (mechanisms) in batches
        var gameIds = collectionItems.Select(c => c.GameId).ToList();
        var games = await _bggApiClient.GetGameDetailsAsync(gameIds, cancellationToken);

        // Step 3: Build user ratings
        var ratings = collectionItems
            .Select(c => new UserRating(c.GameId, username, c.Rating))
            .ToList();

        // Step 4: Persist data
        await _boardGameRepository.SaveAsync(username, games);
        await _userRatingRepository.SaveAsync(username, ratings);

        return new SyncStatusDto("Complete", games.Count, collectionItems.Count, DateTime.UtcNow);
    }
}
