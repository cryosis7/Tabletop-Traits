using BoardGameRankings.Domain.Entities;

namespace BoardGameRankings.Domain.Interfaces;

public interface IBggApiClient
{
    Task<IReadOnlyList<(int GameId, string Name, decimal Rating)>> GetUserRatedCollectionAsync(string username, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BoardGame>> GetGameDetailsAsync(IEnumerable<int> gameIds, CancellationToken cancellationToken = default);
}
