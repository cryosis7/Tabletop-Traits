using BoardGameRankings.Domain.Entities;

namespace BoardGameRankings.Domain.Interfaces;

public interface IBoardGameRepository
{
    Task<IReadOnlyList<BoardGame>> GetAllAsync(string username);
    Task<BoardGame?> GetByIdAsync(string username, int gameId);
    Task SaveAsync(string username, IReadOnlyList<BoardGame> games);
}
