using BoardGameRankings.Domain.Entities;
using BoardGameRankings.Domain.Interfaces;

namespace BoardGameRankings.Infrastructure.BggApi;

public class CachingBggApiClient(IBggApiClient inner, IBoardGameRepository boardGameRepository) : IBggApiClient
{
    public Task<IReadOnlyList<(int GameId, string Name, decimal Rating)>> GetUserRatedCollectionAsync(
        string username, CancellationToken cancellationToken = default)
    {
        return inner.GetUserRatedCollectionAsync(username, cancellationToken);
    }

    public async Task<IReadOnlyList<BoardGame>> GetGameDetailsAsync(
        IEnumerable<int> gameIds, CancellationToken cancellationToken = default)
    {
        var ids = gameIds.ToList();
        var cached = await boardGameRepository.GetByIdsAsync(ids);
        var cachedIds = cached.Select(g => g.Id).ToHashSet();
        var missingIds = ids.Where(id => !cachedIds.Contains(id)).ToList();

        if (missingIds.Count == 0)
            return cached;

        var fetched = await inner.GetGameDetailsAsync(missingIds, cancellationToken);
        await boardGameRepository.SaveAsync(fetched);

        return cached.Concat(fetched).ToList();
    }
}
