using BoardGameRankings.Domain.Entities;

namespace BoardGameRankings.Domain.Interfaces;

public interface IBoardGameRepository
{
    Task<IReadOnlyList<BoardGame>> GetByIdsAsync(IEnumerable<int> gameIds, CancellationToken cancellationToken = default);
}
