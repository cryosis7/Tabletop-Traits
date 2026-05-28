using BoardGameRankings.Domain.Entities;
using BoardGameRankings.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace BoardGameRankings.Infrastructure.Persistence;

public class CachedBoardGameRepository(IBggApiClient bggApiClient, IMemoryCache cache) : IBoardGameRepository
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromDays(1);

    public async Task<IReadOnlyList<BoardGame>> GetByIdsAsync(
        IEnumerable<int> gameIds, CancellationToken cancellationToken = default)
    {
        var ids = gameIds.ToList();
        var results = new List<BoardGame>(ids.Count);
        var missingIds = new List<int>();

        foreach (var id in ids)
        {
            if (cache.TryGetValue(CacheKey(id), out BoardGame? cached) && cached is not null)
                results.Add(cached);
            else
                missingIds.Add(id);
        }

        if (missingIds.Count > 0)
        {
            var fetched = await bggApiClient.GetGameDetailsAsync(missingIds, cancellationToken);

            foreach (var game in fetched)
            {
                cache.Set(CacheKey(game.Id), game, CacheDuration);
                results.Add(game);
            }
        }

        return results;
    }

    private static string CacheKey(int gameId) => $"boardgame:{gameId}";
}
