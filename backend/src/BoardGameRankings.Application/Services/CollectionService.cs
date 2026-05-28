using BoardGameRankings.Application.DTOs;
using BoardGameRankings.Application.Interfaces;
using BoardGameRankings.Domain.Interfaces;

namespace BoardGameRankings.Application.Services;

public class CollectionService(
    IBoardGameRepository boardGameRepository,
    IUserRatingRepository userRatingRepository)
    : ICollectionService
{
    public async Task<IReadOnlyList<BoardGameDto>> GetUserCollectionAsync(string username)
    {
        var ratings = await userRatingRepository.GetAllAsync(username);
        var ratingLookup = ratings.ToDictionary(r => r.GameId, r => r.Rating);

        var games = await boardGameRepository.GetByIdsAsync(ratingLookup.Keys);

        return games
            .Where(g => ratingLookup.ContainsKey(g.Id))
            .Select(g => new BoardGameDto(
                g.Id,
                g.Name,
                g.YearPublished,
                g.ThumbnailUrl,
                ratingLookup[g.Id],
                g.Mechanisms.Select(m => m.Name).ToList()
            ))
            .OrderByDescending(g => g.UserRating)
            .ToList();
    }
}
