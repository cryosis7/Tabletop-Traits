using BoardGameRankings.Application.DTOs;
using BoardGameRankings.Application.Interfaces;
using BoardGameRankings.Domain.Interfaces;

namespace BoardGameRankings.Application.Services;

public class SyncService(
    IBoardGameRepository boardGameRepository,
    IUserRatingRepository userRatingRepository)
    : ISyncService
{
    public async Task<SyncStatusDto> SyncUserCollectionAsync(string username, CancellationToken cancellationToken = default)
    {
        // Invalidate cached ratings so the next read fetches fresh data from BGG
        userRatingRepository.Invalidate(username);

        var ratings = await userRatingRepository.GetAllAsync(username, cancellationToken);

        if (ratings.Count == 0)
        {
            return new SyncStatusDto("Complete", 0, 0, DateTime.UtcNow);
        }

        // Ensure game details are cached
        var gameIds = ratings.Select(r => r.GameId).ToList();
        var games = await boardGameRepository.GetByIdsAsync(gameIds, cancellationToken);

        return new SyncStatusDto("Complete", games.Count, ratings.Count, DateTime.UtcNow);
    }
}
