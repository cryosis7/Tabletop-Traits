namespace BoardGameRankings.Domain.Entities;

public class UserRating
{
    public int GameId { get; }
    public string Username { get; }
    public decimal Rating { get; }

    public UserRating(int gameId, string username, decimal rating)
    {
        GameId = gameId;
        Username = username;
        Rating = rating;
    }
}
