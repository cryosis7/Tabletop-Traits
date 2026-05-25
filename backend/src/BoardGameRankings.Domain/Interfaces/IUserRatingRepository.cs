using BoardGameRankings.Domain.Entities;

namespace BoardGameRankings.Domain.Interfaces;

public interface IUserRatingRepository
{
    Task<IReadOnlyList<UserRating>> GetAllAsync(string username);
    Task SaveAsync(string username, IReadOnlyList<UserRating> ratings);
}
