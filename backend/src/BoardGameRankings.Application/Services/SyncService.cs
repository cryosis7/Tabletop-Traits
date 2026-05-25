using BoardGameRankings.Application.DTOs;
using BoardGameRankings.Application.Interfaces;
using BoardGameRankings.Domain.Entities;
using BoardGameRankings.Domain.Interfaces;

namespace BoardGameRankings.Application.Services;

public class SyncService(
    IBggApiClient bggApiClient,
    IBoardGameRepository boardGameRepository,
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

        // Step 2: Only fetch game details for games we don't already have
        var gameIds = collectionItems.Select(c => c.GameId).ToList();
        var existingGames = await boardGameRepository.GetByIdsAsync(gameIds);
        var existingIds = existingGames.Select(g => g.Id).ToHashSet();
        var missingIds = gameIds.Where(id => !existingIds.Contains(id)).ToList();

        if (missingIds.Count > 0)
        {
            var newGames = await bggApiClient.GetGameDetailsAsync(missingIds, cancellationToken);
            await boardGameRepository.SaveAsync(newGames);
        }

        // Step 3: Build user ratings
        var ratings = collectionItems
            .Select(c => new UserRating(c.GameId, username, c.Rating))
            .ToList();

        // Step 4: Persist ratings
        await userRatingRepository.SaveAsync(username, ratings);

        return new SyncStatusDto("Complete", existingGames.Count + missingIds.Count, collectionItems.Count, DateTime.UtcNow);
    }
}
