using BoardGameRankings.Application.DTOs;
using BoardGameRankings.Application.Interfaces;
using BoardGameRankings.Domain.Interfaces;

namespace BoardGameRankings.Application.Services;

public class CollectionService : ICollectionService
{
    private readonly IBoardGameRepository _boardGameRepository;
    private readonly IUserRatingRepository _userRatingRepository;

    public CollectionService(
        IBoardGameRepository boardGameRepository,
        IUserRatingRepository userRatingRepository)
    {
        _boardGameRepository = boardGameRepository;
        _userRatingRepository = userRatingRepository;
    }

    public async Task<IReadOnlyList<BoardGameDto>> GetUserCollectionAsync(string username)
    {
        var games = await _boardGameRepository.GetAllAsync(username);
        var ratings = await _userRatingRepository.GetAllAsync(username);

        var ratingLookup = ratings.ToDictionary(r => r.GameId, r => r.Rating);

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
