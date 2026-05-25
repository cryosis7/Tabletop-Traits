using System.Text.Json;
using BoardGameRankings.Domain.Entities;
using BoardGameRankings.Domain.Interfaces;

namespace BoardGameRankings.Infrastructure.Persistence;

public class JsonUserRatingRepository : IUserRatingRepository
{
    private readonly string _basePath;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public JsonUserRatingRepository(string basePath)
    {
        _basePath = basePath;
    }

    public async Task<IReadOnlyList<UserRating>> GetAllAsync(string username)
    {
        var filePath = GetFilePath(username);
        if (!File.Exists(filePath))
            return Array.Empty<UserRating>();

        var json = await File.ReadAllTextAsync(filePath);
        var records = JsonSerializer.Deserialize<List<UserRatingRecord>>(json, JsonOptions);
        if (records == null)
            return Array.Empty<UserRating>();

        return records.Select(r => new UserRating(r.GameId, r.Username, r.Rating)).ToList();
    }

    public async Task SaveAsync(string username, IReadOnlyList<UserRating> ratings)
    {
        var filePath = GetFilePath(username);
        var directory = Path.GetDirectoryName(filePath)!;
        Directory.CreateDirectory(directory);

        var records = ratings.Select(r => new UserRatingRecord
        {
            GameId = r.GameId,
            Username = r.Username,
            Rating = r.Rating
        }).ToList();

        var json = JsonSerializer.Serialize(records, JsonOptions);
        await File.WriteAllTextAsync(filePath, json);
    }

    private string GetFilePath(string username) =>
        Path.Combine(_basePath, username.ToLowerInvariant(), "ratings.json");

    private record UserRatingRecord
    {
        public int GameId { get; init; }
        public string Username { get; init; } = string.Empty;
        public decimal Rating { get; init; }
    }
}
