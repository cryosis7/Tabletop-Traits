using BoardGameRankings.Domain.Entities;

namespace BoardGameRankings.Domain.Interfaces;

public interface IUserRatingRepository
{
    Task<IReadOnlyList<UserRating>> GetAllAsync(string username, CancellationToken cancellationToken = default);
    void Invalidate(string username);
}
