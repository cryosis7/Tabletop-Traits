using BoardGameRankings.Application.DTOs;
using BoardGameRankings.Application.Interfaces;
using BoardGameRankings.Domain.Entities;
using BoardGameRankings.Domain.Interfaces;

namespace BoardGameRankings.Application.Services;

public class SyncService(
    IBggApiClient bggApiClient,
    IUserRatingRepository userRatingRepository)
    : ISyncService
{
    public async Task<SyncStatusDto> SyncUserCollectionAsync(string username, CancellationToken cancellationToken = default)
    {
        // Step 1: Fetch user's rated collection from BGG
        var collectionItems = await bggApiClient.GetUserRatedCollectionAsync(username, cancellationToken);

        if (collectionItems.Count == 0)
        {
            return new SyncStatusDto("Complete", 0, 0, DateTime.UtcNow);
        }

        // Step 2: Fetch game details
        var gameIds = collectionItems.Select(c => c.GameId).ToList();
        var games = await bggApiClient.GetGameDetailsAsync(gameIds, cancellationToken);

        // Step 3: Build and persist user ratings
        var ratings = collectionItems
            .Select(c => new UserRating(c.GameId, username, c.Rating))
            .ToList();

        await userRatingRepository.SaveAsync(username, ratings);

        return new SyncStatusDto("Complete", games.Count, collectionItems.Count, DateTime.UtcNow);
    }
}
