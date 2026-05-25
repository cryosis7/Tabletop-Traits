using BoardGameRankings.Application.DTOs;

namespace BoardGameRankings.Application.Interfaces;

public interface ICollectionService
{
    Task<IReadOnlyList<BoardGameDto>> GetUserCollectionAsync(string username);
}
